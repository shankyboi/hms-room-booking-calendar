namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:22
			public class RoomModel
			{

			 public System.Guid ?Roomid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string roomcode{ get; set; }

public Guid block	{ get; set; }

public Guid building	{ get; set; }

public Guid floor	{ get; set; }

public Guid roomtype	{ get; set; }
		public decimal? costperday { get; set; }

		public decimal? advanceperday { get; set; }

		public decimal? bookingdeposit{ get; set; }
		public decimal? attendantadvanceperday { get; set; }

		public decimal? attendantbookingdeposit { get; set; }

		[xssFilter]
		public string? roomtransfercost { get; set; }
		public decimal? attendantcostperday { get; set; }
		public Guid roomgroup	{ get; set; }

[xssFilter]
public string roomnumber{ get; set; }

[xssFilter]
										
               public string? roomimage{ get; set; }



[xssFilter]
public string nextdaycheckin{ get; set; }

[xssFilter]
public string nextdaycheckout{ get; set; }

		[xssFilter]
		public string? easeofaccess { get; set; }


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
			

			public class RoomModelValidator: AbstractValidator<RoomModel>
			{
					 
					public RoomModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Room", () =>
                                    {
                                        {RuleFor(m => m.roomcode)
.MaximumLength(256).WithMessage("The allowed length of Room Code is 256 characters or fewer")
;
RuleFor(m => m.block)
.NotEmpty().WithMessage("Block is required")
;
RuleFor(m => m.building)
.NotEmpty().WithMessage("Building is required")
;
RuleFor(m => m.floor)
.NotEmpty().WithMessage("Floor is required")
;
RuleFor(m => m.roomtype)
.NotEmpty().WithMessage("Room Type is required")
;
RuleFor(m => m.bookingdeposit)
.NotNull().WithMessage("Booking Deposit is required")
.GreaterThan(0).WithMessage("Booking Deposit should be greater than 0")
.LessThanOrEqualTo(99999999).WithMessage("Booking Deposit should be LessThanOrEqualTo 99999999")

;
//RuleFor(m => m.roomgroup)
//.NotEmpty().WithMessage("Room Group is required")
//;
RuleFor(m => m.roomnumber)
.NotEmpty().WithMessage("Room Number is required")
.MaximumLength(128).WithMessage("The allowed length of Room Number is 128 characters or fewer")
;

											RuleFor(m => m.costperday)
											.LessThanOrEqualTo(99999999).WithMessage("Cost Per Day should be LessThanOrEqualTo 99999999")

											;
											RuleFor(m => m.advanceperday)
											.LessThanOrEqualTo(99999999).WithMessage("Advance Per Day should be LessThanOrEqualTo 99999999")

											;
											RuleFor(m => m.attendantcostperday)
.LessThanOrEqualTo(99999999).WithMessage("Cost Per Day should be LessThanOrEqualTo 99999999")

;
											RuleFor(m => m.attendantadvanceperday)
											.LessThanOrEqualTo(99999999).WithMessage("Advance Per Day should be LessThanOrEqualTo 99999999")

											;
											RuleFor(m => m.attendantbookingdeposit)
											.LessThanOrEqualTo(99999999).WithMessage("Booking Deposit should be LessThanOrEqualTo 99999999")

											;


											RuleFor(m => m.nextdaycheckin)
.NotEmpty().WithMessage("Next Day Checkin is required")
;
RuleFor(m => m.nextdaycheckout)
.NotEmpty().WithMessage("Next Day Checkout is required")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Room", () =>
                                    {
                                        {RuleFor(m => m.roomcode)
.MaximumLength(256).WithMessage("The allowed length of Room Code is 256 characters or fewer")
;
RuleFor(m => m.block)
.NotEmpty().WithMessage("Block is required")
;
RuleFor(m => m.building)
.NotEmpty().WithMessage("Building is required")
;
RuleFor(m => m.floor)
.NotEmpty().WithMessage("Floor is required")
;
RuleFor(m => m.roomtype)
.NotEmpty().WithMessage("Room Type is required")
;
RuleFor(m => m.bookingdeposit)
.NotNull().WithMessage("Booking Deposit is required")
.GreaterThan(0).WithMessage("Booking Deposit should be greater than 0")
.LessThanOrEqualTo(99999999).WithMessage("Booking Deposit should be LessThanOrEqualTo 99999999")

;
//RuleFor(m => m.roomgroup)
//.NotEmpty().WithMessage("Room Group is required")
//;
RuleFor(m => m.roomnumber)
.NotEmpty().WithMessage("Room Number is required")
.MaximumLength(128).WithMessage("The allowed length of Room Number is 128 characters or fewer")
;

											RuleFor(m => m.costperday)
											.LessThanOrEqualTo(99999999).WithMessage("Cost Per Day should be LessThanOrEqualTo 99999999")

											;
											RuleFor(m => m.advanceperday)
											.LessThanOrEqualTo(99999999).WithMessage("Advance Per Day should be LessThanOrEqualTo 99999999")

											;
											RuleFor(m => m.attendantcostperday)
											.LessThanOrEqualTo(99999999).WithMessage("Cost Per Day should be LessThanOrEqualTo 99999999")

											;
											RuleFor(m => m.attendantadvanceperday)
											.LessThanOrEqualTo(99999999).WithMessage("Advance Per Day should be LessThanOrEqualTo 99999999")

											;
											RuleFor(m => m.attendantbookingdeposit)
											.LessThanOrEqualTo(99999999).WithMessage("Booking Deposit should be LessThanOrEqualTo 99999999")

											;

											RuleFor(m => m.nextdaycheckin)
.NotEmpty().WithMessage("Next Day Checkin is required")
;
RuleFor(m => m.nextdaycheckout)
.NotEmpty().WithMessage("Next Day Checkout is required")
;
}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
