namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:38:47
			public class TypeofTaskModel
			{

			 public System.Guid ?TypeofTaskid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string? tasktype{ get; set; }

[xssFilter]
public string? description{ get; set; }
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
			

			public class TypeofTaskModelValidator: AbstractValidator<TypeofTaskModel>
			{
					 
					public TypeofTaskModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Type_of_Task", () =>
                                    {
                                        {
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Type_of_Task", () =>
                                    {
                                        {
}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
