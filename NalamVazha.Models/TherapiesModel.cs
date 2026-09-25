namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:30
			public class TherapiesModel
			{

			 public System.Guid ?Therapiesid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid therapycategory	{ get; set; }

[xssFilter]
public string therapyname{ get; set; }

public decimal therapycost{ get; set; }

public int standarddurationinmins{ get; set; }

[xssFilter]
										
               public string? therapyimage{ get; set; }

[xssFilter]
public string? therapyvideourl{ get; set; }

[xssFilter]
public string? therapyinstructions{ get; set; }

public bool isgrouptherapyallowed	{ get; set; }
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
			

			public class TherapiesModelValidator: AbstractValidator<TherapiesModel>
			{
					 
					public TherapiesModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Therapy", () =>
                                    {
                                        {RuleFor(m => m.therapycategory)
.NotEmpty().WithMessage("Therapy Category is required")
;
RuleFor(m => m.therapyname)
.NotEmpty().WithMessage("Therapy Name is required")
.MaximumLength(128).WithMessage("The allowed length of Therapy Name is 128 characters or fewer")
;
RuleFor(m => m.therapycost)
.NotNull().WithMessage("Therapy Cost is required")
.LessThanOrEqualTo(99999999).WithMessage("Therapy Cost should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.standarddurationinmins)
.NotNull().WithMessage("Standard Duration in Mins is required")
.LessThanOrEqualTo(240).WithMessage("Standard Duration in Mins should be LessThanOrEqualTo 240")

;




}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Therapy", () =>
                                    {
                                        {RuleFor(m => m.therapycategory)
.NotEmpty().WithMessage("Therapy Category is required")
;
RuleFor(m => m.therapyname)
.NotEmpty().WithMessage("Therapy Name is required")
.MaximumLength(128).WithMessage("The allowed length of Therapy Name is 128 characters or fewer")
;
RuleFor(m => m.therapycost)
.NotNull().WithMessage("Therapy Cost is required")
.LessThanOrEqualTo(99999999).WithMessage("Therapy Cost should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.standarddurationinmins)
.NotNull().WithMessage("Standard Duration in Mins is required")
.LessThanOrEqualTo(240).WithMessage("Standard Duration in Mins should be LessThanOrEqualTo 240")

;




}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
