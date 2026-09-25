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
using System.Globalization;
				[Route("api/[controller]/[action]")]
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:22
				public class RoomController : BaseController
				{
				    public RoomController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<RoomController>();
					     _connectionSettings = connectionSettings;
					     objRoomDAL = new RoomDAL(_connectionSettings.Value.ConnectionString);
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
				private RoomDAL objRoomDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    
            [HttpPost()]
            [ActionName("Add_Room")]
            public virtual IActionResult Add_Room([FromBody]RoomModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Room";
            try{

            if (ModelState.IsValid)
            {

            	RoomModelValidator validator = new RoomModelValidator();
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

		       
                                 

                                
            		                
                                     message = objRoomDAL.Add_Room(model);
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

            	_logger.LogError("RoomModel - Add_Room , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_Room")]
			  public virtual RoomModel getById_Room(string Roomid,string loginUserID="")
			  { 
				    RoomModel objRoom = new RoomModel();
					try
					{
						  objRoom = objRoomDAL.getById_Room(Roomid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_Room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objRoom;

			  }
			  [HttpPost()]
			  [ActionName("Update_Room")]
			  public virtual IActionResult Update_Room([FromBody]RoomModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Room";

					try{

					if (ModelState.IsValid)
					{

						RoomModelValidator validator = new RoomModelValidator();
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

		       
                                 
		       
                                	
							    message = objRoomDAL.Update_Room(model);	
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

						_logger.LogError("RoomModel - Update_Room, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Room(string Roomid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Room";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objRoomDAL.Remove_Room(Roomid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]

		[ActionName("Room_List")]
		public virtual System.Data.DataTable Room_List(string tenantid
, string block = ""
, string building = ""
, string floor = ""
, string roomtype = ""
, string roomnumber = ""
, string nextdaycheckin = ""
, string nextdaycheckout = ""
)
		{

			DataTable dtRoom = new DataTable();
			try
			{
				tenantid = pvar_tenantid;
				dtRoom = objRoomDAL.Room_List(tenantid
, block
, building
, floor
, roomtype
, roomnumber
, nextdaycheckin
, nextdaycheckout
);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - Room_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtRoom;

		}


		// Real availability based on RoomOccupancyStatus overlap (DB: get_room_list_available)
		[HttpGet()]
			[ActionName("Get_Room_List_Available")]
			public virtual System.Data.DataTable Get_Room_List_Available(string tenantid, string roomtype = "", string fromdate = "", string todate = "")
			{
				DataTable dtRoom = new DataTable();
				try
				{
					// tenantid: from query, or extract from pvar_tenantid (format "usersid|tenantid")
					var tenantIdStr = !string.IsNullOrWhiteSpace(tenantid) ? tenantid.Trim() : "";
					if (string.IsNullOrWhiteSpace(tenantIdStr) && !string.IsNullOrWhiteSpace(pvar_tenantid))
					{
						var parts = pvar_tenantid.Split(new[] { '|' }, StringSplitOptions.None);
						if (parts.Length >= 2) tenantIdStr = parts[1].Trim();
						else tenantIdStr = pvar_tenantid.Trim();
					}
					if (string.IsNullOrWhiteSpace(tenantIdStr) || !Guid.TryParse(tenantIdStr, out var tenantGuid))
						throw new Exception("Tenant id is required");
					Guid? roomTypeGuid = null;
					if (!string.IsNullOrWhiteSpace(roomtype) && !roomtype.Equals("00000000-0000-0000-0000-000000000000", StringComparison.OrdinalIgnoreCase))
					{
						if (Guid.TryParse(roomtype, out var g)) roomTypeGuid = g;
					}

					DateTime ParseDate(string s)
					{
						if (string.IsNullOrWhiteSpace(s)) throw new Exception("From date and To date are required");
						s = s.Trim();
						if (DateTime.TryParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d1)) return d1.Date;
						if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d2)) return d2.Date;
						if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var d3)) return d3.Date;
						throw new Exception("Invalid date format");
					}

					var fd = ParseDate(fromdate);
					var td = ParseDate(todate);

					dtRoom = objRoomDAL.Get_Room_List_Available(tenantGuid, roomTypeGuid, fd, td);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An exception occurred in - Get_Room_List_Available, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
				}
				return dtRoom;
			}
			   
[HttpGet()]
			
			[ActionName("get_all_Room")]
			public virtual System.Data.DataTable get_all_Room(string tenantid,string loginUserID="")
			{
					 
				  	DataTable dtRoom = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtRoom = objRoomDAL.get_all_Room(tenantid);
					}
					catch (Exception ex)
					{
                        _logger.LogError(ex,"An exception occurred in - get_all_Room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					   
					}
					return dtRoom;

			   }
[HttpGet()]

		[ActionName("count_of_Room_occupancystatus")]
		public virtual System.Data.DataTable count_of_Room_occupancystatus(string tenantid
, string block = ""
, string building = ""
, string floor = ""
, string roomtype = ""
, string roomnumber = ""
)
		{

			DataTable dtRoom = new DataTable();
			try
			{
				tenantid = pvar_tenantid;
				dtRoom = objRoomDAL.count_of_Room_occupancystatus(tenantid
, block
, building
, floor
, roomtype
, roomnumber
);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - count_of_Room_occupancystatus, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtRoom;

		}



		[HttpGet()]
			  [ActionName("getById_allinfo_Room")]
			  public virtual System.Data.DataTable getById_allinfo_Room(string Roomid)
			  { 
				    DataTable dtRoom = new DataTable();
					try
					{
						  dtRoom = objRoomDAL.getById_allinfo_Room(Roomid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_Room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtRoom;

			  }
[HttpGet()]
							[ActionName("lookup_Room_block")]
							public virtual System.Data.DataTable lookup_Room_block(String tenantid,string loginUserID="")
							{
									DataTable dtRoom = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoom = objRoomDAL.lookup_Room_block(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Room_block, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoom;
							}
[HttpGet()]
							[ActionName("lookup_Room_building")]
							public virtual System.Data.DataTable lookup_Room_building(String tenantid,String block,string loginUserID="")
							{
									DataTable dtRoom = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoom = objRoomDAL.lookup_Room_building(tenantid,block);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Room_building, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoom;
							}
[HttpGet()]
							[ActionName("lookup_Room_floor")]
							public virtual System.Data.DataTable lookup_Room_floor(String tenantid,String block,String building,string loginUserID="")
							{
									DataTable dtRoom = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoom = objRoomDAL.lookup_Room_floor(tenantid,block,building);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Room_floor, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoom;
							}
[HttpGet()]
							[ActionName("lookup_Room_roomtype")]
							public virtual System.Data.DataTable lookup_Room_roomtype(String tenantid,string loginUserID="")
							{
									DataTable dtRoom = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoom = objRoomDAL.lookup_Room_roomtype(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Room_roomtype, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoom;
							}
[HttpGet()]
							[ActionName("lookup_Room_roomgroup")]
							public virtual System.Data.DataTable lookup_Room_roomgroup(String tenantid,string loginUserID="")
							{
									DataTable dtRoom = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtRoom = objRoomDAL.lookup_Room_roomgroup(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Room_roomgroup, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoom;
							}

[HttpGet()]
							[ActionName("lookup_change_Room_roomtype")]
							public virtual System.Data.DataTable lookup_change_Room_roomtype(string RoomTypeid,string loginUserID="")
							{
									DataTable dtRoomType = new DataTable();
									try
									{
										dtRoomType = objRoomDAL.lookup_change_Room_roomtype(RoomTypeid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_change_Room_roomtype, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomType;
							}





				}


			}
