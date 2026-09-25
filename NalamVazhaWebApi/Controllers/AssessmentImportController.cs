namespace NalamVazhaWebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NalamVazha.DAL;
    using NalamVazha.Models;
    using Npgsql;
    using NpgsqlTypes;

    [Route("api/[controller]/[action]")]
    public class AssessmentImportController : BaseController
    {
        private readonly IOptions<ConnectionSettings> _connectionSettings;
        private readonly ILogger                      _logger;
        private readonly External_System_DAL          _extDAL;
        private Guid _createdUser = Guid.Empty;

        public AssessmentImportController(
            IHttpContextAccessor            httpContextAccessor,
            IOptions<ConnectionSettings>    connectionSettings,
            ILoggerFactory                  loggerFactory,
            IConfiguration                  configuration,
            IWebHostEnvironment             hostingEnvironment)
        {
            _connectionSettings = connectionSettings;
            _logger  = loggerFactory.CreateLogger<AssessmentImportController>();
            _extDAL  = new External_System_DAL(_connectionSettings.Value.ConnectionString);

            var authHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token  = authHeader.Substring("Bearer ".Length);
                var userId = _extDAL.get_users_info_by_token(token);
                if (Guid.TryParse(userId, out var uid) && uid != Guid.Empty)
                    _createdUser = uid;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST api/AssessmentImport/Import_Assessment
        // Receives a pre-parsed payload from the Admin frontend and inserts all
        // records inside a single PostgreSQL transaction.
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ActionName("Import_Assessment")]
        public IActionResult Import_Assessment([FromBody] AssessmentImportPayload payload)
        {
            var result = new AssessmentImportResult();

            if (payload == null || payload.questions == null || payload.questions.Count == 0)
            {
                result.success = false;
                result.message = "No questions found in import payload.";
                return Ok(result);
            }

            Guid? tenantId = null;
            if (!string.IsNullOrWhiteSpace(payload.tenantid) && Guid.TryParse(payload.tenantid, out var tid))
                tenantId = tid;

            if (_createdUser == Guid.Empty)
            {
                result.success = false;
                result.message = "Authorization failed — could not resolve user from token.";
                return Ok(result);
            }

            var today = DateTime.Now.ToString("yyyyMMdd");

            try
            {
                using var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString);
                conn.Open();
                using var tx = conn.BeginTransaction();

                try
                {
                    // ── STEP 1: Upsert QuestionCategory ──────────────────────────────────
                    var catIdMap = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

                    using (var cmd = new NpgsqlCommand(
                        "SELECT * FROM \"AssessmentImport_Get_QuestionCategories\"(@tid)", conn, tx))
                    {
                        cmd.Parameters.Add("tid", NpgsqlDbType.Uuid).Value = (object)tenantId ?? DBNull.Value;
                        using var rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                            catIdMap[rdr.GetString(1)] = Guid.Parse(rdr.GetString(0));
                    }

                    var distinctCats = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var q in payload.questions)
                        if (!string.IsNullOrWhiteSpace(q.categoryname))
                            distinctCats.Add(q.categoryname.Trim());

                    foreach (var catName in distinctCats)
                    {
                        if (catIdMap.ContainsKey(catName)) continue;

                        var newId = Guid.NewGuid();
                        using (var cmd = new NpgsqlCommand(
                            "SELECT \"AssessmentImport_Insert_QuestionCategory\"(@id,@name,@tid,@cu)", conn, tx))
                        {
                            cmd.Parameters.Add("id",   NpgsqlDbType.Uuid).Value    = newId;
                            cmd.Parameters.Add("name", NpgsqlDbType.Varchar).Value = catName;
                            cmd.Parameters.Add("tid",  NpgsqlDbType.Uuid).Value    = (object)tenantId ?? DBNull.Value;
                            cmd.Parameters.Add("cu",   NpgsqlDbType.Uuid).Value    = _createdUser;
                            cmd.ExecuteNonQuery();
                        }
                        catIdMap[catName] = newId;
                        result.categoriesAdded++;
                    }

                    // ── STEP 2: Upsert QuestionSubCategory ───────────────────────────────
                    // Key: (categoryUUID, subCategoryName) → subCategoryUUID
                    var subIdMap = new Dictionary<(Guid catId, string subName), Guid>();

                    using (var cmd = new NpgsqlCommand(
                        "SELECT * FROM \"AssessmentImport_Get_QuestionSubCategories\"(@tid)", conn, tx))
                    {
                        cmd.Parameters.Add("tid", NpgsqlDbType.Uuid).Value = (object)tenantId ?? DBNull.Value;
                        using var rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            var subId = Guid.Parse(rdr.GetString(0));
                            var catId = Guid.Parse(rdr.GetString(1));
                            subIdMap[(catId, rdr.GetString(2))] = subId;
                        }
                    }

                    var distinctSubs = new HashSet<(string cat, string sub)>();
                    foreach (var q in payload.questions)
                        if (!string.IsNullOrWhiteSpace(q.categoryname) && !string.IsNullOrWhiteSpace(q.subcategoryname))
                            distinctSubs.Add((q.categoryname.Trim(), q.subcategoryname.Trim()));

                    foreach (var (catName, subName) in distinctSubs)
                    {
                        if (!catIdMap.TryGetValue(catName, out var catId)) continue;
                        if (subIdMap.ContainsKey((catId, subName))) continue;

                        var newId = Guid.NewGuid();
                        using (var cmd = new NpgsqlCommand(
                            "SELECT \"AssessmentImport_Insert_QuestionSubCategory\"(@id,@catid,@name,@tid,@cu)", conn, tx))
                        {
                            cmd.Parameters.Add("id",    NpgsqlDbType.Uuid).Value    = newId;
                            cmd.Parameters.Add("catid", NpgsqlDbType.Uuid).Value    = catId;
                            cmd.Parameters.Add("name",  NpgsqlDbType.Varchar).Value = subName;
                            cmd.Parameters.Add("tid",   NpgsqlDbType.Uuid).Value    = (object)tenantId ?? DBNull.Value;
                            cmd.Parameters.Add("cu",    NpgsqlDbType.Uuid).Value    = _createdUser;
                            cmd.ExecuteNonQuery();
                        }
                        subIdMap[(catId, subName)] = newId;
                        result.subcategoriesAdded++;
                    }

                    // ── STEP 3: Determine reference number starting point ─────────────────
                    int refCounter;
                    using (var cmd = new NpgsqlCommand(
                        "SELECT \"AssessmentImport_Get_AssessmentQuestion_Ref_Max\"(@prefix)", conn, tx))
                    {
                        cmd.Parameters.Add("prefix", NpgsqlDbType.Varchar).Value = today + "-%";
                        var scalar = cmd.ExecuteScalar();
                        refCounter = (scalar != null && scalar != DBNull.Value) ? Convert.ToInt32(scalar) : 0;
                    }

                    // ── STEP 4: Insert AssessmentQuestion ────────────────────────────────
                    var insertedQuestions = new List<(Guid id, Guid catId, Guid subId, int order)>();

                    for (int i = 0; i < payload.questions.Count; i++)
                    {
                        var q = payload.questions[i];

                        if (string.IsNullOrWhiteSpace(q.questiontext))
                        { result.questionsSkipped++; continue; }

                        if (!catIdMap.TryGetValue(q.categoryname ?? "", out var qCatId))
                        { result.questionsSkipped++; result.warnings.Add($"Row {i + 1}: category '{q.categoryname}' could not be resolved."); continue; }

                        if (!subIdMap.TryGetValue((qCatId, q.subcategoryname ?? ""), out var qSubId))
                        { result.questionsSkipped++; result.warnings.Add($"Row {i + 1}: subcategory '{q.subcategoryname}' under '{q.categoryname}' could not be resolved."); continue; }

                        // Skip exact duplicates (same question text + category + subcategory + tenantid)
                        bool exists;
                        using (var chk = new NpgsqlCommand(
                            "SELECT \"AssessmentImport_AssessmentQuestion_Exists\"(@qt,@cat,@sub,@tid)", conn, tx))
                        {
                            chk.Parameters.Add("qt",  NpgsqlDbType.Varchar).Value = q.questiontext;
                            chk.Parameters.Add("cat", NpgsqlDbType.Uuid).Value    = qCatId;
                            chk.Parameters.Add("sub", NpgsqlDbType.Uuid).Value    = qSubId;
                            chk.Parameters.Add("tid", NpgsqlDbType.Uuid).Value    = (object)tenantId ?? DBNull.Value;
                            exists = Convert.ToInt32(chk.ExecuteScalar()) > 0;
                        }
                        if (exists) { result.questionsSkipped++; continue; }

                        refCounter++;
                        var refNum  = $"{today}-{refCounter:D5}";
                        var newQId  = Guid.NewGuid();

                        using (var ins = new NpgsqlCommand(
							"SELECT \"AssessmentImport_Insert_AssessmentQuestion\"(@id,@tid,@refnum,@cat,@sub,@qt,@at,@ov,@srmin,@srmax,@req,@cu)", conn, tx))
                        {
                            ins.Parameters.Add("id",     NpgsqlDbType.Uuid).Value    = newQId;
                            ins.Parameters.Add("tid",    NpgsqlDbType.Uuid).Value    = (object)tenantId ?? DBNull.Value;
                            ins.Parameters.Add("refnum", NpgsqlDbType.Varchar).Value = refNum;
                            ins.Parameters.Add("cat",    NpgsqlDbType.Uuid).Value    = qCatId;
                            ins.Parameters.Add("sub",    NpgsqlDbType.Uuid).Value    = qSubId;
                            ins.Parameters.Add("qt",     NpgsqlDbType.Varchar).Value = q.questiontext;
                            ins.Parameters.Add("at",     NpgsqlDbType.Varchar).Value = q.answertype ?? "Text";
                            ins.Parameters.Add("ov",     NpgsqlDbType.Varchar).Value = (object)q.optionvalue ?? DBNull.Value;
                            ins.Parameters.Add("srmin",  NpgsqlDbType.Integer).Value = (object)q.scalerangemin ?? DBNull.Value;
                            ins.Parameters.Add("srmax",  NpgsqlDbType.Integer).Value = (object)q.scalerangemax ?? DBNull.Value;
                            ins.Parameters.Add("req",    NpgsqlDbType.Boolean).Value = q.isrequired;
                            ins.Parameters.Add("cu",     NpgsqlDbType.Uuid).Value    = _createdUser;
                            ins.ExecuteNonQuery();
                        }

                        insertedQuestions.Add((newQId, qCatId, qSubId, i + 1));
                        result.questionsAdded++;
                    }

                    // ── STEP 5: AssessmentTemplate + templatequestions (optional) ─────────
                    if (!string.IsNullOrWhiteSpace(payload.templatename))
                    {
                        Guid templateId;
                        bool templateExisted;

                        using (var chk = new NpgsqlCommand(
                            "SELECT * FROM \"AssessmentImport_Get_AssessmentTemplate\"(@tn,@tid)", conn, tx))
                        {
                            chk.Parameters.Add("tn",  NpgsqlDbType.Varchar).Value = payload.templatename;
                            chk.Parameters.Add("tid", NpgsqlDbType.Uuid).Value    = (object)tenantId ?? DBNull.Value;
                            var existing = chk.ExecuteScalar();
                            templateExisted = existing != null && existing != DBNull.Value;
                            templateId = templateExisted ? Guid.Parse(existing.ToString()) : Guid.NewGuid();
                        }

                        if (!templateExisted)
                        {
                            using var tCmd = new NpgsqlCommand(
                                "SELECT \"AssessmentImport_Insert_AssessmentTemplate\"(@id,@tn,@tid,@cu)", conn, tx);
                            tCmd.Parameters.Add("id",  NpgsqlDbType.Uuid).Value    = templateId;
                            tCmd.Parameters.Add("tn",  NpgsqlDbType.Varchar).Value = payload.templatename;
                            tCmd.Parameters.Add("tid", NpgsqlDbType.Uuid).Value    = (object)tenantId ?? DBNull.Value;
                            tCmd.Parameters.Add("cu",  NpgsqlDbType.Uuid).Value    = _createdUser;
                            tCmd.ExecuteNonQuery();
                            result.templateCreated = true;
                            result.templatename    = payload.templatename;
                        }
                        else
                        {
                            result.warnings.Add($"Template '{payload.templatename}' already exists — newly imported questions will be linked to it.");
                            result.templatename = payload.templatename;
                        }

                        // Link every newly inserted question to the template
                        for (int i = 0; i < insertedQuestions.Count; i++)
                        {
                            var (qId, catId, subId, order) = insertedQuestions[i];

                            // Copy metadata from AssessmentQuestion to keep template in sync
                            using var tqCmd = new NpgsqlCommand(
                                "SELECT \"AssessmentImport_Insert_TemplateQuestion\"(@tqid,@tmplid,@ord,@qid)", conn, tx);
                            tqCmd.Parameters.Add("tqid",   NpgsqlDbType.Uuid).Value    = Guid.NewGuid();
                            tqCmd.Parameters.Add("tmplid", NpgsqlDbType.Uuid).Value    = templateId;
                            tqCmd.Parameters.Add("ord",    NpgsqlDbType.Integer).Value = order;
                            tqCmd.Parameters.Add("qid",    NpgsqlDbType.Uuid).Value    = qId;
                            tqCmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();

                    result.success = true;
                    result.message = BuildSummaryMessage(result);
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    _logger.LogError(ex, "AssessmentImport transaction failed");
                    result.success = false;
                    result.message = "Import failed (transaction rolled back): " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AssessmentImport connection error");
                result.success = false;
                result.message = "Import error: " + ex.Message;
            }

            return Ok(result);
        }

        private static string BuildSummaryMessage(AssessmentImportResult r)
        {
            var parts = new List<string>();
            if (r.categoriesAdded    > 0) parts.Add($"{r.categoriesAdded} categor{(r.categoriesAdded == 1 ? "y" : "ies")} added");
            if (r.subcategoriesAdded > 0) parts.Add($"{r.subcategoriesAdded} sub-categories added");
            if (r.questionsAdded     > 0) parts.Add($"{r.questionsAdded} questions imported");
            if (r.questionsSkipped   > 0) parts.Add($"{r.questionsSkipped} questions skipped (duplicates)");
            if (r.templateCreated)        parts.Add($"template '{r.templatename}' created");
            else if (!string.IsNullOrEmpty(r.templatename)) parts.Add($"questions linked to existing template '{r.templatename}'");
            return parts.Count > 0 ? string.Join(", ", parts) + "." : "Nothing to import.";
        }
    }
}
