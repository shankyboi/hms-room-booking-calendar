namespace NalamVazha.Models{
			using System;
			using System.ComponentModel.DataAnnotations;
			using Microsoft.AspNetCore.Mvc;
			using System.Collections.Generic;
			using FluentValidation;
			using System.Linq;
			//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 06/18/2026 05:34:25
			public class GroupBookingModel
			{

			 public System.Guid ?GroupBookingid	{ get; set; }
public System.Guid ?tenantid { get; set; }
public String viewertenantids { get; set; }

[xssFilter]
public string groupcode{ get; set; }

[xssFilter]
public string groupname{ get; set; }

public int? countofmembers{ get; set; }
public ICollection <GroupBooking_patientinfoModel> patientinfo { get; set; }
public ICollection <GroupBooking_contactsModel> contacts { get; set; }
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
			

			public class GroupBookingModelValidator: AbstractValidator<GroupBookingModel>
			{
					 
					public GroupBookingModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Group_Booking", () =>
                                    {
                                        {RuleFor(m => m.groupcode)
.NotEmpty().WithMessage("Group Code is required")
.MaximumLength(128).WithMessage("The allowed length of Group Code is 128 characters or fewer")
;
RuleFor(m => m.groupname)
.NotEmpty().WithMessage("Group Name is required")
.MaximumLength(128).WithMessage("The allowed length of Group Name is 128 characters or fewer")
;
RuleFor(m => m.countofmembers)
.LessThanOrEqualTo(99999999).WithMessage("Count of Members should be LessThanOrEqualTo 99999999")

;
}

                                    });
When(model => model.craftmyapp_actionmethodname == "Update_Group_Booking", () =>
                                    {
                                        {RuleFor(m => m.groupcode)
.NotEmpty().WithMessage("Group Code is required")
.MaximumLength(128).WithMessage("The allowed length of Group Code is 128 characters or fewer")
;
RuleFor(m => m.groupname)
.NotEmpty().WithMessage("Group Name is required")
.MaximumLength(128).WithMessage("The allowed length of Group Name is 128 characters or fewer")
;
RuleFor(m => m.countofmembers)
.LessThanOrEqualTo(99999999).WithMessage("Count of Members should be LessThanOrEqualTo 99999999")

;
}

                                    });

						 RuleForEach(x => x.patientinfo).SetValidator(new GroupBooking_patientinfoModelValidator());
RuleForEach(x => x.contacts).SetValidator(new GroupBooking_contactsModelValidator());

						
					}

			}

                
			 public class GroupBooking_patientinfoModel
			{

			 

public string name{ get; set; }


public string emailid{ get; set; }


public string phonenumber{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?GroupBooking_patientinfoid { get; set; }
public System.Guid ?GroupBookingid { get; set; }



			}
			

			public class GroupBooking_patientinfoModelValidator: AbstractValidator<GroupBooking_patientinfoModel>
			{
				   
					public GroupBooking_patientinfoModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Group_Booking", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Group_Booking", () =>
                                                {
                                                    {}

                                                });

						
					}

			}



			 public class GroupBooking_contactsModel
			{

			 

public string person{ get; set; }


public string mobile{ get; set; }


public string? email{ get; set; }
public string craftmyapp_actionmethodname{ get; set; }
public string cma_client_row_id{ get; set; }
public int? record_order{ get; set; }
public System.Guid ?GroupBooking_contactsid { get; set; }
public System.Guid ?GroupBookingid { get; set; }



			}
			

			public class GroupBooking_contactsModelValidator: AbstractValidator<GroupBooking_contactsModel>
			{
				   
					public GroupBooking_contactsModelValidator()
					{

						 When(model => model.craftmyapp_actionmethodname == "Add_Group_Booking", () =>
                                                {
                                                    {}

                                                });
When(model => model.craftmyapp_actionmethodname == "Update_Group_Booking", () =>
                                                {
                                                    {}

                                                });

						
					}

			}




                

                
 

                

                

        

			}
