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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:20
				public class ShiftPlanningController : BaseController
				{
				    public ShiftPlanningController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<ShiftPlanningController>();
					     _connectionSettings = connectionSettings;
					     objShiftPlanningDAL = new ShiftPlanningDAL(_connectionSettings.Value.ConnectionString);
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
				private ShiftPlanningDAL objShiftPlanningDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    [HttpGet()]
			  [ActionName("getById_people")]
			  public virtual System.Data.DataTable getById_people(string ShiftPlanningid)
			  {
					DataTable dtShiftPlanning_people = new DataTable();
					try
					{
						dtShiftPlanning_people = objShiftPlanningDAL.getById_people(ShiftPlanningid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_people, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtShiftPlanning_people;


			  }


            [HttpPost()]
            [ActionName("Add_Shift_Planning")]
            public virtual IActionResult Add_Shift_Planning([FromBody]ShiftPlanningModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Shift_Planning";
            try{

            if (ModelState.IsValid)
            {

            	ShiftPlanningModelValidator validator = new ShiftPlanningModelValidator();
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

		       
                                 

                                
            		                
                                     message = objShiftPlanningDAL.Add_Shift_Planning(model);
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

            	_logger.LogError("ShiftPlanningModel - Add_Shift_Planning , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Shift_Planning, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_ShiftPlanning")]
			  public virtual ShiftPlanningModel getById_ShiftPlanning(string ShiftPlanningid,string loginUserID="")
			  { 
				    ShiftPlanningModel objShiftPlanning = new ShiftPlanningModel();
					try
					{
						  objShiftPlanning = objShiftPlanningDAL.getById_ShiftPlanning(ShiftPlanningid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_ShiftPlanning, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objShiftPlanning;

			  }
			  [HttpPost()]
			  [ActionName("Update_Shift_Planning")]
			  public virtual IActionResult Update_Shift_Planning([FromBody]ShiftPlanningModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Shift_Planning";

					try{

					if (ModelState.IsValid)
					{

						ShiftPlanningModelValidator validator = new ShiftPlanningModelValidator();
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

		       
                                 
		       
                                	
							    message = objShiftPlanningDAL.Update_Shift_Planning(model);	
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

						_logger.LogError("ShiftPlanningModel - Update_Shift_Planning, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Shift_Planning, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
[HttpGet()]
            public virtual async Task<string> Remove_Shift_Planning(string ShiftPlanningid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Shift_Planning";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objShiftPlanningDAL.Remove_Shift_Planning(ShiftPlanningid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Shift_Planning, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}

            [HttpPost()]
            [ActionName("Upload_Shift_Planning")]
            public virtual IActionResult Upload_Shift_Planning([FromBody]ShiftPlanningModel model)
            {
                string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                obj_access_logsdetailsModel.action_method_name = "Upload_Shift_Planning";
                try
                {
                    var authHeader = HttpContext.Request.Headers["Authorization"][0];
                    if (authHeader.StartsWith("Bearer"))
                    {
                        var token = authHeader.Substring("Bearer ".Length);
                    String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
                    model.createduser = new Guid(userdetails[0].ToString());
                    obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());
                        
                        message = objShiftPlanningDAL.Upload_Shift_Planning(model);
                    }
                    else
                    {
                        message = "Invalid Token";
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    _logger.LogError(ex, "An error occurred in - { ActionMethodName}, Error Message: " + ex.Message);
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
			
			[ActionName("Shift_Planning_List")]
			public virtual JObject Shift_Planning_List(string tenantid
,string shiftname=""
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonShiftPlanning = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonShiftPlanning = objShiftPlanningDAL.Shift_Planning_List( tenantid
, shiftname
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Shift_Planning_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonShiftPlanning;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_ShiftPlanning")]
			public virtual System.Data.DataTable get_all_ShiftPlanning(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtShiftPlanning = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtShiftPlanning = objShiftPlanningDAL.get_all_ShiftPlanning(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_ShiftPlanning, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtShiftPlanning;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_ShiftPlanning")]
			  public virtual System.Data.DataTable getById_allinfo_ShiftPlanning(string ShiftPlanningid)
			  { 
				    DataTable dtShiftPlanning = new DataTable();
					try
					{
						  dtShiftPlanning = objShiftPlanningDAL.getById_allinfo_ShiftPlanning(ShiftPlanningid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_ShiftPlanning, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtShiftPlanning;

			  }
[HttpGet()]
							[ActionName("lookup_ShiftPlanning_shiftname")]
							public virtual System.Data.DataTable lookup_ShiftPlanning_shiftname(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtShiftPlanning = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
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
										dtShiftPlanning = objShiftPlanningDAL.lookup_ShiftPlanning_shiftname(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ShiftPlanning_shiftname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtShiftPlanning;
							}
[HttpGet()]
							[ActionName("lookup_ShiftPlanning_people_personname")]
							public virtual System.Data.DataTable lookup_ShiftPlanning_people_personname(String tenantid, String workprofile, string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtShiftPlanning = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
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
										dtShiftPlanning = objShiftPlanningDAL.lookup_ShiftPlanning_people_personname(tenantid,workprofile,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ShiftPlanning_people_personname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtShiftPlanning;
							}
[HttpGet()]
							[ActionName("lookup_ShiftPlanning_people_workprofile")]
							public virtual System.Data.DataTable lookup_ShiftPlanning_people_workprofile(String tenantid,string loginUserID="")
							{
									DataTable dtShiftPlanning = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtShiftPlanning = objShiftPlanningDAL.lookup_ShiftPlanning_people_workprofile(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ShiftPlanning_people_workprofile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtShiftPlanning;
							}


[HttpGet()]
							[ActionName("lookup_change_people_ShiftPlanning_personname")]
							public virtual System.Data.DataTable lookup_change_people_ShiftPlanning_personname(string Peopleid,string loginUserID="")
							{
									DataTable dtPeople = new DataTable();
									try
									{
										dtPeople = objShiftPlanningDAL.lookup_change_people_ShiftPlanning_personname(Peopleid);
									}
									catch (Exception ex)
									{
									  _logger.LogError(ex,"An exception occurred in - lookup_change_people_ShiftPlanning_personname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPeople;
							}

		[HttpGet()]
		[ActionName("lookup_change_ShiftPlanning_shiftname")]
		public virtual System.Data.DataTable lookup_change_ShiftPlanning_shiftname(string Shiftid, string loginUserID = "")
		{
			DataTable dtShift = new DataTable();
			try
			{
				dtShift = objShiftPlanningDAL.lookup_change_ShiftPlanning_shiftname(Shiftid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_change_ShiftPlanning_shiftname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtShift;
		}


	}


			}
