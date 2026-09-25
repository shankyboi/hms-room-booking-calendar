namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/21/2026 05:37:58
			public class ReceivableModel
			{

			 public System.Guid ?Receivableid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string receivableno{ get; set; }

[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]public DateTime receivabledate	{ get; set; }

public Guid patientname	{ get; set; }

public Guid? patientvisit	{ get; set; }

public Guid? ipdnumber	{ get; set; }

public Guid? opdnumber	{ get; set; }

[xssFilter]
public string receivablefor{ get; set; }
		[xssFilter]
		public string? specifyothers { get; set; }
		public Guid? therapy	{ get; set; }

public decimal? therapycost{ get; set; }

public Guid? therapykit	{ get; set; }

[xssFilter]
public string? kitprice{ get; set; }

public Guid? medicine	{ get; set; }

public decimal? price{ get; set; }

public Guid? package	{ get; set; }

public Guid? room	{ get; set; }

public decimal amount{ get; set; }

[xssFilter]
public string? remarks{ get; set; }
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

		public decimal? paidamount { get; set; }
		 

		public string? paymentstatus { get; set; }

		public System.Guid? billingpaymentid { get; set; }
	}
			

			public class ReceivableModelValidator: AbstractValidator<ReceivableModel>
			{
					 
					public ReceivableModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Receivable", () =>
                                    {
                                        {RuleFor(m => m.receivableno)
.MaximumLength(256).WithMessage("The allowed length of Receivable No is 256 characters or fewer")
;
RuleFor(m => m.receivabledate)
.NotEmpty().WithMessage("Receivable Date is required")
//.GreaterThan(System.DateTime.Now.Date).WithMessage("Receivable Date should be Future Date")

;
RuleFor(m => m.patientname)
.NotEmpty().WithMessage("Patient Name is required")
;


RuleFor(m => m)
.Must(HasEitherIpdOrOpd).WithMessage("Either IPD Number or OPD Number is required")
;

RuleFor(m => m)
.Must(HasOnlyOneVisitNumber).WithMessage("Select either IPD Number or OPD Number, not both")
;


RuleFor(m => m.receivablefor)
.NotEmpty().WithMessage("Receivable For is required")
;

RuleFor(m => m.medicine)
.NotEmpty().When(m => IsReceivableFor(m.receivablefor, "Medicine")).WithMessage("Medicine is required")
;

RuleFor(m => m.therapy)
.NotEmpty().When(m => IsReceivableFor(m.receivablefor, "Therapy")).WithMessage("Therapy is required")
;

RuleFor(m => m.therapykit)
.NotEmpty().When(m => IsReceivableFor(m.receivablefor, "Therapy Kit")).WithMessage("Therapy Kit is required")
;

RuleFor(m => m.therapycost)
.LessThanOrEqualTo(99999999).WithMessage("Therapy Cost should be LessThanOrEqualTo 99999999")

;



RuleFor(m => m.price)
.LessThanOrEqualTo(99999999).WithMessage("Price should be LessThanOrEqualTo 99999999")

;


RuleFor(m => m.amount)
.NotNull().WithMessage("Amount in INR is required")
.NotEqual(0).WithMessage("Amount in INR should not be 0")
;

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Receivable", () =>
                                    {
                                        {RuleFor(m => m.receivableno)
.MaximumLength(256).WithMessage("The allowed length of Receivable No is 256 characters or fewer")
;
RuleFor(m => m.receivabledate)
.NotEmpty().WithMessage("Receivable Date is required")
//.GreaterThan(System.DateTime.Now.Date).WithMessage("Receivable Date should be Future Date")

;
RuleFor(m => m.patientname)
.NotEmpty().WithMessage("Patient Name is required")
;


RuleFor(m => m)
.Must(HasEitherIpdOrOpd).WithMessage("Either IPD Number or OPD Number is required")
;

RuleFor(m => m)
.Must(HasOnlyOneVisitNumber).WithMessage("Select either IPD Number or OPD Number, not both")
;


RuleFor(m => m.receivablefor)
.NotEmpty().WithMessage("Receivable For is required")
;

RuleFor(m => m.medicine)
.NotEmpty().When(m => IsReceivableFor(m.receivablefor, "Medicine")).WithMessage("Medicine is required")
;

RuleFor(m => m.therapy)
.NotEmpty().When(m => IsReceivableFor(m.receivablefor, "Therapy")).WithMessage("Therapy is required")
;

RuleFor(m => m.therapykit)
.NotEmpty().When(m => IsReceivableFor(m.receivablefor, "Therapy Kit")).WithMessage("Therapy Kit is required")
;

RuleFor(m => m.therapycost)
.LessThanOrEqualTo(99999999).WithMessage("Therapy Cost should be LessThanOrEqualTo 99999999")

;



RuleFor(m => m.price)
.LessThanOrEqualTo(99999999).WithMessage("Price should be LessThanOrEqualTo 99999999")

;


RuleFor(m => m.amount)
.NotNull().WithMessage("Amount in INR is required")
.NotEqual(0).WithMessage("Amount in INR should not be 0")
;

}

                                    });

						 
						
					}

                    private static bool HasEitherIpdOrOpd(ReceivableModel model)
                    {
                        return model.ipdnumber.HasValue || model.opdnumber.HasValue;
                    }

                    private static bool HasOnlyOneVisitNumber(ReceivableModel model)
                    {
                        return !(model.ipdnumber.HasValue && model.opdnumber.HasValue);
                    }

                    private static bool IsReceivableFor(string receivableFor, string expected)
                    {
                        var actualValue = (receivableFor ?? string.Empty).Trim();
                        return string.Equals(actualValue, expected, StringComparison.OrdinalIgnoreCase)
                            || string.Equals(actualValue.Replace(" ", string.Empty), expected.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase);
                    }

			}

                

                

                
 

                

                

        

			}
