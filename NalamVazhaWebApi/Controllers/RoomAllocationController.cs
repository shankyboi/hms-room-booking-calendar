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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:10
				public class RoomAllocationController : BaseController
				{
				    public RoomAllocationController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<RoomAllocationController>();
					     _connectionSettings = connectionSettings;
					     objRoomAllocationDAL = new RoomAllocationDAL(_connectionSettings.Value.ConnectionString);
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
				private RoomAllocationDAL objRoomAllocationDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

				    
            [HttpPost]
            [ActionName("SetManualRoomAvailability")]
            public virtual IActionResult SetManualRoomAvailability([FromBody] ManualRoomAvailabilityModel model)
            {
                try
                {
                    var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
                    if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        return Unauthorized("Invalid Token");

                    var token = authHeader.Substring("Bearer ".Length);
                    var userDetails = obj_External_System_DAL.get_users_by_token(token);
                    if (userDetails == null || userDetails.Length == 0 || !Guid.TryParse(userDetails[0]?.ToString(), out var userId))
                        return Unauthorized("Invalid Token");

                    var message = objRoomAllocationDAL.SetManualRoomAvailability(model, userId);
                    return message == "201.1" ? Ok(message) : BadRequest(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SetManualRoomAvailability failed");
                    return StatusCode(500, ex.Message);
                }
            }

            [HttpPost()]
            [ActionName("Add_Room_Allocation")]
            public virtual IActionResult Add_Room_Allocation([FromBody]RoomAllocationModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Room_Allocation";
            try{

            if (ModelState.IsValid)
            {

            	RoomAllocationModelValidator validator = new RoomAllocationModelValidator();
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

		       
                                 

                                
            		                
                                     message = objRoomAllocationDAL.Add_Room_Allocation(model);
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

            	_logger.LogError("RoomAllocationModel - Add_Room_Allocation , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Room_Allocation, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_RoomAllocation")]
			  public virtual RoomAllocationModel getById_RoomAllocation(string RoomAllocationid,string loginUserID="")
			  { 
				    RoomAllocationModel objRoomAllocation = new RoomAllocationModel();
					try
					{
						  objRoomAllocation = objRoomAllocationDAL.getById_RoomAllocation(RoomAllocationid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_RoomAllocation, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objRoomAllocation;

			  }
			  [HttpPost()]
			  [ActionName("Update_Room_Allocation")]
			  public virtual IActionResult Update_Room_Allocation([FromBody]RoomAllocationModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Room_Allocation";

					try{

					if (ModelState.IsValid)
					{

						RoomAllocationModelValidator validator = new RoomAllocationModelValidator();
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

		       
                                 
		       
                                	
							    message = objRoomAllocationDAL.Update_Room_Allocation(model);	
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

						_logger.LogError("RoomAllocationModel - Update_Room_Allocation, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Room_Allocation, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Room_Allocation(string RoomAllocationid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Room_Allocation";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objRoomAllocationDAL.Remove_Room_Allocation(RoomAllocationid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Room_Allocation, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Room_Allocation_List")]
			public virtual JObject Room_Allocation_List(string tenantid
,string loginUserID="",string ipdno="",string block="",string building="",string floor="",string room="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonRoomAllocation = null;
					try
					{
						tenantid=pvar_tenantid;
						jsonRoomAllocation = objRoomAllocationDAL.Room_Allocation_List( tenantid
,ipdno,block,building,floor,room,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Room_Allocation_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonRoomAllocation;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_RoomAllocation")]
			public virtual System.Data.DataTable get_all_RoomAllocation(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtRoomAllocation = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtRoomAllocation = objRoomAllocationDAL.get_all_RoomAllocation(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_RoomAllocation, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtRoomAllocation;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_RoomAllocation")]
			  public virtual System.Data.DataTable getById_allinfo_RoomAllocation(string RoomAllocationid)
			  { 
				    DataTable dtRoomAllocation = new DataTable();
					try
					{
						  dtRoomAllocation = objRoomAllocationDAL.getById_allinfo_RoomAllocation(RoomAllocationid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_RoomAllocation, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtRoomAllocation;

			  }
[HttpGet()]
							[ActionName("lookup_RoomAllocation_ipdno")]
							public virtual System.Data.DataTable lookup_RoomAllocation_ipdno(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtRoomAllocation = new DataTable();
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
										dtRoomAllocation = objRoomAllocationDAL.lookup_RoomAllocation_ipdno(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_RoomAllocation_ipdno, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomAllocation;
							}
[HttpGet()]
							[ActionName("lookup_RoomAllocation_block")]
							public virtual System.Data.DataTable lookup_RoomAllocation_block(String tenantid,string loginUserID="")
							{
									DataTable dtRoomAllocation = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoomAllocation = objRoomAllocationDAL.lookup_RoomAllocation_block(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_RoomAllocation_block, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomAllocation;
							}
[HttpGet()]
							[ActionName("lookup_RoomAllocation_building")]
							public virtual System.Data.DataTable lookup_RoomAllocation_building(String tenantid,String block,string loginUserID="")
							{
									DataTable dtRoomAllocation = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoomAllocation = objRoomAllocationDAL.lookup_RoomAllocation_building(tenantid,block);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_RoomAllocation_building, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomAllocation;
							}
[HttpGet()]
							[ActionName("lookup_RoomAllocation_floor")]
							public virtual System.Data.DataTable lookup_RoomAllocation_floor(String tenantid,String block,String building,string loginUserID="")
							{
									DataTable dtRoomAllocation = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoomAllocation = objRoomAllocationDAL.lookup_RoomAllocation_floor(tenantid,block,building);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_RoomAllocation_floor, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomAllocation;
							}
[HttpGet()]
							[ActionName("lookup_RoomAllocation_room")]
							public virtual System.Data.DataTable lookup_RoomAllocation_room(String tenantid,String block,String building,String floor,string loginUserID="")
							{
									DataTable dtRoomAllocation = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoomAllocation = objRoomAllocationDAL.lookup_RoomAllocation_room(tenantid,block,building,floor);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_RoomAllocation_room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomAllocation;
							}






				}


			}
