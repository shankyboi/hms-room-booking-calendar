namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:36
			public class ClinicalTaskModel
			{

			 public System.Guid ?ClinicalTaskid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid workprofile	{ get; set; }

public Guid competency	{ get; set; }
public ICollection <ClinicalTask_taskdurationModel> taskduration { get; set; }
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
			

			public class ClinicalTaskModelValidator: AbstractValidator<ClinicalTaskModel>
			{
					 
					public ClinicalTaskModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Clinical_Task", () =>
                                    {
                                        {RuleFor(m => m.workprofile)
.NotEmpty().WithMessage("Work Profile is required")
;
RuleFor(m => m.competency)
.NotEmpty().WithMessage("Competency is required")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Clinical_Task", () =>
                                    {
                                        {RuleFor(m => m.workprofile)
.NotEmpty().WithMessage("Work Profile is required")
;
RuleFor(m => m.competency)
.NotEmpty().WithMessage("Competency is required")
;
}

                                    });

						 RuleForEach(x => x.taskduration).SetValidator(new ClinicalTask_taskdurationModelValidator());

						
					}

			}

                
			 public class ClinicalTask_taskdurationModel
			{

			 
public Guid workprofile	{ get; set; }

public Guid tasktype	{ get; set; }

public Guid taskname	{ get; set; }

public int durationinminutes{ get; set; }

public int? overbookingcount{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?ClinicalTask_taskdurationid { get; set; }
public System.Guid ?ClinicalTaskid { get; set; }



			}
			

			public class ClinicalTask_taskdurationModelValidator: AbstractValidator<ClinicalTask_taskdurationModel>
			{
				   
					public ClinicalTask_taskdurationModelValidator()
					{

RuleFor(m => m.durationinminutes)
.LessThanOrEqualTo(1440).WithMessage("Duration in Minutes cannot exceed 1440.")
;

						 When(model => model.craftmyapp_actionmethodname == "Add_Clinical_Task", () =>
                                                {
                                                    {RuleFor(m => m.workprofile)
.NotEmpty().WithMessage("Work Profile is required")
;
}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Clinical_Task", () =>
                                                {
                                                    {RuleFor(m => m.workprofile)
.NotEmpty().WithMessage("Work Profile is required")
;
}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
