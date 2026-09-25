namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 13:27:19
			public class AssessmentQuestionModel
			{

			 public System.Guid ?AssessmentQuestionid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string questionnairereferencenumber{ get; set; }

public Guid questioncategory	{ get; set; }

public Guid questionsubcategory	{ get; set; }

[xssFilter]
public string questiontext{ get; set; }

[xssFilter]
public string answertype{ get; set; }

[xssFilter]
public string? optiontext{ get; set; }

[xssFilter]
public string? optionvalue{ get; set; }

[xssFilter]
public string? defaultvalue{ get; set; }

public int? scalerangemin{ get; set; }

public int? scalerangemax{ get; set; }

public bool isrequired	{ get; set; }

public int? scorevalue{ get; set; }
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
			

			public class AssessmentQuestionModelValidator: AbstractValidator<AssessmentQuestionModel>
			{
					 
					public AssessmentQuestionModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Assessment_Question", () =>
                                    {
                                        {RuleFor(m => m.questionnairereferencenumber)
.MaximumLength(256).WithMessage("The allowed length of Questionnaire Reference Number is 256 characters or fewer")
;
RuleFor(m => m.questioncategory)
.NotEmpty().WithMessage("Question Category is required")
;
RuleFor(m => m.questionsubcategory)
.NotEmpty().WithMessage("Question Sub Category is required")
;
RuleFor(m => m.questiontext)
.NotEmpty().WithMessage("Question Text is required")
.MaximumLength(128).WithMessage("The allowed length of Question Text is 128 characters or fewer")
;
RuleFor(m => m.answertype)
.NotEmpty().WithMessage("Answer Type is required")
;



RuleFor(m => m.scalerangemin)
.LessThanOrEqualTo(99999999).WithMessage("Scale Range Min should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.scalerangemax)
.LessThanOrEqualTo(99999999).WithMessage("Scale Range Max should be LessThanOrEqualTo 99999999")

;

RuleFor(m => m.scorevalue)
.LessThanOrEqualTo(99999999).WithMessage("Score Value should be LessThanOrEqualTo 99999999")

;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Assessment_Question", () =>
                                    {
                                        {RuleFor(m => m.questionnairereferencenumber)
.MaximumLength(256).WithMessage("The allowed length of Questionnaire Reference Number is 256 characters or fewer")
;
RuleFor(m => m.questioncategory)
.NotEmpty().WithMessage("Question Category is required")
;
RuleFor(m => m.questionsubcategory)
.NotEmpty().WithMessage("Question Sub Category is required")
;
RuleFor(m => m.questiontext)
.NotEmpty().WithMessage("Question Text is required")
.MaximumLength(128).WithMessage("The allowed length of Question Text is 128 characters or fewer")
;
RuleFor(m => m.answertype)
.NotEmpty().WithMessage("Answer Type is required")
;



RuleFor(m => m.scalerangemin)
.LessThanOrEqualTo(99999999).WithMessage("Scale Range Min should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.scalerangemax)
.LessThanOrEqualTo(99999999).WithMessage("Scale Range Max should be LessThanOrEqualTo 99999999")

;

RuleFor(m => m.scorevalue)
.LessThanOrEqualTo(99999999).WithMessage("Score Value should be LessThanOrEqualTo 99999999")

;
}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
