namespace Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NalamVazha.Models;

    [AllowAnonymous]
    public class IssueTrackerController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger        _logger;

        public IssueTrackerController(
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger        = loggerFactory.CreateLogger<IssueTrackerController>();
        }

        // ── GET  /IssueTracker/Index ──────────────────────────────────────────
        public IActionResult Index()
        {
            ViewBag.ShowReportToDeveloper = bool.Parse(
                _configuration["IssueTrackerSettings:ShowReportToDeveloper"] ?? "false");
            ViewBag.ApiUrl = _configuration["ApiSettings:apiURL"]?.TrimEnd('/');
            return View();
        }

        // ── GET  /IssueTracker/GetIssues ──────────────────────────────────────
        // Same-origin proxy for the issues table – avoids direct browser → API call.
        [HttpGet]
        public async Task<IActionResult> GetIssues()
        {
            var apiBase = _configuration["ApiSettings:apiURL"]?.TrimEnd('/');
            if (string.IsNullOrEmpty(apiBase))
                return StatusCode(500, "API base URL not configured.");
            try
            {
                using var client   = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
                var response       = await client.GetAsync($"{apiBase}/api/IssueTracker/Get_Issues");
                var json           = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "IssueTracker – GetIssues proxy failed");
                return StatusCode(500, "Could not reach the API.");
            }
        }

        // ── POST /IssueTracker/DeleteIssue ─────────────────────────────────────
        // Same-origin proxy for the delete button in the issues table.
        public class DeleteIssueRequest { public string issuetrackerid { get; set; } }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteIssue([FromBody] DeleteIssueRequest dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.issuetrackerid))
                return BadRequest("Issue ID is required.");

            if (!Guid.TryParse(dto.issuetrackerid, out var guid))
                return BadRequest("Invalid Issue ID format.");

            var model  = new IssueTrackerModel { Issuetrackerid = guid };
            var result = await PostToApiAsync("api/IssueTracker/Delete_Issue", model);
            if (result == null)
                return StatusCode(500, "Could not reach the API. Please try again.");

            return Ok("success");
        }

        // ── GET  /IssueTracker/GetEntities ────────────────────────────────────
        // Always reads distinct controllername values from roleauthorization (via WebApi).
        [HttpGet]
        public async Task<IActionResult> GetEntities()
        {
            var entities = await FetchFromApiAsync<List<string>>("api/IssueTracker/Get_Entities_DB")
                           ?? new List<string>();
            return Json(entities);
        }

        // ── GET  /IssueTracker/GetFiles?entityname=Assessment ─────────────────
        // Always reads distinct actionmethodname values for the controller from
        // roleauthorization (via WebApi).
        [HttpGet]
        public async Task<IActionResult> GetFiles(string entityname)
        {
            if (string.IsNullOrWhiteSpace(entityname))
                return Json(new List<string>());

            var files = await FetchFromApiAsync<List<string>>(
                            $"api/IssueTracker/Get_Files_DB?entityname={Uri.EscapeDataString(entityname)}")
                        ?? new List<string>();
            return Json(files);
        }

        // ── POST /IssueTracker/ReportIssue ────────────────────────────────────
        // Same-origin proxy so the modal in _Layout can submit without CORS.
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ReportIssue([FromBody] IssueTrackerModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.issuetext))
                return BadRequest("Issue text is required.");

            var result = await PostToApiAsync("api/IssueTracker/Add_Issue", model);
            if (result == null)
                return StatusCode(500, "Could not reach the API. Please try again.");

            return Ok("success");
        }

        // ── POST /IssueTracker/UpdateStatus ───────────────────────────────────
        // ── DTO matching the exact JSON shape the browser sends ──────────────
        public class UpdateStatusRequest
        {
            public string issuetrackerid { get; set; }   // lowercase – matches JS payload
            public string status         { get; set; }
        }

        // Same-origin proxy for inline status change in the issues table.
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.issuetrackerid))
                return BadRequest("Issue ID is required.");

            if (!Guid.TryParse(dto.issuetrackerid, out var guid))
                return BadRequest("Invalid Issue ID format.");

            // Build a typed model to forward to the WebApi
            var model = new IssueTrackerModel
            {
                Issuetrackerid = guid,
                status         = dto.status ?? "Open"
            };

            var result = await PostToApiAsync("api/IssueTracker/Update_IssueStatus", model);
            if (result == null)
                return StatusCode(500, "Could not reach the API. Please try again.");

            return Ok("success");
        }

        // ── GET /IssueTracker/GetOpenCount ────────────────────────────────────
        // Returns { count: N } – used by the nav bar bug badge on every page.
        [HttpGet]
        public async Task<IActionResult> GetOpenCount()
        {
            try
            {
                var issues = await FetchFromApiAsync<List<System.Text.Json.JsonElement>>("api/IssueTracker/Get_Issues");
                if (issues == null) return Json(new { count = 0 });

                int open = 0;
                foreach (var item in issues)
                {
                    if (item.TryGetProperty("status", out var s))
                    {
                        var st = s.GetString();
                        if (st == "Open" || st == "Reopen" || st == "In Progress") open++;
                    }
                }
                return Json(new { count = open });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "IssueTracker – GetOpenCount failed");
                return Json(new { count = 0 });
            }
        }

        // ── GET /IssueTracker/GetScreenshot?id=<guid> ─────────────────────────
        // Same-origin proxy – avoids direct browser → API call (CORS / auth).
        [HttpGet]
        public async Task<IActionResult> GetScreenshot(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
                return BadRequest("Invalid issue ID.");

            var apiBase = _configuration["ApiSettings:apiURL"]?.TrimEnd('/');
            if (string.IsNullOrEmpty(apiBase))
                return StatusCode(500, "API base URL not configured.");

            try
            {
                using var client   = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
                var response       = await client.GetAsync($"{apiBase}/api/IssueTracker/Get_Screenshot?id={Uri.EscapeDataString(id)}");

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Screenshot not found.");

                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "IssueTracker – GetScreenshot proxy failed for id {Id}", id);
                return StatusCode(500, "Could not reach the API.");
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        // GET: calls a WebApi endpoint and deserialises the JSON response.
        private async Task<T> FetchFromApiAsync<T>(string relativeUrl) where T : class
        {
            var apiBase = _configuration["ApiSettings:apiURL"]?.TrimEnd('/');
            if (string.IsNullOrEmpty(apiBase)) return null;

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var response = await client.GetAsync($"{apiBase}/{relativeUrl}");
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "IssueTracker GET API call failed: {Url}", relativeUrl);
                return null;
            }
        }

        // POST: serialises payload and sends to WebApi, returns response body.
        private async Task<string> PostToApiAsync(string relativeUrl, object payload)
        {
            var apiBase = _configuration["ApiSettings:apiURL"]?.TrimEnd('/');
            if (string.IsNullOrEmpty(apiBase)) return null;

            try
            {
                using var client  = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
                var json          = JsonSerializer.Serialize(payload);
                var content       = new StringContent(json, Encoding.UTF8, "application/json");
                var response      = await client.PostAsync($"{apiBase}/{relativeUrl}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("IssueTracker POST API returned {Code}: {Body}",
                        (int)response.StatusCode, body);
                    return null;
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "IssueTracker POST API call failed: {Url}", relativeUrl);
                return null;
            }
        }
    }
}
