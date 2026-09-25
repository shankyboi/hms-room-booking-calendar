namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:08
			public class BuildingModel
			{

			 public System.Guid ?Buildingid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid block	{ get; set; }

[xssFilter]
public string buildingcode{ get; set; }

[xssFilter]
public string buildingname{ get; set; }

[xssFilter]
public string? buildingdescription{ get; set; }

[xssFilter]
										
               public string? buildingimage{ get; set; }

[xssFilter]
public string? buildingnearbylandmark{ get; set; }
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
			

			public class BuildingModelValidator: AbstractValidator<BuildingModel>
			{
					 
					public BuildingModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Building", () =>
                                    {
                                        {RuleFor(m => m.block)
.NotEmpty().WithMessage("Block is required")
;
RuleFor(m => m.buildingcode)
.NotEmpty().WithMessage("Building Code is required")
.MaximumLength(128).WithMessage("The allowed length of Building Code is 128 characters or fewer")
;
RuleFor(m => m.buildingname)
.NotEmpty().WithMessage("Building Name is required")
.MaximumLength(128).WithMessage("The allowed length of Building Name is 128 characters or fewer")
;



}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Building", () =>
                                    {
                                        {RuleFor(m => m.block)
.NotEmpty().WithMessage("Block is required")
;
RuleFor(m => m.buildingcode)
.NotEmpty().WithMessage("Building Code is required")
.MaximumLength(128).WithMessage("The allowed length of Building Code is 128 characters or fewer")
;
RuleFor(m => m.buildingname)
.NotEmpty().WithMessage("Building Name is required")
.MaximumLength(128).WithMessage("The allowed length of Building Name is 128 characters or fewer")
;



}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
