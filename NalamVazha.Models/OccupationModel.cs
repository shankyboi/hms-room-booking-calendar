namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:36
			public class OccupationModel
			{

			 public System.Guid ?Occupationid	{ get; set; }

[xssFilter]
public string occupationname{ get; set; }

[xssFilter]
public string? occupationdesc{ get; set; }
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
			

			public class OccupationModelValidator: AbstractValidator<OccupationModel>
			{
					 
					public OccupationModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Occupation", () =>
                                    {
                                        {RuleFor(m => m.occupationname)
.NotEmpty().WithMessage("Occupation Name is required")
.MaximumLength(128).WithMessage("The allowed length of Occupation Name is 128 characters or fewer")
;

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Occupation", () =>
                                    {
                                        {RuleFor(m => m.occupationname)
.NotEmpty().WithMessage("Occupation Name is required")
.MaximumLength(128).WithMessage("The allowed length of Occupation Name is 128 characters or fewer")
;

}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
