namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
	using System.Text.Json;
	using Newtonsoft.Json;

	//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:01
	public class IPDApplicationFormModel
			{

			 public System.Guid ?IPDApplicationFormid	{ get; set; }
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
public string firstname{ get; set; }

[xssFilter]
public string? lastname{ get; set; }

[xssFilter]
public string gender{ get; set; }

[xssFilter]
public string mobilenumber{ get; set; }

[xssFilter]
public string whatsappnumber{ get; set; }

[xssFilter]
public string nationality{ get; set; }

public Guid? countryoforigin	{ get; set; }

[xssFilter]
public string generalcondition{ get; set; }

        [xssFilter]
        public string bookingtype { get; set; }

        [xssFilter]
        public string? groupcode { get; set; }

[xssFilter]
public string bookingstatus{ get; set; }

[xssFilter]
public string? phase{ get; set; }

[DataType(DataType.Date)]
[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
[DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]
public System.DateTime ?bookingstatusdate	{ get; set; }

// Direct IPD uses the existing preferred-admission child record for persistence.
// This field carries the selected start date explicitly through the Direct IPD API flow.
[DataType(DataType.Date)]
[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
[DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]
public System.DateTime? plannedadmissionstartdate { get; set; }

[xssFilter]
public string? groupbooking{ get; set; }

[xssFilter]
public string? areyouthegroupleader{ get; set; }

public int? numberofmember{ get; set; }

[xssFilter]
public string? groupleadersbookingreferencenumber{ get; set; }

[xssFilter]
public string paddressline1{ get; set; }

[xssFilter]
public string? paddressline2{ get; set; }

public int ppincode{ get; set; }

[xssFilter]
public string? ptown{ get; set; }

[xssFilter]
public string? pcityordistrict{ get; set; }

[xssFilter]
public string? pstatename{ get; set; }

public bool sameaspermanentaddress	{ get; set; }

[xssFilter]
public string caddressline1{ get; set; }

[xssFilter]
public string? caddressline2{ get; set; }

public int cpincode{ get; set; }

[xssFilter]
public string? ctown{ get; set; }

[xssFilter]
public string? ccityordistrict{ get; set; }

[xssFilter]
public string? cstatename{ get; set; }

[xssFilter]
public string? flexiblewithdates{ get; set; }

[xssFilter]
public string? flexiblewithroomtype{ get; set; }

[xssFilter]
public string? joinwaitinglist{ get; set; }

[xssFilter]
public string? passportnumber{ get; set; }

public Guid? passportissuingcountry	{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime? passportexpirydate	{ get; set; }

[xssFilter]
										
               public string? uploadpassportcopy{ get; set; }

[xssFilter]
public string? visatype{ get; set; }

[xssFilter]
public string? visanumber{ get; set; }

public Guid? visaissuedcountry	{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime? visaissuedate	{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime? visaexpirydate	{ get; set; }

[xssFilter]
										
               public string? uploadvisacopy{ get; set; }

[xssFilter]
public string? doyourequireahospitalprovidedattendant{ get; set; }

[xssFilter]
public string? preferredduration{ get; set; }

[xssFilter]
public string? admissionreason{ get; set; }

public Guid consentform	{ get; set; }

[xssFilter]
										
               public string? consentfile{ get; set; }

public bool agreefortermsandconditions	{ get; set; }

[xssFilter]
public string? signature{ get; set; }

public Guid? packagename	{ get; set; }

public bool isbookingdepositmandatory	{ get; set; }

[xssFilter]
public string? verifiedstatus{ get; set; }
		[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)] public DateTime? estimatedarrival { get; set; }

		[xssFilter]
		public string? travelarrangement { get; set; }

		[xssFilter]
		public string? typeoftravelrequired { get; set; }

		[xssFilter]
		public string? pickupfrom { get; set; }

		[xssFilter]
		public string? requiredparkingspace { get; set; }

		[xssFilter]
		public string? wheelchairassistance { get; set; }

		[xssFilter]
		public string? requireddinner { get; set; }

		[xssFilter]
		public string? specialrequest { get; set; }

public ICollection <IPDApplicationForm_preferreddatesofadmissionModel> preferreddatesofadmission { get; set; }
public string? bookingemergencycontactsjson { get; set; }
public ICollection <IPDApplicationForm_attendantpreferreddatesModel> attendantpreferreddates { get; set; }
public ICollection <IPDApplicationForm_medicalinfoModel> medicalinfo { get; set; }
public ICollection <IPDApplicationForm_medicationinfoModel> medicationinfo { get; set; }
public ICollection <IPDApplicationForm_medicalrecordsModel> medicalrecords { get; set; }
public ICollection <IPDApplicationForm_attendantinfoModel> attendantinfo { get; set; }
public ICollection <IPDApplicationForm_roompreferenceModel> roompreference { get; set; }
public ICollection <IPDApplicationForm_attendantroompreferenceModel> attendantroompreference { get; set; }
public ICollection <IPDApplicationForm_roomModel> room { get; set; }
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


		public string uploadidproof { get; set; }


			}
			

			public class IPDApplicationFormModelValidator: AbstractValidator<IPDApplicationFormModel>
			{
					 
					public IPDApplicationFormModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_IPD_Application_Form"
						 && !string.Equals(model.bookingstatus, "Draft", StringComparison.OrdinalIgnoreCase), () =>
                                    {
                                        {RuleFor(m => m.bookingreferencenumber)
.MaximumLength(256).WithMessage("The allowed length of Booking Reference Number is 256 characters or fewer")
;
											RuleFor(m => m.patientname)
											.NotEmpty().WithMessage("Patient Name is required")
											;
											RuleFor(m => m.firstname)
.NotEmpty().WithMessage("First Name is required")
.MaximumLength(128).WithMessage("The allowed length of First Name is 128 characters or fewer")
;

RuleFor(m => m.gender)
.NotEmpty().WithMessage("Gender is required")
;
RuleFor(m => m.mobilenumber)
.NotEmpty().WithMessage("Mobile Number is required")
.MaximumLength(20).WithMessage("The allowed length of Mobile Number is 20 characters or fewer ")

;
RuleFor(m => m.whatsappnumber)
.NotEmpty().WithMessage("Whatsapp Number is required")
.MaximumLength(20).WithMessage("The allowed length of Whatsapp Number is 20 characters or fewer ")

;
RuleFor(m => m.nationality)
.NotEmpty().WithMessage("Nationality is required")
;

RuleFor(m => m.generalcondition)
.NotEmpty().WithMessage("General Condition is required")
;
RuleFor(m => m.bookingstatus)
.NotEmpty().WithMessage("Booking Status is required")
;


RuleFor(m => m.numberofmember)
.LessThanOrEqualTo(99999999).WithMessage("Number of Members should be LessThanOrEqualTo 99999999")

;

RuleFor(m => m.paddressline1)
.NotEmpty().WithMessage("Address Line 1 is required")
.MaximumLength(128).WithMessage("The allowed length of Address Line 1 is 128 characters or fewer")
;

RuleFor(m => m.ppincode)
.NotNull().WithMessage("Pin Code is required")
.LessThanOrEqualTo(99999999).WithMessage("Pin Code should be LessThanOrEqualTo 99999999")

;




RuleFor(m => m.caddressline1)
.NotEmpty().WithMessage("Address Line 1 is required")
.MaximumLength(128).WithMessage("The allowed length of Address Line 1 is 128 characters or fewer")
;

RuleFor(m => m.cpincode)
.NotNull().WithMessage("Pin Code is required")
.LessThanOrEqualTo(99999999).WithMessage("Pin Code should be LessThanOrEqualTo 99999999")

;








RuleFor(m => m.passportexpirydate)
.GreaterThan(m => m.visaissuedate).When(m => m.visaissuedate.HasValue && m.passportexpirydate.HasValue)
.WithMessage("Passport Expiry Date should be greater than Visa Issue Date")


;




RuleFor(m => m.visaissuedate)


;
RuleFor(m => m.visaexpirydate)
.GreaterThan(m => m.visaissuedate).When(m => m.visaissuedate.HasValue && m.visaexpirydate.HasValue)
.WithMessage("Visa Expiry Date should be greater than Visa Issue Date")


;




RuleFor(m => m.consentform)
.NotEmpty().WithMessage("Consent Form is required")
;

RuleFor(m => m.agreefortermsandconditions)
.NotEmpty().WithMessage("Agree for Terms and Conditions is required")
.When(m => !string.Equals(m.bookingstatus, "Draft", StringComparison.OrdinalIgnoreCase))
;

RuleFor(m => m.signature)
//.NotEmpty().WithMessage("Signature is required")
;


}

                                    });
When(model => model.craftmyapp_actionmethodname == "Allot_Room", () =>
                                    {
                                        {RuleFor(m => m.bookingreferencenumber)
.MaximumLength(256).WithMessage("The allowed length of Booking Reference Number is 256 characters or fewer")
;


}

                                    });
			When(model => model.craftmyapp_actionmethodname == "Confirm_Arrival", () =>
			{
				{
					RuleFor(m => m.bookingreferencenumber)
.MaximumLength(256).WithMessage("The allowed length of Booking Reference Number is 256 characters or fewer")
;
				}
			});

			When(model => model.craftmyapp_actionmethodname == "Update_IPD_Application_Form"
						 && !string.Equals(model.bookingstatus, "Draft", StringComparison.OrdinalIgnoreCase), () =>
                                    {
                                        {RuleFor(m => m.bookingreferencenumber)
.MaximumLength(256).WithMessage("The allowed length of Booking Reference Number is 256 characters or fewer")
;
											RuleFor(m => m.patientname)
											.NotEmpty().WithMessage("Patient Name is required")
											;
											RuleFor(m => m.firstname)
.NotEmpty().WithMessage("First Name is required")
.MaximumLength(128).WithMessage("The allowed length of First Name is 128 characters or fewer")
;

RuleFor(m => m.gender)
.NotEmpty().WithMessage("Gender is required")
;
RuleFor(m => m.mobilenumber)
.NotEmpty().WithMessage("Mobile Number is required")
.MaximumLength(20).WithMessage("The allowed length of Mobile Number is 20 characters or fewer ")

;
RuleFor(m => m.whatsappnumber)
.NotEmpty().WithMessage("Whatsapp Number is required")
.MaximumLength(20).WithMessage("The allowed length of Whatsapp Number is 20 characters or fewer ")

;
RuleFor(m => m.nationality)
.NotEmpty().WithMessage("Nationality is required")
;

RuleFor(m => m.generalcondition)
.NotEmpty().WithMessage("General Condition is required")
;
RuleFor(m => m.bookingstatus)
.NotEmpty().WithMessage("Booking Status is required")
;


RuleFor(m => m.numberofmember)
.LessThanOrEqualTo(99999999).WithMessage("Number of Members should be LessThanOrEqualTo 99999999")

;

RuleFor(m => m.paddressline1)
.NotEmpty().WithMessage("Address Line 1 is required")
.MaximumLength(128).WithMessage("The allowed length of Address Line 1 is 128 characters or fewer")
;

RuleFor(m => m.ppincode)
.NotNull().WithMessage("Pin Code is required")
.LessThanOrEqualTo(99999999).WithMessage("Pin Code should be LessThanOrEqualTo 99999999")

;




RuleFor(m => m.caddressline1)
.NotEmpty().WithMessage("Address Line 1 is required")
.MaximumLength(128).WithMessage("The allowed length of Address Line 1 is 128 characters or fewer")
;

RuleFor(m => m.cpincode)
.NotNull().WithMessage("Pin Code is required")
.LessThanOrEqualTo(99999999).WithMessage("Pin Code should be LessThanOrEqualTo 99999999")

;








RuleFor(m => m.passportexpirydate)
.GreaterThan(m => m.visaissuedate).When(m => m.visaissuedate.HasValue && m.passportexpirydate.HasValue)
.WithMessage("Passport Expiry Date should be greater than Visa Issue Date")


;




RuleFor(m => m.visaissuedate)


;
RuleFor(m => m.visaexpirydate)
.GreaterThan(m => m.visaissuedate).When(m => m.visaissuedate.HasValue && m.visaexpirydate.HasValue)
.WithMessage("Visa Expiry Date should be greater than Visa Issue Date")


;




RuleFor(m => m.consentform)
.NotEmpty().WithMessage("Consent Form is required")
;

RuleFor(m => m.agreefortermsandconditions)
.NotEmpty().WithMessage("Agree for Terms and Conditions is required")
.When(m => !string.Equals(m.bookingstatus, "Draft", StringComparison.OrdinalIgnoreCase))
;

RuleFor(m => m.preferreddatesofadmission)
.Must(x => x == null || x.Count <= 5).WithMessage("Preferred Dates of Admission can have a maximum of 5 records")
;

RuleFor(m => m.attendantinfo)
.Must(x => x == null || x.Count <= 2).WithMessage("Attendant Info can have a maximum of 2 records")
;


}

                                    });

						 When(model => !string.Equals(model.bookingstatus, "Draft", StringComparison.OrdinalIgnoreCase), () =>
						 {
RuleForEach(x => x.preferreddatesofadmission).SetValidator(new IPDApplicationForm_preferreddatesofadmissionModelValidator());
RuleForEach(x => x.medicalinfo).SetValidator(new IPDApplicationForm_medicalinfoModelValidator());
RuleForEach(x => x.medicationinfo).SetValidator(new IPDApplicationForm_medicationinfoModelValidator());
RuleForEach(x => x.medicalrecords).SetValidator(new IPDApplicationForm_medicalrecordsModelValidator());
RuleForEach(x => x.attendantinfo).SetValidator(new IPDApplicationForm_attendantinfoModelValidator());
RuleForEach(x => x.roompreference).SetValidator(new IPDApplicationForm_roompreferenceModelValidator());
RuleForEach(x => x.attendantpreferreddates).SetValidator(new IPDApplicationForm_attendantpreferreddatesModelValidator());
RuleForEach(x => x.attendantroompreference).SetValidator(new IPDApplicationForm_attendantroompreferenceModelValidator());
RuleForEach(x => x.room).SetValidator(new IPDApplicationForm_roomModelValidator());
						 });

						
					}

			}

                
			 public class IPDApplicationForm_preferreddatesofadmissionModel
			{

			 
public DateTime dateofarrival	{ get; set; }

public DateTime dateofdeparture	{ get; set; }

public int daysofstay{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?IPDApplicationForm_preferreddatesofadmissionid { get; set; }
public System.Guid ?IPDApplicationFormid { get; set; }



			}
			

			public class IPDApplicationForm_preferreddatesofadmissionModelValidator: AbstractValidator<IPDApplicationForm_preferreddatesofadmissionModel>
			{
				   
					public IPDApplicationForm_preferreddatesofadmissionModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_IPD_Application_Form", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Allot_Room", () =>
                                                {
                                                    {}

                                                });
			When(model => model.craftmyapp_actionmethodname == "Confirm_Arrival", () =>
			{
				{ }

			});

			When(model => model.craftmyapp_actionmethodname == "Update_IPD_Application_Form", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			public class IPDApplicationForm_attendantpreferreddatesModel
			{
				public DateTime dateofarrivalatt { get; set; }
				public DateTime dateofdepartureatt { get; set; }
				public int? daysofstayatt { get; set; }
				public string craftmyapp_actionmethodname { get; set; }
				public string cma_client_row_id { get; set; }
				public int? record_order { get; set; }
				public Guid? IPDApplicationForm_attendantpreferreddatesid { get; set; }
				public Guid? IPDApplicationFormid { get; set; }
			}

			public class IPDApplicationForm_attendantpreferreddatesModelValidator : AbstractValidator<IPDApplicationForm_attendantpreferreddatesModel>
			{
				public IPDApplicationForm_attendantpreferreddatesModelValidator()
				{
					RuleFor(m => m.dateofarrivalatt).NotEmpty().WithMessage("Attendant Date of Arrival is required");
					RuleFor(m => m.dateofdepartureatt).NotEmpty().WithMessage("Attendant Date of Departure is required")
						.GreaterThanOrEqualTo(m => m.dateofarrivalatt).WithMessage("Attendant Date of Departure must be on or after Date of Arrival");
					RuleFor(m => m.daysofstayatt).GreaterThan(0).When(m => m.daysofstayatt.HasValue)
						.WithMessage("Attendant Days of Stay must be greater than zero");
				}
			}

				 public class IPDApplicationForm_medicalinfoModel
			{

			 
public Guid medicalconditionname	{ get; set; }

public decimal duration{ get; set; }


public string unit{ get; set; }


public string severitylevel{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?IPDApplicationForm_medicalinfoid { get; set; }
public System.Guid ?IPDApplicationFormid { get; set; }



			}
			

			public class IPDApplicationForm_medicalinfoModelValidator: AbstractValidator<IPDApplicationForm_medicalinfoModel>
			{
				   
					public IPDApplicationForm_medicalinfoModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_IPD_Application_Form", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Allot_Room", () =>
                                                {
                                                    {}

                                                });
			When(model => model.craftmyapp_actionmethodname == "Confirm_Arrival", () =>
			{
				{ }

			});

			When(model => model.craftmyapp_actionmethodname == "Update_IPD_Application_Form", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			 public class IPDApplicationForm_medicationinfoModel
			{

			 

public string medicinename{ get; set; }


public string? frequencyinaday{ get; set; }


public string? medicationduration{ get; set; }

public decimal? quantity { get; set; }

		public string? medicinecoverimage { get; set; }
		public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?IPDApplicationForm_medicationinfoid { get; set; }
public System.Guid ?IPDApplicationFormid { get; set; }



			}
			

			public class IPDApplicationForm_medicationinfoModelValidator: AbstractValidator<IPDApplicationForm_medicationinfoModel>
			{
				   
					public IPDApplicationForm_medicationinfoModelValidator()
					{
						RuleFor(m => m.quantity)
							.GreaterThan(0).When(m => m.quantity.HasValue)
							.WithMessage("Medication Quantity should be greater than zero");

						 When(model => model.craftmyapp_actionmethodname == "Add_IPD_Application_Form", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Allot_Room", () =>
                                                {
                                                    {}

                                                });
			When(model => model.craftmyapp_actionmethodname == "Confirm_Arrival", () =>
			{
				{ }

			});

			When(model => model.craftmyapp_actionmethodname == "Update_IPD_Application_Form", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			 public class IPDApplicationForm_medicalrecordsModel
			{

			 

public string medicalrecordname{ get; set; }

[xssFilter]
public string medicalrecordfile{ get; set; }


		public string? abnormallabfindings { get; set; }

		public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?IPDApplicationForm_medicalrecordsid { get; set; }
public System.Guid ?IPDApplicationFormid { get; set; }



			}
			

			public class IPDApplicationForm_medicalrecordsModelValidator: AbstractValidator<IPDApplicationForm_medicalrecordsModel>
			{
				   
					public IPDApplicationForm_medicalrecordsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_IPD_Application_Form", () =>
                                                {
                                                    {
RuleFor(m => m.medicalrecordfile)
.NotEmpty().WithMessage("Medical Record File is required")
;
}

                                                });
When(model => model.craftmyapp_actionmethodname == "Allot_Room", () =>
                                                {
                                                    {}

                                                });
			When(model => model.craftmyapp_actionmethodname == "Confirm_Arrival", () =>
			{
				{ }

			});

			When(model => model.craftmyapp_actionmethodname == "Update_IPD_Application_Form", () =>
                                                {
                                                    {
RuleFor(m => m.medicalrecordfile)
.NotEmpty().WithMessage("Medical Record File is required")
;
}

                                                });

						
					}

			}



			 public class IPDApplicationForm_attendantinfoModel
			{

			 

public string attendantname{ get; set; }

public int age{ get; set; }


public string gender{ get; set; }


public string phonenumber{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?IPDApplicationForm_attendantinfoid { get; set; }
public System.Guid ?IPDApplicationFormid { get; set; }



			}
			

			public class IPDApplicationForm_attendantinfoModelValidator: AbstractValidator<IPDApplicationForm_attendantinfoModel>
			{
				   
					public IPDApplicationForm_attendantinfoModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_IPD_Application_Form", () =>
                                                {
                                                    {RuleFor(m => m.gender)
.NotEmpty().WithMessage("Gender is required")
;
}

                                                });
When(model => model.craftmyapp_actionmethodname == "Allot_Room", () =>
                                                {
                                                    {}

                                                });
			When(model => model.craftmyapp_actionmethodname == "Confirm_Arrival", () =>
			{
				{ }
			});

			When(model => model.craftmyapp_actionmethodname == "Update_IPD_Application_Form", () =>
                                                {
                                                    {RuleFor(m => m.gender)
.NotEmpty().WithMessage("Gender is required")
;
}

                                                });

						
					}

			}



			 public class IPDApplicationForm_roompreferenceModel
			{

			 
public Guid roomtype	{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?IPDApplicationForm_roompreferenceid { get; set; }
public System.Guid ?IPDApplicationFormid { get; set; }



			}
			

			public class IPDApplicationForm_roompreferenceModelValidator: AbstractValidator<IPDApplicationForm_roompreferenceModel>
			{
				   
					public IPDApplicationForm_roompreferenceModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_IPD_Application_Form", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Allot_Room", () =>
                                                {
                                                    {}

                                                });
			When(model => model.craftmyapp_actionmethodname == "Confirm_Arrival", () =>
			{
				{}
			});

			When(model => model.craftmyapp_actionmethodname == "Update_IPD_Application_Form", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			public class IPDApplicationForm_attendantroompreferenceModel
			{
				public Guid? roomtypeatt { get; set; }
				public string craftmyapp_actionmethodname { get; set; }
				public string cma_client_row_id { get; set; }
				public int? record_order { get; set; }
				public Guid? IPDApplicationForm_attendantroompreferenceid { get; set; }
				public Guid? IPDApplicationFormid { get; set; }
			}

			public class IPDApplicationForm_attendantroompreferenceModelValidator : AbstractValidator<IPDApplicationForm_attendantroompreferenceModel>
			{
				public IPDApplicationForm_attendantroompreferenceModelValidator()
				{
					RuleFor(m => m.roomtypeatt).NotEmpty().When(m => m.roomtypeatt.HasValue)
						.WithMessage("Attendant Room Type is required");
				}
			}

				 public class IPDApplicationForm_roomModel
			{

			 

public string? allottedto{ get; set; }

public Guid? roomnumber	{ get; set; }

public DateTime? fromdate	{ get; set; }

public DateTime? todate	{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?IPDApplicationForm_roomid { get; set; }
public System.Guid ?IPDApplicationFormid { get; set; }

/// <summary>FK to RoomType — required for Receivable generation.</summary>
public Guid?    roomtype      { get; set; }

/// <summary>
/// Effective cost per day for this room row.
/// For patient rooms: package cost when ispackageroom=true, else standard RoomType.costperday.
/// For attendant rooms: always RoomType.attendantcostperday (ispackageroom is always false).
/// </summary>
public decimal? costperday    { get; set; }

/// <summary>True when package pricing is applied to this room (patient rooms only).</summary>
public bool     ispackageroom { get; set; }

/// <summary>
/// Percentage of the room cost covered by the selected TreatmentPackage (0–100).
/// Comes from TreatmentPackage_roomtypes.percentagecovered.
/// Default 100 = fully covered; anything less means a room surcharge receivable is needed.
/// Only meaningful for patient rooms when a package is selected; ignored for attendant rooms.
/// </summary>
public decimal? percentagecovered { get; set; }

/// <summary>Room's "Next Day Checkin" setting (Yes/No), joined from Room master data. Used to allow the estimated arrival date to extend one day beyond the allotted/preferred date.</summary>
public string? nextdaycheckin { get; set; }

			}
			

			public class IPDApplicationForm_roomModelValidator: AbstractValidator<IPDApplicationForm_roomModel>
			{
				   
					public IPDApplicationForm_roomModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_IPD_Application_Form", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Allot_Room", () =>
                                                {
                                                    {}

                                                });
			When(model => model.craftmyapp_actionmethodname == "Confirm_Arrival", () =>
			{
				{}
			});

			When(model => model.craftmyapp_actionmethodname == "Update_IPD_Application_Form", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                 
                                                    public class IPDApplicationFormReviewModel
                                                    {
                                                        public string IPDApplicationFormid { get; set; }
                                                        public string reviewcomments { get; set; }
                                                        public string verifiedstatus { get; set; }
                                                        public string verifiedby { get; set; }



                                                    }

        public class IPDBookingStatusUpdateModel
        {
            public string IPDApplicationFormid { get; set; }
            public string bookingstatus { get; set; }
            public string phase { get; set; }
            public string verifiedstatus { get; set; }
            public bool skiproomreceivables { get; set; }
            public string modifieduser { get; set; }
        }

        public class IPDPackageUpdateModel
        {
            public string IPDApplicationFormid { get; set; }
            public string packagename { get; set; }
        }

	/// <summary>
	/// Payment details returned for IPD booking deposit initiation.
	/// Calculated from package (packagebookingdeposit) or room-type (costperday × days).
	/// </summary>
	public class IPDPaymentDetailsModel
	{
		[JsonProperty("ipdapplicationformid")]
		public string IPDApplicationFormid { get; set; }

		[JsonProperty("bookingreferencenumber")]
		public string bookingreferencenumber { get; set; }

		[JsonProperty("firstname")]
		public string firstname { get; set; }

		[JsonProperty("lastname")]
		public string lastname { get; set; }

		[JsonProperty("mobilenumber")]
		public string mobilenumber { get; set; }

		[JsonProperty("packagename_master")]
		public string packagename_master { get; set; }

		
		[JsonProperty("packagebookingdeposit")]
		public decimal? bookingdepositamount { get; set; }

		
		[JsonProperty("packagebookingadvance")]
		public decimal? advanceamount { get; set; }

		[JsonProperty("totalamount")]
		public decimal? totalamount { get; set; }

		[JsonProperty("daysofstay")]
		public int? daysofstay { get; set; }

		[JsonProperty("accommodationtype")]
		public string accommodationtype { get; set; }

		[JsonProperty("attendantcount")]
		public int? attendantcount { get; set; }

		[JsonProperty("attendant_room_cost")]
		public decimal? attendant_room_cost { get; set; }

		[JsonProperty("bookingstatus")]
		public string bookingstatus { get; set; }

		[JsonProperty("tenantid")]
		public string tenantid { get; set; }

		
		[JsonProperty("patientname")]
		public string patientname { get; set; }

		
		[JsonProperty("packagename")]
		public string packagename { get; set; }

		[JsonProperty("packagecost")]
		public decimal? packagecost { get; set; }

		[JsonProperty("costperday")]
		public decimal? costperday { get; set; }

		[JsonProperty("patient_booking_deposit")]
		public decimal? patient_booking_deposit { get; set; }

		[JsonProperty("attendant_booking_deposit")]
		public decimal? attendant_booking_deposit { get; set; }

		[JsonProperty("bookingdeposit_refundable")]
		public bool? bookingdeposit_refundable { get; set; }

		[JsonProperty("bookingdeposit_refundtype")]
		public string bookingdeposit_refundtype { get; set; }

		[JsonProperty("bookingdeposit_refundpercentage")]
		public decimal? bookingdeposit_refundpercentage { get; set; }

		[JsonProperty("blocked_room_details_json")]
		public string blocked_room_details_json { get; set; }

		[JsonProperty("roomtype_refundpolicy_json")]
		public string roomtype_refundpolicy_json { get; set; }

		[JsonProperty("package_refundpolicy_json")]
		public string package_refundpolicy_json { get; set; }

		[JsonProperty("patientvisitid")]
		public string patientvisitid { get; set; }

		[JsonProperty("booking_deposit_receivables_json")]
		public string booking_deposit_receivables_json { get; set; }

		[JsonProperty("booking_deposit_receivable_balance")]
		public decimal? booking_deposit_receivable_balance { get; set; }
	}
	/// <summary>Request body for creating a Razorpay order for IPD booking deposit.</summary>
	public class IPDRazorpayOrderRequestModel
        {
            public string IPDApplicationFormid { get; set; }
            public string paymenttype { get; set; }
            public List<Guid> receivableids { get; set; } = new List<Guid>();
        }

        /// <summary>Request body sent from Admin after Razorpay payment success, for recording and status update.</summary>
        public class IPDCompletePaymentModel
        {
            public string IPDApplicationFormid { get; set; }
            public string paymentrequestid { get; set; }
            public string razorpay_order_id { get; set; }
            public string razorpay_payment_id { get; set; }
            public string razorpay_signature { get; set; }
            public List<Guid> receivableids { get; set; } = new List<Guid>();
		public decimal? amount { get; set; }
	}

	public class IPDFailedPaymentModel
	{
		public string IPDApplicationFormid { get; set; }
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

	/// <summary>Refund breakdown per refundtype for an IPD cancellation.</summary>
	public class IPDRefundDetailsModel
	{
		[JsonProperty("ipdapplicationformid")]
		public string ipdapplicationformid { get; set; }

		[JsonProperty("bookingreferencenumber")]
		public string bookingreferencenumber { get; set; }

		[JsonProperty("patient_name")]
		public string patient_name { get; set; }

		[JsonProperty("roomnumber")]
		public string roomnumber { get; set; }

		[JsonProperty("roomtype_name")]
		public string roomtype_name { get; set; }

		[JsonProperty("cancellationby")]
		public string cancellationby { get; set; }

		[JsonProperty("refundtype")]
		public string refundtype { get; set; }

		[JsonProperty("cancellationwindowdays")]
		public int cancellationwindowdays { get; set; }

		[JsonProperty("refundpercentage")]
		public decimal refundpercentage { get; set; }

		[JsonProperty("days_until_admission")]
		public int days_until_admission { get; set; }

		[JsonProperty("total_paid_amount")]
		public decimal total_paid_amount { get; set; }

		[JsonProperty("already_refunded_amount")]
		public decimal already_refunded_amount { get; set; }

		[JsonProperty("net_refundable_base")]
		public decimal net_refundable_base { get; set; }

		[JsonProperty("final_refund_amount")]
		public decimal final_refund_amount { get; set; }
	}

	/// <summary>Request body to cancel an IPD booking and trigger refund.</summary>
	public class IPDCancellationRequestModel
	{
		public string IPDApplicationFormid { get; set; }
		public string cancellationby { get; set; }        // "Patient" or "Hospital"
		public string cancellationreason { get; set; }
		public string refundmode { get; set; }            // Cash | UPI | Card | NetBanking | RazorPay
		public string sessionuserrole { get; set; }       // injected by Admin proxy from session
		public decimal? manualrefundamount { get; set; }  // IPD refunds must match the full policy amount; partial overrides are rejected
	}

	/// <summary>Request body for Step 6: Recording patient arrival and charging consultation/admission fees.</summary>
	public class IPDPatientArrivalRequestModel
	{
		public string IPDApplicationFormid { get; set; }
		public string paymentmode { get; set; } = "Cash";
		public decimal consultationfee { get; set; } = 600;
		public decimal admissionfee { get; set; } = 600;
		public string notes { get; set; }
	}

	/// <summary>Request body for Step 7: Final admission confirmation.</summary>
	public class IPDFinalAdmissionRequestModel
	{
		public string IPDApplicationFormid { get; set; }

		public string PatientVisitId { get; set; }
		public string notes { get; set; }
		/// <summary>Advance deposit collected at final admission. If 0 or null, no billing record is created.</summary>
		public decimal advancedeposit { get; set; }
		public string paymentmode { get; set; } = "Cash";
	}

	/// <summary>Request body for Doctor Review step (Step 3).</summary>
	public class IPDDoctorReviewModel
	{
		public string IPDApplicationFormid { get; set; }
		/// <summary>Approved | Rejected</summary>
		public string reviewaction { get; set; }
		public string reviewcomments { get; set; }
		/// <summary>Optional: doctor can suggest a different package.</summary>
		public string packageid { get; set; }
	}

	/// <summary>Request body for anonymous patient arrival confirmation (Step 6).</summary>
	public class IPDPatientArrivalConfirmModel
	{
		public string IPDApplicationFormid { get; set; }
		public string estimatedarrival { get; set; }
		public string specialrequest { get; set; }
	}

	/// <summary>Billing summary model for the IPD Billing Dashboard (Step 8).</summary>
	public class IPDBillingSummaryModel
	{
		public string IPDApplicationFormid { get; set; }
		public string bookingreferencenumber { get; set; }
        public string tenantname { get; set; }
      
        public string tenantphone { get; set; }
        public string tenantemail { get; set; }
        public string addressline1 { get; set; }
        public string tenantaddressline2 { get; set; }
        public string organizationlogo { get; set; }
        public string groupcode { get; set; }
        public string mobilenumber { get; set; }
        public string estimatedarrival { get; set; }
       
        public string patientfullname { get; set; }
		public string bookingstatus { get; set; }
		public decimal totalamount { get; set; }
		public decimal advanceamount { get; set; }
		public decimal bookingdepositpaid { get; set; }
		public decimal consultationfeepaid { get; set; }
		public decimal admissionfeepaid { get; set; }
		public decimal totalamountpaid { get; set; }
		public decimal remainingbalance { get; set; }
		public decimal subtotal { get; set; }
		public decimal discountamount { get; set; }
		public decimal concessionamount { get; set; }
		public decimal depositcreditadjustment { get; set; }
		public decimal amountalreadypaid { get; set; }
		public decimal currentamountpayable { get; set; }
		public string currencycode { get; set; }
		public string amountinwords { get; set; }

		public string patientvisitid { get; set; }

		/// <summary>Blocked room rows from Get_IPD_Payment_Details (same shape as Initiate_Payment).</summary>
		public string blocked_room_details_json { get; set; }

		public int? daysofstay { get; set; }
	}

	/// <summary>Doctor IPD screening: read-only IPD + linked assessment, confirm admission.</summary>
	public class IPDScreeningViewModel
	{
		public IPDApplicationFormModel IpdForm { get; set; }
		public AssessmentModel Assessment { get; set; }
		public List<IPDScreeningQuestionRow> QuestionRows { get; set; } = new List<IPDScreeningQuestionRow>();
		public bool HasAssessment { get; set; }
		public string InfoMessage { get; set; }
		public bool CanConfirmAdmission { get; set; }
	}

	public class IPDScreeningQuestionRow
	{
		public string QuestionText { get; set; }
		public string AnswerText { get; set; }
		public string AnswerType { get; set; }
	}


    public class IPDCalendarActionModel
    {
        public string action { get; set; }
        public string reviewcomments { get; set; }

        public Guid? tenantid { get; set; }
        public Guid? modifieduser { get; set; }

        

        public List<IPDCalendarActionItemModel> items { get; set; }
            = new List<IPDCalendarActionItemModel>();
    }

    public class IPDCalendarActionItemModel
    {
        public Guid? IPDApplicationFormid { get; set; }

        public List<IPDApplicationForm_roomModel> room { get; set; }
            = new List<IPDApplicationForm_roomModel>();

        public Guid? PackageId { get; set; }
    }


}
