namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 11:34:28
			public class PeopleModel
			{
			 public string status { get; set; } = "Active";

			 public System.Guid ?Peopleid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string practitionerid{ get; set; }

[xssFilter]
public string firstname{ get; set; }

[xssFilter]
public string? lastname{ get; set; }

public Guid workprofile	{ get; set; }

public Guid competencylevel	{ get; set; }

public Guid? clinicaltask	{ get; set; }

public Guid designation	{ get; set; }

[xssFilter]
public string contactnumber{ get; set; }

[xssFilter]
public string whatsappnumber{ get; set; }

[xssFilter]
public string emailid{ get; set; }

[xssFilter]
public string gender{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime dob	{ get; set; }

public int? age{ get; set; }

[xssFilter]
public string employmentstatus{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime? joiningdate	{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime? contractrenewaldate	{ get; set; }

[xssFilter]
										
               public string? photo{ get; set; }

[xssFilter]
public string? nationality{ get; set; }

public Guid? specifycountry	{ get; set; }

[xssFilter]
public string idtype{ get; set; }

[xssFilter]
public string idnumber{ get; set; }

[xssFilter]
										[Required(ErrorMessage="ID Document is Required")]
               public string iddocument{ get; set; }

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
public string? pstatename{ get; set; }

public bool sameaspermanentaddress	{ get; set; }

[xssFilter]
public string caddressline1{ get; set; }

[xssFilter]
public string? caddressline2{ get; set; }

public int czip{ get; set; }

[xssFilter]
public string? ctown{ get; set; }

[xssFilter]
public string? ccityordistrict{ get; set; }

[xssFilter]
public string? cstatename{ get; set; }

[xssFilter]
public string? registrationnumber{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime? validtill	{ get; set; }

[xssFilter]
										
               public string? licenceupload{ get; set; }

[xssFilter]
public string? issuingauthority{ get; set; }

[xssFilter]
public string? bio{ get; set; }
		[xssFilter]
		public string? screeningmeetinglink { get; set; }

		public ICollection <People_emergencycontactModel> emergencycontact { get; set; }
public ICollection <People_educationinfoModel> educationinfo { get; set; }
public ICollection <People_workexperienceModel> workexperience { get; set; }
public ICollection <People_preferredlanguageinfoModel> preferredlanguageinfo { get; set; }
public ICollection <People_clinicaltaskinfoModel> clinicaltaskinfo { get; set; }
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
			

			public class PeopleModelValidator: AbstractValidator<PeopleModel>
			{
					private static bool IsValidIdNumber(string idType, string idNumber)
					{
						var value = (idNumber ?? string.Empty).Trim().ToUpperInvariant();
						switch ((idType ?? string.Empty).Trim().ToLowerInvariant())
						{
							case "pan":
							case "pan card":
								return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$");
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

					private static string GetIdNumberMessage(string idType)
					{
						switch ((idType ?? string.Empty).Trim().ToLowerInvariant())
						{
							case "pan":
							case "pan card":
								return "Invalid PAN format (ABCDE1234F)";
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
								return "Invalid ID Number";
						}
					}
					 
					public PeopleModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_People", () =>
                                    {
                                        {RuleFor(m => m.practitionerid)
.MaximumLength(256).WithMessage("The allowed length of Practitioner ID is 256 characters or fewer")
;
RuleFor(m => m.firstname)
.NotEmpty().WithMessage("First Name is required")
.MaximumLength(128).WithMessage("The allowed length of First Name is 128 characters or fewer")
;

RuleFor(m => m.workprofile)
.NotEmpty().WithMessage("Work Profile is required")
;
RuleFor(m => m.competencylevel)
.NotEmpty().WithMessage("Competency Name is required")
;
RuleFor(m => m.designation)
.NotEmpty().WithMessage("Designation is required")
;
RuleFor(m => m.contactnumber)
.NotEmpty().WithMessage("Contact Number is required")
.MaximumLength(20).WithMessage("The allowed length of Contact Number is 20 characters or fewer ")

;
RuleFor(m => m.whatsappnumber)
.NotEmpty().WithMessage("Whatsapp Number is required")
.MaximumLength(20).WithMessage("The allowed length of Whatsapp Number is 20 characters or fewer ")

;
RuleFor(m => m.emailid)
.NotEmpty().WithMessage("Email ID is required")
.MaximumLength(128).WithMessage("The allowed length of Email ID is 128 characters or fewer")
.EmailAddress()

;
RuleFor(m => m.gender)
.NotEmpty().WithMessage("Gender is required")
;
RuleFor(m => m.dob)
.NotEmpty().WithMessage("DOB is required")
.LessThan(System.DateTime.Now.Date).WithMessage("DOB should be Past Date ")

;
RuleFor(m => m.age)
.LessThanOrEqualTo(99999999).WithMessage("Age should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.employmentstatus)
.NotEmpty().WithMessage("Employment Status is required")
;
RuleFor(m => m.joiningdate)
.LessThanOrEqualTo(System.DateTime.Now.Date).WithMessage("Joining Date should be Today or Past Date")

;
 


RuleFor(m => m.idtype)
.NotEmpty().WithMessage("ID Type is required")
;
RuleFor(m => m.idnumber)
.NotEmpty().WithMessage("ID Number is required")
.MaximumLength(128).WithMessage("The allowed length of ID Number is 128 characters or fewer")
.Must((model, idnumber) => IsValidIdNumber(model.idtype, idnumber)).WithMessage(model => GetIdNumberMessage(model.idtype))
;
RuleFor(m => m.iddocument)
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
.NotNull().WithMessage("Zip is required")
.LessThanOrEqualTo(99999999).WithMessage("Zip should be LessThanOrEqualTo 99999999")

;




RuleFor(m => m.validtill)


;



}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_People", () =>
                                    {
                                        {RuleFor(m => m.practitionerid)
.MaximumLength(256).WithMessage("The allowed length of Practitioner ID is 256 characters or fewer")
;
RuleFor(m => m.firstname)
.NotEmpty().WithMessage("First Name is required")
.MaximumLength(128).WithMessage("The allowed length of First Name is 128 characters or fewer")
;

RuleFor(m => m.workprofile)
.NotEmpty().WithMessage("Work Profile is required")
;
RuleFor(m => m.competencylevel)
.NotEmpty().WithMessage("Competency Name is required")
;
RuleFor(m => m.designation)
.NotEmpty().WithMessage("Designation is required")
;
RuleFor(m => m.contactnumber)
.NotEmpty().WithMessage("Contact Number is required")
.MaximumLength(20).WithMessage("The allowed length of Contact Number is 20 characters or fewer ")

;
RuleFor(m => m.whatsappnumber)
.NotEmpty().WithMessage("Whatsapp Number is required")
.MaximumLength(20).WithMessage("The allowed length of Whatsapp Number is 20 characters or fewer ")

;
RuleFor(m => m.emailid)
.NotEmpty().WithMessage("Email ID is required")
.MaximumLength(128).WithMessage("The allowed length of Email ID is 128 characters or fewer")
.EmailAddress()

;
RuleFor(m => m.gender)
.NotEmpty().WithMessage("Gender is required")
;
RuleFor(m => m.dob)
.NotEmpty().WithMessage("DOB is required")
.LessThan(System.DateTime.Now.Date).WithMessage("DOB should be Past Date ")

;
RuleFor(m => m.age)
.LessThanOrEqualTo(99999999).WithMessage("Age should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.employmentstatus)
.NotEmpty().WithMessage("Employment Status is required")
;
RuleFor(m => m.joiningdate)
.LessThanOrEqualTo(System.DateTime.Now.Date).WithMessage("Joining Date should be Today or Past Date")

;
//RuleFor(m => m.contractrenewaldate)
//.GreaterThanOrEqualTo(System.DateTime.Now.Date).WithMessage("Contract Renewal Date should be Today or Future Date")

//;



RuleFor(m => m.idtype)
.NotEmpty().WithMessage("ID Type is required")
;
RuleFor(m => m.idnumber)
.NotEmpty().WithMessage("ID Number is required")
.MaximumLength(128).WithMessage("The allowed length of ID Number is 128 characters or fewer")
.Must((model, idnumber) => IsValidIdNumber(model.idtype, idnumber)).WithMessage(model => GetIdNumberMessage(model.idtype))
;
RuleFor(m => m.iddocument)
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
.NotNull().WithMessage("Zip is required")
.LessThanOrEqualTo(99999999).WithMessage("Zip should be LessThanOrEqualTo 99999999")

;




RuleFor(m => m.validtill)


;



}

                                    });

						 RuleForEach(x => x.emergencycontact).SetValidator(new People_emergencycontactModelValidator());
RuleForEach(x => x.educationinfo).SetValidator(new People_educationinfoModelValidator());
RuleForEach(x => x.workexperience).SetValidator(new People_workexperienceModelValidator());
RuleForEach(x => x.preferredlanguageinfo).SetValidator(new People_preferredlanguageinfoModelValidator());
RuleForEach(x => x.clinicaltaskinfo).SetValidator(new People_clinicaltaskinfoModelValidator());

						
					}

			}

                
			 public class People_emergencycontactModel
			{

			 

public string personname{ get; set; }


public string? relationship{ get; set; }


public string phonenumber{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?People_emergencycontactid { get; set; }
public System.Guid ?Peopleid { get; set; }



			}
			

			public class People_emergencycontactModelValidator: AbstractValidator<People_emergencycontactModel>
			{
				   
					public People_emergencycontactModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_People", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_People", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			 public class People_educationinfoModel
			{

			 

public string fieldofstudy{ get; set; }


public string degree{ get; set; }


public string? educationinstitution{ get; set; }


public string? certificationnumber{ get; set; }

public int? yearofgraduation{ get; set; }


public string? degreestatus{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?People_educationinfoid { get; set; }
public System.Guid ?Peopleid { get; set; }



			}
			

			public class People_educationinfoModelValidator: AbstractValidator<People_educationinfoModel>
			{
				   
					public People_educationinfoModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_People", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_People", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			 public class People_workexperienceModel
			{

			 

public string designation{ get; set; }


public string institutionname{ get; set; }

public DateTime? fromdate	{ get; set; }

public DateTime? todate	{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?People_workexperienceid { get; set; }
public System.Guid ?Peopleid { get; set; }



			}
			

			public class People_workexperienceModelValidator: AbstractValidator<People_workexperienceModel>
			{
				   
					public People_workexperienceModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_People", () =>
                                                {
                                                    {RuleFor(m => m.designation)
.NotEmpty().WithMessage("Designation is required")
.MaximumLength(128).WithMessage("The allowed length of Designation is 128 characters or fewer")
;
}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_People", () =>
                                                {
                                                    {RuleFor(m => m.designation)
.NotEmpty().WithMessage("Designation is required")
.MaximumLength(128).WithMessage("The allowed length of Designation is 128 characters or fewer")
;
}

                                                });

						
					}

			}



			 public class People_preferredlanguageinfoModel
			{

			 

public string languagesknown{ get; set; }


public string proficiency{ get; set; }


public string ability{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?People_preferredlanguageinfoid { get; set; }
public System.Guid ?Peopleid { get; set; }



			}
			

			public class People_preferredlanguageinfoModelValidator: AbstractValidator<People_preferredlanguageinfoModel>
			{
				   
					public People_preferredlanguageinfoModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_People", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_People", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			 public class People_clinicaltaskinfoModel
			{

			 

public string? consultations{ get; set; }

public Guid? workprofile	{ get; set; }

public Guid? tasktype	{ get; set; }

public Guid? taskname	{ get; set; }

public int? durationinminutes{ get; set; }

public int? overbookingcount{ get; set; }


public string availableon{ get; set; }


public string? workhourstarts{ get; set; }


public string? workhourends{ get; set; }


public string? priority{ get; set; }

public decimal? feesamount{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?People_clinicaltaskinfoid { get; set; }
public System.Guid ?Peopleid { get; set; }



			}
			

			public class People_clinicaltaskinfoModelValidator: AbstractValidator<People_clinicaltaskinfoModel>
			{
				   
					public People_clinicaltaskinfoModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_People", () =>
                                                {
                                                    {
}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_People", () =>
                                                {
                                                    {
}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
