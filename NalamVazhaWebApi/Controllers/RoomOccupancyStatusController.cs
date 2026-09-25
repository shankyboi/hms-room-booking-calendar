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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:04
				public class RoomOccupancyStatusController : BaseController
				{
				    public RoomOccupancyStatusController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<RoomOccupancyStatusController>();
					     _connectionSettings = connectionSettings;
					     objRoomOccupancyStatusDAL = new RoomOccupancyStatusDAL(_connectionSettings.Value.ConnectionString);
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
				private RoomOccupancyStatusDAL objRoomOccupancyStatusDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    
            [HttpPost()]
            [ActionName("Add_Room_Occupancy_Status")]
            public virtual IActionResult Add_Room_Occupancy_Status([FromBody]RoomOccupancyStatusModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Room_Occupancy_Status";
            try{

            if (ModelState.IsValid)
            {

            	RoomOccupancyStatusModelValidator validator = new RoomOccupancyStatusModelValidator();
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

		       
                                 

                                
            		                
                                     message = objRoomOccupancyStatusDAL.Add_Room_Occupancy_Status(model);
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

            	_logger.LogError("RoomOccupancyStatusModel - Add_Room_Occupancy_Status , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Room_Occupancy_Status, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_RoomOccupancyStatus")]
			  public virtual RoomOccupancyStatusModel getById_RoomOccupancyStatus(string RoomOccupancyStatusid,string loginUserID="")
			  { 
				    RoomOccupancyStatusModel objRoomOccupancyStatus = new RoomOccupancyStatusModel();
					try
					{
						  objRoomOccupancyStatus = objRoomOccupancyStatusDAL.getById_RoomOccupancyStatus(RoomOccupancyStatusid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_RoomOccupancyStatus, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objRoomOccupancyStatus;

			  }
			  [HttpPost()]
			  [ActionName("Update_Room_Occupancy_Status")]
			  public virtual IActionResult Update_Room_Occupancy_Status([FromBody]RoomOccupancyStatusModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Room_Occupancy_Status";

					try{

					if (ModelState.IsValid)
					{

						RoomOccupancyStatusModelValidator validator = new RoomOccupancyStatusModelValidator();
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

		       
                                 
		       
                                	
							    message = objRoomOccupancyStatusDAL.Update_Room_Occupancy_Status(model);	
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

						_logger.LogError("RoomOccupancyStatusModel - Update_Room_Occupancy_Status, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Room_Occupancy_Status, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Room_Occupancy_Status(string RoomOccupancyStatusid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Room_Occupancy_Status";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objRoomOccupancyStatusDAL.Remove_Room_Occupancy_Status(RoomOccupancyStatusid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Room_Occupancy_Status, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Room_Occupancy_Status_List")]
			public virtual JObject Room_Occupancy_Status_List(string tenantid
,string patientvisit=""
,string patientname=""
,string ipdno=""
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "", bool returnAllRecords=false)
			{
					 
				  	JObject jsonRoomOccupancyStatus = null;
					try
					{
                        tenantid=pvar_tenantid;
						if(returnAllRecords || pagesize == 1000)
						{
							pagesize = null;
							pagenumber = null;
						}
						jsonRoomOccupancyStatus = objRoomOccupancyStatusDAL.Room_Occupancy_Status_List( tenantid
, patientvisit
, patientname
, ipdno
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Room_Occupancy_Status_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonRoomOccupancyStatus;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_RoomOccupancyStatus")]
			public virtual System.Data.DataTable get_all_RoomOccupancyStatus(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtRoomOccupancyStatus = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtRoomOccupancyStatus = objRoomOccupancyStatusDAL.get_all_RoomOccupancyStatus(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_RoomOccupancyStatus, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtRoomOccupancyStatus;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_RoomOccupancyStatus")]
			  public virtual System.Data.DataTable getById_allinfo_RoomOccupancyStatus(string RoomOccupancyStatusid)
			  { 
				    DataTable dtRoomOccupancyStatus = new DataTable();
					try
					{
						  dtRoomOccupancyStatus = objRoomOccupancyStatusDAL.getById_allinfo_RoomOccupancyStatus(RoomOccupancyStatusid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_RoomOccupancyStatus, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtRoomOccupancyStatus;

			  }
[HttpGet()]
							[ActionName("lookup_RoomOccupancyStatus_patientvisit")]
							public virtual System.Data.DataTable lookup_RoomOccupancyStatus_patientvisit(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtRoomOccupancyStatus = new DataTable();
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
										dtRoomOccupancyStatus = objRoomOccupancyStatusDAL.lookup_RoomOccupancyStatus_patientvisit(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_RoomOccupancyStatus_patientvisit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomOccupancyStatus;
							}
[HttpGet()]
							[ActionName("lookup_RoomOccupancyStatus_patientname")]
							public virtual System.Data.DataTable lookup_RoomOccupancyStatus_patientname(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtRoomOccupancyStatus = new DataTable();
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
										dtRoomOccupancyStatus = objRoomOccupancyStatusDAL.lookup_RoomOccupancyStatus_patientname(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_RoomOccupancyStatus_patientname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomOccupancyStatus;
							}
[HttpGet()]
							[ActionName("lookup_RoomOccupancyStatus_ipdno")]
							public virtual System.Data.DataTable lookup_RoomOccupancyStatus_ipdno(String tenantid,String patientname,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtRoomOccupancyStatus = new DataTable();
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
										dtRoomOccupancyStatus = objRoomOccupancyStatusDAL.lookup_RoomOccupancyStatus_ipdno(tenantid,patientname,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_RoomOccupancyStatus_ipdno, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomOccupancyStatus;
							}
[HttpGet()]
							[ActionName("lookup_RoomOccupancyStatus_block")]
							public virtual System.Data.DataTable lookup_RoomOccupancyStatus_block(String tenantid,string loginUserID="")
							{
									DataTable dtRoomOccupancyStatus = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoomOccupancyStatus = objRoomOccupancyStatusDAL.lookup_RoomOccupancyStatus_block(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_RoomOccupancyStatus_block, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomOccupancyStatus;
							}
[HttpGet()]
							[ActionName("lookup_RoomOccupancyStatus_building")]
							public virtual System.Data.DataTable lookup_RoomOccupancyStatus_building(String tenantid,String block,string loginUserID="")
							{
									DataTable dtRoomOccupancyStatus = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoomOccupancyStatus = objRoomOccupancyStatusDAL.lookup_RoomOccupancyStatus_building(tenantid,block);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_RoomOccupancyStatus_building, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomOccupancyStatus;
							}
[HttpGet()]
							[ActionName("lookup_RoomOccupancyStatus_floor")]
							public virtual System.Data.DataTable lookup_RoomOccupancyStatus_floor(String tenantid,String block,String building,string loginUserID="")
							{
									DataTable dtRoomOccupancyStatus = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoomOccupancyStatus = objRoomOccupancyStatusDAL.lookup_RoomOccupancyStatus_floor(tenantid,block,building);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_RoomOccupancyStatus_floor, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomOccupancyStatus;
							}
[HttpGet()]
							[ActionName("lookup_RoomOccupancyStatus_room")]
							public virtual System.Data.DataTable lookup_RoomOccupancyStatus_room(String tenantid,String block,String building,String floor,string loginUserID="")
							{
									DataTable dtRoomOccupancyStatus = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoomOccupancyStatus = objRoomOccupancyStatusDAL.lookup_RoomOccupancyStatus_room(tenantid,block,building,floor);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_RoomOccupancyStatus_room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomOccupancyStatus;
							}

[HttpGet()]
							[ActionName("lookup_change_RoomOccupancyStatus_patientvisit")]
							public virtual System.Data.DataTable lookup_change_RoomOccupancyStatus_patientvisit(string PatientVisitid,string loginUserID="")
							{
									DataTable dtPatientVisit = new DataTable();
									try
									{
										dtPatientVisit = objRoomOccupancyStatusDAL.lookup_change_RoomOccupancyStatus_patientvisit(PatientVisitid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_change_RoomOccupancyStatus_patientvisit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientVisit;
							}





				}


			}
