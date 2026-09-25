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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:50
				public class TreatmentPackageController : BaseController
				{
				    public TreatmentPackageController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<TreatmentPackageController>();
					     _connectionSettings = connectionSettings;
					     objTreatmentPackageDAL = new TreatmentPackageDAL(_connectionSettings.Value.ConnectionString);
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
				private TreatmentPackageDAL objTreatmentPackageDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    [HttpGet()]
			  [ActionName("getById_roomtypes")]
			  public virtual System.Data.DataTable getById_roomtypes(string TreatmentPackageid)
			  {
					DataTable dtTreatmentPackage_roomtypes = new DataTable();
					try
					{
						dtTreatmentPackage_roomtypes = objTreatmentPackageDAL.getById_roomtypes(TreatmentPackageid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_roomtypes, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtTreatmentPackage_roomtypes;


			  }

[HttpGet()]
			  [ActionName("getById_therapy")]
			  public virtual System.Data.DataTable getById_therapy(string TreatmentPackageid)
			  {
					DataTable dtTreatmentPackage_therapy = new DataTable();
					try
					{
						dtTreatmentPackage_therapy = objTreatmentPackageDAL.getById_therapy(TreatmentPackageid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_therapy, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtTreatmentPackage_therapy;


			  }

[HttpGet()]
			  [ActionName("getById_therapykits")]
			  public virtual System.Data.DataTable getById_therapykits(string TreatmentPackageid)
			  {
					DataTable dtTreatmentPackage_therapykits = new DataTable();
					try
					{
						dtTreatmentPackage_therapykits = objTreatmentPackageDAL.getById_therapykits(TreatmentPackageid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_therapykits, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtTreatmentPackage_therapykits;


			  }

[HttpGet()]
			  [ActionName("getById_therapyitems")]
			  public virtual System.Data.DataTable getById_therapyitems(string TreatmentPackageid)
			  {
					DataTable dtTreatmentPackage_therapyitems = new DataTable();
					try
					{
						dtTreatmentPackage_therapyitems = objTreatmentPackageDAL.getById_therapyitems(TreatmentPackageid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_therapyitems, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtTreatmentPackage_therapyitems;


			  }

[HttpGet()]
			  [ActionName("getById_medicines")]
			  public virtual System.Data.DataTable getById_medicines(string TreatmentPackageid)
			  {
					DataTable dtTreatmentPackage_medicines = new DataTable();
					try
					{
						dtTreatmentPackage_medicines = objTreatmentPackageDAL.getById_medicines(TreatmentPackageid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_medicines, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtTreatmentPackage_medicines;


			  }

[HttpGet()]
			  [ActionName("getById_refundpolicy")]
			  public virtual System.Data.DataTable getById_refundpolicy(string TreatmentPackageid)
			  {
					DataTable dtTreatmentPackage_refundpolicy = new DataTable();
					try
					{
						dtTreatmentPackage_refundpolicy = objTreatmentPackageDAL.getById_refundpolicy(TreatmentPackageid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_refundpolicy, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtTreatmentPackage_refundpolicy;


			  }


            [HttpPost()]
            [ActionName("Add_Treatment_Package")]
            public virtual IActionResult Add_Treatment_Package([FromBody]TreatmentPackageModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Treatment_Package";
            try{

            if (ModelState.IsValid)
            {

            	TreatmentPackageModelValidator validator = new TreatmentPackageModelValidator();
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

		       
                                 

                                
            		                
                                     message = objTreatmentPackageDAL.Add_Treatment_Package(model);
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

            	_logger.LogError("TreatmentPackageModel - Add_Treatment_Package , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Treatment_Package, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_TreatmentPackage")]
			  public virtual TreatmentPackageModel getById_TreatmentPackage(string TreatmentPackageid,string loginUserID="")
			  { 
				    TreatmentPackageModel objTreatmentPackage = new TreatmentPackageModel();
					try
					{
						  objTreatmentPackage = objTreatmentPackageDAL.getById_TreatmentPackage(TreatmentPackageid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_TreatmentPackage, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objTreatmentPackage;

			  }
			  [HttpPost()]
			  [ActionName("Update_Treatment_Package")]
			  public virtual IActionResult Update_Treatment_Package([FromBody]TreatmentPackageModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Treatment_Package";

					try{

					if (ModelState.IsValid)
					{

						TreatmentPackageModelValidator validator = new TreatmentPackageModelValidator();
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

		       
                                 
		       
                                	
							    message = objTreatmentPackageDAL.Update_Treatment_Package(model);	
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

						_logger.LogError("TreatmentPackageModel - Update_Treatment_Package, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Treatment_Package, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Treatment_Package(string TreatmentPackageid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Treatment_Package";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objTreatmentPackageDAL.Remove_Treatment_Package(TreatmentPackageid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Treatment_Package, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Treatment_Package_List")]
			public virtual System.Data.DataTable Treatment_Package_List(string tenantid
)
			{
					 
				  	DataTable dtTreatmentPackage = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtTreatmentPackage = objTreatmentPackageDAL.Treatment_Package_List( tenantid
);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - Treatment_Package_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                       
					}
					return dtTreatmentPackage;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_TreatmentPackage")]
			public virtual System.Data.DataTable get_all_TreatmentPackage(string tenantid,string loginUserID="")
			{
					 
				  	DataTable dtTreatmentPackage = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtTreatmentPackage = objTreatmentPackageDAL.get_all_TreatmentPackage(tenantid);
					}
					catch (Exception ex)
					{
                        _logger.LogError(ex,"An exception occurred in - get_all_TreatmentPackage, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					   
					}
					return dtTreatmentPackage;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_TreatmentPackage")]
			  public virtual System.Data.DataTable getById_allinfo_TreatmentPackage(string TreatmentPackageid)
			  { 
				    DataTable dtTreatmentPackage = new DataTable();
					try
					{
						  dtTreatmentPackage = objTreatmentPackageDAL.getById_allinfo_TreatmentPackage(TreatmentPackageid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_TreatmentPackage, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtTreatmentPackage;

			  }
[HttpGet()]
							[ActionName("lookup_TreatmentPackage_roomtypes_roomtype")]
							public virtual System.Data.DataTable lookup_TreatmentPackage_roomtypes_roomtype(String tenantid,string loginUserID="")
							{
									DataTable dtTreatmentPackage = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtTreatmentPackage = objTreatmentPackageDAL.lookup_TreatmentPackage_roomtypes_roomtype(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_TreatmentPackage_roomtypes_roomtype, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTreatmentPackage;
							}
[HttpGet()]
							[ActionName("lookup_TreatmentPackage_therapy_therapyname")]
							public virtual System.Data.DataTable lookup_TreatmentPackage_therapy_therapyname(String tenantid,string loginUserID="")
							{
									DataTable dtTreatmentPackage = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtTreatmentPackage = objTreatmentPackageDAL.lookup_TreatmentPackage_therapy_therapyname(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_TreatmentPackage_therapy_therapyname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTreatmentPackage;
							}
[HttpGet()]
							[ActionName("lookup_TreatmentPackage_therapykits_therapykitname")]
							public virtual System.Data.DataTable lookup_TreatmentPackage_therapykits_therapykitname(String tenantid,string loginUserID="")
							{
									DataTable dtTreatmentPackage = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtTreatmentPackage = objTreatmentPackageDAL.lookup_TreatmentPackage_therapykits_therapykitname(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_TreatmentPackage_therapykits_therapykitname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTreatmentPackage;
							}
[HttpGet()]
							[ActionName("lookup_TreatmentPackage_therapyitems_therapyitem")]
							public virtual System.Data.DataTable lookup_TreatmentPackage_therapyitems_therapyitem(String tenantid,string loginUserID="")
							{
									DataTable dtTreatmentPackage = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtTreatmentPackage = objTreatmentPackageDAL.lookup_TreatmentPackage_therapyitems_therapyitem(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_TreatmentPackage_therapyitems_therapyitem, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTreatmentPackage;
							}
[HttpGet()]
							[ActionName("lookup_TreatmentPackage_medicines_medicinename")]
							public virtual System.Data.DataTable lookup_TreatmentPackage_medicines_medicinename(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtTreatmentPackage = new DataTable();
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
										dtTreatmentPackage = objTreatmentPackageDAL.lookup_TreatmentPackage_medicines_medicinename(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_TreatmentPackage_medicines_medicinename, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTreatmentPackage;
							}


[HttpGet()]
							[ActionName("lookup_change_medicines_TreatmentPackage_medicinename")]
							public virtual System.Data.DataTable lookup_change_medicines_TreatmentPackage_medicinename(string Medicineid,string loginUserID="")
							{
									DataTable dtMedicine = new DataTable();
									try
									{
										dtMedicine = objTreatmentPackageDAL.lookup_change_medicines_TreatmentPackage_medicinename(Medicineid);
									}
									catch (Exception ex)
									{
									  _logger.LogError(ex,"An exception occurred in - lookup_change_medicines_TreatmentPackage_medicinename, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtMedicine;
							}
[HttpGet()]
							[ActionName("lookup_change_therapyitems_TreatmentPackage_therapyitem")]
							public virtual System.Data.DataTable lookup_change_therapyitems_TreatmentPackage_therapyitem(string TherapyItemid,string loginUserID="")
							{
									DataTable dtTherapyItem = new DataTable();
									try
									{
										dtTherapyItem = objTreatmentPackageDAL.lookup_change_therapyitems_TreatmentPackage_therapyitem(TherapyItemid);
									}
									catch (Exception ex)
									{
									  _logger.LogError(ex,"An exception occurred in - lookup_change_therapyitems_TreatmentPackage_therapyitem, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTherapyItem;
							}
[HttpGet()]
							[ActionName("lookup_change_therapykits_TreatmentPackage_therapykitname")]
							public virtual System.Data.DataTable lookup_change_therapykits_TreatmentPackage_therapykitname(string TherapyKitid,string loginUserID="")
							{
									DataTable dtTherapyKit = new DataTable();
									try
									{
										dtTherapyKit = objTreatmentPackageDAL.lookup_change_therapykits_TreatmentPackage_therapykitname(TherapyKitid);
									}
									catch (Exception ex)
									{
									  _logger.LogError(ex,"An exception occurred in - lookup_change_therapykits_TreatmentPackage_therapykitname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTherapyKit;
							}
[HttpGet()]
							[ActionName("lookup_change_therapy_TreatmentPackage_therapyname")]
							public virtual System.Data.DataTable lookup_change_therapy_TreatmentPackage_therapyname(string Therapiesid,string loginUserID="")
							{
									DataTable dtTherapies = new DataTable();
									try
									{
										dtTherapies = objTreatmentPackageDAL.lookup_change_therapy_TreatmentPackage_therapyname(Therapiesid);
									}
									catch (Exception ex)
									{
									  _logger.LogError(ex,"An exception occurred in - lookup_change_therapy_TreatmentPackage_therapyname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTherapies;
							}
[HttpGet()]
							[ActionName("lookup_change_roomtypes_TreatmentPackage_roomtype")]
							public virtual System.Data.DataTable lookup_change_roomtypes_TreatmentPackage_roomtype(string RoomTypeid,string loginUserID="")
							{
									DataTable dtRoomType = new DataTable();
									try
									{
										dtRoomType = objTreatmentPackageDAL.lookup_change_roomtypes_TreatmentPackage_roomtype(RoomTypeid);
									}
									catch (Exception ex)
									{
									  _logger.LogError(ex,"An exception occurred in - lookup_change_roomtypes_TreatmentPackage_roomtype, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtRoomType;
							}




				}


			}
