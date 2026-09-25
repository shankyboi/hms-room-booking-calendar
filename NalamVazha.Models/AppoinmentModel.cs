namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:54:47
			public class AppoinmentModel
			{

			 public System.Guid ?Appoinmentid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid? patient	{ get; set; }

public Guid? origin	{ get; set; }

public Guid? bookingreferencenumber	{ get; set; }

public Guid? doctor	{ get; set; }

public Guid? appointmentdate	{ get; set; }

public Guid? task	{ get; set; }

public Guid? duration	{ get; set; }

public Guid? status	{ get; set; }
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
			

			public class AppoinmentModelValidator: AbstractValidator<AppoinmentModel>
			{
					 
					public AppoinmentModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Appoinment", () =>
                                    {
                                        {







}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Appoinment", () =>
                                    {
                                        {







}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
