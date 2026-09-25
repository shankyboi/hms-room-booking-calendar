namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 14:00:31
			public class PeopleTimePreferenceModel
			{

			 public System.Guid ?PeopleTimePreferenceid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

public Guid shiftname	{ get; set; }
		public Guid workprofile { get; set; }
		public Guid people	{ get; set; }
public ICollection <PeopleTimePreference_timepreferenceModel> timepreference { get; set; }
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
			

			public class PeopleTimePreferenceModelValidator: AbstractValidator<PeopleTimePreferenceModel>
			{
					 
					public PeopleTimePreferenceModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_People_Time_Preference", () =>
                                    {
                                        {RuleFor(m => m.shiftname)
.NotEmpty().WithMessage("Shift Name is required")
;
											RuleFor(m => m.workprofile)
.NotEmpty().WithMessage("Work Profile is required")
;
											RuleFor(m => m.people)
.NotEmpty().WithMessage("People is required")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_People_Time_Preference", () =>
                                    {
                                        {RuleFor(m => m.shiftname)
.NotEmpty().WithMessage("Shift Name is required")
;
											RuleFor(m => m.workprofile)
.NotEmpty().WithMessage("Work Profile is required")
;
											RuleFor(m => m.people)
.NotEmpty().WithMessage("People is required")
;
}

                                    });

						 RuleForEach(x => x.timepreference).SetValidator(new PeopleTimePreference_timepreferenceModelValidator());

						
					}

			}

                
			 public class PeopleTimePreference_timepreferenceModel
			{

			 

public string clinicaltask{ get; set; }

public Guid? tasktype	{ get; set; }

public Guid? taskname	{ get; set; }


public string? availableon{ get; set; }


public string taskstarttime{ get; set; }


public string taskendtime{ get; set; }

public decimal taskhours{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?PeopleTimePreference_timepreferenceid { get; set; }
public System.Guid ?PeopleTimePreferenceid { get; set; }



			}
			

			public class PeopleTimePreference_timepreferenceModelValidator: AbstractValidator<PeopleTimePreference_timepreferenceModel>
			{
				   
					public PeopleTimePreference_timepreferenceModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_People_Time_Preference", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_People_Time_Preference", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
