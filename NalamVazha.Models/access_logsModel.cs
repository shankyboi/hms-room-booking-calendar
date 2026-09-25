namespace NalamVazha.Models
								{
									using System;
									using System.ComponentModel.DataAnnotations;
									using Microsoft.AspNetCore.Mvc;
									//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/09/2026 05:55:01
									
										public class access_logsModel
										{

											public System.Guid? access_logsid { get; set; }


											public String users_id { get; set; }

											public System.DateTime? logged_date { get; set; }
											public System.DateTime? expiry_date { get; set; }

											[xssFilter]
											public String user_token { get; set; }

											[xssFilter]
											public String external_users_id { get; set; }

											[xssFilter]
											public String external_entity_name { get; set; }


											[xssFilter]
											public String latlan { get; set; }

											[xssFilter]
											public String clientipaddress { get; set; }


											[xssFilter]
											public String devicename { get; set; }

											[xssFilter]
											public String browsername { get; set; }


											[xssFilter]
											public String request_type { get; set; }


											[xssFilter]
											public String api_url { get; set; }

											[xssFilter]
											public String request_json { get; set; }

											[xssFilter]
											public String response_json { get; set; }





											public System.Guid? createduser { get; set; }
											[DataType(DataType.Date)]
											[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
											[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
											public System.DateTime? createddate { get; set; }
											public System.Guid? modifieduser { get; set; }
											[DataType(DataType.Date)]
											[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
											[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
											public System.DateTime? modifieddate { get; set; }
											public bool isdeleted { get; set; }
         
											

										}
                                        public class access_logsdetailsModel
										{

											public System.Guid? access_logsid { get; set; }
											[xssFilter]
											public String action_method_name { get; set; }
											[xssFilter]
											public String api_response { get; set; }


									    }


									 
								}
