namespace Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using ClosedXML.Excel;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NalamVazha.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Admin.Controllers;

    // UI controller for HolisticHistory screening linked to an IPD form id.
    public class HolisticHistoryController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly ILogger<HolisticHistoryController> _logger;

        private static readonly object _schemaLock = new object();
        private static List<HolisticHistorySectionModel>? _cachedSections;
        private static bool _schemaLoaded = false;

        public HolisticHistoryController(
            IConfiguration configuration,
            IHttpContextAccessor accessor,
            IOptions<ApiSettings> apiSettings,
            IOptions<MailSettings> mailSettings,
            IWebHostEnvironment env,
            ILogger<HolisticHistoryController> logger) : base(configuration)
        {
            _hostingEnv = env;
            _logger = logger;
        }

        private string GetExcelPath()
        {
            // Admin content root is typically: .../webapp/Admin
            // We need: .../webapp/NalamVazha.DAL/DBScripts/Holistic History.xlsx
            var p = System.IO.Path.Combine(_hostingEnv.ContentRootPath, "..", "NalamVazha.DAL", "DBScripts", "Holistic History.xlsx");
            return System.IO.Path.GetFullPath(p);
        }

        private List<HolisticHistorySectionModel> BuildSchema()
        {
            var excelPath = GetExcelPath();
            using var workbook = new XLWorkbook(excelPath);
            var ws = workbook.Worksheet("Screening");

            var sectionRe = new Regex(@"^\s*\d+\s*[\.:\)\-]", RegexOptions.Compiled);
            var dimRe = new Regex(@"DIMENSION", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Input cells are laid out across columns A..F (1..6).
            const int firstCol = 1;
            const int lastCol = 6;

            int lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
            int startRow = 2; // pandas(header=0) aligns with sheet row 2 as first data row

            var sections = new List<HolisticHistorySectionModel>();
            var sectionIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            string currentSectionTitle = "Holistic History";
            string currentGroupTitle = "";

            // Track the currently active field per column for option lines like: ☐ Low
            var currentFieldByCol = new HolisticHistoryFieldModel?[lastCol + 1];

            int order = 0;

            HolisticHistorySectionModel EnsureSection(string title)
            {
                if (!sectionIndex.TryGetValue(title, out var idx))
                {
                    idx = sections.Count;
                    sections.Add(new HolisticHistorySectionModel { title = title, fields = new List<HolisticHistoryFieldModel>() });
                    sectionIndex[title] = idx;
                }

                return sections[idx];
            }

            string Sanitize(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return "";
                return s.Replace("\u00A0", " ").Trim();
            }

            var underscoreRe = new Regex(@"_\s*[_\-]", RegexOptions.Compiled);

            bool IsMultiSelectQuestion(string? questionLabel)
            {
                if (string.IsNullOrWhiteSpace(questionLabel)) return false;
                var lower = questionLabel.ToLowerInvariant();
                // In the Excel sheet, multi-select questions are explicitly phrased like:
                // "(tick all)" / "tick all".
                if (lower.Contains("tick all")) return true;
                if (lower.Contains("select all")) return true;
                if (lower.Contains("choose all")) return true;
                if (lower.Contains("multiple")) return true;
                return false;
            }

            for (int row = startRow; row <= lastRow; row++)
            {
                for (int col = firstCol; col <= lastCol; col++)
                {
                    var raw = ws.Cell(row, col).GetString();
                    var text = Sanitize(raw);
                    if (string.IsNullOrWhiteSpace(text)) continue;

                    bool isSection = dimRe.IsMatch(text) || sectionRe.IsMatch(text);
                    if (isSection)
                    {
                        currentSectionTitle = text;
                        currentGroupTitle = "";
                        currentFieldByCol[col] = null;
                        EnsureSection(currentSectionTitle);
                        continue;
                    }

                    // Sub-headings (like "Indicators" / "Habits & Context") for better UX.
                    bool looksLikeGroupHeading = (text.Contains("Indicators", StringComparison.OrdinalIgnoreCase) ||
                                                  text.Contains("Habits", StringComparison.OrdinalIgnoreCase)) &&
                                                 !text.Contains(":") &&
                                                 !text.StartsWith("☐", StringComparison.Ordinal);

                    if (looksLikeGroupHeading)
                    {
                        currentGroupTitle = text;
                        currentFieldByCol[col] = null;
                        continue;
                    }

                    // Option lines: ☐ Low
                    if (text.StartsWith("☐", StringComparison.Ordinal))
                    {
                        var f = currentFieldByCol[col];
                        if (f != null)
                        {
                            var opt = text.Replace("☐", "").Trim();
                            if (!string.IsNullOrWhiteSpace(opt))
                                f.options.Add(opt);
                        }
                        continue;
                    }

					if (text.StartsWith("○", StringComparison.Ordinal))
					{
						var f = currentFieldByCol[col];
						if (f != null)
						{
							var opt = text.Replace("○", "").Trim();
							if (!string.IsNullOrWhiteSpace(opt))
								f.options.Add(opt);
						}
						continue;
					}

					// Inline question with options: "Trying ... ☐ Yes ☐ No"
					if (text.Contains("☐", StringComparison.Ordinal))
                    {
                        var parts = text.Split('☐');
                        var label = parts[0].Trim();
                        var opts = parts.Skip(1).Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)).ToList();

                        if (!string.IsNullOrWhiteSpace(label) && opts.Count > 0)
                        {
                            // Excel uses "☐" for option markers. Per your requirement:
                            // - default: treat as SINGLE choice => radio
                            // - if question is multi => checkbox
                            var inputType = IsMultiSelectQuestion(label) ? "checkbox" : "radio";

                            var f = new HolisticHistoryFieldModel
                            {
                                fieldId = $"f{order:0000}",
                                label = label,
                                inputType = inputType,
                                options = opts,
                                group = currentGroupTitle
                            };
                            order++;
                            EnsureSection(currentSectionTitle).fields.Add(f);
                            currentFieldByCol[col] = f;
                            continue;
                        }
                    }

					if (text.Contains("○", StringComparison.Ordinal))
					{
						var parts = text.Split('○');
						var label = parts[0].Trim();
						var opts = parts.Skip(1).Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)).ToList();

						if (!string.IsNullOrWhiteSpace(label) && opts.Count > 0)
						{
							var lowerOpts = opts.Select(o => o.ToLowerInvariant()).ToList();
							bool isRadio = text.Contains("○", StringComparison.OrdinalIgnoreCase) ||
											(opts.Count == 2 && lowerOpts.Contains("yes") && lowerOpts.Contains("no"));

							var inputType = isRadio ? "radio" : "checkbox";

							var f = new HolisticHistoryFieldModel
							{
								fieldId = $"f{order:0000}",
								label = label,
								inputType = inputType,
								options = opts,
								group = currentGroupTitle
							};
							order++;
							EnsureSection(currentSectionTitle).fields.Add(f);
							currentFieldByCol[col] = f;
							continue;
						}
					}


					// Text input line with placeholders.
					bool looksLikeTextPlaceholder = text.Contains("___", StringComparison.Ordinal) ||
                                                     text.Contains("________", StringComparison.Ordinal) ||
                                                     underscoreRe.IsMatch(text);
                    if (looksLikeTextPlaceholder)
                    {
                        var f = new HolisticHistoryFieldModel
                        {
                            fieldId = $"f{order:0000}",
                            label = text,
                            inputType = "text",
                            options = new List<string>(),
                            group = currentGroupTitle
                        };
                        order++;
                        EnsureSection(currentSectionTitle).fields.Add(f);
                        currentFieldByCol[col] = null;
                        continue;
                    }

                    // Standalone Yes/No.
                    if (text.Contains("Y / N", StringComparison.OrdinalIgnoreCase))
                    {
                        var f = new HolisticHistoryFieldModel
                        {
                            fieldId = $"f{order:0000}",
                            label = text,
                            inputType = "radio",
                            options = new List<string> { "Yes", "No" },
                            group = currentGroupTitle
                        };
                        order++;
                        EnsureSection(currentSectionTitle).fields.Add(f);
                        currentFieldByCol[col] = null;
                        continue;
                    }

                    // Question prompts (option lines will follow in subsequent rows).
                    bool looksLikeQuestion = text.Contains("?") || Regex.IsMatch(text, @":\s*$");
                    if (looksLikeQuestion)
                    {
                        // Default: "☐" options are SINGLE choice => radio.
                        // Multi-select questions should be explicitly marked "tick all".
                        var inputType = IsMultiSelectQuestion(text) ? "checkbox" : "radio";
                        var f = new HolisticHistoryFieldModel
                        {
                            fieldId = $"f{order:0000}",
                            label = text,
                            inputType = inputType,
                            options = new List<string>(),
                            group = currentGroupTitle
                        };
                        order++;
                        EnsureSection(currentSectionTitle).fields.Add(f);
                        currentFieldByCol[col] = f;
                        continue;
                    }
                }
            }

            // Post-process: if a checkbox question ended up with no options, it is actually free text.
            foreach (var sec in sections)
            {
                foreach (var f in sec.fields)
                {
                    if (string.Equals(f.inputType, "checkbox", StringComparison.OrdinalIgnoreCase) && (f.options == null || f.options.Count == 0))
                    {
                        f.inputType = "text";
                    }

                    if (string.Equals(f.inputType, "checkbox", StringComparison.OrdinalIgnoreCase) &&
                        f.options != null && f.options.Count == 2)
                    {
                        var lower = new HashSet<string>(f.options.Select(o => (o ?? "").ToLowerInvariant()));
                        if (lower.SetEquals(new HashSet<string>(new[] { "yes", "no" })))
                            f.inputType = "radio";
                    }
                }
            }

            return sections;
        }

        private List<HolisticHistorySectionModel> GetSchema()
        {
            if (_schemaLoaded && _cachedSections != null) return _cachedSections;

            lock (_schemaLock)
            {
                if (_schemaLoaded && _cachedSections != null) return _cachedSections;
                _cachedSections = BuildSchema();
                _schemaLoaded = true;
                return _cachedSections;
            }
        }

        [HttpGet()]
        public virtual async Task<IActionResult> Add_Holistic_History(string IPDFormid)
        {
            var tenantIdStr = HttpContext.Session.GetString("NalamVazhachoosedtenantid");
            Guid? tenantId = null;
            if (!string.IsNullOrWhiteSpace(tenantIdStr) && Guid.TryParse(tenantIdStr, out var t))
                tenantId = t;

            ViewBag.IPDFormid = IPDFormid;
            var model = new HolisticHistoryModel
            {
                tenantid = tenantId,
                sections = GetSchema()
            };

            // Pre-fill saved answers (if any) for this IPDFormid.
            try
            {
                var apiUrl =
                    "api/HolisticHistory/HolisticHistory_ByIPDFormid?IPDFormid=" + Uri.EscapeDataString(IPDFormid ?? string.Empty) +
                    "&tenantid=" + Uri.EscapeDataString(tenantId.HasValue ? tenantId.Value.ToString() : string.Empty) +
                    "&loginUserID=" + Uri.EscapeDataString(HttpContext.Session.GetString("NalamVazhaloginUserID") ?? string.Empty);

                var apiResponse = await ApiClient.Get_ApiValues(getHttpClient(), apiUrl);
                if (!string.IsNullOrWhiteSpace(apiResponse) && apiResponse.Trim().StartsWith("{"))
                {
                    var parsed = JsonConvert.DeserializeObject<JObject>(apiResponse);
                    var detail = parsed?["detail"] as JArray;
                    if (detail != null && detail.Count > 0)
                    {
                        var first = detail[0];
                        var ans = first?["holisticanswers"];
                        if (ans != null && ans.Type != JTokenType.Null)
                            model.holisticanswers = ans.ToString(Formatting.None);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while pre-filling HolisticHistory");
            }

            return View("Add_Holistic_History", model);
        }

        [HttpPost()]
        public virtual async Task<IActionResult> Add_Holistic_History(HolisticHistoryModel model, IFormCollection collection, string IPDFormid)
        {
            try
            {
                if (model.tenantid == null)
                {
                    var tenantIdStr = HttpContext.Session.GetString("NalamVazhachoosedtenantid");
                    if (!string.IsNullOrWhiteSpace(tenantIdStr) && Guid.TryParse(tenantIdStr, out var t))
                        model.tenantid = t;
                }

                var schema = GetSchema();
                var fields = schema.SelectMany(s => s.fields ?? new List<HolisticHistoryFieldModel>()).ToList();

                var answers = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                foreach (var f in fields)
                {
                    if (string.IsNullOrWhiteSpace(f.fieldId)) continue;

                    var inputType = f.inputType ?? "text";
                    if (inputType == "checkbox" && f.options != null && f.options.Count > 0)
                    {
                        var values = collection[f.fieldId];
                        if (values.Count > 0)
                        {
                            answers[f.fieldId] = values.ToArray().ToList();
                        }
                    }
                    else if (inputType == "radio")
                    {
                        var val = collection[f.fieldId].ToString();
                        if (!string.IsNullOrWhiteSpace(val))
                            answers[f.fieldId] = val;
                    }
                    else // text (includes checkbox-with-no-options promoted to text)
                    {
                        var val = collection[f.fieldId].ToString();
                        if (!string.IsNullOrWhiteSpace(val))
                            answers[f.fieldId] = val;
                    }
                }

                var answersJson = System.Text.Json.JsonSerializer.Serialize(answers);

                var reqModel = new HolisticHistoryModel
                {
                    tenantid = model.tenantid,
                    holisticanswers = answersJson,
                    verifiedstatus = "Submitted"
                };

                string strReturnMessage = await ApiClient.Post_ApiValuesGetString(
                    getHttpClient(),
                    "api/HolisticHistory/Add_HolisticHistory?IPDFormid=" + Uri.EscapeDataString(IPDFormid ?? string.Empty),
                    reqModel
                );

                ViewBag.IPDFormid = IPDFormid;
                model.sections = schema;
                ViewData["message"] = strReturnMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while submitting HolisticHistory");
                model.sections = GetSchema();
                ViewData["message"] = ex.Message;
            }

            return View("Add_Holistic_History", model);
        }
    }
}

