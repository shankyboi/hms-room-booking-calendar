namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
	//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:29
	public class BillingPaymentModel
	{

		public System.Guid? BillingPaymentid { get; set; }
		public System.Guid? tenantid { get; set; }
		public String viewertenantids { get; set; }

		[xssFilter]
		public string receiptno { get; set; }

		//[xssFilter]
		//public string? receivablefor { get; set; }
		[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)] public DateTime paymentdate { get; set; }

		public Guid? patientname { get; set; }

		public Guid? patientvisit { get; set; }

		public Guid? ipdnumber { get; set; }

		public Guid? opdnumber { get; set; }

		[xssFilter]
		public string receivablefor { get; set; }

		public Guid? therapy { get; set; }

		public decimal? therapycost { get; set; }

		public Guid? therapykit { get; set; }

		[xssFilter]
		public string? kitprice { get; set; }

		public Guid? medicine { get; set; }

		public decimal? price { get; set; }

		public Guid? room { get; set; }

		[xssFilter]
		public string currency { get; set; }

		public decimal? receivedamount { get; set; }

		public decimal? conversionrate { get; set; }

		public decimal amount { get; set; }

		[xssFilter]
		public string paymentmode { get; set; }

		[xssFilter]
		public string? transactionreference { get; set; }

		[xssFilter]
		public string? bankname { get; set; }

		[DataType(DataType.Date)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)] public DateTime? chequedddate { get; set; }

		[xssFilter]
		public string paymentstatus { get; set; }

		public Guid? collectedby { get; set; }

		[xssFilter]
		public string? refundmode { get; set; }

		public decimal? refundedamount { get; set; }

		public Guid? refundedby { get; set; }

		[xssFilter]
		public string? refundreferencenumber { get; set; }

		[xssFilter]
		public string? refundbankname { get; set; }

		[xssFilter]
		public string? refundreason { get; set; }

		[xssFilter]
		public string? refundstatus { get; set; }

		[xssFilter]
		public string? counterid { get; set; }

		[xssFilter]
		public string? remarks { get; set; }
		public System.Guid? createduser { get; set; }
		[DataType(DataType.Date)]
		[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public System.DateTime? createddate { get; set; }
		public System.Guid? modifieduser { get; set; }
		[DataType(DataType.Date)]
		[ModelBinder(BinderType = typeof(DateTimeModelBinder))]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public System.DateTime? modifieddate { get; set; }
		public bool isdeleted { get; set; }
		[xssFilter]
		[Required(ErrorMessage = "craftmyapp_actionmethodname is required,please pass current action name")]
		public String craftmyapp_actionmethodname { get; set; }

		public decimal? estimatedamount { get; set; }

	}

	public class BillingPaymentModelValidator: AbstractValidator<BillingPaymentModel>
			{
					 
					public BillingPaymentModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Billing_Payment", () =>
                                    {
                                        {RuleFor(m => m.receiptno)
.MaximumLength(256).WithMessage("The allowed length of Receipt No is 256 characters or fewer")
 ;
//RuleFor(m => m.receivablefor)
//.NotEmpty().WithMessage("Payment For is required")
//;


RuleFor(m => m.amount)
.NotNull().WithMessage("Amount is required")
;
RuleFor(m => m.paymentmode)
.NotEmpty().WithMessage("Payment Mode is required")
;

RuleFor(m => m.paymentstatus)
.NotEmpty().WithMessage("Payment Status is required")
;



}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Billing_Payment", () =>
                                    {
                                        {RuleFor(m => m.receiptno)
.MaximumLength(256).WithMessage("The allowed length of Receipt No is 256 characters or fewer")
;
//RuleFor(m => m.receivablefor)
//.NotEmpty().WithMessage("Payment For is required")
//;


RuleFor(m => m.amount)
.NotNull().WithMessage("Amount is required")
;
RuleFor(m => m.paymentmode)
.NotEmpty().WithMessage("Payment Mode is required")
;

RuleFor(m => m.paymentstatus)
.NotEmpty().WithMessage("Payment Status is required")
;



}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			
	public class OPDBillingSummaryModel
	{
		public System.Guid? patientvisitid      { get; set; }
		public string       visitnumber         { get; set; }
		public string       patientname         { get; set; }
		public string       consultingdoctor    { get; set; }
		public decimal      consultationcharge  { get; set; }
		public decimal      additionalcharge    { get; set; }
		public decimal      totalcharge         { get; set; }
		public decimal      totalpaid           { get; set; }
		public decimal      totalrefunded       { get; set; }
		public decimal      netpaid             { get; set; }
		public decimal      balance             { get; set; }
		public decimal      subtotal            { get; set; }
		public decimal      discountamount      { get; set; }
		public decimal      concessionamount    { get; set; }
		public decimal      depositcreditadjustment { get; set; }
		public decimal      currentamountpayable { get; set; }
		public decimal      refunddue { get; set; }
		public string       currencycode        { get; set; }
		public string       amountinwords       { get; set; }
	}
}
