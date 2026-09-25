namespace NalamVazha.Models
								{
									using System;
									using System.ComponentModel.DataAnnotations;
									using Microsoft.AspNetCore.Mvc;
									//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/09/2026 05:55:01
									
									public class userloginModel
									{
										[Required(ErrorMessage="UserName is Required")]	
										[xssFilter]	
										public String username {get; set; }

										[Required(ErrorMessage="Password is Required")]
										[xssFilter]
										public String userpassword{ get; set; }

										public Guid? tenantid { get; set; }

										[xssFilter]
										public String latlan{ get; set; }

										[xssFilter]	
										public String clientipaddress{ get; set; }
										
										[xssFilter]
										public String devicename { get; set; }

										[xssFilter]
										public String browsername { get; set; }
  
										[xssFilter]
										public String deviceid { get; set; }

										[xssFilter]
										public String source { get; set; }
										
										[xssFilter]
										public String accesskey { get; set; }

										[xssFilter]
										public String notificationid { get; set; }
										
										[xssFilter]
										public String mobilenumber {get; set; }
										
									}
									public class userlockoutModel
									{
										public String lockoutid{get;set;}

										[xssFilter]	
										public String username {get; set; }
										public int numberOfFailedTry {get; set; }
										[DataType(DataType.Date)]
										[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
										[DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]
										public System.DateTime ? lastupdateddate {get; set; }
										public String lockremovedby {get; set; }
										public String latlan {get; set; }
										public String remoteipaddress {get; set; }
										public String clientipaddress {get; set; }
										public String loginUser {get; set; }
									}
									public class response_model
									{
										public String message{get;set;}
										public String id{get;set;}
									}
									
								}
