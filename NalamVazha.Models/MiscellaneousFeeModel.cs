namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 16:38:54
			public class MiscellaneousFeeModel
			{

			 public System.Guid ?MiscellaneousFeeid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string feetype{ get; set; }

public decimal admissionfee{ get; set; }

public bool isrefundable	{ get; set; }
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
			

			public class MiscellaneousFeeModelValidator: AbstractValidator<MiscellaneousFeeModel>
			{
					 
					public MiscellaneousFeeModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Miscellaneous_Fee", () =>
                                    {
                                        {RuleFor(m => m.feetype)
.NotEmpty().WithMessage("Fee Type is required")
;
RuleFor(m => m.admissionfee)
.NotNull().WithMessage("Admission Fee is required")
;

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Miscellaneous_Fee", () =>
                                    {
                                        {RuleFor(m => m.feetype)
.NotEmpty().WithMessage("Fee Type is required")
;
RuleFor(m => m.admissionfee)
.NotNull().WithMessage("Admission Fee is required")
;

}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
