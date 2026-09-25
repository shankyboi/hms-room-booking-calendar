namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 13:35:09
			public class AssessmentTemplateModel
			{

			 public System.Guid ?AssessmentTemplateid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string templatename{ get; set; }

		public string? taskname { get; set; }

		public bool isdefaulttemplate { get; set; }

		
		public ICollection <AssessmentTemplate_templatequestionsModel> templatequestions { get; set; }
public ICollection <AssessmentTemplate_templateapplicabilityModel> templateapplicability { get; set; }
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
			

			public class AssessmentTemplateModelValidator: AbstractValidator<AssessmentTemplateModel>
			{
					 
					public AssessmentTemplateModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Assessment_Template", () =>
                                    {
                                        {RuleFor(m => m.templatename)
.NotEmpty().WithMessage("Template Name is required")
.MaximumLength(128).WithMessage("The allowed length of Template Name is 128 characters or fewer")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Assessment_Template", () =>
                                    {
                                        {RuleFor(m => m.templatename)
.NotEmpty().WithMessage("Template Name is required")
.MaximumLength(128).WithMessage("The allowed length of Template Name is 128 characters or fewer")
;
}

                                    });

						 RuleForEach(x => x.templatequestions).SetValidator(new AssessmentTemplate_templatequestionsModelValidator());
RuleForEach(x => x.templateapplicability).SetValidator(new AssessmentTemplate_templateapplicabilityModelValidator());

						
					}

			}

                
			 public class AssessmentTemplate_templatequestionsModel
			{

			 
public Guid questioncategory	{ get; set; }

public Guid questionsub	{ get; set; }

public Guid question	{ get; set; }


public string? optiontext{ get; set; }


public string? optionvalue{ get; set; }


public string? defaultvalue{ get; set; }


public string? answertype{ get; set; }

public int? scalerangemin{ get; set; }

public int? scalerangemax{ get; set; }

public bool isrequired	{ get; set; }

public int? scorevalue{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?AssessmentTemplate_templatequestionsid { get; set; }
public System.Guid ?AssessmentTemplateid { get; set; }



			}
			

			public class AssessmentTemplate_templatequestionsModelValidator: AbstractValidator<AssessmentTemplate_templatequestionsModel>
			{
				   
					public AssessmentTemplate_templatequestionsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Assessment_Template", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Assessment_Template", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			 public class AssessmentTemplate_templateapplicabilityModel
			{

			 
public Guid? patientcategory	{ get; set; }


public string? gender{ get; set; }


public string? relationshipstatus{ get; set; }

public Guid? medicalcondition	{ get; set; }


public string? agegroup{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?AssessmentTemplate_templateapplicabilityid { get; set; }
public System.Guid ?AssessmentTemplateid { get; set; }



			}
			

			public class AssessmentTemplate_templateapplicabilityModelValidator: AbstractValidator<AssessmentTemplate_templateapplicabilityModel>
			{
				   
					public AssessmentTemplate_templateapplicabilityModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Assessment_Template", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Assessment_Template", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
