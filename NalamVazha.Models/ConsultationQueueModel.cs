namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 14:05:35
			public class ConsultationQueueModel
			{

			 public System.Guid ?ConsultationQueueid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string queuename{ get; set; }

[xssFilter]
public string? queuenumber{ get; set; }

public Guid? department	{ get; set; }
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
			

			public class ConsultationQueueModelValidator: AbstractValidator<ConsultationQueueModel>
			{
					 
					public ConsultationQueueModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Consultation_Queue", () =>
                                    {
                                        {RuleFor(m => m.queuename)
.NotEmpty().WithMessage("Queue Name is required")
.MaximumLength(128).WithMessage("The allowed length of Queue Name is 128 characters or fewer")
;


}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Consultation_Queue", () =>
                                    {
                                        {RuleFor(m => m.queuename)
.NotEmpty().WithMessage("Queue Name is required")
.MaximumLength(128).WithMessage("The allowed length of Queue Name is 128 characters or fewer")
;


}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
