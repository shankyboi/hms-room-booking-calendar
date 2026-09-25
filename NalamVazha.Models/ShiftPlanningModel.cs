namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:20
			public class ShiftPlanningModel
			{

			 public System.Guid ?ShiftPlanningid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid shiftname	{ get; set; }
		[xssFilter]
		public string? shiftstarttime { get; set; }

		[xssFilter]
		public string? shiftendtime { get; set; }
		[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime validfrom	{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime validto	{ get; set; }
public ICollection <ShiftPlanning_bulkuploadModel> ShiftPlanning_bulkupload{ get; set; }
public ICollection <ShiftPlanning_peopleModel> people { get; set; }
public ICollection <ShiftPlanning_people_bulkuploadModel> ShiftPlanning_people_bulkupload { get; set; }
public String ShiftPlanning_people_bulkuploadfile{ get; set; }
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
			

			public class ShiftPlanningModelValidator: AbstractValidator<ShiftPlanningModel>
			{
					 
					public ShiftPlanningModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Shift_Planning", () =>
                                    {
                                        {RuleFor(m => m.shiftname)
.NotEmpty().WithMessage("Shift Name is required")
;
RuleFor(m => m.validfrom)
.NotEmpty().WithMessage("Valid From is required")
.GreaterThanOrEqualTo(System.DateTime.Now.Date).WithMessage("Valid From should be Today or Future Date")

;
RuleFor(m => m.validto)
.NotEmpty().WithMessage("Valid To is required")
.GreaterThan(System.DateTime.Now.Date).WithMessage("Valid To should be Future Date")
.GreaterThan(m => m.validfrom).WithMessage("Valid To should be greater than Valid From")

;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Shift_Planning", () =>
                                    {
                                        {RuleFor(m => m.shiftname)
.NotEmpty().WithMessage("Shift Name is required")
;
RuleFor(m => m.validfrom)
.NotEmpty().WithMessage("Valid From is required")

;
RuleFor(m => m.validto)
.NotEmpty().WithMessage("Valid To is required")
.GreaterThan(m => m.validfrom).WithMessage("Valid To should be greater than Valid From")

;
}

                                    });

						 RuleForEach(x => x.people).SetValidator(new ShiftPlanning_peopleModelValidator());
RuleForEach(x => x.ShiftPlanning_people_bulkupload).SetValidator(new ShiftPlanning_people_bulkuploadModelValidator());

						
					}

			}

                
			 public class ShiftPlanning_peopleModel
			{

			 
public Guid personname	{ get; set; }

public Guid? workprofile	{ get; set; }


public string coveragetype{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?ShiftPlanning_peopleid { get; set; }
public System.Guid ?ShiftPlanningid { get; set; }



			}
			

			public class ShiftPlanning_peopleModelValidator: AbstractValidator<ShiftPlanning_peopleModel>
			{
				   
					public ShiftPlanning_peopleModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Shift_Planning", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Shift_Planning", () =>
                                                {
                                                    {}

                                                });

						
					}

			}
public class ShiftPlanning_people_bulkuploadModel{		[Display(Name = "Person Name")]
		public string personname { get; set; }		[Display(Name = "Work Profile")]
		public string workprofile { get; set; }		[Display(Name = "Coverage Type")]
		public string coveragetype { get; set; }		[Display(Name = "craftmyapp_actionmethodname")]
		public string craftmyapp_actionmethodname { get; set; }
		[Display(Name = "cma_client_row_id")]
		public string cma_client_row_id { get; set; }
		[Display(Name = "record_order")]
		public int? record_order { get; set; }
}
public class ShiftPlanning_people_bulkuploadModelValidator : AbstractValidator<ShiftPlanning_people_bulkuploadModel>
{
    public ShiftPlanning_people_bulkuploadModelValidator()
    {
        RuleFor(m => m.personname)
.NotEmpty().WithMessage("Person Name is required")
;

RuleFor(m => m.coveragetype)
.NotEmpty().WithMessage("Coverage Type  is required")
;

    }
}


                

                
 

                

                

        
public class ShiftPlanning_bulkuploadModel{		[Display(Name = "craftmyapp_actionmethodname")]
		public string craftmyapp_actionmethodname { get; set; }
		[Display(Name = "cma_client_row_id")]
		public string cma_client_row_id { get; set; }
		[Display(Name = "record_order")]
		public int? record_order { get; set; }
}
			}
