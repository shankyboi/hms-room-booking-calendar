namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:43:41
			public class DischargeModel
			{

			 public System.Guid ?Dischargeid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid? ipdnumber	{ get; set; }

public Guid? patient	{ get; set; }

public Guid? room	{ get; set; }

public Guid discharge	{ get; set; }

public int? daysofstay{ get; set; }

public Guid? pendingamount	{ get; set; }

public Guid? paymentstatus	{ get; set; }

public Guid? refundamount	{ get; set; }

public Guid? refundstatus	{ get; set; }

[xssFilter]
public string? feedbackstatus{ get; set; }
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
			

			public class DischargeModelValidator: AbstractValidator<DischargeModel>
			{
					 
					public DischargeModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Discharge", () =>
                                    {
                                        {


RuleFor(m => m.discharge)
.NotEmpty().WithMessage("Discharge is required")
;
RuleFor(m => m.daysofstay)
.LessThanOrEqualTo(99999999).WithMessage("Days Of Stay should be LessThanOrEqualTo 99999999")

;





}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Discharge", () =>
                                    {
                                        {


RuleFor(m => m.discharge)
.NotEmpty().WithMessage("Discharge is required")
;
RuleFor(m => m.daysofstay)
.LessThanOrEqualTo(99999999).WithMessage("Days Of Stay should be LessThanOrEqualTo 99999999")

;





}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
