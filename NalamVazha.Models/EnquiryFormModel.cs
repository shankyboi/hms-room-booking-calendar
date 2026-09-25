namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:41
			public class EnquiryFormModel
			{

			 public System.Guid ?EnquiryFormid	{ get; set; }
public System.Guid ?verifiedby	{ get; set; }
[DataType(DataType.Date)]
[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
[DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]
public System.DateTime ?verifieddate	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string enquirynumber{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime enquirydate	{ get; set; }

public Guid enquirytype	{ get; set; }

[xssFilter]
public string? isroombookingrelated{ get; set; }

public Guid? patientname	{ get; set; }

[xssFilter]
public string firstname{ get; set; }

[xssFilter]
public string? lastname{ get; set; }

[xssFilter]
public string gender{ get; set; }

public System.Int64 age{ get; set; }

[xssFilter]
public string phonenumber{ get; set; }

[xssFilter]
public string? emailaddress{ get; set; }

[xssFilter]
public string? preferredcontactmethod{ get; set; }

[xssFilter]
public string? enquiryreason{ get; set; }

[xssFilter]
public string? enquiredvia{ get; set; }

public Guid? preferredroomtype	{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime? preferreddateofarrival	{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime? preferreddateofdeparture	{ get; set; }

public bool joinwaitinglist	{ get; set; }

[xssFilter]
public string enquirystatus{ get; set; }

[xssFilter]
public string? verifiedstatus{ get; set; }
public ICollection <EnquiryForm_medicalinfoModel> medicalinfo { get; set; }
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
			

			public class EnquiryFormModelValidator: AbstractValidator<EnquiryFormModel>
			{
					 
					public EnquiryFormModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Enquiry", () =>
                                    {
                                        {RuleFor(m => m.enquirynumber)
.MaximumLength(256).WithMessage("The allowed length of Enquiry Number is 256 characters or fewer")
;
RuleFor(m => m.enquirydate)
.NotEmpty().WithMessage("Enquiry Date is required")
.Equal(System.DateTime.Now.Date).WithMessage("Enquiry Date should be equal to TODAY")

;
RuleFor(m => m.enquirytype)
.NotEmpty().WithMessage("Enquiry Type is required")
;


RuleFor(m => m.firstname)
.NotEmpty().WithMessage("First Name is required")
.MaximumLength(128).WithMessage("The allowed length of First Name is 128 characters or fewer")
;

RuleFor(m => m.gender)
.NotEmpty().WithMessage("Gender is required")
;
RuleFor(m => m.age)
.NotEmpty().WithMessage("Age is required")
.LessThanOrEqualTo(99999999).WithMessage("Age should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.phonenumber)
.NotEmpty().WithMessage("Phone Number is required")
.MaximumLength(20).WithMessage("The allowed length of Phone Number is 20 characters or fewer ")

;
RuleFor(m => m.emailaddress)
.MaximumLength(128).WithMessage("The allowed length of Email Address is 128 characters or fewer")
.EmailAddress()

;




RuleFor(m => m.preferreddateofarrival)
.GreaterThan(System.DateTime.Now.Date).WithMessage("Preferred Date of Arrival should be Future Date")

;
RuleFor(m => m.preferreddateofdeparture)
.GreaterThan(System.DateTime.Now.Date).WithMessage("Preferred Date of Departure should be Future Date")

;

RuleFor(m => m.enquirystatus)
.NotEmpty().WithMessage("Enquiry Status is required")
;

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Enquiry", () =>
                                    {
                                        {RuleFor(m => m.enquirynumber)
.MaximumLength(256).WithMessage("The allowed length of Enquiry Number is 256 characters or fewer")
;
RuleFor(m => m.enquirydate)
.NotEmpty().WithMessage("Enquiry Date is required")
.Equal(System.DateTime.Now.Date).WithMessage("Enquiry Date should be equal to TODAY")

;
RuleFor(m => m.enquirytype)
.NotEmpty().WithMessage("Enquiry Type is required")
;


RuleFor(m => m.firstname)
.NotEmpty().WithMessage("First Name is required")
.MaximumLength(128).WithMessage("The allowed length of First Name is 128 characters or fewer")
;

RuleFor(m => m.gender)
.NotEmpty().WithMessage("Gender is required")
;
RuleFor(m => m.age)
.NotEmpty().WithMessage("Age is required")
.LessThanOrEqualTo(99999999).WithMessage("Age should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.phonenumber)
.NotEmpty().WithMessage("Phone Number is required")
.MaximumLength(20).WithMessage("The allowed length of Phone Number is 20 characters or fewer ")

;
RuleFor(m => m.emailaddress)
.MaximumLength(128).WithMessage("The allowed length of Email Address is 128 characters or fewer")
.EmailAddress()

;




RuleFor(m => m.preferreddateofarrival)
.GreaterThan(System.DateTime.Now.Date).WithMessage("Preferred Date of Arrival should be Future Date")

;
RuleFor(m => m.preferreddateofdeparture)
.GreaterThan(System.DateTime.Now.Date).WithMessage("Preferred Date of Departure should be Future Date")

;

RuleFor(m => m.enquirystatus)
.NotEmpty().WithMessage("Enquiry Status is required")
;

}

                                    });

						 RuleForEach(x => x.medicalinfo).SetValidator(new EnquiryForm_medicalinfoModelValidator());

						
					}

			}

                
			 public class EnquiryForm_medicalinfoModel
			{

			 
public Guid? medicalcondition	{ get; set; }


public string? conditionname{ get; set; }


public string? duration{ get; set; }


public string? severity{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?EnquiryForm_medicalinfoid { get; set; }
public System.Guid ?EnquiryFormid { get; set; }



			}
			

			public class EnquiryForm_medicalinfoModelValidator: AbstractValidator<EnquiryForm_medicalinfoModel>
			{
				   
					public EnquiryForm_medicalinfoModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Enquiry", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Enquiry", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                 
                                                    public class EnquiryFormReviewModel
                                                    {
                                                        public string EnquiryFormid { get; set; }
                                                        public string reviewcomments { get; set; }
                                                        public string verifiedstatus { get; set; }
                                                        public string verifiedby { get; set; }

														

                                                    }

        

			}
