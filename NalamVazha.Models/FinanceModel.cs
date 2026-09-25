namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 10:01:30
			public class FinanceModel
			{

			 public System.Guid ?Financeid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid? paymentdate	{ get; set; }

public Guid? paymentmode	{ get; set; }

public Guid? receiptnumber	{ get; set; }

public Guid? patient	{ get; set; }

public Guid? receivablefor	{ get; set; }

public Guid? bookingreferencenumber	{ get; set; }

public Guid? billedamount	{ get; set; }

public Guid? receivedamount	{ get; set; }

public int? pendingamount{ get; set; }

public Guid? paymentstatus	{ get; set; }

public Guid? collectedby	{ get; set; }

public Guid? refundmode	{ get; set; }

public Guid? refundedamount	{ get; set; }

public Guid? refundedby	{ get; set; }

public Guid? remarks	{ get; set; }
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
			

			public class FinanceModelValidator: AbstractValidator<FinanceModel>
			{
					 
					public FinanceModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Finance", () =>
                                    {
                                        {







RuleFor(m => m.pendingamount)
.LessThanOrEqualTo(99999999).WithMessage("Pending Amount should be LessThanOrEqualTo 99999999")

;






}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Finance", () =>
                                    {
                                        {







RuleFor(m => m.pendingamount)
.LessThanOrEqualTo(99999999).WithMessage("Pending Amount should be LessThanOrEqualTo 99999999")

;






}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
