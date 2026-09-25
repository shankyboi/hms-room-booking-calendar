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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 13:35:12
				
                
                
                
                
				public class AssessmentTemplateController : BaseController
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
                    private readonly ILogger<AssessmentTemplateController> _logger;
                    
                    
                    StorageUtil util;
					public AssessmentTemplateController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<AssessmentTemplateController> logger):base( configuration)
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
	              

					
				
			  public virtual async Task<string> getById_templatequestions(string AssessmentTemplateid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/getById_templatequestions?AssessmentTemplateid="+AssessmentTemplateid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_templateapplicability(string AssessmentTemplateid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/getById_templateapplicability?AssessmentTemplateid="+AssessmentTemplateid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }

			  [HttpPost()]
			  public virtual async Task<string> Clone_Assessment_Template(string AssessmentTemplateid, string templatename, string taskname, string tenantid)
			  {
					string strReturnMessage = "";
					try
					{
						if (HttpContext.Session.GetString("NalamVazhaloginUserID") == null)
							return "Session Expired";

						if (string.IsNullOrWhiteSpace(AssessmentTemplateid))
							return "Assessment Template is required";

						if (string.IsNullOrWhiteSpace(templatename))
							return "Template Name is required";

						if (string.IsNullOrWhiteSpace(taskname))
							return "Task Name is required";

						if (string.IsNullOrWhiteSpace(tenantid))
							tenantid = HttpContext.Session.GetString("NalamVazhachoosedtenantid");

						if (!Guid.TryParse(tenantid, out Guid selectedTenantId))
							return "Tenant is required";

						var jsonObjAssessmentTemplate = await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/getById_AssessmentTemplate?AssessmentTemplateid="+AssessmentTemplateid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						if (string.IsNullOrWhiteSpace(jsonObjAssessmentTemplate) || jsonObjAssessmentTemplate.Length <= 2)
							return "Assessment Template not found";

						var model = JsonConvert.DeserializeObject<AssessmentTemplateModel>(jsonObjAssessmentTemplate);
						if (model == null)
							return "Assessment Template not found";

						var templateQuestionsJson = await getById_templatequestions(AssessmentTemplateid);
						var templateApplicabilityJson = await getById_templateapplicability(AssessmentTemplateid);

						model.AssessmentTemplateid = Guid.NewGuid();
						model.tenantid = selectedTenantId;
						model.templatename = templatename.Trim();
						model.taskname = taskname;
						model.createduser = new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
						model.modifieduser = null;
						model.createddate = null;
						model.modifieddate = null;
						model.isdeleted = false;
						// A clone is an additional variant (for example, another language),
						// so it must not inherit the source template's default designation.
						model.isdefaulttemplate = false;
						model.craftmyapp_actionmethodname = "Add_Assessment_Template";
						model.templatequestions = string.IsNullOrWhiteSpace(templateQuestionsJson)
							? new List<AssessmentTemplate_templatequestionsModel>()
							: JsonConvert.DeserializeObject<List<AssessmentTemplate_templatequestionsModel>>(templateQuestionsJson) ?? new List<AssessmentTemplate_templatequestionsModel>();
						model.templateapplicability = string.IsNullOrWhiteSpace(templateApplicabilityJson)
							? new List<AssessmentTemplate_templateapplicabilityModel>()
							: JsonConvert.DeserializeObject<List<AssessmentTemplate_templateapplicabilityModel>>(templateApplicabilityJson) ?? new List<AssessmentTemplate_templateapplicabilityModel>();

						foreach (var question in model.templatequestions)
						{
							question.AssessmentTemplate_templatequestionsid = Guid.NewGuid();
							question.AssessmentTemplateid = model.AssessmentTemplateid;
							question.craftmyapp_actionmethodname = "Add_Assessment_Template";
						}

						foreach (var applicability in model.templateapplicability)
						{
							applicability.AssessmentTemplate_templateapplicabilityid = Guid.NewGuid();
							applicability.AssessmentTemplateid = model.AssessmentTemplateid;
							applicability.craftmyapp_actionmethodname = "Add_Assessment_Template";
						}

						strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/AssessmentTemplate/Add_Assessment_Template", model);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex,"An exception occurred in - AssessmentTemplate / Clone_Assessment_Template, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
						strReturnMessage = ex.Message;
					}

					if(strReturnMessage.Replace("\"", "").Contains("201.1"))
					{
						return "Success";
					}
					else if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
					{
						return strReturnMessage.Replace("\"", "").Replace("BadRequest :","");
					}
					else
					{
						if(strReturnMessage=="401.1")
							strReturnMessage = "Authorization Failed";

						return strReturnMessage.Replace("\"", "");
					}
			  }


			  public virtual IActionResult Add_Assessment_Template()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Assessment_Template(AssessmentTemplateModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("AssessmentTemplateid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Assessment_Template";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 AssessmentTemplateModelValidator validator = new AssessmentTemplateModelValidator();
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
								 model.AssessmentTemplateid =Guid.NewGuid(); 
                                 
                                  
								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/AssessmentTemplate/Add_Assessment_Template", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - AssessmentTemplate / Add_Assessment_Template, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
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

				
			  public virtual async Task<IActionResult> Update_Assessment_Template(string AssessmentTemplateid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='AssessmentTemplate' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjAssessmentTemplate = await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/getById_AssessmentTemplate?AssessmentTemplateid="+AssessmentTemplateid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjAssessmentTemplate.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<AssessmentTemplateModel>(jsonObjAssessmentTemplate);


                
                                     
                                    return View("Add_Assessment_Template", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - AssessmentTemplate / Update_Assessment_Template, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Assessment_Template(AssessmentTemplateModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("AssessmentTemplateid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Assessment_Template";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            
                            
							if (ModelState.IsValid)
							{
									AssessmentTemplateModelValidator validator = new AssessmentTemplateModelValidator();
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
                                        
										
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/AssessmentTemplate/Update_Assessment_Template", model);
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
                      _logger.LogError(ex,"An exception occurred in - AssessmentTemplate / Update_Assessment_Template, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Assessment_Template(string AssessmentTemplateid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/Remove_Assessment_Template?AssessmentTemplateid="+AssessmentTemplateid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - AssessmentTemplate / Remove_Assessment_Template, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='AssessmentTemplate' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult Detail_Assessment_Template()
                                        {
                                            return View();
                                        }

			public virtual IActionResult Assessment_Template_List()
			{
				return View();
			}


		[HttpGet()]
		public virtual async Task<string> get_Assessment_Template_List(string tenantid
, string taskname
)
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/Assessment_Template_List?tenantid=" + tenantid + "&taskname=" + taskname + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID")
);
		}




		public virtual async Task<string> getById_allinfo_AssessmentTemplate(string AssessmentTemplateid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/getById_allinfo_AssessmentTemplate?AssessmentTemplateid="+AssessmentTemplateid);
					 
			  }

[HttpGet()]
                        public virtual async Task<string> lookup_AssessmentTemplate_templatequestions_questioncategory(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/lookup_AssessmentTemplate_templatequestions_questioncategory?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_AssessmentTemplate_templatequestions_questionsub(String tenantid,String questioncategory)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/lookup_AssessmentTemplate_templatequestions_questionsub?tenantid="+tenantid+"&questioncategory="+questioncategory+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_AssessmentTemplate_templatequestions_question(String tenantid,String questioncategory,String questionsub)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/lookup_AssessmentTemplate_templatequestions_question?tenantid="+tenantid+"&questioncategory="+questioncategory+"&questionsub="+questionsub+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_AssessmentTemplate_templateapplicability_patientcategory(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/lookup_AssessmentTemplate_templateapplicability_patientcategory?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_AssessmentTemplate_templateapplicability_medicalcondition(string searchterm, int? pagesize, int? pagenumber)
                        {
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/lookup_AssessmentTemplate_templateapplicability_medicalcondition?searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }

 
                            [HttpGet()]
                            public virtual async Task<string> get_all_MedicalCondition(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
                            {

                                return await ApiClient.Get_ApiValues(getHttpClient(), "api/MedicalCondition/get_all_MedicalCondition?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                            }
                            
 
											[HttpGet()]
											public virtual async Task<string> get_all_PatientCategory(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientCategory/get_all_PatientCategory?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
[HttpGet()]
			    public virtual async Task<string> lookup_AssessmentTemplate_question(String tenantid,String questionsubcategory,String questioncategory)
			    {
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/lookup_AssessmentTemplate_question?tenantid="+tenantid+"&questionsubcategory="+questionsubcategory+"&questioncategory="+questioncategory+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    } 
											[HttpGet()]
											public virtual async Task<string> get_all_AssessmentQuestion(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentQuestion/get_all_AssessmentQuestion?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
[HttpGet()]
			    public virtual async Task<string> lookup_AssessmentTemplate_questionsub(String tenantid,String questioncategoryname)
			    {
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/lookup_AssessmentTemplate_questionsub?tenantid="+tenantid+"&questioncategoryname="+questioncategoryname+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    } 
											[HttpGet()]
											public virtual async Task<string> get_all_QuestionSubCategory(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/QuestionSubCategory/get_all_QuestionSubCategory?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
 
											[HttpGet()]
											public virtual async Task<string> get_all_QuestionCategory(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/QuestionCategory/get_all_QuestionCategory?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											


[HttpGet()]
			public virtual async Task<string> lookup_change_templatequestions_AssessmentTemplate_question(string AssessmentQuestionid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/lookup_change_templatequestions_AssessmentTemplate_question?AssessmentQuestionid="+AssessmentQuestionid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}



		[HttpGet()]
		public virtual async Task<string> lookup_AssessmentTemplate_taskname(String tenantid)
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/lookup_AssessmentTemplate_taskname?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}



	}


}
