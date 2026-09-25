namespace Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using ClosedXML.Excel;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NalamVazha.Models;
    using Newtonsoft.Json;

    public class AssessmentImportController : BaseController
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AssessmentImportController> _logger;

        public AssessmentImportController(
            IConfiguration configuration,
            IHttpContextAccessor accessor,
            IOptions<ApiSettings> apiSettings,
            IOptions<MailSettings> mailSettings,
            IWebHostEnvironment env,
            ILogger<AssessmentImportController> logger)
            : base(configuration)
        {
            _env    = env;
            _logger = logger;
        }

        // ── GET: /AssessmentImport/Import_Assessment ─────────────────────────
        [HttpGet]
        public IActionResult Import_Assessment()
        {
            if (HttpContext.Session.GetString("NalamVazhaloginUserID") == null)
                return Redirect("Login");
            return View();
        }

        // ── POST: /AssessmentImport/Import_Assessment ────────────────────────
        [HttpPost]
        public async Task<string> Import_Assessment(
            IFormFile excelFile,
            string    templatename,
            string    tenantid)
        {
            if (HttpContext.Session.GetString("NalamVazhaloginUserID") == null)
                return "Session Expired";

            try
            {
                if (excelFile == null || excelFile.Length == 0)
                    return JsonConvert.SerializeObject(new AssessmentImportResult
                    { success = false, message = "Please select an Excel file to upload." });

                var ext = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
                if (ext != ".xlsx" && ext != ".xls")
                    return JsonConvert.SerializeObject(new AssessmentImportResult
                    { success = false, message = "Only .xlsx / .xls files are supported." });

                // ── Parse Excel ───────────────────────────────────────────────
                var questions = ParseExcel(excelFile);

                if (questions.Count == 0)
                    return JsonConvert.SerializeObject(new AssessmentImportResult
                    { success = false, message = "No valid question rows found in the uploaded file." });

                // ── Build payload and call WebApi ─────────────────────────────
                var payload = new AssessmentImportPayload
                {
                    templatename = templatename?.Trim(),
                    tenantid     = string.IsNullOrWhiteSpace(tenantid) ? null : tenantid.Trim(),
                    questions    = questions
                };

                var resultJson = await ApiClient.Post_ApiValuesGetString(
                    getHttpClient(),
                    "api/AssessmentImport/Import_Assessment",
                    payload);

                return resultJson;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AssessmentImport controller error");
                return JsonConvert.SerializeObject(new AssessmentImportResult
                { success = false, message = "Error: " + ex.Message });
            }
        }

        // ── Excel parsing ─────────────────────────────────────────────────────
        // Columns expected (row 1 = header, skipped):
        //   A: Category      – blank cells inherit the last non-blank value above
        //   B: SubCategory   – blank cells inherit the last non-blank value above
        //   C: Question Text
        //   D: Option Value  (comma-separated options, may be empty)
        //   E: Field Type    (SingleSelect|MultiSelect|Boolean|Text|Number|Scale|MultilineText)
        private static List<AssessmentImportQuestion> ParseExcel(IFormFile file)
        {
            var questions  = new List<AssessmentImportQuestion>();
            var validTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                { "SingleSelect","MultiSelect","Boolean","Text","Number","Scale","MultilineText" };

            string lastCategory    = "";
            string lastSubCategory = "";

            using var ms = new MemoryStream();
            file.CopyTo(ms);
            ms.Position = 0;

            using var workbook = new XLWorkbook(ms);
            var ws = workbook.Worksheet(1);

            bool headerSkipped = false;
            foreach (var row in ws.RowsUsed())
            {
                if (!headerSkipped) { headerSkipped = true; continue; }

                var category    = (row.Cell(1).GetString() ?? "").Trim();
                var subcategory = (row.Cell(2).GetString() ?? "").Trim();
                var qText       = (row.Cell(3).GetString() ?? "").Trim();
                var optVal      = (row.Cell(4).GetString() ?? "").Trim();
                var ftRaw       = (row.Cell(5).GetString() ?? "").Trim();

                if (string.IsNullOrEmpty(qText)) continue;

                // ── Inherit blank cells from the last non-blank row ───────────
                if (!string.IsNullOrEmpty(category))    lastCategory    = category;
                else                                     category    = lastCategory;

                if (!string.IsNullOrEmpty(subcategory)) lastSubCategory = subcategory;
                else                                     subcategory = lastSubCategory;

                if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(subcategory)) continue;

                // ── Resolve Field Type ────────────────────────────────────────
                string fieldType;
                if (!string.IsNullOrEmpty(ftRaw) && validTypes.Contains(ftRaw))
                    fieldType = ftRaw;
                else if (!string.IsNullOrEmpty(optVal))
                    fieldType = "SingleSelect";   // has options but no explicit type
                else
                    fieldType = "Text";

                // ── Scale range ───────────────────────────────────────────────
                int? scaleMin = null, scaleMax = null;
                if (string.Equals(fieldType, "Scale", StringComparison.OrdinalIgnoreCase))
                {
                    var m = Regex.Match(optVal ?? "", @"(\d+)\s*[-–]\s*(\d+)");
                    scaleMin = m.Success ? int.Parse(m.Groups[1].Value) : 0;
                    scaleMax = m.Success ? int.Parse(m.Groups[2].Value) : 10;
                }

                questions.Add(new AssessmentImportQuestion
                {
                    categoryname    = category,
                    subcategoryname = subcategory,
                    questiontext    = qText,
                    answertype      = fieldType,
                    optionvalue     = string.IsNullOrEmpty(optVal) ? null : optVal,
                    scalerangemin   = scaleMin,
                    scalerangemax   = scaleMax,
                    isrequired      = true
                });
            }

            return questions;
        }
    }
}
