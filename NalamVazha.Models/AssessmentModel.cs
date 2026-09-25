namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 16:41:42
			public class AssessmentModel
			{

			 public System.Guid ?Assessmentid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid patientname	{ get; set; }

public Guid? patientvisit	{ get; set; }

[xssFilter]
public string? assessedby{ get; set; }

public Guid? doctorname	{ get; set; }

public Guid? ipdform	{ get; set; }

public Guid? opdform	{ get; set; }

[DataType(DataType.DateTime)][ModelBinder(BinderType = typeof(DateTimeModelBinder))][DisplayFormat(DataFormatString="{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode=true)]public DateTime assessmentdate	{ get; set; }

public Guid questionnairetemplate	{ get; set; }

[xssFilter]
public string? assessmentnotes{ get; set; }
public ICollection <Assessment_assessmentquestionsModel> assessmentquestions { get; set; }
public string? patientanswers_json { get; set; }    // raw JSON collected from patient questionnaire UI
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
		public string? eligibleforfinaladmission { get; set; }

		public string? taskname { get; set; }

		// Audit context used when appending an immutable AssessmentHistory snapshot.
		// These fields are not stored on the current Assessment row.
		public string? historyactiontype { get; set; }
		public string? workflowstage { get; set; }
		public string? actionbyrole { get; set; }
		public string? reviewnotes { get; set; }

		 


	}
			

			public class AssessmentModelValidator: AbstractValidator<AssessmentModel>
			{
					 
					public AssessmentModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Assessment", () =>
                                    {
                                        {RuleFor(m => m.patientname)
.NotEmpty().WithMessage("Patient Name is required")
;





RuleFor(m => m.assessmentdate)
.NotEmpty().WithMessage("Assessment Date is required")
;
RuleFor(m => m.questionnairetemplate)
.NotEmpty().WithMessage("Questionnaire Template is required")
;

}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Assessment", () =>
                                    {
                                        {RuleFor(m => m.patientname)
.NotEmpty().WithMessage("Patient Name is required")
;





RuleFor(m => m.assessmentdate)
.NotEmpty().WithMessage("Assessment Date is required")
;
RuleFor(m => m.questionnairetemplate)
.NotEmpty().WithMessage("Questionnaire Template is required")
;

}

                                    });

						 RuleForEach(x => x.assessmentquestions).SetValidator(new Assessment_assessmentquestionsModelValidator());

						
					}

			}

                

    public class Assessment_patientanswersModel
    {
        public System.Guid? Assessment_patientanswersid { get; set; }
        public System.Guid? Assessmentid               { get; set; }
        public string?      questionsref               { get; set; }
        public System.Guid? questioncategory           { get; set; }
        public System.Guid? questionsub                { get; set; }
        public System.Guid? question                   { get; set; }
        public string?      answertype                 { get; set; }
        public string?      answervalue                { get; set; }
        public int?         record_order               { get; set; }
    }

			 public class Assessment_assessmentquestionsModel
			{

			 

public string? questions{ get; set; }


public string? optiontext{ get; set; }


public string? optionvalue{ get; set; }


public string? defaultvalue{ get; set; }

public Guid? questioncategory	{ get; set; }

public Guid? questionsub	{ get; set; }

public Guid? question	{ get; set; }


public string? answertype{ get; set; }

public int? scalerangemin{ get; set; }

public int? scalerangemax{ get; set; }

public bool isrequired	{ get; set; }

public int? scorevalue{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?Assessment_assessmentquestionsid { get; set; }
public System.Guid ?Assessmentid { get; set; }



			}
			

			public class Assessment_assessmentquestionsModelValidator: AbstractValidator<Assessment_assessmentquestionsModel>
			{
				   
					public Assessment_assessmentquestionsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Assessment", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Assessment", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
