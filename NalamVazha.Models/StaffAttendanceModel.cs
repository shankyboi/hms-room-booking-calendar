namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:24
			public class StaffAttendanceModel
			{

			 public System.Guid ?StaffAttendanceid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime shiftdate	{ get; set; }

public Guid shift	{ get; set; }

[xssFilter]
public string? shiftstarttime{ get; set; }

[xssFilter]
public string? shiftendtime{ get; set; }

// This is a snapshot copy of the selected Shift's shifthours (see lookup_change_StaffAttendance_shift),
// which is now decimal(18,2) to support fractional-hour shifts (e.g. "8.00", "1.50").
// Keeping this as int caused the same "value 'X.XX' is not valid for shifthours" model
// binding failure as the Add/Update Shift forms.
public decimal? shifthours{ get; set; }

public Guid workprofile	{ get; set; }

public Guid peoplename	{ get; set; }

[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode=true)]public DateTime punchdateandtime	{ get; set; }

public decimal? earlyinmin{ get; set; }

public decimal? earlyoutmin{ get; set; }

public decimal? latemin{ get; set; }

public int? workhours{ get; set; }
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
			

			public class StaffAttendanceModelValidator: AbstractValidator<StaffAttendanceModel>
			{
					 
					public StaffAttendanceModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Staff_Attendance", () =>
                                    {
                                        {RuleFor(m => m.shiftdate)
.NotEmpty().WithMessage("Shift Date is required")


;
RuleFor(m => m.shift)
.NotEmpty().WithMessage("Shift is required")
;


RuleFor(m => m.shifthours)
.LessThanOrEqualTo(99999999).WithMessage("Shift Hours should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.workprofile)
.NotEmpty().WithMessage("Work Profile is required")
;
RuleFor(m => m.peoplename)
.NotEmpty().WithMessage("People Name is required")
;
RuleFor(m => m.punchdateandtime)
.NotEmpty().WithMessage("Punch Date and Time is required")
;



RuleFor(m => m.workhours)
.LessThanOrEqualTo(99999999).WithMessage("Work Hours should be LessThanOrEqualTo 99999999")

;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Staff_Attendance", () =>
                                    {
                                        {RuleFor(m => m.shiftdate)
.NotEmpty().WithMessage("Shift Date is required")


;
RuleFor(m => m.shift)
.NotEmpty().WithMessage("Shift is required")
;


RuleFor(m => m.shifthours)
.LessThanOrEqualTo(99999999).WithMessage("Shift Hours should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.workprofile)
.NotEmpty().WithMessage("Work Profile is required")
;
RuleFor(m => m.peoplename)
.NotEmpty().WithMessage("People Name is required")
;
RuleFor(m => m.punchdateandtime)
.NotEmpty().WithMessage("Punch Date and Time is required")
;



RuleFor(m => m.workhours)
.LessThanOrEqualTo(99999999).WithMessage("Work Hours should be LessThanOrEqualTo 99999999")

;
}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
