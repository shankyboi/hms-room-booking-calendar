namespace NalamVazhaWebApi.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using NalamVazha.DAL;
    using NalamVazha.Models;
    using Newtonsoft.Json.Linq;
    using System.Data;

    [Route("api/[controller]/[action]")]
    public class HolisticHistoryController : BaseController
    {
        private readonly IOptions<ConnectionSettings> _connectionSettings;
        private readonly ILogger<HolisticHistoryController> _logger;
        private readonly IWebHostEnvironment _hostingEnv;

        public HolisticHistoryController(IHttpContextAccessor httpContextAccessor, IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
            : base()
        {
            _connectionSettings = connectionSettings;
            _logger = loggerFactory.CreateLogger<HolisticHistoryController>();
            _hostingEnv = hostingEnvironment;
        }

        private HolisticHistoryDAL CreateDal()
        {
            return new HolisticHistoryDAL(_connectionSettings.Value.ConnectionString);
        }

        private External_System_DAL CreateExternalDal()
        {
            return new External_System_DAL(_connectionSettings.Value.ConnectionString);
        }

        [HttpPost()]
        [ActionName("Add_HolisticHistory")]
        public virtual IActionResult Add_HolisticHistory([FromBody] HolisticHistoryModel model, string IPDFormid)
        {
            string message = "";

            try
            {
                if (model == null)
                    return BadRequest("Model is required");

                if (string.IsNullOrEmpty(model.verifiedstatus))
                    model.verifiedstatus = "Submitted";

                var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length);
                    string usersid = CreateExternalDal().get_users_info_by_token(token);
                    if (!string.IsNullOrEmpty(usersid))
                        model.createduser = new Guid(usersid);
                }

                var dal = CreateDal();
                message = dal.Add_HolisticHistory(model, IPDFormid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred in Add_HolisticHistory");
                message = ex.Message;
            }

            if (message.Replace("\"", "").Contains("201.1"))
                return Ok(message);
            else if (message.Replace("\"", "") == "401.1")
                return Unauthorized(message);
            else
                return BadRequest(message);
        }

        [HttpGet()]
        [ActionName("HolisticHistory_ByIPDFormid")]
        public virtual JObject HolisticHistory_ByIPDFormid(string IPDFormid, string loginUserID = "")
        {
            // Optional endpoint for pre-fill; uses List by IPDFormid.
            // If tenantid is not provided by caller, caller still needs to pass it in token-driven way from existing UI flows.
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
                string usersid = "";
                if (authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length);
                    usersid = CreateExternalDal().get_users_info_by_token(token);
                }

                string tenantid = Request.Query["tenantid"].ToString();
                // tenantid in this codebase is usually sent as the GUID chosen for the current session.
                // If it is not provided, list function will fall back to tenant-wide results depending on SQL implementation.

                string tenantParam = string.IsNullOrEmpty(tenantid) ? (usersid + "|") : (usersid + "|" + tenantid);
                return CreateDal().HolisticHistory_List(tenantParam, IPDFormid, 1000, 0, "", "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred in HolisticHistory_ByIPDFormid");
                return new JObject();
            }
        }
    }
}

