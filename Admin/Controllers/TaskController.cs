namespace Admin.Controllers
{
	using System;
	using System.Data;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc;
	using Newtonsoft.Json;
	using System.Net.Http;
	using System.Net.Http.Formatting;
	using System.Threading.Tasks;
	using System.Net.Http.Headers;
	using Microsoft.Extensions.Options;
	using Microsoft.AspNetCore.Http;
	using System.Collections.Generic;
	using System.IO;
	using Microsoft.AspNetCore.Hosting;
	using System.Net;
	using FluentValidation.Results;
	using NalamVazha.Models;
	using Microsoft.AspNetCore.Mvc.Infrastructure;
	using Microsoft.AspNetCore.HttpOverrides;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;
	using System.Threading;
	using System.Globalization;
	using System.Text.Json;
	using Microsoft.AspNetCore.Authorization;




	//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/30/2026 16:03:29





	public class TaskController : BaseController
	{
		private IWebHostEnvironment hostingEnv;
		private IOptions<ApiSettings> _balSettings;
		private IOptions<MailSettings> _mailSettings;
					private string  url = "";
					private string  baseUrl = "";
                    private string  adminUrl = "";
                    private string  clientUrl = "";
                    private string  accesskey = "";
		private IHttpContextAccessor _accessor;
		public IConfiguration Configuration { get; }
		private readonly ILogger<TaskController> _logger;


		StorageUtil util;
					public TaskController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<TaskController> logger):base( configuration)
		{
			_logger = logger;
			this.hostingEnv = env;
			_balSettings = ApiSettings;
			_mailSettings = MailSettings;
			url = _balSettings.Value.apiURL;
			baseUrl = _balSettings.Value.baseURL;
			adminUrl = _balSettings.Value.adminURL;
			clientUrl = _balSettings.Value.clientURL;
			accesskey = _balSettings.Value.accesskey;

			_accessor = accessor;
			Configuration = configuration;
			util = new StorageUtil(configuration);
		}


		public virtual IActionResult audit()
		{
			return View();
		}




		public virtual async Task<string> getById_screeningtemplates(string Taskid)
		{
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/Task/getById_screeningtemplates?Taskid="+Taskid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));

		}


		public virtual IActionResult Add_Task()
		{
			return View();
		}
		[HttpPost()]
		public virtual async Task<string> Add_Task(TaskModel model, IFormCollection collection)
		{
			string strReturnMessage = "";

			try
			{
				ModelState.Remove("Taskid");
				ModelState.Remove("createduser");
				ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Task";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
				else
					return "Session Expired";




				if (ModelState.IsValid)
				{
					TaskModelValidator validator = new TaskModelValidator();
					ValidationResult results = validator.Validate(model);
					if (!results.IsValid)
					{
								 var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}",e.PropertyName)));
						strReturnMessage = errorCollection.ToString();
						foreach (var failure in results.Errors)
						{
							ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
						}
					}
					else
					{
								 model.Taskid =Guid.NewGuid(); 



                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/Task/Add_Task", model);


					}
				}
				else
				{
					var errorMessages = ModelState.Where(entry => entry.Value.Errors.Any()).SelectMany(entry => entry.Value.Errors.Select(error => $"{entry.Key}: {error.ErrorMessage}"));
					var errorCollection = string.Join(" | ", errorMessages);
					strReturnMessage = errorCollection;
				}
			}
			catch (Exception ex)
			{

                 _logger.LogError(ex,"An exception occurred in - Task / Add_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

				strReturnMessage = ex.Message;
			}
			ViewData["message"] = strReturnMessage;
			 if(strReturnMessage.Replace("\"", "").Contains("201.1")){
				TempData["message"] = "Success";


				return "Success";
			}
			else if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
			{
				strReturnMessage= strReturnMessage.Replace("\"", "").Replace("BadRequest :","");
				TempData["message"] = strReturnMessage;

				return strReturnMessage;
			}
             else{
				  if(strReturnMessage=="401.1")
					strReturnMessage = "Authorization Failed";

				return strReturnMessage;
			}

		}


		public virtual async Task<IActionResult> Update_Task(string Taskid)
		{

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
				DataView dv = new DataView(NalamVazharole_JSON);
				dv.RowFilter = "controllername='Task' AND viewname='list'";

                            if(dv.Count  >0){
					redirectTo = dv[0]["actionmethodname"] as string;

				}

                            try{
                                     var jsonObjTask = await ApiClient.Get_ApiValues(getHttpClient(), "api/Task/getById_Task?Taskid="+Taskid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjTask.Length > 2)
				{

						var model = JsonConvert.DeserializeObject<TaskModel>(jsonObjTask);




						return View("Add_Task", model);
					}
					else
					{

						TempData["message"] = "Data Not Found - Contact Administrator";
						return RedirectToAction(redirectTo);

					}

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - Task / Update_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					TempData["errMessage"] = "Error while fetching data - Contact Administrator";
					return RedirectToAction(redirectTo);
				}

			}
			TempData["errMessage"] = "Session Expired";
			return RedirectToAction("Logout", "users");
		}
		[HttpPost()]
		public virtual async Task<string> Update_Task(TaskModel model, IFormCollection collection)
		{
			string strReturnMessage = "";
			try
			{
				ModelState.Remove("Taskid");
				ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Task";


							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
				else
					return "Session Expired";



				if (ModelState.IsValid)
				{
					TaskModelValidator validator = new TaskModelValidator();
					ValidationResult results = validator.Validate(model);
					if (!results.IsValid)
					{
										var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}",e.PropertyName)));
						strReturnMessage = errorCollection.ToString();
						foreach (var failure in results.Errors)
						{
							ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
						}
					}
					else
					{




                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/Task/Update_Task", model);
					}
				}
				else
				{
					var errorMessages = ModelState.Where(entry => entry.Value.Errors.Any()).SelectMany(entry => entry.Value.Errors.Select(error => $"{entry.Key}: {error.ErrorMessage}"));
					var errorCollection = string.Join(" | ", errorMessages);
					strReturnMessage = errorCollection;
				}
			}
			catch (Exception ex)
			{
                      _logger.LogError(ex,"An exception occurred in - Task / Update_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

				strReturnMessage = ex.Message;
			}
			ViewData["message"] = strReturnMessage;
					    if(strReturnMessage.Replace("\"", "")=="201.1"){
				TempData["message"] = "Success";

				return "Success";
			}
			else if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
			{
				strReturnMessage= strReturnMessage.Replace("\"", "").Replace("BadRequest :","");
				TempData["message"] = strReturnMessage;

				return strReturnMessage;
			}
                         else{
							if(strReturnMessage=="401.1")
					strReturnMessage = "Authorization Failed";

				return strReturnMessage;
			}

		}
		public virtual async Task<IActionResult> Remove_Task(string Taskid)
		{
			string message = "";
			try
			{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/Task/Remove_Task?Taskid="+Taskid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
				{
					TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
				}



			}
			catch (Exception ex)
			{
                     _logger.LogError(ex,"An exception occurred in - Task / Remove_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));


				TempData["errMessage"] = ex.Message;
				message = ex.Message;
			}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
				DataView dv = new DataView(NalamVazharole_JSON);
				dv.RowFilter = "controllername='Task' AND viewname='list'";

						if(dv.Count  >0){
					redirectTo = dv[0]["actionmethodname"] as string;

				}

			}

			return RedirectToAction(redirectTo);
		}

		public virtual IActionResult View_Task()
		{
			return View();
		}

		public virtual IActionResult Task_List()
		{
			return View();
		}

		[HttpGet()]
		public virtual async Task<string> get_Task_List(string tenantid
,string tasktype
)
		{

				return await ApiClient.Get_ApiValues(getHttpClient(), "api/Task/Task_List?tenantid="+tenantid+"&tasktype="+tasktype+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
		}

		[HttpGet()]
		public virtual async Task<string> get_all_TaskType(string tenantid)
		{

											return await ApiClient.Get_ApiValues(getHttpClient(), "api/TaskType/get_all_TaskType?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}



		public virtual async Task<string> getById_allinfo_Task(string Taskid)
		{
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/Task/getById_allinfo_Task?Taskid="+Taskid);

		}
		[HttpGet()]
		public virtual async Task<string> lookup_Task_tasktype(String tenantid)
		{

				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Task/lookup_Task_tasktype?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> lookup_Task_screeningtemplates_screeningtemplate(String tenantid)
		{
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/Task/lookup_Task_screeningtemplates_screeningtemplate?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}


		[HttpGet()]
		public virtual async Task<string> get_all_AssessmentTemplate(string tenantid)
		{

											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/get_all_AssessmentTemplate?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}









	}


}
