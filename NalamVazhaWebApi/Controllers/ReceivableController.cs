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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/21/2026 05:37:58
				public class ReceivableController : BaseController
				{
				    public ReceivableController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<ReceivableController>();
					     _connectionSettings = connectionSettings;
					     objReceivableDAL = new ReceivableDAL(_connectionSettings.Value.ConnectionString);
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
				private ReceivableDAL objReceivableDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    
            [HttpPost()]
            [ActionName("Add_Receivable")]
            public virtual IActionResult Add_Receivable([FromBody]ReceivableModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Receivable";
            try{

            if (ModelState.IsValid)
            {

            	ReceivableModelValidator validator = new ReceivableModelValidator();
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

		       
                                 

                                
            		                
                                     message = objReceivableDAL.Add_Receivable(model);
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

            	_logger.LogError("ReceivableModel - Add_Receivable , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Receivable, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_Receivable")]
			  public virtual ReceivableModel getById_Receivable(string Receivableid,string loginUserID="")
			  { 
				    ReceivableModel objReceivable = new ReceivableModel();
					try
					{
						  objReceivable = objReceivableDAL.getById_Receivable(Receivableid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_Receivable, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objReceivable;

			  }
			  [HttpPost()]
			  [ActionName("Update_Receivable")]
			  public virtual IActionResult Update_Receivable([FromBody]ReceivableModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Receivable";

					try{

					if (ModelState.IsValid)
					{

						ReceivableModelValidator validator = new ReceivableModelValidator();
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

		       
                                 
		       
                                	
							    message = objReceivableDAL.Update_Receivable(model);	
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

						_logger.LogError("ReceivableModel - Update_Receivable, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Receivable, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Receivable(string Receivableid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Receivable";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objReceivableDAL.Remove_Receivable(Receivableid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Receivable, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Receivables")]
			public virtual JObject Receivables(string tenantid
,string receivabledate_automatonfrom=""
,string receivabledate_automatonto=""
,string patientname=""
,string ipdnumber=""
,string opdnumber=""
,string receivablefor=""
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonReceivable = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonReceivable = objReceivableDAL.Receivables( tenantid
, receivabledate_automatonfrom
, receivabledate_automatonto
, patientname
, ipdnumber
, opdnumber
, receivablefor
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Receivables, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonReceivable;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_Receivable")]
			public virtual System.Data.DataTable get_all_Receivable(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtReceivable = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtReceivable = objReceivableDAL.get_all_Receivable(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_Receivable, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtReceivable;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_Receivable")]
			  public virtual System.Data.DataTable getById_allinfo_Receivable(string Receivableid)
			  { 
				    DataTable dtReceivable = new DataTable();
					try
					{
						  dtReceivable = objReceivableDAL.getById_allinfo_Receivable(Receivableid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_Receivable, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtReceivable;

			  }
[HttpGet()]
							[ActionName("lookup_Receivable_patientname")]
							public virtual System.Data.DataTable lookup_Receivable_patientname(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtReceivable = new DataTable();
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
										dtReceivable = objReceivableDAL.lookup_Receivable_patientname(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Receivable_patientname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtReceivable;
							}
[HttpGet()]
							[ActionName("lookup_Receivable_patientvisit")]
							public virtual System.Data.DataTable lookup_Receivable_patientvisit(String tenantid,String patientname,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtReceivable = new DataTable();
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
										dtReceivable = objReceivableDAL.lookup_Receivable_patientvisit(tenantid,patientname,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Receivable_patientvisit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtReceivable;
							}
[HttpGet()]
							[ActionName("lookup_Receivable_ipdnumber")]
							public virtual System.Data.DataTable lookup_Receivable_ipdnumber(String tenantid,String patientname,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtReceivable = new DataTable();
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
										dtReceivable = objReceivableDAL.lookup_Receivable_ipdnumber(tenantid,patientname,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Receivable_ipdnumber, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtReceivable;
							}
[HttpGet()]
							[ActionName("lookup_Receivable_opdnumber")]
							public virtual System.Data.DataTable lookup_Receivable_opdnumber(String tenantid,String patientname,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtReceivable = new DataTable();
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
										dtReceivable = objReceivableDAL.lookup_Receivable_opdnumber(tenantid,patientname,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Receivable_opdnumber, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtReceivable;
							}
[HttpGet()]
							[ActionName("get_Approved_Concession_Amount")]
							public virtual System.Data.DataTable get_Approved_Concession_Amount(String tenantid,String patientname,String ipdnumber,string loginUserID="")
							{
									DataTable dtReceivable = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
										dtReceivable = objReceivableDAL.get_Approved_Concession_Amount(tenantid,patientname,ipdnumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - get_Approved_Concession_Amount, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtReceivable;
							}
[HttpGet()]
							[ActionName("lookup_Receivable_therapy")]
							public virtual System.Data.DataTable lookup_Receivable_therapy(String tenantid,string loginUserID="")
							{
									DataTable dtReceivable = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtReceivable = objReceivableDAL.lookup_Receivable_therapy(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Receivable_therapy, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtReceivable;
							}
[HttpGet()]
							[ActionName("lookup_Receivable_therapykit")]
							public virtual System.Data.DataTable lookup_Receivable_therapykit(String tenantid,string loginUserID="")
							{
									DataTable dtReceivable = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtReceivable = objReceivableDAL.lookup_Receivable_therapykit(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Receivable_therapykit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtReceivable;
							}
[HttpGet()]
							[ActionName("lookup_Receivable_medicine")]
							public virtual System.Data.DataTable lookup_Receivable_medicine(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtReceivable = new DataTable();
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
										dtReceivable = objReceivableDAL.lookup_Receivable_medicine(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Receivable_medicine, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtReceivable;
							}
[HttpGet()]
							[ActionName("lookup_Receivable_package")]
							public virtual System.Data.DataTable lookup_Receivable_package(String tenantid,string loginUserID="")
							{
									DataTable dtReceivable = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtReceivable = objReceivableDAL.lookup_Receivable_package(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Receivable_package, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtReceivable;
							}
[HttpGet()]
							[ActionName("lookup_Receivable_room")]
							public virtual System.Data.DataTable lookup_Receivable_room(String tenantid,string loginUserID="")
							{
									DataTable dtReceivable = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtReceivable = objReceivableDAL.lookup_Receivable_room(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Receivable_room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtReceivable;
							}

[HttpGet()]
							[ActionName("lookup_change_Receivable_medicine")]
							public virtual System.Data.DataTable lookup_change_Receivable_medicine(string Medicineid,string loginUserID="")
							{
									DataTable dtMedicine = new DataTable();
									try
									{
										dtMedicine = objReceivableDAL.lookup_change_Receivable_medicine(Medicineid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_change_Receivable_medicine, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtMedicine;
							}
[HttpGet()]
							[ActionName("lookup_change_Receivable_therapykit")]
							public virtual System.Data.DataTable lookup_change_Receivable_therapykit(string TherapyKitid,string loginUserID="")
							{
									DataTable dtTherapyKit = new DataTable();
									try
									{
										dtTherapyKit = objReceivableDAL.lookup_change_Receivable_therapykit(TherapyKitid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_change_Receivable_therapykit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTherapyKit;
							}
[HttpGet()]
							[ActionName("lookup_change_Receivable_therapy")]
							public virtual System.Data.DataTable lookup_change_Receivable_therapy(string Therapiesid,string loginUserID="")
							{
									DataTable dtTherapies = new DataTable();
									try
									{
										dtTherapies = objReceivableDAL.lookup_change_Receivable_therapy(Therapiesid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_change_Receivable_therapy, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTherapies;
							}





				}


			}
