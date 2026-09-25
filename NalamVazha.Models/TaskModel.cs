namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/30/2026 16:03:22
			public class TaskModel
			{

			 public System.Guid ?Taskid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid tasktype	{ get; set; }

[xssFilter]
public string taskname{ get; set; }

[xssFilter]
public string? taskdesc{ get; set; }

[xssFilter]
public string? allowpatienttoselectappointmentslot{ get; set; }

[xssFilter]
public string? taskfeestype{ get; set; }

public decimal? taskcharges{ get; set; }

public double? slainhrs{ get; set; }
public ICollection <Task_screeningtemplatesModel> screeningtemplates { get; set; }
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
			

			public class TaskModelValidator: AbstractValidator<TaskModel>
			{
					 
					public TaskModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Task", () =>
                                    {
                                        {RuleFor(m => m.tasktype)
.NotEmpty().WithMessage("Task Type is required")
;
RuleFor(m => m.taskname)
.NotEmpty().WithMessage("Task Name is required")
.MaximumLength(128).WithMessage("The allowed length of Task Name is 128 characters or fewer")
;





}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Task", () =>
                                    {
                                        {RuleFor(m => m.tasktype)
.NotEmpty().WithMessage("Task Type is required")
;
RuleFor(m => m.taskname)
.NotEmpty().WithMessage("Task Name is required")
.MaximumLength(128).WithMessage("The allowed length of Task Name is 128 characters or fewer")
;





}

                                    });

						 RuleForEach(x => x.screeningtemplates).SetValidator(new Task_screeningtemplatesModelValidator());

						
					}

			}

                
			 public class Task_screeningtemplatesModel
			{

			 
public Guid? screeningtemplate	{ get; set; }


public string? isdefaulttemplate{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?Task_screeningtemplatesid { get; set; }
public System.Guid ?Taskid { get; set; }



			}
			

			public class Task_screeningtemplatesModelValidator: AbstractValidator<Task_screeningtemplatesModel>
			{
				   
					public Task_screeningtemplatesModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Task", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Task", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
