namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/30/2026 13:05:38
			public class ProcessSLAModel
			{

			 public System.Guid ?ProcessSLAid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string processname{ get; set; }
public ICollection <ProcessSLA_processslaModel> processsla { get; set; }
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
			

			public class ProcessSLAModelValidator: AbstractValidator<ProcessSLAModel>
			{
					 
					public ProcessSLAModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Process_SLA", () =>
                                    {
                                        {RuleFor(m => m.processname)
.NotEmpty().WithMessage("Process Name is required")
.MaximumLength(128).WithMessage("The allowed length of Process Name is 128 characters or fewer")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Process_SLA", () =>
                                    {
                                        {RuleFor(m => m.processname)
.NotEmpty().WithMessage("Process Name is required")
.MaximumLength(128).WithMessage("The allowed length of Process Name is 128 characters or fewer")
;
}

                                    });

						 RuleForEach(x => x.processsla).SetValidator(new ProcessSLA_processslaModelValidator());

						
					}

			}

                
			 public class ProcessSLA_processslaModel
			{

			 

public string initialstage{ get; set; }


public string targetstage{ get; set; }

public int slaindays{ get; set; }

public decimal slainhrs{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?ProcessSLA_processslaid { get; set; }
public System.Guid ?ProcessSLAid { get; set; }



			}
			

			public class ProcessSLA_processslaModelValidator: AbstractValidator<ProcessSLA_processslaModel>
			{
				   
					public ProcessSLA_processslaModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Process_SLA", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Process_SLA", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
