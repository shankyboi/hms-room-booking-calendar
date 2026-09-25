namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:05
			public class BlockModel
			{

			 public System.Guid ?Blockid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string blockcode{ get; set; }

[xssFilter]
public string blockname{ get; set; }

[xssFilter]
public string? blockdescription{ get; set; }

[xssFilter]
										
               public string? blockimage{ get; set; }

[xssFilter]
public string? blocklocationurl{ get; set; }

[xssFilter]
public string? blocknearbylandmark{ get; set; }
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
			

			public class BlockModelValidator: AbstractValidator<BlockModel>
			{
					 
					public BlockModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Block", () =>
                                    {
                                        {RuleFor(m => m.blockcode)
.NotEmpty().WithMessage("Block Code is required")
.MaximumLength(128).WithMessage("The allowed length of Block Code is 128 characters or fewer")
;
RuleFor(m => m.blockname)
.NotEmpty().WithMessage("Block Name is required")
.MaximumLength(128).WithMessage("The allowed length of Block Name is 128 characters or fewer")
;




}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Block", () =>
                                    {
                                        {RuleFor(m => m.blockcode)
.NotEmpty().WithMessage("Block Code is required")
.MaximumLength(128).WithMessage("The allowed length of Block Code is 128 characters or fewer")
;
RuleFor(m => m.blockname)
.NotEmpty().WithMessage("Block Name is required")
.MaximumLength(128).WithMessage("The allowed length of Block Name is 128 characters or fewer")
;




}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
