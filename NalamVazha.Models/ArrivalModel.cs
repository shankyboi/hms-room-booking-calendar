namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 08:07:23
			public class ArrivalModel
			{

			 public System.Guid ?Arrivalid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid? ipdnumber	{ get; set; }

public Guid? patient	{ get; set; }

public Guid? room	{ get; set; }

public Guid? estimatedarrival	{ get; set; }

public Guid? bookingstatus	{ get; set; }

public Guid? travelarrangement	{ get; set; }

public Guid? pickupfrom	{ get; set; }

public Guid? wheelchairassistance	{ get; set; }

public Guid? requireddinner	{ get; set; }

public Guid? specialrequest	{ get; set; }

public Guid? paymentstatus	{ get; set; }

public Guid? pendingamount	{ get; set; }
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
			

			public class ArrivalModelValidator: AbstractValidator<ArrivalModel>
			{
					 
					public ArrivalModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Arrival", () =>
                                    {
                                        {











}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Arrival", () =>
                                    {
                                        {











}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
