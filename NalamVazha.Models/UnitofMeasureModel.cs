namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:42
			public class UnitofMeasureModel
			{

			 public System.Guid ?UnitofMeasureid	{ get; set; }

[xssFilter]
public string unitofmeasurename{ get; set; }

[xssFilter]
public string? unitofmeasuredesc{ get; set; }
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
			

			public class UnitofMeasureModelValidator: AbstractValidator<UnitofMeasureModel>
			{
					 
					public UnitofMeasureModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Unit_of_Measure", () =>
                                    {
                                        {RuleFor(m => m.unitofmeasurename)
.NotEmpty().WithMessage("Unit of Measure Name is required")
.MaximumLength(128).WithMessage("The allowed length of Unit of Measure Name is 128 characters or fewer")
;

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Unit_of_Measure", () =>
                                    {
                                        {RuleFor(m => m.unitofmeasurename)
.NotEmpty().WithMessage("Unit of Measure Name is required")
.MaximumLength(128).WithMessage("The allowed length of Unit of Measure Name is 128 characters or fewer")
;

}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
