namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:40:36
			public class TaskActionLogModel
			{

			 public System.Guid ?TaskActionLogid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid? taskname	{ get; set; }

public Guid? tasktype	{ get; set; }

[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode=true)]public DateTime? actiondate	{ get; set; }

public Guid? actionby	{ get; set; }

[xssFilter]
public string? comments{ get; set; }

public Guid? assignto	{ get; set; }

public Guid? escalateto	{ get; set; }

[xssFilter]
public string? summary{ get; set; }

[xssFilter]
public string? description{ get; set; }
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
			

			public class TaskActionLogModelValidator: AbstractValidator<TaskActionLogModel>
			{
					 
					public TaskActionLogModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Finance_Task", () =>
                                    {
                                        {



}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Finance_Task", () =>
                                    {
                                        {



}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
