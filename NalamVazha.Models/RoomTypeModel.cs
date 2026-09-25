namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:17
			public class RoomTypeModel
			{

			 public System.Guid ?RoomTypeid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string name{ get; set; }

public int prebookingdaylimit{ get; set; }

public int minbookingdays{ get; set; }

public int maxbookingdays{ get; set; }

[xssFilter]
public string concessoneligibility{ get; set; }

[xssFilter]
public string suitabilityforvip{ get; set; }

[xssFilter]
public string? gendersuitability{ get; set; }

[xssFilter]
										
               public string? roomtypeicon{ get; set; }

[xssFilter]
public string? deposittype{ get; set; }

public decimal costperday{ get; set; }

public decimal advanceperday{ get; set; }

public decimal? bookingdeposit{ get; set; }

public decimal? variableofbookingdays{ get; set; }

public decimal attendantcostperday{ get; set; }

public decimal attendantadvanceperday{ get; set; }

public decimal? attendantbookingdeposit{ get; set; }

public decimal? attendantvariableofbookingdays{ get; set; }

public bool hourlychargesapplicable	{ get; set; }

public decimal? chargeperhour{ get; set; }

[xssFilter]
public string? billingwaiverfordelayedstart{ get; set; }

public decimal? waiverpercentage{ get; set; }

[xssFilter]
public string? roomtransfercost{ get; set; }

[xssFilter]
public string? description{ get; set; }
public ICollection <RoomType_refundpolicyModel> refundpolicy { get; set; }
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
			

			public class RoomTypeModelValidator: AbstractValidator<RoomTypeModel>
			{
					 
					public RoomTypeModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Room_Type", () =>
                                    {
                                        {RuleFor(m => m.name)
.NotEmpty().WithMessage("Name is required")
.MaximumLength(128).WithMessage("The allowed length of Name is 128 characters or fewer")
;
RuleFor(m => m.prebookingdaylimit)
.NotNull().WithMessage("Post-booking Day Limit is required")
.LessThanOrEqualTo(99999999).WithMessage("Post-booking Day Limit should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.minbookingdays)
.NotNull().WithMessage("Min Booking Days is required")
.LessThanOrEqualTo(99999999).WithMessage("Min Booking Days should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.maxbookingdays)
.NotNull().WithMessage("Max Booking Days is required")
.LessThanOrEqualTo(99999999).WithMessage("Max Booking Days should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.concessoneligibility)
.NotEmpty().WithMessage("Concesson Eligibility is required")
;
RuleFor(m => m.suitabilityforvip)
.NotEmpty().WithMessage("Suitability for VIP is required")
;



RuleFor(m => m.costperday)
.NotNull().WithMessage("Cost Per Day is required")
.LessThanOrEqualTo(99999999).WithMessage("Cost Per Day should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.advanceperday)
.NotNull().WithMessage("Advance Per Day is required")
.LessThanOrEqualTo(99999999).WithMessage("Advance Per Day should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.bookingdeposit)
.NotNull().WithMessage("Booking Deposit is required")
.GreaterThan(0).WithMessage("Booking Deposit should be greater than 0")
.LessThanOrEqualTo(99999999).WithMessage("Booking Deposit should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.variableofbookingdays)
.LessThanOrEqualTo(100).WithMessage("Variable ( of booking days)  should be LessThanOrEqualTo 100")

;
RuleFor(m => m.attendantcostperday)
.NotNull().WithMessage("Cost Per Day is required")
.LessThanOrEqualTo(99999999).WithMessage("Cost Per Day should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.attendantadvanceperday)
.NotNull().WithMessage("Advance Per Day is required")
.LessThanOrEqualTo(99999999).WithMessage("Advance Per Day should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.attendantbookingdeposit)
.NotNull().WithMessage("Booking Deposit is required")
.GreaterThan(0).WithMessage("Booking Deposit should be greater than 0")
.LessThanOrEqualTo(99999999).WithMessage("Booking Deposit should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.attendantvariableofbookingdays)
.LessThanOrEqualTo(100).WithMessage("Variable ( of booking days)  should be LessThanOrEqualTo 100")

;

RuleFor(m => m.chargeperhour)
.LessThanOrEqualTo(99999999).WithMessage("Charge per Hour should be LessThanOrEqualTo 99999999")

;

RuleFor(m => m.waiverpercentage)
.LessThanOrEqualTo(100).WithMessage("Waiver Percentage should be LessThanOrEqualTo 100")

;


}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Room_Type", () =>
                                    {
                                        {RuleFor(m => m.name)
.NotEmpty().WithMessage("Name is required")
.MaximumLength(128).WithMessage("The allowed length of Name is 128 characters or fewer")
;
RuleFor(m => m.prebookingdaylimit)
.NotNull().WithMessage("Post-booking Day Limit is required")
.LessThanOrEqualTo(99999999).WithMessage("Post-booking Day Limit should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.minbookingdays)
.NotNull().WithMessage("Min Booking Days is required")
.LessThanOrEqualTo(99999999).WithMessage("Min Booking Days should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.maxbookingdays)
.NotNull().WithMessage("Max Booking Days is required")
.LessThanOrEqualTo(99999999).WithMessage("Max Booking Days should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.concessoneligibility)
.NotEmpty().WithMessage("Concesson Eligibility is required")
;
RuleFor(m => m.suitabilityforvip)
.NotEmpty().WithMessage("Suitability for VIP is required")
;



RuleFor(m => m.costperday)
.NotNull().WithMessage("Cost Per Day is required")
.LessThanOrEqualTo(99999999).WithMessage("Cost Per Day should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.advanceperday)
.NotNull().WithMessage("Advance Per Day is required")
.LessThanOrEqualTo(99999999).WithMessage("Advance Per Day should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.bookingdeposit)
.NotNull().WithMessage("Booking Deposit is required")
.GreaterThan(0).WithMessage("Booking Deposit should be greater than 0")
.LessThanOrEqualTo(99999999).WithMessage("Booking Deposit should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.variableofbookingdays)
.LessThanOrEqualTo(100).WithMessage("Variable ( of booking days)  should be LessThanOrEqualTo 100")

;
RuleFor(m => m.attendantcostperday)
.NotNull().WithMessage("Cost Per Day is required")
.LessThanOrEqualTo(99999999).WithMessage("Cost Per Day should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.attendantadvanceperday)
.NotNull().WithMessage("Advance Per Day is required")
.LessThanOrEqualTo(99999999).WithMessage("Advance Per Day should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.attendantbookingdeposit)
.NotNull().WithMessage("Booking Deposit is required")
.GreaterThan(0).WithMessage("Booking Deposit should be greater than 0")
.LessThanOrEqualTo(99999999).WithMessage("Booking Deposit should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.attendantvariableofbookingdays)
.LessThanOrEqualTo(100).WithMessage("Variable ( of booking days)  should be LessThanOrEqualTo 100")

;

RuleFor(m => m.chargeperhour)
.LessThanOrEqualTo(99999999).WithMessage("Charge per Hour should be LessThanOrEqualTo 99999999")

;

RuleFor(m => m.waiverpercentage)
.LessThanOrEqualTo(100).WithMessage("Waiver Percentage should be LessThanOrEqualTo 100")

;


}

                                    });

						 RuleForEach(x => x.refundpolicy).SetValidator(new RoomType_refundpolicyModelValidator());

						
					}

			}

                
			 public class RoomType_refundpolicyModel
			{

			 

public string refundtype{ get; set; }


public string cancellationby{ get; set; }

public int cancellationwindowdays{ get; set; }

public decimal refundpercentage{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?RoomType_refundpolicyid { get; set; }
public System.Guid ?RoomTypeid { get; set; }



			}
			

			public class RoomType_refundpolicyModelValidator: AbstractValidator<RoomType_refundpolicyModel>
			{
				   
					public RoomType_refundpolicyModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Room_Type", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Room_Type", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
