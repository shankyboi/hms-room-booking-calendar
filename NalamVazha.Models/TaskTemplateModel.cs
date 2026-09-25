namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:27:26
			public class TaskTemplateModel
			{

			 public System.Guid ?TaskTemplateid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid? tasktype	{ get; set; }

[xssFilter]
public string? taskname{ get; set; }

[xssFilter]
public string? priority{ get; set; }

public int? duration{ get; set; }
public ICollection <TaskTemplate_escalationdetailsModel> escalationdetails { get; set; }
public ICollection <TaskTemplate_nextactiondetailsModel> nextactiondetails { get; set; }
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
			

			public class TaskTemplateModelValidator: AbstractValidator<TaskTemplateModel>
			{
					 
					public TaskTemplateModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Task_Template", () =>
                                    {
                                        {


RuleFor(m => m.duration)
.LessThanOrEqualTo(99999999).WithMessage("Duration should be LessThanOrEqualTo 99999999")

;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Task_Template", () =>
                                    {
                                        {


RuleFor(m => m.duration)
.LessThanOrEqualTo(99999999).WithMessage("Duration should be LessThanOrEqualTo 99999999")

;
}

                                    });

						 RuleForEach(x => x.escalationdetails).SetValidator(new TaskTemplate_escalationdetailsModelValidator());
RuleForEach(x => x.nextactiondetails).SetValidator(new TaskTemplate_nextactiondetailsModelValidator());

						
					}

			}

                
			 public class TaskTemplate_escalationdetailsModel
			{

			 

public string? priority{ get; set; }

public Guid? notifyto	{ get; set; }


public string? emailid{ get; set; }


public string? mobilenumber{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?TaskTemplate_escalationdetailsid { get; set; }
public System.Guid ?TaskTemplateid { get; set; }



			}
			

			public class TaskTemplate_escalationdetailsModelValidator: AbstractValidator<TaskTemplate_escalationdetailsModel>
			{
				   
					public TaskTemplate_escalationdetailsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Task_Template", () =>
                                                {
                                                    {
}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Task_Template", () =>
                                                {
                                                    {
}

                                                });

						
					}

			}



			 public class TaskTemplate_nextactiondetailsModel
			{

			 
public Guid? actiontype	{ get; set; }

public Guid? actionname	{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?TaskTemplate_nextactiondetailsid { get; set; }
public System.Guid ?TaskTemplateid { get; set; }



			}
			

			public class TaskTemplate_nextactiondetailsModelValidator: AbstractValidator<TaskTemplate_nextactiondetailsModel>
			{
				   
					public TaskTemplate_nextactiondetailsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Task_Template", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Task_Template", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
