namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:57
			public class PatientProfileModel
			{

			 public System.Guid ?PatientProfileid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string registrationid{ get; set; }

[xssFilter]
public string firstname{ get; set; }

[xssFilter]
public string? lastname{ get; set; }

[xssFilter]
public string gender{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime dateofbirth	{ get; set; }

public int? age{ get; set; }
		[xssFilter]
		public string? bloodgroup { get; set; }

		[xssFilter]
public string nationality{ get; set; }

public Guid? countryoforigin	{ get; set; }

[xssFilter]
public string emailaddress{ get; set; }

[xssFilter]
public string mobilenumber{ get; set; }

[xssFilter]
public string whatsappnumber{ get; set; }

[xssFilter]
										
               public string? photo{ get; set; }

[xssFilter]
public string paddressline1{ get; set; }

[xssFilter]
public string? paddressline2{ get; set; }

public int pzip{ get; set; }

[xssFilter]
public string? ptown{ get; set; }

[xssFilter]
public string? pcityordistrict{ get; set; }

[xssFilter]
public string? ppstatename{ get; set; }

public bool sameaspermanentaddress	{ get; set; }

[xssFilter]
public string? caddressline1{ get; set; }

[xssFilter]
public string? caddressline2{ get; set; }

public int? czip{ get; set; }

[xssFilter]
public string? ctown{ get; set; }

[xssFilter]
public string? ccityordistrict{ get; set; }

[xssFilter]
public string? cstatename{ get; set; }

[xssFilter]
public string idprooftype{ get; set; }

[xssFilter]
public string idproofnumber{ get; set; }

[xssFilter]
										[Required(ErrorMessage="Upload ID Proof is Required")]
               public string uploadidproof{ get; set; }

[xssFilter]
public string? languagesknown{ get; set; }

[xssFilter]
public string? languagespreferrable{ get; set; }

[xssFilter]
public string? otherlanguages{ get; set; }

[xssFilter]
public string? maritalstatus{ get; set; }

[xssFilter]
public string? education{ get; set; }

public Guid? occupation	{ get; set; }

[xssFilter]
public string? meditationpractice{ get; set; }

[xssFilter]
public string? typeofpractice{ get; set; }

[xssFilter]
public string? creativeactivities{ get; set; }

[xssFilter]
public string? othercreativeactivities{ get; set; }

[xssFilter]
public string? insurancetype{ get; set; }

[xssFilter]
public string? insurancecompany{ get; set; }

[xssFilter]
public string? policynumber{ get; set; }

public decimal? policyclaimlimit{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime? policyexpirydate	{ get; set; }

public Guid? referralsource	{ get; set; }

[xssFilter]
public string? referraltype{ get; set; }

[xssFilter]
public string? referrername{ get; set; }

[xssFilter]
public string? referrerphonenumber{ get; set; }

[xssFilter]
public string? magazinename{ get; set; }

[xssFilter]
public string? socialmediaplatform{ get; set; }

[xssFilter]
public string? otherreferral{ get; set; }

[xssFilter]
public string? blacklisted{ get; set; }

public Guid? reasonforblacklisting	{ get; set; }

[xssFilter]
public string? detailedremarks{ get; set; }

public bool deceased	{ get; set; }



[xssFilter]
public string? causeofdeath{ get; set; }


		[xssFilter]
		public Guid? patientcategory { get; set; }

		 

[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode=true)]public DateTime? dateandtimeofdeath	{ get; set; }
public ICollection <PatientProfile_emergencycontactinfoModel> emergencycontactinfo { get; set; }
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
			

			public class PatientProfileModelValidator: AbstractValidator<PatientProfileModel>
			{
					private static bool IsValidIdProofNumber(string idProofType, string idProofNumber)
					{
						var value = (idProofNumber ?? string.Empty).Trim().ToUpperInvariant();
						switch ((idProofType ?? string.Empty).Trim().ToLowerInvariant())
						{
							case "pan":
							case "pan card":
								return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$");
							case "adhaar":
							case "adhaar card":
							case "aadhar":
							case "aadhaar":
							case "aadhar card":
							case "aadhaar card":
								return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-9]{12}$");
							case "passport":
								return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Z]{1}[0-9]{7}$");
							case "voter id":
							case "voter card":
								return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Z]{3}[0-9]{7}$");
							case "driving license":
							case "driving licence":
							case "driver license":
							case "driver licence":
							case "dl":
								return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Z]{2}[0-9]{13}$");
							default:
								return true;
						}
					}

					private static string GetIdProofNumberMessage(string idProofType)
					{
						switch ((idProofType ?? string.Empty).Trim().ToLowerInvariant())
						{
							case "pan":
							case "pan card":
								return "Invalid PAN format (ABCDE1234F)";
							case "adhaar":
							case "adhaar card":
							case "aadhar":
							case "aadhaar":
							case "aadhar card":
							case "aadhaar card":
								return "Aadhar must be 12 digits";
							case "passport":
								return "Invalid Passport format (A1234567)";
							case "voter id":
							case "voter card":
								return "Invalid Voter ID format (ABC1234567)";
							case "driving license":
							case "driving licence":
							case "driver license":
							case "driver licence":
							case "dl":
								return "Invalid Driving License format";
							default:
								return "Invalid ID Proof Number";
						}
					}
					 
					public PatientProfileModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Patient_Profile", () =>
                                    {
                                        {RuleFor(m => m.registrationid)
.MaximumLength(256).WithMessage("The allowed length of Registration ID is 256 characters or fewer")
;
RuleFor(m => m.firstname)
.NotEmpty().WithMessage("First Name is required")
.MaximumLength(128).WithMessage("The allowed length of First Name is 128 characters or fewer")
;

RuleFor(m => m.gender)
.NotEmpty().WithMessage("Gender is required")
;
RuleFor(m => m.dateofbirth)
.NotEmpty().WithMessage("Date of Birth is required")
.LessThanOrEqualTo(System.DateTime.Now.Date).WithMessage("Date of Birth should be Today or Past Date")
.NotEqual(System.DateTime.Now.Date).WithMessage("Date of Birth should not be Today")

//;
//RuleFor(m => m.age)
//.NotNull().WithMessage("Age is required")
//.LessThanOrEqualTo(99999999).WithMessage("Age should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.nationality)
.NotEmpty().WithMessage("Nationality is required")
;

RuleFor(m => m.emailaddress)
.NotEmpty().WithMessage("Email Address is required")
.MaximumLength(128).WithMessage("The allowed length of Email Address is 128 characters or fewer")
.EmailAddress()

;
RuleFor(m => m.mobilenumber)
.NotEmpty().WithMessage("Mobile Number is required")
.MaximumLength(20).WithMessage("The allowed length of Mobile Number is 20 characters or fewer ")

;
RuleFor(m => m.whatsappnumber)
.NotEmpty().WithMessage("Whatsapp Number is required")
.MaximumLength(20).WithMessage("The allowed length of Whatsapp Number is 20 characters or fewer ")

;

RuleFor(m => m.paddressline1)
.NotEmpty().WithMessage("AddressLine1 is required")
.MinimumLength(1).WithMessage("The minimum length of AddressLine1 is 1 characters ")
.MaximumLength(256).WithMessage("The allowed length of AddressLine1 is 256 characters or fewer")
;

RuleFor(m => m.pzip)
.NotNull().WithMessage("Zip is required")
.LessThanOrEqualTo(99999999).WithMessage("Zip should be LessThanOrEqualTo 99999999")

;






RuleFor(m => m.caddressline1)
.NotEmpty().WithMessage("AddressLine1 is required")
.MaximumLength(128).WithMessage("The allowed length of AddressLine1 is 128 characters or fewer")
;

RuleFor(m => m.czip)
.LessThanOrEqualTo(99999999).WithMessage("Zip should be LessThanOrEqualTo 99999999")

;



RuleFor(m => m.idprooftype)
.NotEmpty().WithMessage("ID Proof Type is required")
;
RuleFor(m => m.idproofnumber)
.NotEmpty().WithMessage("ID Proof Number is required")
.MaximumLength(128).WithMessage("The allowed length of ID Proof Number is 128 characters or fewer")
.Must((model, idproofnumber) => IsValidIdProofNumber(model.idprooftype, idproofnumber)).WithMessage(model => GetIdProofNumberMessage(model.idprooftype))
;
RuleFor(m => m.uploadidproof)
;













RuleFor(m => m.policyclaimlimit)
.LessThanOrEqualTo(99999999).WithMessage("Policy Claim Limit should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.policyexpirydate)
.GreaterThanOrEqualTo(System.DateTime.Now.Date).WithMessage("Policy Expiry Date should be Today or Future Date")

;



RuleFor(m => m.referrerphonenumber)
.MaximumLength(20).WithMessage("The allowed length of Referrer Phone Number is 20 characters or fewer ")

;



}

                                    });
