namespace NalamVazhaWebApi.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using NalamVazha.Models;
    using NalamVazha.DAL;

    [Route("api/[controller]/[action]")]
    [AllowAnonymous]                         // No JWT required – anonymous access allowed
    public class IssueTrackerController : Controller
    {
        private readonly IOptions<ConnectionSettings> _connectionSettings;
        private readonly IConfiguration               _configuration;
        private readonly ILogger                      _logger;
        private readonly IssueTrackerDAL              _dal;

        public IssueTrackerController(
            IOptions<ConnectionSettings> connectionSettings,
            ILoggerFactory               loggerFactory,
            IConfiguration               configuration)
        {
            _connectionSettings = connectionSettings;
            _configuration      = configuration;
            _logger             = loggerFactory.CreateLogger<IssueTrackerController>();
            _dal                = new IssueTrackerDAL(_connectionSettings.Value.ConnectionString);
        }

        // ── POST  api/IssueTracker/Add_Issue ─────────────────────────────────
        [HttpPost]
        public IActionResult Add_Issue([FromBody] IssueTrackerModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.issuetext))
                    return BadRequest("Issue text is required.");

                var result = _dal.Add_Issue(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IssueTracker – Add_Issue failed");
                return StatusCode(500, ex.Message);
            }
        }

        // ── GET   api/IssueTracker/Get_Issues ────────────────────────────────
        [HttpGet]
        public IActionResult Get_Issues()
        {
            try
            {
                var result = _dal.Get_Issues();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IssueTracker – Get_Issues failed");
                return StatusCode(500, ex.Message);
            }
        }

        // ── POST  api/IssueTracker/Update_IssueStatus ────────────────────────
        [HttpPost]
        public IActionResult Update_IssueStatus([FromBody] IssueTrackerModel model)
        {
            try
            {
                if (model?.Issuetrackerid == null)
                    return BadRequest("Issue ID is required.");

                var result = _dal.Update_IssueStatus(model.Issuetrackerid.Value, model.status ?? "Open");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IssueTracker – Update_IssueStatus failed");
                return StatusCode(500, ex.Message);
            }
        }

        // ── POST  api/IssueTracker/Delete_Issue ──────────────────────────────
        [HttpPost]
        public IActionResult Delete_Issue([FromBody] IssueTrackerModel model)
        {
            try
            {
                if (model?.Issuetrackerid == null)
                    return BadRequest("Issue ID is required.");

                var result = _dal.Delete_Issue(model.Issuetrackerid.Value);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IssueTracker – Delete_Issue failed");
                return StatusCode(500, ex.Message);
            }
        }

        // ── GET   api/IssueTracker/Get_Screenshot?id=<guid> ──────────────────
        // Returns only the base64 screenshot blob for a single issue.
        // The list endpoint (Get_Issues) deliberately omits the blob to keep
        // payload sizes small; the front-end calls this on demand.
        [HttpGet]
        public IActionResult Get_Screenshot(Guid id)
        {
            try
            {
                var data = _dal.Get_Screenshot(id);
                if (data == null) return NotFound("No screenshot attached.");
                return Ok(new { screenshotdata = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IssueTracker – Get_Screenshot failed");
                return StatusCode(500, ex.Message);
            }
        }

        // ── GET   api/IssueTracker/Get_Entities_DB ───────────────────────────
        // Returns distinct controllername values from roleauthorization.
        // Used as fallback when the Admin/Views folder is not accessible
        // (e.g. DLL deployment to a different server).
        [HttpGet]
        public IActionResult Get_Entities_DB()
        {
            try
            {
                var result = _dal.GetEntitiesFromDB();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IssueTracker – Get_Entities_DB failed");
                return StatusCode(500, ex.Message);
            }
        }

        // ── GET   api/IssueTracker/Get_Files_DB?entityname=Assessment ────────
        // Returns distinct actionmethodname values for the given controller
        // from roleauthorization.
        [HttpGet]
        public IActionResult Get_Files_DB(string entityname)
        {
            try
            {
                var result = _dal.GetFilesFromDB(entityname ?? "");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IssueTracker – Get_Files_DB failed");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
