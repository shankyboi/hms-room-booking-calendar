namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:17
			public class ShiftModel
			{

			 public System.Guid ?Shiftid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string shiftcode{ get; set; }

[xssFilter]
public string shiftname{ get; set; }

[xssFilter]
public string shiftstarttime{ get; set; }

[xssFilter]
public string shiftendtime{ get; set; }

// Shift hours must support fractional values (e.g. 7.5h) -- the Add/Update Shift UI
// computes this from start/end time and always formats it with 2 decimal places
// (e.g. "8.00"). Must stay `decimal` to match the Numeric-typed DB parameter in
// ShiftDAL.cs and the decimal(18,2) column in Shift.sql -- NOT string (Npgsql
// rejects writing a raw .NET string into a Numeric parameter) and NOT int (int
// can't bind "8.00" at all).
public decimal shifthours{ get; set; }

[xssFilter]
public string? description{ get; set; }

[xssFilter]
public string totalbreakinmins{ get; set; }

public decimal totalbreakinhrs{ get; set; }

[xssFilter]
public string workhours{ get; set; }
public ICollection <Shift_breakdurationdetailsModel> breakdurationdetails { get; set; }
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
			

			public class ShiftModelValidator: AbstractValidator<ShiftModel>
			{
					 
					public ShiftModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Shift", () =>
                                    {
                                        {RuleFor(m => m.shiftcode)
.MaximumLength(256).WithMessage("The allowed length of Shift Code is 256 characters or fewer")
;
RuleFor(m => m.shiftname)
.NotEmpty().WithMessage("Shift Name is required")
.MaximumLength(128).WithMessage("The allowed length of Shift Name is 128 characters or fewer")
;
RuleFor(m => m.shiftstarttime)
.NotEmpty().WithMessage("Shift Start Time is required")
;
RuleFor(m => m.shiftendtime)
.NotEmpty().WithMessage("Shift End Time is required")
;
RuleFor(m => m.shifthours)
.NotNull().WithMessage("Shift Hours is required")
.LessThanOrEqualTo(99999999).WithMessage("Shift Hours should be LessThanOrEqualTo 99999999")

;

RuleFor(m => m.totalbreakinmins)
.NotEmpty().WithMessage("Total Break in Mins is required")
.MaximumLength(256).WithMessage("The allowed length of Total Break in Mins is 256 characters or fewer")
;
RuleFor(m => m.totalbreakinhrs)
.NotNull().WithMessage("Total Break in Hrs is required")
;
RuleFor(m => m.workhours)
.NotEmpty().WithMessage("Work Hours is required")
.MaximumLength(256).WithMessage("The allowed length of Work Hours is 256 characters or fewer")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Shift", () =>
                                    {
                                        {RuleFor(m => m.shiftcode)
.MaximumLength(256).WithMessage("The allowed length of Shift Code is 256 characters or fewer")
;
RuleFor(m => m.shiftname)
.NotEmpty().WithMessage("Shift Name is required")
.MaximumLength(128).WithMessage("The allowed length of Shift Name is 128 characters or fewer")
;
RuleFor(m => m.shiftstarttime)
.NotEmpty().WithMessage("Shift Start Time is required")
;
RuleFor(m => m.shiftendtime)
.NotEmpty().WithMessage("Shift End Time is required")
;
RuleFor(m => m.shifthours)
.NotNull().WithMessage("Shift Hours is required")
.LessThanOrEqualTo(99999999).WithMessage("Shift Hours should be LessThanOrEqualTo 99999999")

;

RuleFor(m => m.totalbreakinmins)
.NotEmpty().WithMessage("Total Break in Mins is required")
.MaximumLength(256).WithMessage("The allowed length of Total Break in Mins is 256 characters or fewer")
;
RuleFor(m => m.totalbreakinhrs)
.NotNull().WithMessage("Total Break in Hrs is required")
;
RuleFor(m => m.workhours)
.NotEmpty().WithMessage("Work Hours is required")
.MaximumLength(256).WithMessage("The allowed length of Work Hours is 256 characters or fewer")
;
}

                                    });

						 RuleForEach(x => x.breakdurationdetails).SetValidator(new Shift_breakdurationdetailsModelValidator());

						
					}

			}

                
			 public class Shift_breakdurationdetailsModel
			{

			 

public string breakname{ get; set; }


public string starttime{ get; set; }


public string endtime{ get; set; }

public decimal durationinmin{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?Shift_breakdurationdetailsid { get; set; }
public System.Guid ?Shiftid { get; set; }



			}
			

			public class Shift_breakdurationdetailsModelValidator: AbstractValidator<Shift_breakdurationdetailsModel>
			{
				   
					public Shift_breakdurationdetailsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Shift", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Shift", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
