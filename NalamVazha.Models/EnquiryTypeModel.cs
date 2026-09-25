namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:50
			public class EnquiryTypeModel
			{

			 public System.Guid ?EnquiryTypeid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string enquiryname{ get; set; }

[xssFilter]
public string? enquirydesc{ get; set; }

[xssFilter]
public string isroombookingrelated{ get; set; }
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
			

			public class EnquiryTypeModelValidator: AbstractValidator<EnquiryTypeModel>
			{
					 
					public EnquiryTypeModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Enquiry_Type", () =>
                                    {
                                        {RuleFor(m => m.enquiryname)
.NotEmpty().WithMessage("Enquiry Name is required")
.MaximumLength(128).WithMessage("The allowed length of Enquiry Name is 128 characters or fewer")
;

RuleFor(m => m.isroombookingrelated)
.NotEmpty().WithMessage("Is Room Booking Related is required")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Enquiry_Type", () =>
                                    {
                                        {RuleFor(m => m.enquiryname)
.NotEmpty().WithMessage("Enquiry Name is required")
.MaximumLength(128).WithMessage("The allowed length of Enquiry Name is 128 characters or fewer")
;

RuleFor(m => m.isroombookingrelated)
.NotEmpty().WithMessage("Is Room Booking Related is required")
;
}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
