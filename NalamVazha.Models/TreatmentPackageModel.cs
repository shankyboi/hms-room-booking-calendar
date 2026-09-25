namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:49
			public class TreatmentPackageModel
			{

			 public System.Guid ?TreatmentPackageid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string packagename{ get; set; }

public int noofdays{ get; set; }

[xssFilter]
public string? roomtypeamountaverage{ get; set; }

[xssFilter]
public string? therapyamount{ get; set; }

[xssFilter]
public string? therapykitamount{ get; set; }

[xssFilter]
public string? therapyitemamount{ get; set; }

[xssFilter]
public string? medicineamount{ get; set; }

[xssFilter]
public string calculatedpackagecost{ get; set; }

public decimal packagecost{ get; set; }

public decimal packagebookingdeposit{ get; set; }

public decimal packagebookingadvance{ get; set; }

[xssFilter]
public string billingwaiverfordelayedstart{ get; set; }

public decimal? waiverpercentage{ get; set; }

[xssFilter]
public string? roomtransfercost{ get; set; }

[xssFilter]
public string? packagedescription{ get; set; }
public ICollection <TreatmentPackage_roomtypesModel> roomtypes { get; set; }
public ICollection <TreatmentPackage_therapyModel> therapy { get; set; }
public ICollection <TreatmentPackage_therapykitsModel> therapykits { get; set; }
public ICollection <TreatmentPackage_therapyitemsModel> therapyitems { get; set; }
public ICollection <TreatmentPackage_medicinesModel> medicines { get; set; }
public ICollection <TreatmentPackage_refundpolicyModel> refundpolicy { get; set; }
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
			

			public class TreatmentPackageModelValidator: AbstractValidator<TreatmentPackageModel>
			{
					 
					public TreatmentPackageModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Treatment_Package", () =>
                                    {
                                        {RuleFor(m => m.packagename)
.NotEmpty().WithMessage("Package Name is required")
.MaximumLength(128).WithMessage("The allowed length of Package Name is 128 characters or fewer")
;
RuleFor(m => m.noofdays)
.NotNull().WithMessage("No of Days is required")
.LessThanOrEqualTo(99999999).WithMessage("No of Days should be LessThanOrEqualTo 99999999")

;





RuleFor(m => m.calculatedpackagecost)
.NotEmpty().WithMessage("Calculated Package Cost is required")
.MaximumLength(256).WithMessage("The allowed length of Calculated Package Cost is 256 characters or fewer")
;
RuleFor(m => m.packagecost)
.NotNull().WithMessage("Package Cost is required")
 

;
RuleFor(m => m.packagebookingdeposit)
.NotNull().WithMessage("Booking Deposit is required")
.LessThanOrEqualTo(99999999).WithMessage("Booking Deposit should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.packagebookingadvance)
.NotNull().WithMessage("Booking Advance is required")
.LessThanOrEqualTo(99999999).WithMessage("Booking Advance should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.billingwaiverfordelayedstart)
.NotEmpty().WithMessage("billingwaiverfordelayedstart is required")
;
RuleFor(m => m.waiverpercentage)
.LessThanOrEqualTo(100).WithMessage("Waiver Percentage should be LessThanOrEqualTo 100")

;


}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Treatment_Package", () =>
                                    {
                                        {RuleFor(m => m.packagename)
.NotEmpty().WithMessage("Package Name is required")
.MaximumLength(128).WithMessage("The allowed length of Package Name is 128 characters or fewer")
;
RuleFor(m => m.noofdays)
.NotNull().WithMessage("No of Days is required")
.LessThanOrEqualTo(99999999).WithMessage("No of Days should be LessThanOrEqualTo 99999999")

;





RuleFor(m => m.calculatedpackagecost)
.NotEmpty().WithMessage("Calculated Package Cost is required")
.MaximumLength(256).WithMessage("The allowed length of Calculated Package Cost is 256 characters or fewer")
;
RuleFor(m => m.packagecost)
.NotNull().WithMessage("Package Cost is required")
 
;
RuleFor(m => m.packagebookingdeposit)
.NotNull().WithMessage("Booking Deposit is required")
.LessThanOrEqualTo(99999999).WithMessage("Booking Deposit should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.packagebookingadvance)
.NotNull().WithMessage("Booking Advance is required")
.LessThanOrEqualTo(99999999).WithMessage("Booking Advance should be LessThanOrEqualTo 99999999")

;
RuleFor(m => m.billingwaiverfordelayedstart)
.NotEmpty().WithMessage("billingwaiverfordelayedstart is required")
;
RuleFor(m => m.waiverpercentage)
.LessThanOrEqualTo(100).WithMessage("Waiver Percentage should be LessThanOrEqualTo 100")

;


}

                                    });

						 RuleForEach(x => x.roomtypes).SetValidator(new TreatmentPackage_roomtypesModelValidator());
RuleForEach(x => x.therapy).SetValidator(new TreatmentPackage_therapyModelValidator());
RuleForEach(x => x.therapykits).SetValidator(new TreatmentPackage_therapykitsModelValidator());
RuleForEach(x => x.therapyitems).SetValidator(new TreatmentPackage_therapyitemsModelValidator());
RuleForEach(x => x.medicines).SetValidator(new TreatmentPackage_medicinesModelValidator());
RuleForEach(x => x.refundpolicy).SetValidator(new TreatmentPackage_refundpolicyModelValidator());

						
					}

			}

                
			 public class TreatmentPackage_roomtypesModel
			{

			 
public Guid roomtype	{ get; set; }

public decimal? costperday{ get; set; }

public decimal percentagecovered{ get; set; }


public string roomcost{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?TreatmentPackage_roomtypesid { get; set; }
public System.Guid ?TreatmentPackageid { get; set; }



			}
			

			public class TreatmentPackage_roomtypesModelValidator: AbstractValidator<TreatmentPackage_roomtypesModel>
			{
				   
					public TreatmentPackage_roomtypesModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Treatment_Package", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Treatment_Package", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			 public class TreatmentPackage_therapyModel
			{

			 
public Guid therapyname	{ get; set; }

public decimal? therapycost{ get; set; }

public int numberoftimes{ get; set; }


public string therapyprice{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?TreatmentPackage_therapyid { get; set; }
public System.Guid ?TreatmentPackageid { get; set; }



			}
			

			public class TreatmentPackage_therapyModelValidator: AbstractValidator<TreatmentPackage_therapyModel>
			{
				   
					public TreatmentPackage_therapyModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Treatment_Package", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Treatment_Package", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			 public class TreatmentPackage_therapykitsModel
			{

			 
public Guid therapykitname	{ get; set; }


public string? kitprice{ get; set; }

public int numberofkits{ get; set; }


public string therapykitcost{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?TreatmentPackage_therapykitsid { get; set; }
public System.Guid ?TreatmentPackageid { get; set; }



			}
			

			public class TreatmentPackage_therapykitsModelValidator: AbstractValidator<TreatmentPackage_therapykitsModel>
			{
				   
					public TreatmentPackage_therapykitsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Treatment_Package", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Treatment_Package", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			 public class TreatmentPackage_therapyitemsModel
			{

			 
public Guid therapyitem	{ get; set; }

public decimal? price{ get; set; }

public int therapyitemcount{ get; set; }


public string therapyitemcost{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?TreatmentPackage_therapyitemsid { get; set; }
public System.Guid ?TreatmentPackageid { get; set; }



			}
			

			public class TreatmentPackage_therapyitemsModelValidator: AbstractValidator<TreatmentPackage_therapyitemsModel>
			{
				   
					public TreatmentPackage_therapyitemsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Treatment_Package", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Treatment_Package", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			 public class TreatmentPackage_medicinesModel
			{

			 
public Guid medicinename	{ get; set; }

public decimal? price{ get; set; }

public int medicinecount{ get; set; }


public string medicinecost{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?TreatmentPackage_medicinesid { get; set; }
public System.Guid ?TreatmentPackageid { get; set; }



			}
			

			public class TreatmentPackage_medicinesModelValidator: AbstractValidator<TreatmentPackage_medicinesModel>
			{
				   
					public TreatmentPackage_medicinesModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Treatment_Package", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Treatment_Package", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			 public class TreatmentPackage_refundpolicyModel
			{

			 

public string refundtype{ get; set; }


public string cancellationby{ get; set; }

public int cancellationwindowdays{ get; set; }

public decimal refundpercentage{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?TreatmentPackage_refundpolicyid { get; set; }
public System.Guid ?TreatmentPackageid { get; set; }



			}
			

			public class TreatmentPackage_refundpolicyModelValidator: AbstractValidator<TreatmentPackage_refundpolicyModel>
			{
				   
					public TreatmentPackage_refundpolicyModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Treatment_Package", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Treatment_Package", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
