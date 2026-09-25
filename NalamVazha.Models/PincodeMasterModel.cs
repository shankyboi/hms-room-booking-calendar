namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:33
			public class PincodeMasterModel
			{

			 public System.Guid ?PincodeMasterid	{ get; set; }

[xssFilter]
public string? circlename{ get; set; }

[xssFilter]
public string? regionname{ get; set; }

[xssFilter]
public string? divisionname{ get; set; }

[xssFilter]
public string? officename{ get; set; }

[xssFilter]
public string? pincode{ get; set; }

[xssFilter]
public string? officetype{ get; set; }

[xssFilter]
public string? delivery{ get; set; }

[xssFilter]
public string? district{ get; set; }

[xssFilter]
public string? statename{ get; set; }

[xssFilter]
public string? latitude{ get; set; }

[xssFilter]
public string? longitude{ get; set; }
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
			

			public class PincodeMasterModelValidator: AbstractValidator<PincodeMasterModel>
			{
					 
					public PincodeMasterModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Pincode", () =>
                                    {
                                        {










}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Pincode", () =>
                                    {
                                        {










}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
