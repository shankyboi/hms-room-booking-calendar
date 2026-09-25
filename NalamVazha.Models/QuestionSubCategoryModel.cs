namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 13:14:22
			public class QuestionSubCategoryModel
			{

			 public System.Guid ?QuestionSubCategoryid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid questioncategoryname	{ get; set; }

[xssFilter]
public string questionsubcategoryname{ get; set; }

[xssFilter]
public string? questionsubcategorydesc{ get; set; }
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
			

			public class QuestionSubCategoryModelValidator: AbstractValidator<QuestionSubCategoryModel>
			{
					 
					public QuestionSubCategoryModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Question_Sub_Category", () =>
                                    {
                                        {RuleFor(m => m.questioncategoryname)
.NotEmpty().WithMessage("Question Category Name is required")
;
RuleFor(m => m.questionsubcategoryname)
.NotEmpty().WithMessage("Question Sub Category Name is required")
.MaximumLength(128).WithMessage("The allowed length of Question Sub Category Name is 128 characters or fewer")
;

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Question_Sub_Category", () =>
                                    {
                                        {RuleFor(m => m.questioncategoryname)
.NotEmpty().WithMessage("Question Category Name is required")
;
RuleFor(m => m.questionsubcategoryname)
.NotEmpty().WithMessage("Question Sub Category Name is required")
.MaximumLength(128).WithMessage("The allowed length of Question Sub Category Name is 128 characters or fewer")
;

}

                                    });

						 
						
					}

			}

                

                

                
 

                

                

        

			}
