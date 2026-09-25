namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:16
			public class PaymentConfigModel
			{

			 public System.Guid ?PaymentConfigid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string paymentgatewayprovider{ get; set; }
public ICollection <PaymentConfig_keyinfoModel> keyinfo { get; set; }
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
			

			public class PaymentConfigModelValidator: AbstractValidator<PaymentConfigModel>
			{
					 
					public PaymentConfigModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Payment_Config", () =>
                                    {
                                        {RuleFor(m => m.paymentgatewayprovider)
.NotEmpty().WithMessage("Payment Gateway Provider is required")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Payment_Config", () =>
                                    {
                                        {RuleFor(m => m.paymentgatewayprovider)
.NotEmpty().WithMessage("Payment Gateway Provider is required")
;
}

                                    });

						 RuleForEach(x => x.keyinfo).SetValidator(new PaymentConfig_keyinfoModelValidator());

						
					}

			}

                
			 public class PaymentConfig_keyinfoModel
			{

			 

public string keytype{ get; set; }


public string keyid{ get; set; }


public string keysecret{ get; set; }


public string returnurl{ get; set; }


public string notifyurl{ get; set; }


public string merchantid{ get; set; }


public string status{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?PaymentConfig_keyinfoid { get; set; }
public System.Guid ?PaymentConfigid { get; set; }



			}
			

			public class PaymentConfig_keyinfoModelValidator: AbstractValidator<PaymentConfig_keyinfoModel>
			{
				   
					public PaymentConfig_keyinfoModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Payment_Config", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Payment_Config", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
