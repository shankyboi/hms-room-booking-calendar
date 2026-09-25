namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:52
			public class PatientConsentModel
			{

			 public System.Guid ?PatientConsentid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string consenttype{ get; set; }

[xssFilter]
public string consentlanguage{ get; set; }

[xssFilter]
										[Required(ErrorMessage="Consent File is Required")]
               public string consentfile{ get; set; }
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
			

			public class PatientConsentModelValidator: AbstractValidator<PatientConsentModel>
			{
					 
					public PatientConsentModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Patient_Consent", () =>
                                    {
                                        {RuleFor(m => m.consenttype)
.NotEmpty().WithMessage("Consent Type is required")
;
RuleFor(m => m.consentlanguage)
.NotEmpty().WithMessage("Consent Language is required")
;
RuleFor(m => m.consentfile)
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Patient_Consent", () =>
                                    {
                                        {RuleFor(m => m.consenttype)
.NotEmpty().WithMessage("Consent Type is required")
;
RuleFor(m => m.consentlanguage)
.NotEmpty().WithMessage("Consent Language is required")
;
RuleFor(m => m.consentfile)
;
}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
