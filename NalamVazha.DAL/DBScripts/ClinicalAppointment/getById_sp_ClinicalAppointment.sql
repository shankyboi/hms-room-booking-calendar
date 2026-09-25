 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_ClinicalAppointment"
			  (
				  pvar_ClinicalAppointmentid Varchar
			  )
			  RETURNS TABLE(
                tasktype Varchar
,patient uuid
,practitioner uuid
,photo Varchar
,actualpractitioner uuid
,appointmentdate date
,durationfrom Varchar
,durationto Varchar
,status Varchar
,origin Varchar
,bookingid Varchar
,tokennumber Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,ClinicalAppointmentid uuid
                ,reshedulehistory JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:33*/
               
              RETURN QUERY
			  SELECT 
				 ClinicalAppointment.tasktype
,ClinicalAppointment.patient
,ClinicalAppointment.practitioner
,ClinicalAppointment.photo
,ClinicalAppointment.actualpractitioner
,ClinicalAppointment.appointmentdate
,ClinicalAppointment.durationfrom
,ClinicalAppointment.durationto
,ClinicalAppointment.status
,ClinicalAppointment.origin
,ClinicalAppointment.bookingid
,ClinicalAppointment.tokennumber

				 ,ClinicalAppointment.createduser,ClinicalAppointment.createddate,ClinicalAppointment.modifieduser,ClinicalAppointment.modifieddate
				 ,ClinicalAppointment.tenantid
                 ,ClinicalAppointment.ClinicalAppointmentid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 ClinicalAppointment_reshedulehistory.ClinicalAppointmentid
                                             ,ClinicalAppointment_reshedulehistory.ClinicalAppointment_reshedulehistoryid   
											 ,ClinicalAppointment_reshedulehistory.resheduleddatetime
,ClinicalAppointment_reshedulehistory.reshedulereason
 
											  
											 FROM ClinicalAppointment_reshedulehistory
											 WHERE 
											 ClinicalAppointment_reshedulehistory.ClinicalAppointmentid=ClinicalAppointment.ClinicalAppointmentid
                                             
                                             ORDER BY record_order DESC
											) J) as reshedulehistory
   
			  FROM ClinicalAppointment
			  WHERE CAST(ClinicalAppointment.ClinicalAppointmentid AS Varchar)=pvar_ClinicalAppointmentid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

