namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:45
			public class TherapyKitModel
			{

			 public System.Guid ?TherapyKitid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string therapykitname{ get; set; }

[xssFilter]
public string kitprice{ get; set; }
public ICollection <TherapyKit_kititemsModel> kititems { get; set; }
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
			

			public class TherapyKitModelValidator: AbstractValidator<TherapyKitModel>
			{
					 
					public TherapyKitModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Therapy_Kit", () =>
                                    {
                                        {RuleFor(m => m.therapykitname)
.NotEmpty().WithMessage("Therapy Kit Name is required")
.MaximumLength(128).WithMessage("The allowed length of Therapy Kit Name is 128 characters or fewer")
;
RuleFor(m => m.kitprice)
.NotEmpty().WithMessage("Kit Price is required")
.MaximumLength(256).WithMessage("The allowed length of Kit Price is 256 characters or fewer")
;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Therapy_Kit", () =>
                                    {
                                        {RuleFor(m => m.therapykitname)
.NotEmpty().WithMessage("Therapy Kit Name is required")
.MaximumLength(128).WithMessage("The allowed length of Therapy Kit Name is 128 characters or fewer")
;
RuleFor(m => m.kitprice)
.NotEmpty().WithMessage("Kit Price is required")
.MaximumLength(256).WithMessage("The allowed length of Kit Price is 256 characters or fewer")
;
}

                                    });

						 RuleForEach(x => x.kititems).SetValidator(new TherapyKit_kititemsModelValidator());

						
					}

			}

                
			 public class TherapyKit_kititemsModel
			{

			 
public Guid therapyitem	{ get; set; }

public decimal? price{ get; set; }

public int count{ get; set; }


public string linetotal{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?TherapyKit_kititemsid { get; set; }
public System.Guid ?TherapyKitid { get; set; }



			}
			

			public class TherapyKit_kititemsModelValidator: AbstractValidator<TherapyKit_kititemsModel>
			{
				   
					public TherapyKit_kititemsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Therapy_Kit", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Therapy_Kit", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
