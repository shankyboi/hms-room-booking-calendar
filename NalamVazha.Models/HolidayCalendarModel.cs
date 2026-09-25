namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/30/2026 12:48:23
			public class HolidayCalendarModel
			{

			 public System.Guid ?HolidayCalendarid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string holidayname{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime holidaydate	{ get; set; }
public ICollection <HolidayCalendar_taskallowedModel> taskallowed { get; set; }
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
			

			public class HolidayCalendarModelValidator: AbstractValidator<HolidayCalendarModel>
			{
					 
					public HolidayCalendarModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Holiday", () =>
                                    {
                                        {RuleFor(m => m.holidayname)
.NotEmpty().WithMessage("Holiday Name is required")
.MaximumLength(128).WithMessage("The allowed length of Holiday Name is 128 characters or fewer")
;
RuleFor(m => m.holidaydate)
.NotEmpty().WithMessage("Holiday Date is required")


;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Holiday", () =>
                                    {
                                        {RuleFor(m => m.holidayname)
.NotEmpty().WithMessage("Holiday Name is required")
.MaximumLength(128).WithMessage("The allowed length of Holiday Name is 128 characters or fewer")
;
RuleFor(m => m.holidaydate)
.NotEmpty().WithMessage("Holiday Date is required")


;
}

                                    });

						 RuleForEach(x => x.taskallowed).SetValidator(new HolidayCalendar_taskallowedModelValidator());

						
					}

			}

                
			 public class HolidayCalendar_taskallowedModel
			{

			 
public Guid taskname	{ get; set; }


public string isallowed{ get; set; }

public System.Int64 count{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?HolidayCalendar_taskallowedid { get; set; }
public System.Guid ?HolidayCalendarid { get; set; }



			}
			

			public class HolidayCalendar_taskallowedModelValidator: AbstractValidator<HolidayCalendar_taskallowedModel>
			{
				   
					public HolidayCalendar_taskallowedModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Holiday", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Holiday", () =>
                                                {
                                                    {}

                                                });

						
					}

			}

    public class CalendarHolidayDto
    {
        public DateTime holidaydate { get; set; }

        public string holidayname { get; set; }
    }













}
