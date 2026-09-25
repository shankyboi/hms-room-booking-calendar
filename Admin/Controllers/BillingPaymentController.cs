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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:30
				
                
                
                
                
				public class BillingPaymentController : BaseController
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
                    private readonly ILogger<BillingPaymentController> _logger;
                    
                    
                    StorageUtil util;
					public BillingPaymentController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<BillingPaymentController> logger):base( configuration)
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
	              

					
			  public virtual IActionResult Add_Billing_Payment()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Billing_Payment(BillingPaymentModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("BillingPaymentid");
                    ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Billing_Payment";
					// Some workflow-specific forms (including Pre-Admission Consultation)
					// do not render paymentdate. Do not send DateTime.MinValue to PostgreSQL,
					// where Npgsql maps it to -infinity.
					if (model.paymentdate == default || model.paymentdate.Year < 1900)
						model.paymentdate = DateTime.Now.Date;
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 BillingPaymentModelValidator validator = new BillingPaymentModelValidator();
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
								 model.BillingPaymentid =Guid.NewGuid(); 
                                 
                                  
								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/BillingPayment/Add_Billing_Payment", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - BillingPayment / Add_Billing_Payment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
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

				
			  public virtual async Task<IActionResult> Update_Billing_Payment(string BillingPaymentid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='BillingPayment' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjBillingPayment = await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/getById_BillingPayment?BillingPaymentid="+BillingPaymentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjBillingPayment.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<BillingPaymentModel>(jsonObjBillingPayment);


                
                                     
                                    return View("Add_Billing_Payment", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - BillingPayment / Update_Billing_Payment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Billing_Payment(BillingPaymentModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("BillingPaymentid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Billing_Payment";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            
                            
							if (ModelState.IsValid)
							{
									BillingPaymentModelValidator validator = new BillingPaymentModelValidator();
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
                                        
										
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/BillingPayment/Update_Billing_Payment", model);
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
                      _logger.LogError(ex,"An exception occurred in - BillingPayment / Update_Billing_Payment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Billing_Payment(string BillingPaymentid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/Remove_Billing_Payment?BillingPaymentid="+BillingPaymentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - BillingPayment / Remove_Billing_Payment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='BillingPayment' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult Detail_Billing_Payment()
                                        {
                                            return View();
                                        }

			        public virtual IActionResult Billing_Payment_List()
			        {
				        return View();
			        }

		[HttpGet()]
		public virtual async Task<string> get_Billing_Payment_List(string tenantid
, string paymentdate_automatonfrom
, string paymentdate_automatonto
, string patientname
, string receivablefor
, int? pagesize = 100, int? pagenumber = 0, string searchterm = "", string sortFieldsJson = "")
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/Billing_Payment_List?tenantid=" + tenantid + "&paymentdate_automatonfrom=" + paymentdate_automatonfrom + "&paymentdate_automatonto=" + paymentdate_automatonto + "&patientname=" + patientname + "&receivablefor=" + receivablefor + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber=" + pagenumber + "&searchterm=" + searchterm + "&sort_fields=" + sortFieldsJson);
		}



		[HttpGet()]
		public virtual async Task<string> lookup_BillingPayment_refundedby(String tenantid)
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_BillingPayment_refundedby?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
											public virtual async Task<string> get_all_PatientVisit(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientVisit/get_all_PatientVisit?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_PatientProfile(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/get_all_PatientProfile?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_users(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/users/get_all_users?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_BillingPayment(string BillingPaymentid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/getById_allinfo_BillingPayment?BillingPaymentid="+BillingPaymentid);
					 
			  }
		[HttpGet()]
		public virtual async Task<string> lookup_BillingPayment_patientvisit(String tenantid, String patientname, string searchterm, int? pagesize, int? pagenumber)
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_BillingPayment_patientvisit?tenantid=" + tenantid + "&patientname=" + patientname + "&searchterm=" + searchterm + "&pagesize=" + pagesize + "&pagenumber=" + pagenumber + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}
		[HttpGet()]
                    public virtual async Task<string> lookup_BillingPayment_patientname(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_BillingPayment_patientname?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
			    public virtual async Task<string> lookup_BillingPayment_collectedby(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_BillingPayment_collectedby?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }



[HttpGet()]
			public virtual async Task<string> lookup_change_BillingPayment_patientvisit(string PatientVisitid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_change_BillingPayment_patientvisit?PatientVisitid="+PatientVisitid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}


		[HttpGet()]
		public virtual async Task<string> lookup_BillingPayment_ipdnumber(String tenantid, String patientname, string searchterm, int? pagesize, int? pagenumber)
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_BillingPayment_ipdnumber?tenantid=" + tenantid + "&patientname=" + patientname + "&searchterm=" + searchterm + "&pagesize=" + pagesize + "&pagenumber=" + pagenumber + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}
		[HttpGet()]
		public virtual async Task<string> lookup_BillingPayment_opdnumber(String tenantid, String patientname, string searchterm, int? pagesize, int? pagenumber)
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_BillingPayment_opdnumber?tenantid=" + tenantid + "&patientname=" + patientname + "&searchterm=" + searchterm + "&pagesize=" + pagesize + "&pagenumber=" + pagenumber + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}


		[HttpGet()]
		public virtual async Task<string> get_all_IPDApplicationForm(string tenantid, string searchterm, string pagesize = "1000", string pagenumber = "1")
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/get_all_IPDApplicationForm?tenantid=" + tenantid + "&searchterm=" + searchterm + "&pagesize=" + pagesize + "&pagenumber=" + pagenumber + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> get_all_OPDForm(string tenantid, string searchterm, string pagesize = "1000", string pagenumber = "1")
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/get_all_OPDForm?tenantid=" + tenantid + "&searchterm=" + searchterm + "&pagesize=" + pagesize + "&pagenumber=" + pagenumber + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> get_all_Therapies(string tenantid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/Therapies/get_all_Therapies?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> get_all_TherapyKit(string tenantid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/TherapyKit/get_all_TherapyKit?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> get_all_Medicine(string tenantid, string searchterm, string pagesize = "1000", string pagenumber = "1")
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/Medicine/get_all_Medicine?tenantid=" + tenantid + "&searchterm=" + searchterm + "&pagesize=" + pagesize + "&pagenumber=" + pagenumber + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> get_all_Room(string tenantid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/Room/get_all_Room?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}
		[HttpGet()]
		public virtual async Task<string> lookup_BillingPayment_therapy(String tenantid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_BillingPayment_therapy?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> lookup_BillingPayment_therapykit(String tenantid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_BillingPayment_therapykit?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> lookup_BillingPayment_medicine(String tenantid, string searchterm, int? pagesize, int? pagenumber)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_BillingPayment_medicine?tenantid=" + tenantid + "&searchterm=" + searchterm + "&pagesize=" + pagesize + "&pagenumber=" + pagenumber + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> lookup_BillingPayment_room(String tenantid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_BillingPayment_room?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> lookup_change_BillingPayment_medicine(string Medicineid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_change_BillingPayment_medicine?Medicineid=" + Medicineid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> lookup_change_BillingPayment_therapykit(string TherapyKitid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_change_BillingPayment_therapykit?TherapyKitid=" + TherapyKitid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> lookup_change_BillingPayment_therapy(string Therapiesid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/lookup_change_BillingPayment_therapy?Therapiesid=" + Therapiesid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<IActionResult> UnifiedPayment(Guid patientid, string type = "OPD", Guid? ipdNo = null, string bookingstatus = "")
		{
			ViewBag.BookingStatus = bookingstatus ?? "";
			var model = new UnifiedPaymentCollectionViewModel
			{
				PatientID = patientid,
				IPDNo = ipdNo,
				Type = string.IsNullOrWhiteSpace(type) ? "OPD" : type.ToUpperInvariant()
			};

			try
			{
				var json = await ApiClient.Get_ApiValues(
					getHttpClient(),
					"api/BillingPayment/Get_Unified_Pending_Receivables?PatientID=" + patientid
					+ "&IPDNo=" + (ipdNo.HasValue ? ipdNo.Value.ToString() : string.Empty)
					+ "&Type=" + model.Type
					+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));

				var rows = JsonConvert.DeserializeObject<List<UnifiedReceivableItemModel>>(json ?? "[]") ?? new List<UnifiedReceivableItemModel>();
				if (string.Equals(model.Type, "IPD", StringComparison.OrdinalIgnoreCase)
					&& string.Equals(bookingstatus, "Admitted", StringComparison.OrdinalIgnoreCase))
				{
					foreach (var row in rows)
						NormalizeDirectIpdRoomChargeDescription(row);

					rows = rows
						.OrderBy(x => GetAdmittedIpdReceivableSortOrder(x.receivablefor))
						.ToList();
				}
				model.Bills = rows;
				model.Payments = rows
					.Select(x => new UnifiedPaymentInputModel
					{
						Receivableid = x.Receivableid,
						Balance = x.balance,
						IsMandatory = x.ismandatory,
						PayNow = 0
					})
					.ToList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "UnifiedPayment load failed: " + ex.Message);
				ViewData["message"] = ex.Message;
			}
            if (model.IPDNo.HasValue &&
    string.Equals(model.Type, "IPD", StringComparison.OrdinalIgnoreCase))
            {
                var roomSummaryJson = await ApiClient.Get_ApiValues(
                    getHttpClient(),
                    "api/BillingPayment/Get_Unified_Room_Cost_Summary?IPDNo=" + model.IPDNo.Value);

                 model.RoomCostSummary =
                    JsonConvert.DeserializeObject<List<UnifiedRoomCostSummaryModel>>(roomSummaryJson ?? "[]")
                    ?? new List<UnifiedRoomCostSummaryModel>();
            }
            return View("UnifiedPayment", model);
		}

		private static int GetAdmittedIpdReceivableSortOrder(string receivableFor)
		{
			if ((receivableFor ?? "").IndexOf("Booking Deposit", StringComparison.OrdinalIgnoreCase) >= 0)
				return 0;
			if (string.Equals(receivableFor, "Package", StringComparison.OrdinalIgnoreCase))
				return 1;
			if (string.Equals(receivableFor, "Room", StringComparison.OrdinalIgnoreCase))
				return 2;
			return 3;
		}

		private static void NormalizeDirectIpdRoomChargeDescription(UnifiedReceivableItemModel row)
		{
			if (row == null
				|| !string.Equals(row.receivablefor, "Room", StringComparison.OrdinalIgnoreCase))
				return;

			var details = string.IsNullOrWhiteSpace(row.billdetails) ? row.remarks : row.billdetails;
			if (string.IsNullOrWhiteSpace(details)
				|| details.IndexOf("Direct IPD", StringComparison.OrdinalIgnoreCase) < 0)
				return;

			var dateRangeStart = details.LastIndexOf(" (", StringComparison.Ordinal);
			if (dateRangeStart < 0
				|| details.IndexOf(" to ", dateRangeStart, StringComparison.OrdinalIgnoreCase) < 0
				|| !TryParseReceivableDate(row.receivabledate, out var chargeDate))
				return;

			var dailyDetails = details.Substring(0, dateRangeStart)
				+ " (" + chargeDate.ToString("dd/MM/yyyy") + ")";
			row.billdetails = dailyDetails;
			row.remarks = dailyDetails;
		}

		private static bool TryParseReceivableDate(string value, out DateTime date)
		{
			var formats = new[]
			{
				"dd/MM/yyyy",
				"dd/MM/yyyy HH:mm:ss",
				"yyyy-MM-dd",
				"yyyy-MM-dd HH:mm:ss",
				"yyyy-MM-ddTHH:mm:ss"
			};
			return DateTime.TryParseExact(
				value,
				formats,
				CultureInfo.InvariantCulture,
				DateTimeStyles.AllowWhiteSpaces,
				out date)
				|| DateTime.TryParse(
					value,
					CultureInfo.InvariantCulture,
					DateTimeStyles.AllowWhiteSpaces,
					out date);
		}

		[HttpPost()]
		public virtual async Task<string> UnifiedPayment(UnifiedPaymentPostModel model)
		{
			try
			{
				if (HttpContext.Session.GetString("NalamVazhaloginUserID") == null)
				{
					return "Session Expired";
				}
				var httpclient = getHttpClient();
				var json = await ApiClient.Get_ApiValues(
					httpclient,
					"api/BillingPayment/Get_Unified_Pending_Receivables?PatientID=" + model.PatientID
					+ "&IPDNo=" + (model.IPDNo.HasValue ? model.IPDNo.Value.ToString() : string.Empty)
					+ "&Type=" + (string.IsNullOrWhiteSpace(model.Type) ? "OPD" : model.Type)
					+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
				var currentRows = JsonConvert.DeserializeObject<List<UnifiedReceivableItemModel>>(json ?? "[]") ?? new List<UnifiedReceivableItemModel>();
				var outstandingCredit = currentRows.Where(x => x.balance < 0).Sum(x => x.balance);
				var mandatoryBalance = Math.Max(0m,
					currentRows.Where(x => x.ismandatory && x.balance > 0).Sum(x => x.balance) + outstandingCredit);
				var payingNow = model.receivedamount ?? 0;
				if (string.Equals(model.receivablefor, "IPD Admission Fees", StringComparison.OrdinalIgnoreCase)
					&& mandatoryBalance > 0 && payingNow + 0.009m < mandatoryBalance)
				{
					return "Mandatory bills must be paid first. Required now: " + mandatoryBalance.ToString("0.00");
				}

				var result = await ApiClient.Post_ApiValuesGetString(
					httpclient,
					"api/BillingPayment/Apply_Unified_Receivable_Payments",
					model);

				if ((result ?? string.Empty).Replace("\"", "").Contains("201.1"))
				{
					return "Success";
				}

				if ((result ?? string.Empty).StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
				{
					return result.Replace("\"", "").Replace("BadRequest : ", "");
				}

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "UnifiedPayment save failed: " + ex.Message);
				return ex.Message;
			}
		}


	}


}
