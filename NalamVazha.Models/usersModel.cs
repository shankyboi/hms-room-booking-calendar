namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:22
			public class usersModel
			{

			 public System.Guid ?usersid	{ get; set; }
public System.Guid ?tenantid { get; set; }

[xssFilter]
public string firstname{ get; set; }

[xssFilter]
public string? lastname{ get; set; }

[xssFilter]
										
               public string? profilepicture{ get; set; }

[xssFilter]
public string username{ get; set; }

[xssFilter]
public string userpassword{ get; set; }

[xssFilter]
public string? passwordkey{ get; set; }

[xssFilter]
public string emailid{ get; set; }

[xssFilter]
public string mobilenumber{ get; set; }

[xssFilter]
public string userrole{ get; set; }
public System.Guid ?createduser	{ get; set; }
[DataType(DataType.Date)]
[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
[DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]
public System.DateTime ?createddate	{ get; set; }
public System.Guid ?modifieduser	{ get; set; }
[DataType(DataType.Date)]
[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
[DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]
public System.DateTime ?modifieddate	{ get; set; }
public bool isdeleted	{ get; set; }
[xssFilter]
                        [Required(ErrorMessage = "craftmyapp_actionmethodname is required,please pass current action name")]
                        public String craftmyapp_actionmethodname{ get; set; }



			}
			

			public class usersModelValidator: AbstractValidator<usersModel>
			{
					 
					public usersModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Register_Profile", () =>
                                    {
                                        {RuleFor(m => m.firstname)
.NotEmpty().WithMessage("First Name is required")
.MaximumLength(50).WithMessage("The allowed length of First Name is 50 characters or fewer")
;


RuleFor(m => m.username)
.NotEmpty().WithMessage("UserName is required")
.MinimumLength(1).WithMessage("The minimum length of UserName is 1 characters ")
.MaximumLength(150).WithMessage("The allowed length of UserName is 150 characters or fewer")
;
RuleFor(m => m.userpassword)
.NotEmpty().WithMessage("User Password is required")
.MaximumLength(128).WithMessage("The allowed length of User Password is 128 characters or fewer")
;
RuleFor(m => m.emailid)
.NotEmpty().WithMessage("Email ID is required")
.MaximumLength(128).WithMessage("The allowed length of Email ID is 128 characters or fewer")
.EmailAddress()

;
RuleFor(m => m.mobilenumber)
.NotEmpty().WithMessage("Mobile Number is required")
.MaximumLength(20).WithMessage("The allowed length of Mobile Number is 20 characters or fewer ")

;
RuleFor(m => m.userrole)
.NotEmpty().WithMessage("User Role is required")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Profile", () =>
                                    {
                                        {RuleFor(m => m.firstname)
.NotEmpty().WithMessage("First Name is required")
.MaximumLength(50).WithMessage("The allowed length of First Name is 50 characters or fewer")
;


RuleFor(m => m.username)
.NotEmpty().WithMessage("UserName is required")
.MinimumLength(1).WithMessage("The minimum length of UserName is 1 characters ")
.MaximumLength(150).WithMessage("The allowed length of UserName is 150 characters or fewer")
;
RuleFor(m => m.emailid)
.NotEmpty().WithMessage("Email ID is required")
.MaximumLength(128).WithMessage("The allowed length of Email ID is 128 characters or fewer")
.EmailAddress()

;
RuleFor(m => m.mobilenumber)
.NotEmpty().WithMessage("Mobile Number is required")
.MaximumLength(20).WithMessage("The allowed length of Mobile Number is 20 characters or fewer ")

;
RuleFor(m => m.userrole)
.NotEmpty().WithMessage("User Role is required")
;
}

                                    });

						 
						
					}

			}

                

                 public class usersChangePasswordModel
                {
                    public System.Guid ?usersid { get; set; }
                    [xssFilter]
                    public String userpassword{ get; set; }
                    public String passwordkey{ get; set; }
                    public System.Guid ?modifieduser	{ get; set; }



                }

                
 

                

                

        

			}
