namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:59
			public class PatientVisitModel
			{

			 public System.Guid ?PatientVisitid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string visitnumber{ get; set; }

[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode=true)]public DateTime visitdatetime	{ get; set; }

public Guid patientname	{ get; set; }

[xssFilter]
public string visittype{ get; set; }

public Guid? ipdnumber	{ get; set; }

public Guid? opdnumber	{ get; set; }

public Guid? consultingdoctor	{ get; set; }

[xssFilter]
public string? visitstatus{ get; set; }

[xssFilter]
public string? notes{ get; set; }
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
			

			public class PatientVisitModelValidator: AbstractValidator<PatientVisitModel>
			{
					 
					public PatientVisitModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Patient_Visit", () =>
                                    {
                                        {RuleFor(m => m.visitnumber)
.MaximumLength(256).WithMessage("The allowed length of Visit Number is 256 characters or fewer")
;
RuleFor(m => m.visitdatetime)
.NotEmpty().WithMessage("Visit Date Time is required")
;
RuleFor(m => m.patientname)
.NotEmpty().WithMessage("Patient Name is required")
;
RuleFor(m => m.visittype)
.NotEmpty().WithMessage("Visit Type is required")
;





}

                                    });
When(model => model.craftmyapp_actionmethodname == "Change_Status", () =>
                                    {
                                        {

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Patient_Visit", () =>
                                    {
                                        {RuleFor(m => m.visitnumber)
.MaximumLength(256).WithMessage("The allowed length of Visit Number is 256 characters or fewer")
;
RuleFor(m => m.visitdatetime)
.NotEmpty().WithMessage("Visit Date Time is required")
;
RuleFor(m => m.patientname)
.NotEmpty().WithMessage("Patient Name is required")
;
RuleFor(m => m.visittype)
.NotEmpty().WithMessage("Visit Type is required")
;





}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
