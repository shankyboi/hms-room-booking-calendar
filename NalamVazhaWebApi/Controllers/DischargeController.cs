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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:43:41
				public class DischargeController : BaseController
				{
				    public DischargeController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<DischargeController>();
					     _connectionSettings = connectionSettings;
					     objDischargeDAL = new DischargeDAL(_connectionSettings.Value.ConnectionString);
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
				private DischargeDAL objDischargeDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    
            [HttpPost()]
            [ActionName("Add_Discharge")]
            public virtual IActionResult Add_Discharge([FromBody]DischargeModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Discharge";
            try{

            if (ModelState.IsValid)
            {

            	DischargeModelValidator validator = new DischargeModelValidator();
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

		       
                                 

                                
            		                
                                     message = objDischargeDAL.Add_Discharge(model);
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

            	_logger.LogError("DischargeModel - Add_Discharge , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Discharge, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_Discharge")]
			  public virtual DischargeModel getById_Discharge(string Dischargeid,string loginUserID="")
			  { 
				    DischargeModel objDischarge = new DischargeModel();
					try
					{
						  objDischarge = objDischargeDAL.getById_Discharge(Dischargeid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_Discharge, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objDischarge;

			  }
			  [HttpPost()]
			  [ActionName("Update_Discharge")]
			  public virtual IActionResult Update_Discharge([FromBody]DischargeModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Discharge";

					try{

					if (ModelState.IsValid)
					{

						DischargeModelValidator validator = new DischargeModelValidator();
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

		       
                                 
		       
                                	
							    message = objDischargeDAL.Update_Discharge(model);	
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

						_logger.LogError("DischargeModel - Update_Discharge, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Discharge, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Discharge(string Dischargeid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Discharge";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objDischargeDAL.Remove_Discharge(Dischargeid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Discharge, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Discharge_List")]
			public virtual JObject Discharge_List(string tenantid
,string ipdnumber=""
,string patient=""
,string room=""
, string discharge = ""
, string paymentstatus=""
,string refundstatus=""
,string feedbackstatus=""
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonDischarge = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonDischarge = objDischargeDAL.Discharge_List( tenantid
, ipdnumber
, patient
, room
, discharge
, paymentstatus
, refundstatus
, feedbackstatus
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Discharge_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonDischarge;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_Discharge")]
			public virtual System.Data.DataTable get_all_Discharge(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtDischarge = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtDischarge = objDischargeDAL.get_all_Discharge(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_Discharge, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtDischarge;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_Discharge")]
			  public virtual System.Data.DataTable getById_allinfo_Discharge(string Dischargeid)
			  { 
				    DataTable dtDischarge = new DataTable();
					try
					{
						  dtDischarge = objDischargeDAL.getById_allinfo_Discharge(Dischargeid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_Discharge, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtDischarge;

			  }
[HttpGet()]
							[ActionName("lookup_Discharge_ipdnumber")]
							public virtual System.Data.DataTable lookup_Discharge_ipdnumber(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtDischarge = new DataTable();
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
										dtDischarge = objDischargeDAL.lookup_Discharge_ipdnumber(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Discharge_ipdnumber, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtDischarge;
							}
[HttpGet()]
							[ActionName("lookup_Discharge_patient")]
							public virtual System.Data.DataTable lookup_Discharge_patient(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtDischarge = new DataTable();
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
										dtDischarge = objDischargeDAL.lookup_Discharge_patient(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Discharge_patient, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtDischarge;
							}
[HttpGet()]
							[ActionName("lookup_Discharge_room")]
							public virtual System.Data.DataTable lookup_Discharge_room(String tenantid,string loginUserID="")
							{
									DataTable dtDischarge = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtDischarge = objDischargeDAL.lookup_Discharge_room(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Discharge_room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtDischarge;
							}
[HttpGet()]
							[ActionName("lookup_Discharge_discharge")]
							public virtual System.Data.DataTable lookup_Discharge_discharge(String tenantid,String ipdnumber,String room,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtDischarge = new DataTable();
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
										dtDischarge = objDischargeDAL.lookup_Discharge_discharge(tenantid,ipdnumber,room,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Discharge_discharge, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtDischarge;
							}
[HttpGet()]
							[ActionName("lookup_Discharge_pendingamount")]
							public virtual System.Data.DataTable lookup_Discharge_pendingamount(String tenantid,String ipdnumber,String patient,String room,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtDischarge = new DataTable();
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
										dtDischarge = objDischargeDAL.lookup_Discharge_pendingamount(tenantid,ipdnumber,patient,room,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Discharge_pendingamount, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtDischarge;
							}
[HttpGet()]
							[ActionName("lookup_Discharge_paymentstatus")]
							public virtual System.Data.DataTable lookup_Discharge_paymentstatus(String tenantid,String ipdnumber,String patient,String room,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtDischarge = new DataTable();
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
										dtDischarge = objDischargeDAL.lookup_Discharge_paymentstatus(tenantid,ipdnumber,patient,room,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Discharge_paymentstatus, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtDischarge;
							}
[HttpGet()]
							[ActionName("lookup_Discharge_refundamount")]
							public virtual System.Data.DataTable lookup_Discharge_refundamount(String tenantid,String ipdnumber,String patient,String room,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtDischarge = new DataTable();
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
										dtDischarge = objDischargeDAL.lookup_Discharge_refundamount(tenantid,ipdnumber,patient,room,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Discharge_refundamount, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtDischarge;
							}
[HttpGet()]
							[ActionName("lookup_Discharge_refundstatus")]
							public virtual System.Data.DataTable lookup_Discharge_refundstatus(String tenantid,String ipdnumber,String patient,String room,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtDischarge = new DataTable();
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
										dtDischarge = objDischargeDAL.lookup_Discharge_refundstatus(tenantid,ipdnumber,patient,room,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Discharge_refundstatus, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtDischarge;
							}






				}


			}
