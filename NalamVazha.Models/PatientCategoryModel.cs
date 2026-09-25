namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/17/2026 05:47:03
			public class PatientCategoryModel
			{

			 public System.Guid ?PatientCategoryid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string patientcategoryname{ get; set; }

[xssFilter]
public string? patientcategorydesc{ get; set; }
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
			

			public class PatientCategoryModelValidator: AbstractValidator<PatientCategoryModel>
			{
					 
					public PatientCategoryModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Patient_Category", () =>
                                    {
                                        {RuleFor(m => m.patientcategoryname)
.NotEmpty().WithMessage("Patient Category Name is required")
.MaximumLength(128).WithMessage("The allowed length of Patient Category Name is 128 characters or fewer")
;

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Patient_Category", () =>
                                    {
                                        {RuleFor(m => m.patientcategoryname)
.NotEmpty().WithMessage("Patient Category Name is required")
.MaximumLength(128).WithMessage("The allowed length of Patient Category Name is 128 characters or fewer")
;

}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
