namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:54
			public class OPDFormModel
			{

			 public System.Guid ?OPDFormid	{ get; set; }
public System.Guid ?verifiedby	{ get; set; }
[DataType(DataType.Date)]
[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
[DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]
public System.DateTime ?verifieddate	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string bookingreferencenumber{ get; set; }

public Guid patientname	{ get; set; }
		[xssFilter]
public string? appointmentmode{ get; set; }
		public Guid? task { get; set; }
		public Guid? preferreddoctor	{ get; set; }
		[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)] public DateTime? preferreddate { get; set; }
		[xssFilter]
public string? verifiedstatus{ get; set; }
public string? bookingemergencycontactsjson { get; set; }
public ICollection <OPDForm_medicalinfoModel> medicalinfo { get; set; }
public ICollection <OPDForm_medicationinfoModel> medicationinfo { get; set; }
public ICollection <OPDForm_medicalrecordsModel> medicalrecords { get; set; }
public ICollection <OPDForm_appointmentpreferencesModel> appointmentpreferences { get; set; }
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


			public class OPDFormModelValidator: AbstractValidator<OPDFormModel>
			{

					public OPDFormModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_OPD_Form", () =>
                                    {
                                        {RuleFor(m => m.bookingreferencenumber)
.MaximumLength(256).WithMessage("The allowed length of Booking Reference Number is 256 characters or fewer")
;
RuleFor(m => m.patientname)
.NotEmpty().WithMessage("Patient Name is required")
;
										}

									});
When(model => model.craftmyapp_actionmethodname == "Update_OPD_Form", () =>
                                    {
                                        {RuleFor(m => m.bookingreferencenumber)
.MaximumLength(256).WithMessage("The allowed length of Booking Reference Number is 256 characters or fewer")
;
RuleFor(m => m.patientname)
.NotEmpty().WithMessage("Patient Name is required")
;

										}

									});

RuleForEach(x => x.medicalinfo).SetValidator(new OPDForm_medicalinfoModelValidator());
RuleForEach(x => x.medicationinfo).SetValidator(new OPDForm_medicationinfoModelValidator());
RuleForEach(x => x.medicalrecords).SetValidator(new OPDForm_medicalrecordsModelValidator());
RuleForEach(x => x.appointmentpreferences).SetValidator(new OPDForm_appointmentpreferencesModelValidator());


					}

			}


			 public class OPDForm_medicalinfoModel
			{


public Guid medicalconditionname	{ get; set; }

public decimal duration{ get; set; }


public string unit{ get; set; }


public string severitylevel{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?OPDForm_medicalinfoid { get; set; }
public System.Guid ?OPDFormid { get; set; }



			}


			public class OPDForm_medicalinfoModelValidator: AbstractValidator<OPDForm_medicalinfoModel>
			{

					public OPDForm_medicalinfoModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_OPD_Form", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_OPD_Form", () =>
                                                {
                                                    {}

                                                });


					}

			}



			 public class OPDForm_medicationinfoModel
			{



public string medicinename{ get; set; }


public string frequencyinaday{ get; set; }


public string? medicationduration{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?OPDForm_medicationinfoid { get; set; }
public System.Guid ?OPDFormid { get; set; }



			}


			public class OPDForm_medicationinfoModelValidator: AbstractValidator<OPDForm_medicationinfoModel>
			{

					public OPDForm_medicationinfoModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_OPD_Form", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_OPD_Form", () =>
                                                {
                                                    {}

                                                });


					}

			}



			 public class OPDForm_medicalrecordsModel
			{



public string medicalrecordname{ get; set; }

[xssFilter]
public string medicalrecordfile{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?OPDForm_medicalrecordsid { get; set; }
public System.Guid ?OPDFormid { get; set; }



			}


			public class OPDForm_medicalrecordsModelValidator: AbstractValidator<OPDForm_medicalrecordsModel>
			{

					public OPDForm_medicalrecordsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_OPD_Form", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_OPD_Form", () =>
                                                {
                                                    {}

                                                });


					}

			}


			 public class OPDForm_appointmentpreferencesModel
			{


public DateTime? preferreddate	{ get; set; }


public string? slotpreference{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?OPDForm_appointmentpreferencesid { get; set; }
public System.Guid ?OPDFormid { get; set; }



			}


			public class OPDForm_appointmentpreferencesModelValidator: AbstractValidator<OPDForm_appointmentpreferencesModel>
			{

					public OPDForm_appointmentpreferencesModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_OPD_Form", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_OPD_Form", () =>
                                                {
                                                    {}

                                                });


					}

			}







    public class OPDBulkReviewRequest
    {
        public string verifiedstatus { get; set; }

        public string reviewcomments { get; set; }

        public List<OPDBulkReviewItem> items { get; set; }
    }


    public class OPDBulkReviewItem
    {
        public string OPDFormid { get; set; }

        public string tenantid { get; set; }

        public string patient { get; set; }

        public string practitioner { get; set; }

        public string appointmentdate { get; set; }

        public string durationfrom { get; set; }

        public string durationto { get; set; }

        public string task { get; set; }

        public string appointmentmode { get; set; }
    }






    public class OPDFormReviewModel
                                                    {
                                                        public string OPDFormid { get; set; }
                                                        public string reviewcomments { get; set; }
                                                        public string verifiedstatus { get; set; }
                                                        public string verifiedby { get; set; }

                                                        /* Appointment fields — sent by Review_OPD when Approving with a selected slot.
                                                           Used to create a Scheduled appointment (scenario 2: date-only health seeker, or reviewer-assigned slot). */
                                                        public string patient { get; set; }
                                                        public string practitioner { get; set; }
                                                        public string appointmentdate { get; set; }
                                                        public string durationfrom { get; set; }
                                                        public string durationto { get; set; }
                                                        public string tenantid { get; set; }
                                                        public string task { get; set; }
                                                        public string appointmentmode { get; set; }

                                                    }

	public class OPDRazorpayOrderRequestModel
	{
		public string OPDFormid { get; set; }
	}

	public class OPDCompletePaymentModel
	{
		public string OPDFormid { get; set; }
		public string paymentrequestid { get; set; }
		public string razorpay_order_id { get; set; }
		public string razorpay_payment_id { get; set; }
		public string razorpay_signature { get; set; }
	}

	public class OPDFailedPaymentModel
	{
		public string OPDFormid { get; set; }
		public string paymentrequestid { get; set; }
		public string razorpay_order_id { get; set; }
		public string razorpay_payment_id { get; set; }
		public string error_code { get; set; }
		public string error_description { get; set; }
		public string error_source { get; set; }
		public string error_step { get; set; }
		public string error_reason { get; set; }
		public string failed_at { get; set; }
	}

	public class OPDRazorpayRefundRequestModel
	{
		public string OPDFormid { get; set; }
		public string tenantid { get; set; }
		public decimal refundamount { get; set; }
		public string cancellationreason { get; set; }
	}

}
