namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:54
			public class MedicalConditionModel
			{

			 public System.Guid ?MedicalConditionid	{ get; set; }

[xssFilter]
public string conditionname{ get; set; }

[xssFilter]
public string? snomedid{ get; set; }

[xssFilter]
public string? description{ get; set; }
public ICollection <MedicalCondition_synonymsModel> synonyms { get; set; }
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
			

			public class MedicalConditionModelValidator: AbstractValidator<MedicalConditionModel>
			{
					 
					public MedicalConditionModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Medical_Condition", () =>
                                    {
                                        {RuleFor(m => m.conditionname)
.NotEmpty().WithMessage("Condition Name is required")
.MaximumLength(128).WithMessage("The allowed length of Condition Name is 128 characters or fewer")
;


}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Medical_Condition", () =>
                                    {
                                        {RuleFor(m => m.conditionname)
.NotEmpty().WithMessage("Condition Name is required")
.MaximumLength(128).WithMessage("The allowed length of Condition Name is 128 characters or fewer")
;


}

                                    });

						 RuleForEach(x => x.synonyms).SetValidator(new MedicalCondition_synonymsModelValidator());

						
					}

			}

                
			 public class MedicalCondition_synonymsModel
			{

			 

public string synonymname{ get; set; }


public string slanguage{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?MedicalCondition_synonymsid { get; set; }
public System.Guid ?MedicalConditionid { get; set; }



			}
			

			public class MedicalCondition_synonymsModelValidator: AbstractValidator<MedicalCondition_synonymsModel>
			{
				   
					public MedicalCondition_synonymsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Medical_Condition", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Medical_Condition", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
