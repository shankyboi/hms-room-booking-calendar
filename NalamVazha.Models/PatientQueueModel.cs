namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/30/2026 16:15:01
			public class PatientQueueModel
			{

			 public System.Guid ?PatientQueueid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid patientvisit	{ get; set; }

[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode=true)]public DateTime? visitdatetime	{ get; set; }

public Guid? patientname	{ get; set; }

public Guid queuename	{ get; set; }

[xssFilter]
public string queuestatus{ get; set; }

[xssFilter]
public string? priority{ get; set; }

[xssFilter]
public string? checkintime{ get; set; }

[xssFilter]
public string? endtime{ get; set; }
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
			

			public class PatientQueueModelValidator: AbstractValidator<PatientQueueModel>
			{
					 
					public PatientQueueModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Patient_Queue", () =>
                                    {
                                        {RuleFor(m => m.patientvisit)
.NotEmpty().WithMessage("Patient Visit is required")
;


RuleFor(m => m.queuename)
.NotEmpty().WithMessage("Queue Name is required")
;
RuleFor(m => m.queuestatus)
.NotEmpty().WithMessage("Queue Status is required")
;



}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Patient_Queue", () =>
                                    {
                                        {RuleFor(m => m.patientvisit)
.NotEmpty().WithMessage("Patient Visit is required")
;


RuleFor(m => m.queuename)
.NotEmpty().WithMessage("Queue Name is required")
;
RuleFor(m => m.queuestatus)
.NotEmpty().WithMessage("Queue Status is required")
;



}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
