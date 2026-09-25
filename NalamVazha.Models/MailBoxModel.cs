namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:14
			public class MailBoxModel
			{

			 public System.Guid ?MailBoxid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string senderdisplayname{ get; set; }

[xssFilter]
public string senderemail{ get; set; }

[xssFilter]
public string password{ get; set; }

[xssFilter]
public string emailhostname{ get; set; }

public int portnumber{ get; set; }

[xssFilter]
public string applicableservice{ get; set; }

[xssFilter]
public string? emailfooter{ get; set; }
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
			

			public class MailBoxModelValidator: AbstractValidator<MailBoxModel>
			{
					 
					public MailBoxModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Mail_Box", () =>
                                    {
                                        {RuleFor(m => m.senderdisplayname)
.NotEmpty().WithMessage("Sender Display Name is required")
.MaximumLength(128).WithMessage("The allowed length of Sender Display Name is 128 characters or fewer")
;
RuleFor(m => m.senderemail)
.NotEmpty().WithMessage("Sender Email is required")
.MaximumLength(128).WithMessage("The allowed length of Sender Email is 128 characters or fewer")
.EmailAddress()

;
RuleFor(m => m.password)
.NotEmpty().WithMessage("Password is required")
.MaximumLength(128).WithMessage("The allowed length of Password is 128 characters or fewer")
;
RuleFor(m => m.emailhostname)
.NotEmpty().WithMessage("Email Host Name is required")
.MaximumLength(128).WithMessage("The allowed length of Email Host Name is 128 characters or fewer")
;
RuleFor(m => m.portnumber)
.NotNull().WithMessage("Port Number is required")
.LessThanOrEqualTo(99999999).WithMessage("Port Number should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.applicableservice)
.NotEmpty().WithMessage("Applicable Service is required")
;

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Mail_Box", () =>
                                    {
                                        {RuleFor(m => m.senderdisplayname)
.NotEmpty().WithMessage("Sender Display Name is required")
.MaximumLength(128).WithMessage("The allowed length of Sender Display Name is 128 characters or fewer")
;
RuleFor(m => m.senderemail)
.NotEmpty().WithMessage("Sender Email is required")
.MaximumLength(128).WithMessage("The allowed length of Sender Email is 128 characters or fewer")
.EmailAddress()

;
RuleFor(m => m.password)
.NotEmpty().WithMessage("Password is required")
.MaximumLength(128).WithMessage("The allowed length of Password is 128 characters or fewer")
;
RuleFor(m => m.emailhostname)
.NotEmpty().WithMessage("Email Host Name is required")
.MaximumLength(128).WithMessage("The allowed length of Email Host Name is 128 characters or fewer")
;
RuleFor(m => m.portnumber)
.NotNull().WithMessage("Port Number is required")
.LessThanOrEqualTo(99999999).WithMessage("Port Number should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.applicableservice)
.NotEmpty().WithMessage("Applicable Service is required")
;

}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
