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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:22
				public class usersController : BaseController
				{
				    public usersController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<usersController>();
					     _connectionSettings = connectionSettings;
					     objusersDAL = new usersDAL(_connectionSettings.Value.ConnectionString);
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
				private usersDAL objusersDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    [AllowAnonymous()]
            [HttpPost()]
            [ActionName("Register_Profile")]
            public virtual IActionResult Register_Profile([FromBody]usersModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Register_Profile";
            try{

            if (ModelState.IsValid)
            {

            	usersModelValidator validator = new usersModelValidator();
            	ValidationResult results = validator.Validate(model);
            	if (!results.IsValid)
            	{
            		var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}",e.PropertyName)));
             		message = ("Validation Error : " + errorCollection);


            	}else{

                                   var authHeader = HttpContext.Request.Headers["Authorization"][0];
                                if (authHeader.StartsWith("Bearer "))
                                {
                                     
		                      
                                var token = authHeader.Substring("Bearer ".Length);
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
                                model.createduser=new Guid(userdetails[0].ToString());
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());

		       
                                 

                                
            		                
                                     message = objusersDAL.Register_Profile(model);
                               }
                                else{
                                  message = "Invalid Token";
                                 }

            	}


            }
            else
            {
            	var errorCollection = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            	message = errorCollection.ToString();

            	_logger.LogError("usersModel - Register_Profile , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Register_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
            }
            if(obj_access_logsdetailsModel.access_logsid !=null)
            {
                 obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                 obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
            }

            if(message.Replace("\"","").Contains("201.1"))
                    return Ok(message);
                    else if(message.Replace("\"","")=="401.1")
                    return Unauthorized(message);
                    else
                    return BadRequest(message);


             }
[HttpGet()]
			  [ActionName("getById_users")]
			  public virtual usersModel getById_users(string usersid,string loginUserID="")
			  { 
				    usersModel objusers = new usersModel();
					try
					{
						  objusers = objusersDAL.getById_users(usersid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_users, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objusers;

			  }
			  [HttpPost()]
			  [ActionName("Update_Profile")]
			  public virtual IActionResult Update_Profile([FromBody]usersModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Profile";

					try{

					if (ModelState.IsValid)
					{

						usersModelValidator validator = new usersModelValidator();
						ValidationResult results = validator.Validate(model);
						if (!results.IsValid)
						{
							var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}",e.PropertyName)));
							message = errorCollection.ToString();
							//return BadRequest("Validation Error : " + message);

						}else{
                            var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                       
                                 
		                      
                                var token = authHeader.Substring("Bearer ".Length);
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
                                model.modifieduser=new Guid(userdetails[0].ToString());
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());

		       
                                 
		       
                                	
							    message = objusersDAL.Update_Profile(model);	
                            }
                            else{
                                message = "Invalid Token";
                                 
                            }
							
						}


					}
					else
					{
						var errorCollection = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
						message = errorCollection.ToString();

						_logger.LogError("usersModel - Update_Profile, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
					}

                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                            obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                            obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }

					if(message.Replace("\"","")=="201.1")
					return Ok(message);
					else if(message.Replace("\"","")=="401.1")
					return Unauthorized(message);
					else
					return BadRequest(message);

					


			   }


        [AllowAnonymous]
        [HttpPost]
        [ActionName("RequestPatientOTP")]
        public virtual async Task<IActionResult> RequestPatientOTP([FromBody] userloginModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.username))
                    return BadRequest("Email address is required.");

                // Validate email format
               

                // Generate 6-digit OTP
                int otp = new Random().Next(100000, 999999);

                // Save OTP to otplogs table using stored procedure
                DataTable dt = objusersDAL.ManageOTP(model.username, otp.ToString(), "SAVE");

                if (dt != null && dt.Rows.Count > 0)
                {
                    string status = dt.Rows[0]["status"].ToString();
                    string message = dt.Rows[0]["message"].ToString();
                    string otplogsid = dt.Rows[0]["rotplogsid"].ToString();

                    if (status == "SUCCESS")
                    {
                        string tenantid = dt.Columns.Contains("tenantid") ? dt.Rows[0]["tenantid"]?.ToString() : "";
                        // OTP saved successfully
                        return Ok(new
                        {
                            success = true,
                            otp = otp.ToString(),
                            email = model.username,
                            otpsid = otplogsid,
                            tenantid = tenantid,
                            message = "OTP sent successfully to your email"
                        });
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = message });
                    }
                }

                return BadRequest(new { success = false, message = "Failed to generate OTP. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RequestPatientOTP");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ActionName("VerifyPatientOTP")]
        public virtual async Task<IActionResult> VerifyPatientOTP([FromBody] JObject data)
        {
            try
            {
                string email = data["email"]?.ToString();
                string otp = data["otp"]?.ToString();

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
                    return BadRequest(new { success = false, message = "Email and OTP are required." });

                // Validate OTP format (should be 6 digits)
                if (!System.Text.RegularExpressions.Regex.IsMatch(otp, @"^\d{6}$"))
                    return BadRequest(new { success = false, message = "OTP must be 6 digits." });

                // Verify OTP from otplogs table
                DataTable dtVerify = objusersDAL.ManageOTP(email, otp, "VERIFY");

                if (dtVerify != null && dtVerify.Rows.Count > 0)
                {
                    string status = dtVerify.Rows[0]["status"].ToString();
                    string message = dtVerify.Rows[0]["message"].ToString();

                    if (status == "SUCCESS")
                    {
                        // OTP verified successfully
                        return Ok(new
                        {
                            success = true,
                            message = "OTP verified successfully",
                            email = email,
                            redirect = true
                        });
                    }
                    else
                    {
                        // OTP verification failed
                        return BadRequest(new { success = false, message = message });
                    }
                }

                return BadRequest(new { success = false, message = "Invalid or Expired OTP. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in VerifyPatientOTP");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ActionName("ManageProfileEmailChange")]
        public virtual IActionResult ManageProfileEmailChange([FromBody] JObject data)
        {
            if (!Guid.TryParse(pvar_usersid, out var modifiedUser))
                return Unauthorized("API Authorization failed");

            if (!Guid.TryParse(data["profileid"]?.ToString(), out var profileId)
                || !Guid.TryParse(data["tenantid"]?.ToString(), out var tenantId))
                return BadRequest("Invalid profile or tenant.");

            var apply = data["apply"]?.Value<bool>() == true;
            if (apply)
            {
                var email = data["newemail"]?.ToString();
                var otp = data["otp"]?.ToString();
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp)
                    || !System.Text.RegularExpressions.Regex.IsMatch(otp, @"^\d{6}$"))
                    return BadRequest("A valid 6-digit OTP is required.");

                var verification = objusersDAL.ManageOTP(email, otp, "VERIFY");
                if (verification == null || verification.Rows.Count == 0
                    || !string.Equals(verification.Rows[0]["status"]?.ToString(), "SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    var verificationMessage = verification != null && verification.Rows.Count > 0
                        ? verification.Rows[0]["message"]?.ToString()
                        : "Invalid or expired OTP.";
                    return BadRequest(verificationMessage);
                }
            }

            var message = objusersDAL.ManageProfileEmailChange(
                data["profiletype"]?.ToString(), profileId, tenantId,
                data["newemail"]?.ToString(), modifiedUser,
                apply);

            return message == "201.1" ? Ok(message) : BadRequest(message);
        }

        // Helper method to validate email format
        


        [HttpGet()]
            public virtual async Task<string> Suspend_Profile(string usersid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Suspend_Profile";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objusersDAL.Suspend_Profile(usersid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Suspend_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}

            [AllowAnonymous()]
		[HttpPost()]
		[ActionName("get_decryptedPassword")]
		public virtual string get_decryptedPassword([FromBody] userloginModel model)
		{
			string decryptedPassword = objusersDAL.get_decryptedPassword(model.username?.Trim(), model.tenantid);

			return decryptedPassword;
		}
			[AllowAnonymous()]
[HttpGet()]
[ActionName("get_roleAuthorizations")]
public virtual System.Data.DataTable get_roleAuthorizations(string viewactionroles)
{
	 
	DataTable dtusers = new DataTable();
	try
	{
		dtusers = objusersDAL.get_roleAuthorizations(viewactionroles);
	}
	catch (Exception ex)
	{
         _logger.LogError(ex,"An exception occurred in - get_roleAuthorizations, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
	}
	return dtusers;

}
[AllowAnonymous()]
[HttpGet()]
[ActionName("get_project_Menu")]
public virtual System.Data.DataTable get_project_Menu(string viewactionroles,string SubSystem)
{
	 
	DataTable dtusers = new DataTable();
	try
	{
		dtusers = objusersDAL.get_project_Menu(viewactionroles,SubSystem);
	}
	catch (Exception ex)
	{
         _logger.LogError(ex,"An exception occurred in - get_project_Menu, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
  
	}
	return dtusers;

}
			[AllowAnonymous()]
        [HttpGet()]
		[ActionName("get_roles")]
		public virtual System.Data.DataTable get_roles(string roles,string parentname)
		{
			    DataTable dt= objusersDAL.get_roles(roles,parentname);
				return dt;
		}
			
			
			[AllowAnonymous()]
		[HttpPost()]
		[ActionName("CheckAuthentication")]
		public virtual System.Data.DataTable CheckAuthentication([FromBody]userloginModel model)
		{
			    model.username = model.username?.Trim();
			    DataTable dt= objusersDAL.CheckAuthentication(model);
				if (dt.Rows.Count > 0)
				{
				dt.Columns.Add("token");


     			 var userClaims = new Dictionary<string, object>
                {
                    { "username", model.username },
                    { "usersid",  dt.Rows[0]["usersid"].ToString() },
                    { "tenantid",  dt.Rows[0]["tenantid"].ToString() }
                };
                
                var claims = new List<Claim>();
                foreach (var claim in userClaims)
                {
                    claims.Add(new Claim(claim.Key, claim.Value.ToString()));
                }


				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
				var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
				var token = new JwtSecurityToken(
				issuer: _configuration["ValidIssuer"],
				audience: "yourdomain.com",
				claims: claims,
				expires: DateTime.Now.AddMinutes(3000),
				signingCredentials: creds);
				string token_ = new JwtSecurityTokenHandler().WriteToken(token);
				dt.Rows[0]["token"] = token_;


                         
                        access_logsModel objExternalapilogsModel = new access_logsModel();
                        objExternalapilogsModel.logged_date = DateTime.Now;
                        objExternalapilogsModel.expiry_date = DateTime.Now.AddMinutes(60);
                        objExternalapilogsModel.external_entity_name = "";
                        objExternalapilogsModel.user_token = token_;
                        objExternalapilogsModel.users_id =dt.Rows[0]["usersid"].ToString();
                        objExternalapilogsModel.latlan = "";
                        objExternalapilogsModel.clientipaddress = "";
                        objExternalapilogsModel.devicename = "";
                        objExternalapilogsModel.browsername = "";
                        objExternalapilogsModel.external_users_id = "";
                        objExternalapilogsModel.createduser = new Guid(dt.Rows[0]["usersid"].ToString());
                        obj_External_System_DAL.Create_access_logs(objExternalapilogsModel);

                        ExternalSystemUtility objExternalSystemUtitlity = new ExternalSystemUtility(_connectionSettings, _configuration);
				        objExternalSystemUtitlity.Insert_external_tokens(dt.Rows[0]["usersid"].ToString(),model);

                }
				return dt
				;
		}
        [HttpGet()]
		[ActionName("get_Dashboard_Items")]
		public virtual System.Data.DataTable get_Dashboard_Items(string viewactionroles)
		{
			DataTable dtusers = new DataTable();
			try
			{
				dtusers = objusersDAL.get_Dashboard_Items(viewactionroles);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - get_Dashboard_Items, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtusers;

		}
[HttpGet()]
			
			[ActionName("List_of_User_Profiles")]
			public virtual System.Data.DataTable List_of_User_Profiles(string tenantid
)
			{
					 
				  	DataTable dtusers = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtusers = objusersDAL.List_of_User_Profiles( tenantid
);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - List_of_User_Profiles, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                       
					}
					return dtusers;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_users")]
			public virtual System.Data.DataTable get_all_users(string tenantid,string loginUserID="")
			{
					 
				  	DataTable dtusers = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtusers = objusersDAL.get_all_users(tenantid);
					}
					catch (Exception ex)
					{
                        _logger.LogError(ex,"An exception occurred in - get_all_users, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					   
					}
					return dtusers;

			   }
 
			  [HttpPost()]
			  [ActionName("ChangePassword")]
			  public virtual string ChangePassword([FromBody]usersChangePasswordModel model)
			  { 
				  string message = "";
					try{
						
						  message = objusersDAL.ChangePassword(model);	
						 

					}catch(Exception ex){
						message=ex.Message;
                          _logger.LogError(ex,"An exception occurred in - ChangePassword, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					}
					
					return message;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_users")]
			  public virtual System.Data.DataTable getById_allinfo_users(string usersid)
			  { 
				    DataTable dtusers = new DataTable();
					try
					{
						  dtusers = objusersDAL.getById_allinfo_users(usersid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_users, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtusers;

			  }






				}


			}
