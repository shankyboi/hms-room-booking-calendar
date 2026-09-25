namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:45
			public class otplogsModel
			{

			 public System.Guid ?otplogsid	{ get; set; }

[xssFilter]
public string? username{ get; set; }

public int? otpcode{ get; set; }

[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode=true)]public DateTime? expirytime	{ get; set; }

public bool isused	{ get; set; }
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
			

			public class otplogsModelValidator: AbstractValidator<otplogsModel>
			{
					 
					public otplogsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_otplogs", () =>
                                    {
                                        {
RuleFor(m => m.otpcode)
.LessThanOrEqualTo(99999999).WithMessage("otpcode should be LessThanOrEqualTo 99999999")

;


}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_otplogs", () =>
                                    {
                                        {
RuleFor(m => m.otpcode)
.LessThanOrEqualTo(99999999).WithMessage("otpcode should be LessThanOrEqualTo 99999999")

;


}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
