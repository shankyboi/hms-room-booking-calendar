namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 05/26/2026 07:14:49
			public class ConcessionFormModel
			{

			 public System.Guid ?ConcessionFormid	{ get; set; }
public System.Guid ?verifiedby	{ get; set; }
[DataType(DataType.Date)]
[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
[DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]
public System.DateTime ?verifieddate	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid patientname	{ get; set; }

public Guid bookingreferencenumber	{ get; set; }

[xssFilter]
public string residentialhousetype{ get; set; }

[xssFilter]
public string totalannualfamilyincome{ get; set; }

[xssFilter]
public string whichofthesedoyouown{ get; set; }

[xssFilter]
public string? paidservicesaccessed{ get; set; }

[xssFilter]
public string haveyoutraveledinternationallyinthelast5years{ get; set; }

[xssFilter]
public string? purposeoftravel{ get; set; }

[xssFilter]
public string? otherpleasespecify{ get; set; }

[xssFilter]
public string doanyofthefollowingapplytoyou{ get; set; }

[xssFilter]
public string? specifyotherindustry{ get; set; }

public decimal? requestedconcessionamount{ get; set; }

public decimal? approvedconcessionamount{ get; set; }

[xssFilter]
public string? verifiedstatus{ get; set; }
public ICollection <ConcessionForm_earningmembersModel> earningmembers { get; set; }
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
			

			public class ConcessionFormModelValidator: AbstractValidator<ConcessionFormModel>
			{
					 
					public ConcessionFormModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Concession_Form", () =>
                                    {
                                        {RuleFor(m => m.patientname)
.NotEmpty().WithMessage("Patient Name is required")
;
RuleFor(m => m.bookingreferencenumber)
.NotEmpty().WithMessage("Booking Reference Number is required")
;
RuleFor(m => m.residentialhousetype)
.NotEmpty().WithMessage("Residential House Type is required")
;
RuleFor(m => m.totalannualfamilyincome)
.NotEmpty().WithMessage("Total Annual Family Income is required")
;
RuleFor(m => m.whichofthesedoyouown)
.NotEmpty().WithMessage("Which of these do you own? is required")
;

RuleFor(m => m.haveyoutraveledinternationallyinthelast5years)
.NotEmpty().WithMessage("Have you traveled internationally in the last 5 years? is required")
;


RuleFor(m => m.doanyofthefollowingapplytoyou)
.NotEmpty().WithMessage("Do any of the following apply to you? is required")
;

RuleFor(m => m.requestedconcessionamount)
.LessThanOrEqualTo(99999999).WithMessage("Requested Concession Amount should be LessThanOrEqualTo 99999999")
.GreaterThanOrEqualTo(-99999999).WithMessage("Requested Concession Amount should be GreaterThanOrEqualTo -99999999")

;

When(m => !string.IsNullOrWhiteSpace(m.doanyofthefollowingapplytoyou) && m.doanyofthefollowingapplytoyou.Split(',', System.StringSplitOptions.RemoveEmptyEntries).Any(v => v.Trim().Equals("Others", System.StringComparison.OrdinalIgnoreCase) || v.Trim().Equals("Other", System.StringComparison.OrdinalIgnoreCase)), () =>
{
RuleFor(m => m.specifyotherindustry)
.NotEmpty().WithMessage("Specify other industry is required")
;
});

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Concession_Form", () =>
                                    {
                                        {RuleFor(m => m.patientname)
.NotEmpty().WithMessage("Patient Name is required")
;
RuleFor(m => m.bookingreferencenumber)
.NotEmpty().WithMessage("Booking Reference Number is required")
;
RuleFor(m => m.residentialhousetype)
.NotEmpty().WithMessage("Residential House Type is required")
;
RuleFor(m => m.totalannualfamilyincome)
.NotEmpty().WithMessage("Total Annual Family Income is required")
;
RuleFor(m => m.whichofthesedoyouown)
.NotEmpty().WithMessage("Which of these do you own? is required")
;

RuleFor(m => m.haveyoutraveledinternationallyinthelast5years)
.NotEmpty().WithMessage("Have you traveled internationally in the last 5 years? is required")
;


RuleFor(m => m.doanyofthefollowingapplytoyou)
.NotEmpty().WithMessage("Do any of the following apply to you? is required")
;

RuleFor(m => m.requestedconcessionamount)
.LessThanOrEqualTo(99999999).WithMessage("Requested Concession Amount should be LessThanOrEqualTo 99999999")
.GreaterThanOrEqualTo(-99999999).WithMessage("Requested Concession Amount should be GreaterThanOrEqualTo -99999999")

;

When(m => !string.IsNullOrWhiteSpace(m.doanyofthefollowingapplytoyou) && m.doanyofthefollowingapplytoyou.Split(',', System.StringSplitOptions.RemoveEmptyEntries).Any(v => v.Trim().Equals("Others", System.StringComparison.OrdinalIgnoreCase) || v.Trim().Equals("Other", System.StringComparison.OrdinalIgnoreCase)), () =>
{
RuleFor(m => m.specifyotherindustry)
.NotEmpty().WithMessage("Specify other industry is required")
;
});

}

                                    });

						 RuleForEach(x => x.earningmembers).SetValidator(new ConcessionForm_earningmembersModelValidator());

						
					}

			}

                
			 public class ConcessionForm_earningmembersModel
			{

			 

public string earningmembername{ get; set; }


public string relation{ get; set; }


public string designationoccupation{ get; set; }

public decimal approxannualincome{ get; set; }

public decimal? incomebyinterestandallkinds{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?ConcessionForm_earningmembersid { get; set; }
public System.Guid ?ConcessionFormid { get; set; }



			}
			

			public class ConcessionForm_earningmembersModelValidator: AbstractValidator<ConcessionForm_earningmembersModel>
			{
				   
					public ConcessionForm_earningmembersModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Concession_Form", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Concession_Form", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                 
                                                    public class ConcessionFormReviewModel
                                                    {
                                                        public string ConcessionFormid { get; set; }
                                                        public string reviewcomments { get; set; }
                                                        public string verifiedstatus { get; set; }
                                                        public string verifiedby { get; set; }
                                                        public decimal? approvedconcessionamount { get; set; }

														

                                                    }

        

			}
