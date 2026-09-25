CREATE OR REPLACE FUNCTION "getById_sp_ClinicalAppointment_reshedulehistory"(
												 pvar_ClinicalAppointmentid Varchar(50)
											 )
                                             RETURNS TABLE("ClinicalAppointmentid" uuid,"ClinicalAppointment_reshedulehistoryid" uuid ,resheduleddatetime Timestamp(3)
,reshedulereason Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 ClinicalAppointment_reshedulehistory.ClinicalAppointmentid
                                             ,ClinicalAppointment_reshedulehistory.ClinicalAppointment_reshedulehistoryid   
											 ,ClinicalAppointment_reshedulehistory.resheduleddatetime
,ClinicalAppointment_reshedulehistory.reshedulereason
 
											 
											 FROM ClinicalAppointment_reshedulehistory
											 WHERE 
											 CAST(ClinicalAppointment_reshedulehistory.ClinicalAppointmentid AS VARCHAR)=pvar_ClinicalAppointmentid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

