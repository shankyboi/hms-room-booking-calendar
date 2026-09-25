namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:39:40
			public class DailyTaskModel
			{

			 public System.Guid ?DailyTaskid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string taskno{ get; set; }

[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode=true)]public DateTime? dateandtime	{ get; set; }

public Guid tasktype	{ get; set; }

public Guid? taskname	{ get; set; }

public Guid? patientname	{ get; set; }

[xssFilter]
public string? patientcategory{ get; set; }

public Guid? ipdreferencenumber	{ get; set; }

public Guid? opdreferencenumber	{ get; set; }

[xssFilter]
public string? status{ get; set; }

public int? amount{ get; set; }
public string? activityname { get; set; }
public string? description { get; set; }
public string? priority { get; set; }
public Guid? assignedto { get; set; }
public string? assignedto_master { get; set; }
public Guid? closedby { get; set; }
[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode=true)]public DateTime? closeddate { get; set; }
public string? closedby_master { get; set; }
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

			public class CloseDailyTaskModel
			{
				public Guid DailyTaskid { get; set; }
				public Guid? tenantid { get; set; }
				public string? comments { get; set; }
			}
			

			public class DailyTaskModelValidator: AbstractValidator<DailyTaskModel>
			{
					 
					public DailyTaskModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Daily_Task", () =>
                                    {
                                        {RuleFor(m => m.taskno)
.MaximumLength(256).WithMessage("The allowed length of Task No  is 256 characters or fewer")
;
RuleFor(m => m.tasktype)
.NotEmpty().WithMessage("Task Type  is required")
;





RuleFor(m => m.amount)
.LessThanOrEqualTo(99999999).WithMessage("Amount should be LessThanOrEqualTo 99999999")

;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Daily_Task", () =>
                                    {
                                        {RuleFor(m => m.tasktype)
.NotEmpty().WithMessage("Task Type  is required")
;





RuleFor(m => m.amount)
.LessThanOrEqualTo(99999999).WithMessage("Amount should be LessThanOrEqualTo 99999999")

;
}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
