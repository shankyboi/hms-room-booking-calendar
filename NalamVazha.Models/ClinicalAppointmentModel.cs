namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:34
			public class ClinicalAppointmentModel
			{

			 public System.Guid ?ClinicalAppointmentid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string tasktype{ get; set; }

public Guid? patient	{ get; set; }

public Guid? practitioner	{ get; set; }

        public string? taskname { get; set; }

        [xssFilter]
										
               public string? photo{ get; set; }

public Guid? actualpractitioner	{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime appointmentdate	{ get; set; }

[xssFilter]
public string durationfrom{ get; set; }

[xssFilter]
public string durationto{ get; set; }

[xssFilter]
public string status{ get; set; }

[xssFilter]
public string? origin{ get; set; }

[xssFilter]
public string? bookingid{ get; set; }

[xssFilter]
public string tokennumber{ get; set; }
public ICollection <ClinicalAppointment_reshedulehistoryModel> reshedulehistory { get; set; }
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
			

			public class ClinicalAppointmentModelValidator: AbstractValidator<ClinicalAppointmentModel>
			{
					 
					public ClinicalAppointmentModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Clinical_Appointment", () =>
                                    {
                                        {RuleFor(m => m.tasktype)
.NotEmpty().WithMessage("Task Type is required")
;
//RuleFor(m => m.patient)
//.NotEmpty().WithMessage("Patient is required")
//;



RuleFor(m => m.appointmentdate)
.NotEmpty().WithMessage("Appointment Date is required")
 
;
RuleFor(m => m.durationfrom)
.NotEmpty().WithMessage("Duration From is required")
;
RuleFor(m => m.durationto)
.NotEmpty().WithMessage("Duration To is required")
;
RuleFor(m => m.status)
.NotEmpty().WithMessage("Status is required")
;


RuleFor(m => m.tokennumber)
.MaximumLength(256).WithMessage("The allowed length of Token Number is 256 characters or fewer")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Reschedule_Appointment", () =>
                                    {
                                        {RuleFor(m => m.tasktype)
.NotEmpty().WithMessage("Task Type is required")
;
//RuleFor(m => m.patient)
//.NotEmpty().WithMessage("Patient is required")
//;



RuleFor(m => m.appointmentdate)
.NotEmpty().WithMessage("Appointment Date is required")
 
;
RuleFor(m => m.durationfrom)
.NotEmpty().WithMessage("Duration From is required")
;
RuleFor(m => m.durationto)
.NotEmpty().WithMessage("Duration To is required")
;
RuleFor(m => m.status)
.NotEmpty().WithMessage("Status is required")
;


RuleFor(m => m.tokennumber)
.MaximumLength(256).WithMessage("The allowed length of Token Number is 256 characters or fewer")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Clinical_Appointment", () =>
                                    {
                                        {RuleFor(m => m.tasktype)
.NotEmpty().WithMessage("Task Type is required")
;
//RuleFor(m => m.patient)
//.NotEmpty().WithMessage("Patient is required")
//;



//RuleFor(m => m.appointmentdate)
//.NotEmpty().WithMessage("Appointment Date is required")
//.LessThanOrEqualTo(System.DateTime.Now.Date).WithMessage("Appointment Date should be Today or Past Date")

//;
RuleFor(m => m.durationfrom)
.NotEmpty().WithMessage("Duration From is required")
;
RuleFor(m => m.durationto)
.NotEmpty().WithMessage("Duration To is required")
;
RuleFor(m => m.status)
.NotEmpty().WithMessage("Status is required")
;


RuleFor(m => m.tokennumber)
.MaximumLength(256).WithMessage("The allowed length of Token Number is 256 characters or fewer")
;
}

                                    });

						 RuleForEach(x => x.reshedulehistory).SetValidator(new ClinicalAppointment_reshedulehistoryModelValidator());

						
					}

			}

                
			 public class ClinicalAppointment_reshedulehistoryModel
			{

			 
public DateTime? resheduleddatetime	{ get; set; }


public string? reshedulereason{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?ClinicalAppointment_reshedulehistoryid { get; set; }
public System.Guid ?ClinicalAppointmentid { get; set; }



			}
			

			public class ClinicalAppointment_reshedulehistoryModelValidator: AbstractValidator<ClinicalAppointment_reshedulehistoryModel>
			{
				   
					public ClinicalAppointment_reshedulehistoryModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Clinical_Appointment", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Reschedule_Appointment", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Clinical_Appointment", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			
	public class AppointmentStatusUpdateModel
	{
		public System.Guid? ClinicalAppointmentid { get; set; }

		[xssFilter]
		public string status { get; set; }
	}

	/// <summary>Marks IP Screening clinical appointments Completed for an IPD booking (after admission confirmation).</summary>
	public class CompleteIPScreeningForIPDRequestModel
	{
		public string IPDApplicationFormid { get; set; }
	}
}
