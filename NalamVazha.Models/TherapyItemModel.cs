namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:42
			public class TherapyItemModel
			{

			 public System.Guid ?TherapyItemid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid therapyitemcategory	{ get; set; }

[xssFilter]
public string therapyitemname{ get; set; }

public decimal price{ get; set; }

[xssFilter]
										
               public string? therapyitemimage{ get; set; }
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
			

			public class TherapyItemModelValidator: AbstractValidator<TherapyItemModel>
			{
					 
					public TherapyItemModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Therapy_Item", () =>
                                    {
                                        {RuleFor(m => m.therapyitemcategory)
.NotEmpty().WithMessage("Therapy Item Category is required")
;
RuleFor(m => m.therapyitemname)
.NotEmpty().WithMessage("Therapy Item Name is required")
.MaximumLength(128).WithMessage("The allowed length of Therapy Item Name is 128 characters or fewer")
;
RuleFor(m => m.price)
.NotNull().WithMessage("Price is required")
.LessThanOrEqualTo(99999999).WithMessage("Price should be LessThanOrEqualTo 99999999")

;

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Therapy_Item", () =>
                                    {
                                        {RuleFor(m => m.therapyitemcategory)
.NotEmpty().WithMessage("Therapy Item Category is required")
;
RuleFor(m => m.therapyitemname)
.NotEmpty().WithMessage("Therapy Item Name is required")
.MaximumLength(128).WithMessage("The allowed length of Therapy Item Name is 128 characters or fewer")
;
RuleFor(m => m.price)
.NotNull().WithMessage("Price is required")
.LessThanOrEqualTo(99999999).WithMessage("Price should be LessThanOrEqualTo 99999999")

;

}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
