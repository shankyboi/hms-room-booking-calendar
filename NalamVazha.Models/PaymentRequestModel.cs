namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:48
			public class PaymentRequestModel
			{

			 public System.Guid ?PaymentRequestid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string paymentgateway{ get; set; }

[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode=true)]public DateTime? requestdatetime	{ get; set; }

public Guid? patientname	{ get; set; }

public Guid? people	{ get; set; }

[xssFilter]
public string paymenttype{ get; set; }

[xssFilter]
public string merchantid{ get; set; }

[xssFilter]
public string orderid{ get; set; }

[xssFilter]
public string paymentid{ get; set; }

public decimal amount{ get; set; }

[xssFilter]
public string currency{ get; set; }

[xssFilter]
public string customername{ get; set; }

[xssFilter]
public string? customeremail{ get; set; }

[xssFilter]
public string customerphone{ get; set; }

[xssFilter]
public string? orderpaymentdesc{ get; set; }

[xssFilter]
public string? returnurl{ get; set; }

[xssFilter]
public string? notifyurl{ get; set; }

[xssFilter]
public string? signatureorchecksum{ get; set; }
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
			

			public class PaymentRequestModelValidator: AbstractValidator<PaymentRequestModel>
			{
					 
					public PaymentRequestModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Payment_Request", () =>
                                    {
                                        {RuleFor(m => m.paymentgateway)
.NotEmpty().WithMessage("Payment Gateway is required")
;



RuleFor(m => m.paymenttype)
.NotEmpty().WithMessage("Payment Type is required")
;
RuleFor(m => m.merchantid)
.NotEmpty().WithMessage("Merchant ID is required")
.MaximumLength(128).WithMessage("The allowed length of Merchant ID is 128 characters or fewer")
;
RuleFor(m => m.orderid)
.NotEmpty().WithMessage("Order ID is required")
.MaximumLength(128).WithMessage("The allowed length of Order ID is 128 characters or fewer")
;
RuleFor(m => m.paymentid)
.NotEmpty().WithMessage("Payment Id is required")
.MaximumLength(128).WithMessage("The allowed length of Payment Id is 128 characters or fewer")
;
RuleFor(m => m.amount)
.NotNull().WithMessage("Amount is required")
.LessThanOrEqualTo(99999999).WithMessage("Amount should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.currency)
.NotEmpty().WithMessage("Currency is required")
;
RuleFor(m => m.customername)
.NotEmpty().WithMessage("Customer Name is required")
.MaximumLength(128).WithMessage("The allowed length of Customer Name is 128 characters or fewer")
;
RuleFor(m => m.customeremail)
.MaximumLength(128).WithMessage("The allowed length of Customer Email is 128 characters or fewer")
.EmailAddress()

;
RuleFor(m => m.customerphone)
.NotEmpty().WithMessage("Customer Phone is required")
.MaximumLength(20).WithMessage("The allowed length of Customer Phone is 20 characters or fewer ")

;




}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Payment_Request", () =>
                                    {
                                        {RuleFor(m => m.paymentgateway)
.NotEmpty().WithMessage("Payment Gateway is required")
;



RuleFor(m => m.paymenttype)
.NotEmpty().WithMessage("Payment Type is required")
;
RuleFor(m => m.merchantid)
.NotEmpty().WithMessage("Merchant ID is required")
.MaximumLength(128).WithMessage("The allowed length of Merchant ID is 128 characters or fewer")
;
RuleFor(m => m.orderid)
.NotEmpty().WithMessage("Order ID is required")
.MaximumLength(128).WithMessage("The allowed length of Order ID is 128 characters or fewer")
;
RuleFor(m => m.paymentid)
.NotEmpty().WithMessage("Payment Id is required")
.MaximumLength(128).WithMessage("The allowed length of Payment Id is 128 characters or fewer")
;
RuleFor(m => m.amount)
.NotNull().WithMessage("Amount is required")
.LessThanOrEqualTo(99999999).WithMessage("Amount should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.currency)
.NotEmpty().WithMessage("Currency is required")
;
RuleFor(m => m.customername)
.NotEmpty().WithMessage("Customer Name is required")
.MaximumLength(128).WithMessage("The allowed length of Customer Name is 128 characters or fewer")
;
RuleFor(m => m.customeremail)
.MaximumLength(128).WithMessage("The allowed length of Customer Email is 128 characters or fewer")
.EmailAddress()

;
RuleFor(m => m.customerphone)
.NotEmpty().WithMessage("Customer Phone is required")
.MaximumLength(20).WithMessage("The allowed length of Customer Phone is 20 characters or fewer ")

;




}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
