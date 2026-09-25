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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:21
				
                
                
                
                
				public class ShiftPlanningController : BaseController
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
                    private readonly ILogger<ShiftPlanningController> _logger;
                    
                    
                    StorageUtil util;
					public ShiftPlanningController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<ShiftPlanningController> logger):base( configuration)
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
	              

					
				
			  public virtual async Task<string> getById_people(string ShiftPlanningid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/ShiftPlanning/getById_people?ShiftPlanningid="+ShiftPlanningid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


			  public virtual IActionResult Add_Shift_Planning()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Shift_Planning(ShiftPlanningModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("ShiftPlanningid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Shift_Planning";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 ShiftPlanningModelValidator validator = new ShiftPlanningModelValidator();
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
								 model.ShiftPlanningid =Guid.NewGuid(); 
                                 
                                  var files = Request.Form.Files;
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"ShiftPlanning_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
var filanameFolder = hostingEnv.WebRootPath +  "/uploads/"; 
if (!Directory.Exists(filanameFolder))
{
Directory.CreateDirectory(filanameFolder);
}
filename  = filanameFolder + uploadFileName;
using (FileStream fs = System.IO.File.Create(filename))
{
file.CopyTo(fs);
fs.Flush();
}

                        if (file.Name == "peoplebulkupload")
                        {
                            model.ShiftPlanning_people_bulkupload = ExcelHelper.ReadExcelToModel<ShiftPlanning_people_bulkuploadModel>(filename);
                        }
}
}

								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/ShiftPlanning/Add_Shift_Planning", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - ShiftPlanning / Add_Shift_Planning, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
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

				
			  public virtual async Task<IActionResult> Update_Shift_Planning(string ShiftPlanningid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='ShiftPlanning' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjShiftPlanning = await ApiClient.Get_ApiValues(getHttpClient(), "api/ShiftPlanning/getById_ShiftPlanning?ShiftPlanningid="+ShiftPlanningid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjShiftPlanning.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<ShiftPlanningModel>(jsonObjShiftPlanning);


                
                                     
                                    return View(model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - ShiftPlanning / Update_Shift_Planning, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Shift_Planning(ShiftPlanningModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("ShiftPlanningid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Shift_Planning";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            
                            
							if (ModelState.IsValid)
							{
									ShiftPlanningModelValidator validator = new ShiftPlanningModelValidator();
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
                                        
										var files = Request.Form.Files;
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"ShiftPlanning_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
var filanameFolder = hostingEnv.WebRootPath +  "/uploads/"; 
if (!Directory.Exists(filanameFolder))
{
Directory.CreateDirectory(filanameFolder);
}
filename  = filanameFolder + uploadFileName;
using (FileStream fs = System.IO.File.Create(filename))
{
file.CopyTo(fs);
fs.Flush();
}

                        if (file.Name == "peoplebulkupload")
                        {
                            model.ShiftPlanning_people_bulkupload = ExcelHelper.ReadExcelToModel<ShiftPlanning_people_bulkuploadModel>(filename);
                        }
}
}

                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/ShiftPlanning/Update_Shift_Planning", model);
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
                      _logger.LogError(ex,"An exception occurred in - ShiftPlanning / Update_Shift_Planning, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Shift_Planning(string ShiftPlanningid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/ShiftPlanning/Remove_Shift_Planning?ShiftPlanningid="+ShiftPlanningid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - ShiftPlanning / Remove_Shift_Planning, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='ShiftPlanning' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult View_Shift_Planning()
                                        {
                                            return View();
                                        }

[HttpPost]
public async Task<JsonResult> Upload_Shift_Planning(ShiftPlanningModel model, IFormFile formFile)
{
   model.createduser = new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
    string strReturnMessage = "";

    var file = Request.Form.Files[0];
    var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
    string fileExtension = "." + filename.Split('.').Last();
    Random rnd = new Random();

    string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty) + "_" + "ShiftPlanning_" + rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss") + fileExtension;

    var filenameFolder = hostingEnv.WebRootPath + "/uploads/";
    if (!Directory.Exists(filenameFolder))
    {
        Directory.CreateDirectory(filenameFolder);
    }
    filename = filenameFolder + uploadFileName;

    using (FileStream fs = System.IO.File.Create(filename))
    {
        file.CopyTo(fs);
        fs.Flush();
    }

       if (file.Name == "ShiftPlanning_bulkupload")
                        {
                            model.ShiftPlanning_bulkupload = ExcelHelper.ReadExcelToModel<ShiftPlanning_bulkuploadModel>(filename);
                        }
    strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/ShiftPlanning/Upload_Shift_Planning", model);

    ViewData["message"] = strReturnMessage;

    if (strReturnMessage.Replace("\"", "") == "201.1")
    {
        TempData["message"] = "Success";
        return Json(new { message = "Success"});
    }
    else
    {
        return Json(new { message = strReturnMessage });
    }
    
}

[HttpGet]
public async Task<JsonResult> getById_allinfo_ShiftPlanningerror(string ShiftPlanningid)
{

    var jsonResponse = await ApiClient.Get_ApiValues(getHttpClient(), "api/ShiftPlanning/getById_allinfo_ShiftPlanning?ShiftPlanningid=" + ShiftPlanningid);
    dynamic response = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
    var bulkUploadError=""; var bulkUploadErrorCount =""; var tableData="";

    if (response != null && response.HasValues && response.Count > 0)
	{
      bulkUploadError = response[0].automaton_ShiftPlanning_bulkuploaderror;
      bulkUploadErrorCount = response[0].automaton_ShiftPlanning_bulkuploaderrorcount;
      tableData = response[0].automaton_ShiftPlanning_bulkuploaderror; 
    }

        return Json(new
     {
         tableData,
         bulkUploadError,
         bulkUploadErrorCount
     });
     
    
}

			        public virtual IActionResult Shift_Planning_List()
			        {
				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Shift_Planning_List(string tenantid
,string shiftname
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
			        {
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/ShiftPlanning/Shift_Planning_List?tenantid="+tenantid+"&shiftname="+shiftname+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
			        }
			          
											[HttpGet()]
											public virtual async Task<string> get_all_Shift(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Shift/get_all_Shift?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_ShiftPlanning(string ShiftPlanningid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/ShiftPlanning/getById_allinfo_ShiftPlanning?ShiftPlanningid="+ShiftPlanningid);
					 
			  }
[HttpGet()]
                    public virtual async Task<string> lookup_ShiftPlanning_shiftname(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/ShiftPlanning/lookup_ShiftPlanning_shiftname?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }

[HttpGet()]
                        public virtual async Task<string> lookup_ShiftPlanning_people_personname(String tenantid, String workprofile, string searchterm, int? pagesize, int? pagenumber)
                        {
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/ShiftPlanning/lookup_ShiftPlanning_people_personname?tenantid="+tenantid+"&workprofile="+workprofile+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_ShiftPlanning_people_workprofile(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/ShiftPlanning/lookup_ShiftPlanning_people_workprofile?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }

 
											[HttpGet()]
											public virtual async Task<string> get_all_WorkProfile(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/WorkProfile/get_all_WorkProfile?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
[HttpGet()]
			    public virtual async Task<string> lookup_ShiftPlanning_personname(String tenantid,String workprofile)
			    {
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/ShiftPlanning/lookup_ShiftPlanning_personname?tenantid="+tenantid+"&workprofile="+workprofile+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    } 
                            [HttpGet()]
                            public virtual async Task<string> get_all_People(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
                            {

                                return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/get_all_People?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                            }
                            


[HttpGet()]
			public virtual async Task<string> lookup_change_people_ShiftPlanning_personname(string Peopleid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/ShiftPlanning/lookup_change_people_ShiftPlanning_personname?Peopleid="+Peopleid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}


                    
                     
                        public ActionResult Download_peopleExcelTemplate()
				    {
					    byte[] fileContents = ExcelHelper.GenerateEmptyExcelTemplate<ShiftPlanning_people_bulkuploadModel>();

					    return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Upload_ShiftPlanning_people.xlsx");

				    }
public ActionResult Download_ShiftPlanningExcelTemplate()
				    {
					    byte[] fileContents = ExcelHelper.GenerateEmptyExcelTemplate<ShiftPlanning_bulkuploadModel>();

					    return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Upload_ShiftPlanning.xlsx");

				    }
		[HttpGet()]
		public virtual async Task<string> lookup_change_ShiftPlanning_shiftname(string Shiftid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/ShiftPlanning/lookup_change_ShiftPlanning_shiftname?Shiftid=" + Shiftid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

	}


			}
