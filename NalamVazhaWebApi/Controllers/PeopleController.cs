namespace NalamVazhaWebApi.Controllers
{
	using System;
	using System.Data;
	using System.Linq;
	using Newtonsoft.Json.Linq;
	using Microsoft.AspNetCore.Mvc;
	using System.Collections.Generic;
	using Microsoft.Extensions.Options;
	using Microsoft.Extensions.Logging;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.Extensions.Configuration;
	using System.IdentityModel.Tokens.Jwt;
	using System.Security.Claims;
	using System.Text;
	using Microsoft.IdentityModel.Tokens;
	using NalamVazha.Models;
	using NalamVazha.DAL;
	using FluentValidation.Results;

	using Microsoft.AspNetCore.Hosting;
	using System.IO;
	using System.Net.Http.Headers;
	using Microsoft.AspNetCore.Http;
	using Newtonsoft.Json;
	using System.Threading.Tasks;
	[Route("api/[controller]/[action]")]
	//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 11:34:28
	public class PeopleController : BaseController
	{
				    public PeopleController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
		{
			_configuration = configuration;
			_logger = loggerFactory.CreateLogger<PeopleController>();
			_connectionSettings = connectionSettings;
			objPeopleDAL = new PeopleDAL(_connectionSettings.Value.ConnectionString);
                         obj_External_System_DAL =new External_System_DAL(_connectionSettings.Value.ConnectionString);
			objExternalSystemUtitlity = new ExternalSystemUtility(_connectionSettings, _configuration);
			hostingEnv = hostingEnvironment;

			var authHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

			if (authHeader.StartsWith("Bearer "))
			{
				var token = authHeader.Substring("Bearer ".Length);
				string usersid = obj_External_System_DAL.get_users_info_by_token(token);
                                   string tenantid = string.IsNullOrEmpty(httpContextAccessor.HttpContext.Request.Query["tenantid"]) ? Guid.Empty.ToString(): httpContextAccessor.HttpContext.Request.Query["tenantid"].ToString();
				pvar_tenantid = usersid + "|" + tenantid;//usersid+tenantid
                                   pvar_usersid=usersid;

			}
		}
		private PeopleDAL objPeopleDAL;
		private External_System_DAL obj_External_System_DAL;
		private IOptions<ConnectionSettings> _connectionSettings;
		private ILogger _logger;
		private IConfiguration _configuration;
		private IWebHostEnvironment hostingEnv;
		private ExternalSystemUtility objExternalSystemUtitlity;
		private string pvar_tenantid = "|";
		private string pvar_usersid = "";

		private string ValidateEmergencyContactPhoneNumbers(PeopleModel model, string Peopleid = "")
		{
			if (model.emergencycontact == null)
			{
				return "";
			}

			var phoneNumbers = model.emergencycontact
				.Where(x => !string.IsNullOrWhiteSpace(x.phonenumber))
				.Select(x => x.phonenumber.Trim())
				.ToList();

			if (phoneNumbers.GroupBy(x => x).Any(x => x.Count() > 1))
			{
				return "Duplicate Emergency Contact Phone Number is not allowed.";
			}

			foreach (var phonenumber in phoneNumbers)
			{
				if (objPeopleDAL.CheckEmergencyContactPhoneExists(phonenumber, Peopleid))
				{
					return "Emergency Contact Phone Number already exists.";
				}
			}

			return "";
		}

		[HttpGet()]
		[ActionName("getById_emergencycontact")]
		public virtual System.Data.DataTable getById_emergencycontact(string Peopleid)
		{
			DataTable dtPeople_emergencycontact = new DataTable();
			try
			{
				dtPeople_emergencycontact = objPeopleDAL.getById_emergencycontact(Peopleid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - getById_emergencycontact, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople_emergencycontact;


		}

		[HttpGet()]
		[ActionName("getById_educationinfo")]
		public virtual System.Data.DataTable getById_educationinfo(string Peopleid)
		{
			DataTable dtPeople_educationinfo = new DataTable();
			try
			{
				dtPeople_educationinfo = objPeopleDAL.getById_educationinfo(Peopleid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - getById_educationinfo, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople_educationinfo;


		}

		[HttpGet()]
		[ActionName("getById_workexperience")]
		public virtual System.Data.DataTable getById_workexperience(string Peopleid)
		{
			DataTable dtPeople_workexperience = new DataTable();
			try
			{
				dtPeople_workexperience = objPeopleDAL.getById_workexperience(Peopleid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - getById_workexperience, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople_workexperience;


		}

		[HttpGet()]
		[ActionName("getById_preferredlanguageinfo")]
		public virtual System.Data.DataTable getById_preferredlanguageinfo(string Peopleid)
		{
			DataTable dtPeople_preferredlanguageinfo = new DataTable();
			try
			{
				dtPeople_preferredlanguageinfo = objPeopleDAL.getById_preferredlanguageinfo(Peopleid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - getById_preferredlanguageinfo, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople_preferredlanguageinfo;


		}

		[HttpGet()]
		[ActionName("getById_clinicaltaskinfo")]
		public virtual System.Data.DataTable getById_clinicaltaskinfo(string Peopleid)
		{
			DataTable dtPeople_clinicaltaskinfo = new DataTable();
			try
			{
				dtPeople_clinicaltaskinfo = objPeopleDAL.getById_clinicaltaskinfo(Peopleid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - getById_clinicaltaskinfo, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople_clinicaltaskinfo;


		}
		[HttpGet()]
		[ActionName("GetDuration_ByPractitioner")]
		public virtual System.Data.DataTable GetDuration_ByPractitioner(string Peopleid, string tenantid)
		{
			DataTable dtResult = new DataTable();
			try
			{
				dtResult = objPeopleDAL.GetDuration_ByPractitioner(Peopleid, tenantid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - GetDuration_ByPractitioner, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
			}
			return dtResult;
		}

		[HttpGet()]
		[ActionName("prefill_People_clinicaltaskinfo")]
		public virtual System.Data.DataTable prefill_People_clinicaltaskinfo(string workprofile, string competencylevel)
		{
			DataTable dtPrefill = new DataTable();
			try
			{
				dtPrefill = objPeopleDAL.prefill_People_clinicaltaskinfo(workprofile, competencylevel);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - getById_prefill_People_clinicaltaskinfo, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPrefill;


		}

		[HttpGet()]
		[ActionName("CheckEmailExists")]
		public virtual IActionResult CheckEmailExists(string emailid, string tenantid)
		{
			try
			{
				bool exists = objPeopleDAL.CheckEmailExists(emailid, tenantid);
				return Ok(new { exists });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - CheckEmailExists, Error Message : " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpPost()]
		[ActionName("Add_People")]
		public virtual IActionResult Add_People([FromBody] PeopleModel model)
		{
			string message = "";
			var isDraft = string.Equals(model.status, "Draft", StringComparison.OrdinalIgnoreCase);
			if (isDraft) ModelState.Clear();

			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "Add_People";
			try
			{
				if (!model.tenantid.HasValue || model.tenantid.Value == Guid.Empty)
					return BadRequest("Validation Error : Unable to determine the Healthcare Provider (tenant) for this record. Please choose a Healthcare Provider and try again.");

				if (ModelState.IsValid)
				{

					PeopleModelValidator validator = new PeopleModelValidator();
					ValidationResult results = isDraft ? new ValidationResult() : validator.Validate(model);
					if (!results.IsValid)
					{
						var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}", e.PropertyName)));
						message = ("Validation Error : " + errorCollection);


					}
					else
					{
						message = isDraft ? "" : ValidateEmergencyContactPhoneNumbers(model);
						if (!string.IsNullOrWhiteSpace(message))
						{
							message = "Validation Error : " + message;
						}
						else
						{

						var authHeader = HttpContext.Request.Headers["Authorization"][0];
						if (authHeader.StartsWith("Bearer "))
						{


							var token = authHeader.Substring("Bearer ".Length);
							String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
							model.createduser = new Guid(userdetails[0].ToString());
							obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());






							message = objPeopleDAL.Add_People(model);
						}
						else
						{
							message = "Invalid Token";
						}
						}

					}


				}
				else
				{
					var errorCollection = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
					message = errorCollection.ToString();

					_logger.LogError("PeopleModel - Add_People , Validation Error :" + message);
					message = ("Validation Error : " + message);
				}






			}
			catch (Exception ex)
			{
				message = ex.Message;
				_logger.LogError(ex, "An exception occurred in - Add_People, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			if (obj_access_logsdetailsModel.access_logsid != null)
			{
				obj_access_logsdetailsModel.api_response = message.Replace("\"", "");
				obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
			}

			if (message.Replace("\"", "").Contains("201.1"))
				return Ok(message);
			else if (message.Replace("\"", "") == "401.1")
				return Unauthorized(message);
			else
				return BadRequest(message);


		}

		[HttpGet()]
		[ActionName("getById_People")]
		public virtual PeopleModel getById_People(string Peopleid, string loginUserID = "")
		{
			PeopleModel objPeople = null;
			try
			{
				// Current drafts are full People records, so use the normal read path first.
				objPeople = objPeopleDAL.getById_People(Peopleid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - getById_People, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
			}

			// Compatibility only for rows saved by the retired draftdata implementation.
			if (objPeople == null || string.IsNullOrWhiteSpace(objPeople.firstname))
			{
				try
				{
					var legacyDraft = objPeopleDAL.GetDraft(Peopleid);
					if (legacyDraft != null) objPeople = legacyDraft;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Unable to read legacy People draft " + Peopleid);
				}
			}

			return objPeople ?? new PeopleModel();

		}
		[HttpPost()]
		[ActionName("Update_People")]
		public virtual IActionResult Update_People([FromBody] PeopleModel model)
		{
			string message = "";
			var isDraft = string.Equals(model.status, "Draft", StringComparison.OrdinalIgnoreCase);
			if (isDraft) ModelState.Clear();
			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "Update_People";

			try
			{
				if (!model.tenantid.HasValue || model.tenantid.Value == Guid.Empty)
					return BadRequest("Validation Error : Unable to determine the Healthcare Provider (tenant) for this record. Please choose a Healthcare Provider and try again.");

				if (ModelState.IsValid)
				{

					PeopleModelValidator validator = new PeopleModelValidator();
					ValidationResult results = isDraft ? new ValidationResult() : validator.Validate(model);
					if (!results.IsValid)
					{
						var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}", e.PropertyName)));
						message = errorCollection.ToString();
						//return BadRequest("Validation Error : " + message);

					}
					else
					{
						message = isDraft ? "" : ValidateEmergencyContactPhoneNumbers(model, model.Peopleid?.ToString() ?? "");
						if (!string.IsNullOrWhiteSpace(message))
						{
							message = "Validation Error : " + message;
						}
						else
						{
						var authHeader = HttpContext.Request.Headers["Authorization"][0];
						if (authHeader.StartsWith("Bearer "))
						{



							var token = authHeader.Substring("Bearer ".Length);
							String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
							model.modifieduser = new Guid(userdetails[0].ToString());
							obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());





							message = objPeopleDAL.Update_People(model);
						}
						else
						{
							message = "Invalid Token";

						}
						}

					}


				}
				else
				{
					var errorCollection = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
					message = errorCollection.ToString();

					_logger.LogError("PeopleModel - Update_People, Validation Error :" + message);

					//return BadRequest("Validation Error : " + message);
				}






			}
			catch (Exception ex)
			{

				message = ex.Message;
				_logger.LogError(ex, "An exception occurred in - Update_People, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}

			if (obj_access_logsdetailsModel.access_logsid != null)
			{
				obj_access_logsdetailsModel.api_response = message.Replace("\"", "");
				obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
			}

			if (message.Replace("\"", "").Contains("201.1"))
				return Ok(message);
			else if (message.Replace("\"", "") == "401.1")
				return Unauthorized(message);
			else
				return BadRequest(message);




		}
		[HttpGet()]
		public virtual async Task<string> Remove_People(string Peopleid, string loginUserID = "")
		{
			string message = "";
			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "Remove_People";

			try
			{

				var authHeader = HttpContext.Request.Headers["Authorization"][0];
				if (authHeader.StartsWith("Bearer "))
				{



					var token = authHeader.Substring("Bearer ".Length);

					String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
					loginUserID = userdetails[0].ToString();
					obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());


					message = objPeopleDAL.Remove_People(Peopleid, loginUserID);
				}
				else
				{
					message = "Invalid Token";

				}


			}
			catch (Exception ex)
			{
				message = ex.Message;
				_logger.LogError(ex, "An exception occurred in - Remove_People, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			if (obj_access_logsdetailsModel.access_logsid != null)
			{
				obj_access_logsdetailsModel.api_response = message.Replace("\"", "");
				obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
			}

			return message;

		}
		[HttpGet()]

		[ActionName("People_List")]
		public virtual JObject People_List(string tenantid
		, string workprofile = "", string loginUserID = "", int? pagesize = 100, int? pagenumber = 0, string searchterm = "", string sort_fields = "")
		{

			JObject jsonPeople = null;
			try
			{
				tenantid = pvar_tenantid;
				jsonPeople = objPeopleDAL.People_List(tenantid
, workprofile, pagesize, pagenumber, searchterm, sort_fields);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - People_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return jsonPeople;

		}

		[HttpGet()]

		[ActionName("get_all_People")]
		public virtual System.Data.DataTable get_all_People(string tenantid, string searchterm = "", string pagesize = "50", string pagenumber = "1", string loginUserID = "")
		{
			int? pageSize = null;
			if (!string.IsNullOrEmpty(pagesize))
			{
				pageSize = int.Parse(pagesize);
			}
			int? pageNumber = null;
			if (!string.IsNullOrEmpty(pagenumber))
			{

				pageNumber = int.Parse(pagenumber);
				pageNumber--;//0 based index for page numbers
			}
			DataTable dtPeople = new DataTable();
			try
			{
				tenantid = pvar_tenantid;
				dtPeople = objPeopleDAL.get_all_People(tenantid, searchterm, pageSize, pageNumber);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - get_all_People, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));


			}
			return dtPeople;

		}
		[HttpGet()]
		[ActionName("getById_allinfo_People")]
		public virtual System.Data.DataTable getById_allinfo_People(string Peopleid)
		{
			DataTable dtPeople = new DataTable();
			try
			{
				dtPeople = objPeopleDAL.getById_allinfo_People(Peopleid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - getById_allinfo_People, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople;

		}
		[HttpGet()]
		[ActionName("lookup_People_workprofile")]
		public virtual System.Data.DataTable lookup_People_workprofile(String tenantid, string loginUserID = "")
		{
			DataTable dtPeople = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtPeople = objPeopleDAL.lookup_People_workprofile(tenantid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_People_workprofile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople;
		}
		[HttpGet()]
		[ActionName("lookup_People_competencylevel")]
		public virtual System.Data.DataTable lookup_People_competencylevel(String tenantid, string loginUserID = "")
		{
			DataTable dtPeople = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtPeople = objPeopleDAL.lookup_People_competencylevel(tenantid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_People_competencylevel, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople;
		}
		[HttpGet()]
		[ActionName("lookup_People_clinicaltask")]
		public virtual System.Data.DataTable lookup_People_clinicaltask(String tenantid, String workprofile, String competencylevel, string loginUserID = "")
		{
			DataTable dtPeople = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtPeople = objPeopleDAL.lookup_People_clinicaltask(tenantid, workprofile, competencylevel);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_People_clinicaltask, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople;
		}
		[HttpGet()]
		[ActionName("lookup_People_designation")]
		public virtual System.Data.DataTable lookup_People_designation(String tenantid, String workprofile, string loginUserID = "")
		{
			DataTable dtPeople = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtPeople = objPeopleDAL.lookup_People_designation(tenantid, workprofile);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_People_designation, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople;
		}
		[HttpGet()]
		[ActionName("lookup_People_specifycountry")]
		public virtual System.Data.DataTable lookup_People_specifycountry(string loginUserID = "")
		{
			DataTable dtPeople = new DataTable();
			try
			{


				dtPeople = objPeopleDAL.lookup_People_specifycountry();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_People_specifycountry, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople;
		}
		[HttpGet()]
		[ActionName("lookup_People_clinicaltaskinfo_consultations")]
		public virtual System.Data.DataTable lookup_People_clinicaltaskinfo_consultations(String tenantid, string loginUserID = "")
		{
			DataTable dtPeople = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtPeople = objPeopleDAL.lookup_People_clinicaltaskinfo_consultations(tenantid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_People_clinicaltaskinfo_consultations, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople;
		}
		[HttpGet()]
		[ActionName("lookup_People_clinicaltaskinfo_workprofile")]
		public virtual System.Data.DataTable lookup_People_clinicaltaskinfo_workprofile(String tenantid, string loginUserID = "")
		{
			DataTable dtPeople = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtPeople = objPeopleDAL.lookup_People_clinicaltaskinfo_workprofile(tenantid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_People_clinicaltaskinfo_workprofile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople;
		}
		[HttpGet()]
		[ActionName("lookup_People_clinicaltaskinfo_tasktype")]
		public virtual System.Data.DataTable lookup_People_clinicaltaskinfo_tasktype(String tenantid, string loginUserID = "")
		{
			DataTable dtPeople = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtPeople = objPeopleDAL.lookup_People_clinicaltaskinfo_tasktype(tenantid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_People_clinicaltaskinfo_tasktype, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople;
		}
		[HttpGet()]
		[ActionName("lookup_People_clinicaltaskinfo_taskname")]
		public virtual System.Data.DataTable lookup_People_clinicaltaskinfo_taskname(String tenantid, String tasktype, string loginUserID = "")
		{
			DataTable dtPeople = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtPeople = objPeopleDAL.lookup_People_clinicaltaskinfo_taskname(tenantid, tasktype);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_People_clinicaltaskinfo_taskname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPeople;
		}










		[HttpGet()]
		[ActionName("GetPeopleIdByUserId")]
		public virtual IActionResult GetPeopleIdByUserId(string usersid)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(usersid))
					return Ok(new { peopleid = "" });

				using var conn = new Npgsql.NpgsqlConnection(_connectionSettings.Value.ConnectionString);
				conn.Open();
				using var cmd = new Npgsql.NpgsqlCommand(
					"SELECT * FROM \"Get_PeopleId_By_UserId\"(@pvar_usersid)", conn);
				cmd.Parameters.AddWithValue("pvar_usersid", usersid);
				var result = cmd.ExecuteScalar();
				return Ok(new { peopleid = result?.ToString() ?? "" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetPeopleIdByUserId failed for {UsersId}", usersid);
				return Ok(new { peopleid = "" });
			}
		}
	}
}
