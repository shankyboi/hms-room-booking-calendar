namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:53
			public class PaymentResponseModel
			{

			 public System.Guid ?PaymentResponseid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid? paymentrequest	{ get; set; }

[xssFilter]
public string? paymenttype{ get; set; }

[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode=true)]public DateTime transactiontime	{ get; set; }

[xssFilter]
public string orderid{ get; set; }

[xssFilter]
public string? paymentid{ get; set; }

[xssFilter]
public string? status{ get; set; }

public decimal? amount{ get; set; }

[xssFilter]
public string? paymentmethod{ get; set; }

[xssFilter]
public string? banktransactionid{ get; set; }

[xssFilter]
public string? gatewayresponsecode{ get; set; }

[xssFilter]
public string? gatewayresponsemessage{ get; set; }

[xssFilter]
public string? responsesignature{ get; set; }

public decimal? refundedamount{ get; set; }

[xssFilter]
public string? refundreason{ get; set; }
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
			

			public class PaymentResponseModelValidator: AbstractValidator<PaymentResponseModel>
			{
					 
					public PaymentResponseModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Payment_Response", () =>
                                    {
                                        {

RuleFor(m => m.transactiontime)
.NotEmpty().WithMessage("Transaction Time is required")
;
RuleFor(m => m.orderid)
.NotEmpty().WithMessage("Order Id is required")
.MaximumLength(128).WithMessage("The allowed length of Order Id is 128 characters or fewer")
;


RuleFor(m => m.amount)
.LessThanOrEqualTo(99999999).WithMessage("Amount should be LessThanOrEqualTo 99999999")

;





}

                                    });
When(model => model.craftmyapp_actionmethodname == "Refund", () =>
                                    {
                                        {

RuleFor(m => m.transactiontime)
.NotEmpty().WithMessage("Transaction Time is required")
;
RuleFor(m => m.orderid)
.NotEmpty().WithMessage("Order Id is required")
.MaximumLength(128).WithMessage("The allowed length of Order Id is 128 characters or fewer")
;

RuleFor(m => m.refundedamount)
.LessThanOrEqualTo(99999999).WithMessage("Refunded Amount should be LessThanOrEqualTo 99999999")

;

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Payment_Response", () =>
                                    {
                                        {

RuleFor(m => m.transactiontime)
.NotEmpty().WithMessage("Transaction Time is required")
;
RuleFor(m => m.orderid)
.NotEmpty().WithMessage("Order Id is required")
.MaximumLength(128).WithMessage("The allowed length of Order Id is 128 characters or fewer")
;


RuleFor(m => m.amount)
.LessThanOrEqualTo(99999999).WithMessage("Amount should be LessThanOrEqualTo 99999999")

;





}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
