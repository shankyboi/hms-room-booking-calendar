namespace Admin.Controllers
			{
    using DocumentFormat.OpenXml.Office2013.Word;
				using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
				using Microsoft.AspNetCore.Hosting;
				using Microsoft.AspNetCore.Http;
				using Microsoft.AspNetCore.HttpOverrides;
				using Microsoft.AspNetCore.Mvc;
				using Microsoft.AspNetCore.Mvc.Infrastructure;
                using Microsoft.Extensions.Configuration;
                using Microsoft.Extensions.Logging;
				using Microsoft.Extensions.Options;
				using NalamVazha.Models;
				using Newtonsoft.Json;
				using Newtonsoft.Json.Linq;
				using System;
				using System.Collections.Generic;
				using System.Data;
	            using System.Globalization;
				using System.IO;
				using System.Linq;
				using System.Net;
				using System.Net.Http;
				using System.Net.Http.Formatting;
				using System.Net.Http.Headers;
                using System.Text.Json;
                using System.Threading;
				using System.Threading.Tasks;




    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 11:34:29
				
                
                
                
                
				public class PeopleController : BaseController
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
                    private readonly ILogger<PeopleController> _logger;
                    
                    
                    StorageUtil util;
					public PeopleController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<PeopleController> logger):base( configuration)
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
	              

					
				
			  public virtual async Task<string> getById_emergencycontact(string Peopleid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/getById_emergencycontact?Peopleid="+Peopleid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_educationinfo(string Peopleid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/getById_educationinfo?Peopleid="+Peopleid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_workexperience(string Peopleid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/getById_workexperience?Peopleid="+Peopleid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_preferredlanguageinfo(string Peopleid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/getById_preferredlanguageinfo?Peopleid="+Peopleid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_clinicaltaskinfo(string Peopleid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/getById_clinicaltaskinfo?Peopleid="+Peopleid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));

			  }

			[HttpGet()]
			public virtual async Task<string> GetDuration_ByPractitioner(string Peopleid, string tenantid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/GetDuration_ByPractitioner?Peopleid="+Peopleid+"&tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}


			  public virtual async Task<string> prefill_People_clinicaltaskinfo(string workprofile, string competencylevel)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/prefill_People_clinicaltaskinfo?workprofile="+workprofile+"&competencylevel="+competencylevel+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }

			  [HttpGet()]
			  public virtual async Task<string> CheckEmailExists(string emailid)
			  {
					string tenantid = HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? HttpContext.Session.GetString("NalamVazhatenantid") ?? Guid.Empty.ToString();
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/CheckEmailExists?emailid=" + Uri.EscapeDataString(emailid ?? "") + "&tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
			  }

				  public virtual IActionResult Add_People()
			  {
					return View();
			  }
			  [HttpPost()]
			public virtual async Task<string> Add_People(PeopleModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				var isDraft = string.Equals(model.status, "Draft", StringComparison.OrdinalIgnoreCase);
				var client = getHttpClient();
				try
				{
					ModelState.Remove("Peopleid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_People";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					EnsurePeopleTenant(model);
					model.photo = collection["photo_existing"];
					model.iddocument = collection["iddocument_existing"];
					model.licenceupload = collection["licenceupload_existing"];
					ModelState.Remove("photo");
ModelState.Remove("iddocument");
					ModelState.Remove("licenceupload");
					if (isDraft)
						ModelState.Clear();

                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 PeopleModelValidator validator = new PeopleModelValidator();
							 ValidationResult results = isDraft ? new ValidationResult() : validator.Validate(model);
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
								 model.Peopleid ??= Guid.NewGuid();
                                 model.photo = collection["photo_existing"];
model.iddocument = collection["iddocument_existing"];
model.licenceupload = collection["licenceupload_existing"];

                                  var files = Request.Form.Files;
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"People_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "photo")
{
uploadFileName = "|" + fileURL +"|";
model.photo +=  uploadFileName ;
}
if (file.Name == "iddocument")
{
uploadFileName = "|" + fileURL +"|";
model.iddocument +=  uploadFileName ;
}
if (file.Name == "licenceupload")
{
uploadFileName = "|" + fileURL +"|";
model.licenceupload +=  uploadFileName ;
}
}
}

								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(client,"api/People/Add_People", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - People / Add_People, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
				 strReturnMessage = ex.Message;
			 }
		     ViewData["message"] = strReturnMessage;
			 
                if (strReturnMessage.Replace("\"", "").Contains("201.1"))
                {
                    TempData["message"] = "Success";
					bool credentialsSent = false;
					var parts = strReturnMessage.Replace("\"", "").Split('|');
				if (!isDraft && parts.Length >= 3)
				{
					string username = parts[1];
					string plainPassword = parts[2];

					MailSender maillog = new MailSender();
					credentialsSent = await maillog.sendMail(
						"OTP",
						"User credientils",
					   model.Peopleid.ToString(),
						"OTP",
						_mailSettings,
					   model.createduser.ToString(),
						client,

						 plainPassword,
						 username,
						 tenantid: model.tenantid?.ToString() ?? ""
					);
				}
						return credentialsSent ? "Success|CredentialsSent" : "Success";
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

				
			  public virtual async Task<IActionResult> Update_People(string Peopleid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='People' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjPeople = await ApiClient.Get_ApiValues(getHttpClient(), "api/People/getById_People?Peopleid="+Peopleid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjPeople.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<PeopleModel>(jsonObjPeople);
									EnsurePeopleTenant(model);


                
                                     
                                    return View("Add_People", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - People / Update_People, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_People(PeopleModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					string previousEmail = "";
					string requestedEmail = model.emailid?.Trim() ?? "";
					var isDraft = string.Equals(model.status, "Draft", StringComparison.OrdinalIgnoreCase);
					bool wasDraft = false;
					PeopleModel existingPeople = null;
					var client = getHttpClient();
					try
					{
						// Read the persisted value before saving so an OTP is sent only when
						// the email address has actually changed.
						if (model.Peopleid != Guid.Empty)
						{
							var existingJson = await ApiClient.Get_ApiValues(client,
								"api/People/getById_People?Peopleid=" + model.Peopleid
								+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
							if (!string.IsNullOrWhiteSpace(existingJson) && existingJson.Length > 2)
								{
									existingPeople = JsonConvert.DeserializeObject<PeopleModel>(existingJson);
									previousEmail = existingPeople?.emailid?.Trim() ?? "";
									wasDraft = string.Equals(existingPeople?.status, "Draft", StringComparison.OrdinalIgnoreCase);
								}
						}

							ModelState.Remove("Peopleid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_People";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							EnsurePeopleTenant(model);
							
                            model.photo = collection["photo_existing"];
model.iddocument = collection["iddocument_existing"];
model.licenceupload = collection["licenceupload_existing"];

                            ModelState.Remove("photo");
ModelState.Remove("iddocument");
ModelState.Remove("licenceupload");
							if (isDraft)
								ModelState.Clear();

							if (ModelState.IsValid)
							{
									PeopleModelValidator validator = new PeopleModelValidator();
									ValidationResult results = isDraft ? new ValidationResult() : validator.Validate(model);
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
										var activatingDraft = !isDraft && wasDraft;
										var emailChanged = !isDraft && !activatingDraft && !string.IsNullOrWhiteSpace(requestedEmail)
											&& !string.Equals(previousEmail, requestedEmail, StringComparison.OrdinalIgnoreCase);
										if (emailChanged)
										{
											var validationMessage = await ApiClient.Post_ApiValuesGetString(client,
												"api/users/ManageProfileEmailChange", new JObject
												{
													["profiletype"] = "People",
													["profileid"] = model.Peopleid,
													["tenantid"] = model.tenantid,
													["newemail"] = requestedEmail,
													["apply"] = false
												});
											if (validationMessage.Replace("\"", "") != "201.1")
												return validationMessage.Replace("\"", "").Replace("BadRequest :", "");

											// Save the remaining fields while keeping the verified login email unchanged.
											model.emailid = previousEmail;
										}
                                        
										var files = Request.Form.Files;
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"People_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "photo")
{
uploadFileName = "|" + fileURL +"|";
model.photo +=  uploadFileName ;
}
if (file.Name == "iddocument")
{
uploadFileName = "|" + fileURL +"|";
model.iddocument +=  uploadFileName ;
}
if (file.Name == "licenceupload")
{
uploadFileName = "|" + fileURL +"|";
model.licenceupload +=  uploadFileName ;
}
}
}

                                        
                                        
										strReturnMessage = await ApiClient.Post_ApiValuesGetString(client,"api/People/Update_People", model);
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
                      _logger.LogError(ex,"An exception occurred in - People / Update_People, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
						strReturnMessage = ex.Message;
					}
					ViewData["message"] = strReturnMessage;
					    if(strReturnMessage.Replace("\"", "").Contains("201.1")){
							TempData["message"] = "Success";

							var responseParts = strReturnMessage.Replace("\"", "").Split('|');
							bool activatingDraft = !isDraft && wasDraft;
							bool hasCredentials = activatingDraft && responseParts.Length >= 3
								&& !string.IsNullOrWhiteSpace(responseParts[1])
								&& !string.IsNullOrWhiteSpace(responseParts[2]);

							// Compatibility fallback for databases/API instances that return only
							// "201.1" after activating a draft. The linked account already contains
							// the generated password, so retrieve it for the one-time credentials mail.
							string credentialEmail = hasCredentials ? responseParts[1] : requestedEmail;
							string credentialPassword = hasCredentials ? responseParts[2] : "";
							if (activatingDraft && string.IsNullOrWhiteSpace(credentialPassword))
							{
								credentialPassword = (await ApiClient.Post_ApiValuesGetString(
									client,
									"api/users/get_decryptedPassword",
									new userloginModel { username = requestedEmail, tenantid = model.tenantid }))
									.Replace("\"", "").Trim();
								hasCredentials = !string.IsNullOrWhiteSpace(credentialEmail)
									&& !string.IsNullOrWhiteSpace(credentialPassword);
							}

							bool emailChanged = !isDraft && !activatingDraft && !string.IsNullOrWhiteSpace(requestedEmail)
								&& !string.Equals(previousEmail, requestedEmail, StringComparison.OrdinalIgnoreCase);
							if (hasCredentials)
							{
								var mailSender = new MailSender();
								var credentialsSent = await mailSender.sendMail(
									"OTP", "User credientils", model.Peopleid.ToString(), "OTP", _mailSettings,
									model.modifieduser.ToString(), client, credentialPassword, credentialEmail,
									tenantid: model.tenantid?.ToString() ?? "");
								if (!credentialsSent)
								{
									_logger.LogError("Credentials email failed for activated People record {Peopleid} and email {Email}", model.Peopleid, credentialEmail);
									return "People saved, but the credentials email could not be sent. Please verify the tenant mail configuration and recipient address.";
								}
								return "Success|CredentialsSent";
							}
							if (activatingDraft)
								return "People saved, but credentials could not be found for the linked user account.";
							if (emailChanged)
							{
								var otpResponse = await ApiClient.Post_ApiValuesGetRespnse(
									client,
									"api/users/RequestPatientOTP",
									new userloginModel { username = requestedEmail });

								if (!otpResponse.IsSuccessStatusCode)
								{
									var otpError = (await otpResponse.Content.ReadAsStringAsync()).Replace("\"", "");
									return "Profile updated, but OTP could not be generated. Your login email was not changed. " + otpError;
								}

								var otpResult = JObject.Parse(await otpResponse.Content.ReadAsStringAsync());
								string otp = otpResult["otp"]?.ToString();
								string targetEmail = otpResult["email"]?.ToString()?.Trim();
								string otpLogId = otpResult["otpsid"]?.ToString();
								string tenantId = otpResult["tenantid"]?.ToString()?.Trim();

								// A newly changed People email may not have a matching login account yet,
								// so manage_user_otp cannot always infer its tenant. Use the tenant from
								// the saved People record to select the correct Registration mailbox.
								if (string.IsNullOrWhiteSpace(tenantId)
									|| tenantId == Guid.Empty.ToString())
								{
										tenantId = existingPeople?.tenantid?.ToString()
										?? model.tenantid?.ToString()
										?? HttpContext.Session.GetString("NalamVazhachoosedtenantid")
										?? HttpContext.Session.GetString("NalamVazhatenantid");
								}

								if (string.IsNullOrWhiteSpace(targetEmail))
									targetEmail = requestedEmail;

								if (string.IsNullOrWhiteSpace(otp)
									|| string.IsNullOrWhiteSpace(otpLogId)
									|| string.IsNullOrWhiteSpace(targetEmail)
									|| string.IsNullOrWhiteSpace(tenantId))
								{
									return "OTP response was incomplete. Your login email was not changed.";
								}

								var mailSender = new MailSender();
								bool otpSent = await mailSender.sendMail(
									"OTP", "OTP Generator", otpLogId, "OTP", _mailSettings,
									HttpContext.Session.GetString("NalamVazhaloginUserID"),
									client, otp, targetEmail, tenantid: tenantId);

								if (!otpSent)
									return "Profile updated, but the OTP email could not be sent. Your login email was not changed.";

								var pendingKey = model.Peopleid.ToString();
								HttpContext.Session.SetString("PendingPeopleEmail:" + pendingKey, requestedEmail);
								HttpContext.Session.SetString("PendingPeopleTenant:" + pendingKey, model.tenantid?.ToString() ?? "");
								HttpContext.Session.SetString("PendingPeopleEmailExpires:" + pendingKey,
									DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));
								return "OTP_REQUIRED|" + requestedEmail;
							}
							
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

		[HttpPost]
		public virtual async Task<string> Verify_People_Email_OTP(string Peopleid, string otp)
		{
			if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("NalamVazhaloginUserID")))
				return "Session Expired";
			if (!Guid.TryParse(Peopleid, out var profileId))
				return "Invalid profile.";

			var emailKey = "PendingPeopleEmail:" + Peopleid;
			var tenantKey = "PendingPeopleTenant:" + Peopleid;
			var expiryKey = "PendingPeopleEmailExpires:" + Peopleid;
			var email = HttpContext.Session.GetString(emailKey);
			var tenant = HttpContext.Session.GetString(tenantKey);
			if (string.IsNullOrWhiteSpace(email) || !Guid.TryParse(tenant, out var tenantId)
				|| !long.TryParse(HttpContext.Session.GetString(expiryKey), out var expiry)
				|| DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiry)
				return "The email verification request has expired. Please save again.";

			var client = getHttpClient();
			var result = await ApiClient.Post_ApiValuesGetString(client,
				"api/users/ManageProfileEmailChange", new JObject
				{
					["profiletype"] = "People", ["profileid"] = profileId,
					["tenantid"] = tenantId, ["newemail"] = email, ["otp"] = otp, ["apply"] = true
				});
			if (result.Replace("\"", "") != "201.1")
				return result.Replace("\"", "").Replace("BadRequest :", "");

			HttpContext.Session.Remove(emailKey);
			HttpContext.Session.Remove(tenantKey);
			HttpContext.Session.Remove(expiryKey);
			if (HttpContext.Session.GetString("NalamVazhaloginUserID") == Peopleid)
				HttpContext.Session.SetString("NalamVazhausername", email);
			return "Success";
		}
        private async Task MergePeopleUploads(PeopleModel model)
        {
            foreach (var file in Request.Form.Files)
            {
                var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var extension = Path.GetExtension(filename);
                if (string.IsNullOrWhiteSpace(extension)) continue;
                var safeName = System.Text.RegularExpressions.Regex.Replace(Path.GetFileNameWithoutExtension(filename), @"[^0-9a-zA-Z_.]+", "");
                var uploadName = safeName + "_People_" + Guid.NewGuid().ToString("N") + extension;
                using var stream = file.OpenReadStream();
                var fileUrl = await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadName);
                var storedValue = "|" + fileUrl + "|";
                if (file.Name == "photo") model.photo += storedValue;
                else if (file.Name == "iddocument") model.iddocument += storedValue;
                else if (file.Name == "licenceupload") model.licenceupload += storedValue;
            }
        }

        private void EnsurePeopleTenant(PeopleModel model)
        {
            if (model.tenantid.HasValue && model.tenantid.Value != Guid.Empty) return;
            // "NalamVazhachoosedtenantid" is deliberately blanked out at login for admins who
            // must pick a tenant from the UI (multi-tenant/showtenant="Y"); an empty string there
            // is not a usable fallback, so treat it the same as "not set" and fall through to the
            // admin's home tenant instead of leaving model.tenantid null.
            var sessionTenant = HttpContext.Session.GetString("NalamVazhachoosedtenantid");
            if (string.IsNullOrWhiteSpace(sessionTenant))
                sessionTenant = HttpContext.Session.GetString("NalamVazhatenantid");
            if (Guid.TryParse(sessionTenant, out var tenantId) && tenantId != Guid.Empty)
                model.tenantid = tenantId;
        }
        public virtual async Task<IActionResult> Remove_People(string Peopleid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/People/Remove_People?Peopleid="+Peopleid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - People / Remove_People, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='People' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult View_People()
                                        {
                                            return View();
                                        }

			        public virtual IActionResult People_List()
			        {
				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_People_List(string tenantid
, string workprofile = "", int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
			        {
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/People_List?tenantid="+tenantid+"&workprofile="+workprofile+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
			        }
			          
											[HttpGet()]
											public virtual async Task<string> get_all_WorkProfile(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/WorkProfile/get_all_WorkProfile?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Competency(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Competency/get_all_Competency?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_ClinicalTask(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalTask/get_all_ClinicalTask?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Designation(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Designation/get_all_Designation?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Country(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Country/get_all_Country?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_People(string Peopleid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/getById_allinfo_People?Peopleid="+Peopleid);
					 
			  }
[HttpGet()]
			    public virtual async Task<string> lookup_People_workprofile(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/lookup_People_workprofile?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_People_competencylevel(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/lookup_People_competencylevel?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_People_clinicaltask(String tenantid,String workprofile,String competencylevel)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/lookup_People_clinicaltask?tenantid="+tenantid+"&workprofile="+workprofile+"&competencylevel="+competencylevel+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_People_designation(String tenantid,String workprofile)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/lookup_People_designation?tenantid="+tenantid+"&workprofile="+workprofile+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_People_specifycountry()
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/lookup_People_specifycountry?loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }

[HttpGet()]
                        public virtual async Task<string> lookup_People_clinicaltaskinfo_consultations(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/lookup_People_clinicaltaskinfo_consultations?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_People_clinicaltaskinfo_workprofile(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/lookup_People_clinicaltaskinfo_workprofile?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_People_clinicaltaskinfo_tasktype(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/lookup_People_clinicaltaskinfo_tasktype?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_People_clinicaltaskinfo_taskname(String tenantid,String tasktype)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/lookup_People_clinicaltaskinfo_taskname?tenantid="+tenantid+"&tasktype="+tasktype+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }

 
											[HttpGet()]
											public virtual async Task<string> get_all_(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api//get_all_?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
[HttpGet()]
			    public virtual async Task<string> lookup_People_taskname(String tenantid,String tasktype)
			    {
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/lookup_People_taskname?tenantid="+tenantid+"&tasktype="+tasktype+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    } 
											[HttpGet()]
											public virtual async Task<string> get_all_Task(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Task/get_all_Task?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
 
											[HttpGet()]
											public virtual async Task<string> get_all_TaskType(string tenantid)
											{

											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/TaskType/get_all_TaskType?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}

[HttpGet()]
			public virtual async Task<string> GetPincodeList(string pincode, int? pagesize = 100, int? pagenumber = 0)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/PincodeMaster/Pincode_List?pincode=" + (string.IsNullOrEmpty(pincode) ? "" : Uri.EscapeDataString(pincode)) + "&pagesize=" + (pagesize ?? 100) + "&pagenumber=" + (pagenumber ?? 0) + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}




				}


			}