When(model => model.craftmyapp_actionmethodname == "Add_to_Blacklist", () =>
                                    {
                                        {RuleFor(m => m.registrationid)
.MaximumLength(256).WithMessage("The allowed length of Registration ID is 256 characters or fewer")
;



}

                                    });
When(model => model.craftmyapp_actionmethodname == "Mark_as_Deceased", () =>
                                    {
                                        {RuleFor(m => m.registrationid)
.MaximumLength(256).WithMessage("The allowed length of Registration ID is 256 characters or fewer")
;



}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Patient_Profile", () =>
                                    {
                                        {RuleFor(m => m.registrationid)
.MaximumLength(256).WithMessage("The allowed length of Registration ID is 256 characters or fewer")
;
RuleFor(m => m.firstname)
.NotEmpty().WithMessage("First Name is required")
.MaximumLength(128).WithMessage("The allowed length of First Name is 128 characters or fewer")
;

RuleFor(m => m.gender)
.NotEmpty().WithMessage("Gender is required")
;
RuleFor(m => m.dateofbirth)
.NotEmpty().WithMessage("Date of Birth is required")
.LessThanOrEqualTo(System.DateTime.Now.Date).WithMessage("Date of Birth should be Today or Past Date")

;
//RuleFor(m => m.age)
//.NotNull().WithMessage("Age is required")
//.LessThanOrEqualTo(99999999).WithMessage("Age should be LessThanOrEqualTo 99999999")

//;
RuleFor(m => m.nationality)
.NotEmpty().WithMessage("Nationality is required")
;

RuleFor(m => m.emailaddress)
.NotEmpty().WithMessage("Email Address is required")
.MaximumLength(128).WithMessage("The allowed length of Email Address is 128 characters or fewer")
.EmailAddress()

;
RuleFor(m => m.mobilenumber)
.NotEmpty().WithMessage("Mobile Number is required")
.MaximumLength(20).WithMessage("The allowed length of Mobile Number is 20 characters or fewer ")

;
RuleFor(m => m.whatsappnumber)
.NotEmpty().WithMessage("Whatsapp Number is required")
.MaximumLength(20).WithMessage("The allowed length of Whatsapp Number is 20 characters or fewer ")

;

RuleFor(m => m.paddressline1)
.NotEmpty().WithMessage("AddressLine1 is required")
.MinimumLength(1).WithMessage("The minimum length of AddressLine1 is 1 characters ")
.MaximumLength(256).WithMessage("The allowed length of AddressLine1 is 256 characters or fewer")
;

RuleFor(m => m.pzip)
.NotNull().WithMessage("Zip is required")
.LessThanOrEqualTo(99999999).WithMessage("Zip should be LessThanOrEqualTo 99999999")

;






RuleFor(m => m.caddressline1)
.NotEmpty().WithMessage("AddressLine1 is required")
.MaximumLength(128).WithMessage("The allowed length of AddressLine1 is 128 characters or fewer")
;

RuleFor(m => m.czip)
.LessThanOrEqualTo(99999999).WithMessage("Zip should be LessThanOrEqualTo 99999999")

;



RuleFor(m => m.idprooftype)
.NotEmpty().WithMessage("ID Proof Type is required")
;
RuleFor(m => m.idproofnumber)
.NotEmpty().WithMessage("ID Proof Number is required")
.MaximumLength(128).WithMessage("The allowed length of ID Proof Number is 128 characters or fewer")
.Must((model, idproofnumber) => IsValidIdProofNumber(model.idprooftype, idproofnumber)).WithMessage(model => GetIdProofNumberMessage(model.idprooftype))
;
RuleFor(m => m.uploadidproof)
;













RuleFor(m => m.policyclaimlimit)
.LessThanOrEqualTo(99999999).WithMessage("Policy Claim Limit should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.policyexpirydate)
.GreaterThanOrEqualTo(System.DateTime.Now.Date).WithMessage("Policy Expiry Date should be Today or Future Date")

;



RuleFor(m => m.referrerphonenumber)
.MaximumLength(20).WithMessage("The allowed length of Referrer Phone Number is 20 characters or fewer ")

;



}

                                    });

						 RuleForEach(x => x.emergencycontactinfo).SetValidator(new PatientProfile_emergencycontactinfoModelValidator());

						
					}

			}

                
			 public class PatientProfile_emergencycontactinfoModel
			{

			 

public string personname{ get; set; }


public string relationship{ get; set; }


public string phonenumber{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?PatientProfile_emergencycontactinfoid { get; set; }
public System.Guid ?PatientProfileid { get; set; }



			}

			public class PatientProfileEmergencyContactUpdateModel
			{
				public System.Guid PatientProfileid { get; set; }
				public ICollection<PatientProfile_emergencycontactinfoModel> emergencycontactinfo { get; set; }
			}
			

			public class PatientProfile_emergencycontactinfoModelValidator: AbstractValidator<PatientProfile_emergencycontactinfoModel>
			{
				   
					public PatientProfile_emergencycontactinfoModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Patient_Profile", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Add_to_Blacklist", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Mark_as_Deceased", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Patient_Profile", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
