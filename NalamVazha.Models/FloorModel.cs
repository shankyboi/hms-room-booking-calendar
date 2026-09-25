namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:10
			public class FloorModel
			{

			 public System.Guid ?Floorid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid block	{ get; set; }

public Guid building	{ get; set; }

[xssFilter]
public string floorname{ get; set; }
		public System.Int64 floornumber { get; set; }

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
			

			public class FloorModelValidator: AbstractValidator<FloorModel>
			{
					 
					public FloorModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Floor", () =>
                                    {
                                        {RuleFor(m => m.block)
.NotEmpty().WithMessage("Block is required")
;
RuleFor(m => m.building)
.NotEmpty().WithMessage("Building is required")
;
RuleFor(m => m.floorname)
.NotEmpty().WithMessage("Floor Name is required")
.MaximumLength(128).WithMessage("The allowed length of Floor Name is 128 characters or fewer")
;
											RuleFor(m => m.floornumber)
.NotEmpty().WithMessage("Floor Number is required")
.LessThanOrEqualTo(99999999).WithMessage("Floor Number should be LessThanOrEqualTo 99999999")

;

										}

									});
When(model => model.craftmyapp_actionmethodname == "Update_Floor", () =>
                                    {
                                        {RuleFor(m => m.block)
.NotEmpty().WithMessage("Block is required")
;
RuleFor(m => m.building)
.NotEmpty().WithMessage("Building is required")
;
RuleFor(m => m.floorname)
.NotEmpty().WithMessage("Floor Name is required")
.MaximumLength(128).WithMessage("The allowed length of Floor Name is 128 characters or fewer")
;
											RuleFor(m => m.floornumber)
.NotEmpty().WithMessage("Floor Number is required")
.LessThanOrEqualTo(99999999).WithMessage("Floor Number should be LessThanOrEqualTo 99999999")

;

										}

									});

						 
						
					}

			}

                

                

                
 

                

                

        

			}
