CREATE OR REPLACE FUNCTION "getById_sp_GroupBooking_patientinfo"(
												 pvar_GroupBookingid Varchar(50)
											 )
                                             RETURNS TABLE("GroupBookingid" uuid,"GroupBooking_patientinfoid" uuid ,name Varchar
,emailid Varchar
,phonenumber Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 GroupBooking_patientinfo.GroupBookingid
                                             ,GroupBooking_patientinfo.GroupBooking_patientinfoid   
											 ,GroupBooking_patientinfo.name
,GroupBooking_patientinfo.emailid
,GroupBooking_patientinfo.phonenumber
 
											 
											 FROM GroupBooking_patientinfo
											 WHERE 
											 CAST(GroupBooking_patientinfo.GroupBookingid AS VARCHAR)=pvar_GroupBookingid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

