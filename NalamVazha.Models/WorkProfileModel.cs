namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:30
			public class WorkProfileModel
			{

			 public System.Guid ?WorkProfileid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid department	{ get; set; }

[xssFilter]
public string workprofilename{ get; set; }

[xssFilter]
public string rolename{ get; set; }

[xssFilter]
public string isthisaclinicalprofile{ get; set; }

[xssFilter]
public string? workprofiledescription{ get; set; }
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
			

			public class WorkProfileModelValidator: AbstractValidator<WorkProfileModel>
			{
					 
					public WorkProfileModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Work_Profile", () =>
                                    {
                                        {RuleFor(m => m.department)
.NotEmpty().WithMessage("Department is required")
;
RuleFor(m => m.workprofilename)
.NotEmpty().WithMessage("Work Profile Name is required")
.MaximumLength(128).WithMessage("The allowed length of Work Profile Name is 128 characters or fewer")
;
RuleFor(m => m.rolename)
.NotEmpty().WithMessage("Role Name is required")
;
RuleFor(m => m.isthisaclinicalprofile)
.NotEmpty().WithMessage("Is This a Clinical Profile is required")
;

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Work_Profile", () =>
                                    {
                                        {RuleFor(m => m.department)
.NotEmpty().WithMessage("Department is required")
;
RuleFor(m => m.workprofilename)
.NotEmpty().WithMessage("Work Profile Name is required")
.MaximumLength(128).WithMessage("The allowed length of Work Profile Name is 128 characters or fewer")
;
RuleFor(m => m.rolename)
.NotEmpty().WithMessage("Role Name is required")
;
RuleFor(m => m.isthisaclinicalprofile)
.NotEmpty().WithMessage("Is This a Clinical Profile is required")
;

}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
