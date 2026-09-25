namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:03
			public class RoomOccupancyStatusModel
			{

			 public System.Guid ?RoomOccupancyStatusid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string roomallocationno{ get; set; }

public Guid? patientvisit	{ get; set; }

public Guid? patientname	{ get; set; }

public Guid ipdno	{ get; set; }

public Guid block	{ get; set; }

public Guid building	{ get; set; }

public Guid floor	{ get; set; }

public Guid room	{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime bookeddate	{ get; set; }

[xssFilter]
public string status{ get; set; }

[xssFilter]
public string bookedfor{ get; set; }

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
			

			public class RoomOccupancyStatusModelValidator: AbstractValidator<RoomOccupancyStatusModel>
			{
					 
					public RoomOccupancyStatusModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Room_Occupancy_Status", () =>
                                    {
                                        {RuleFor(m => m.roomallocationno)
.MaximumLength(256).WithMessage("The allowed length of Room Allocation No is 256 characters or fewer")
;
//RuleFor(m => m.patientvisit)
//.NotEmpty().WithMessage("Patient Visit is required")
//;

RuleFor(m => m.ipdno)
.NotEmpty().WithMessage("IPD No is required")
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
RuleFor(m => m.room)
.NotEmpty().WithMessage("Room is required")
;
RuleFor(m => m.bookeddate)
.NotEmpty().WithMessage("Booked Date is required")
.GreaterThanOrEqualTo(System.DateTime.Now.Date).WithMessage("Booked Date should be Today or Future Date")

;
RuleFor(m => m.status)
.NotEmpty().WithMessage("Status is required")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Room_Occupancy_Status", () =>
                                    {
                                        {RuleFor(m => m.roomallocationno)
.MaximumLength(256).WithMessage("The allowed length of Room Allocation No is 256 characters or fewer")
;
//RuleFor(m => m.patientvisit)
//.NotEmpty().WithMessage("Patient Visit is required")
//;

RuleFor(m => m.ipdno)
.NotEmpty().WithMessage("IPD No is required")
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
RuleFor(m => m.room)
.NotEmpty().WithMessage("Room is required")
;
RuleFor(m => m.bookeddate)
.NotEmpty().WithMessage("Booked Date is required")
.GreaterThanOrEqualTo(System.DateTime.Now.Date).WithMessage("Booked Date should be Today or Future Date")

;
RuleFor(m => m.status)
.NotEmpty().WithMessage("Status is required")
;
}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
