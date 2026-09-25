
CREATE OR REPLACE FUNCTION public."getById_sp_OPDForm"(
	pvar_opdformid character varying)
    RETURNS TABLE(bookingreferencenumber character varying, patientname uuid, appointmentmode character varying, preferreddoctor uuid, verifiedstatus character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, tenantid uuid, opdformid uuid, medicalinfo json, medicationinfo json, medicalrecords json, appointmentpreferences json) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:53*/
               
              RETURN QUERY
			  SELECT 
				 OPDForm.bookingreferencenumber
,OPDForm.patientname
,OPDForm.appointmentmode
,OPDForm.preferreddoctor
,OPDForm.verifiedstatus

				 ,OPDForm.createduser,OPDForm.createddate,OPDForm.modifieduser,OPDForm.modifieddate
				 ,OPDForm.tenantid
                 ,OPDForm.OPDFormid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 OPDForm_medicalinfo.OPDFormid
                                             ,OPDForm_medicalinfo.OPDForm_medicalinfoid   
											 ,OPDForm_medicalinfo.medicalconditionname
,OPDForm_medicalinfo.duration
,OPDForm_medicalinfo.unit
,OPDForm_medicalinfo.severitylevel
 
											  
											 FROM OPDForm_medicalinfo
											 WHERE 
											 OPDForm_medicalinfo.OPDFormid=OPDForm.OPDFormid
                                             
                                             ORDER BY record_order DESC
											) J) as medicalinfo
,(SELECT json_agg(J) FROM (
											 SELECT 
											 OPDForm_medicationinfo.OPDFormid
                                             ,OPDForm_medicationinfo.OPDForm_medicationinfoid   
											 ,OPDForm_medicationinfo.medicinename
,OPDForm_medicationinfo.frequencyinaday
,OPDForm_medicationinfo.medicationduration
 
											  
											 FROM OPDForm_medicationinfo
											 WHERE 
											 OPDForm_medicationinfo.OPDFormid=OPDForm.OPDFormid
                                             
                                             ORDER BY record_order DESC
											) J) as medicationinfo
,(SELECT json_agg(J) FROM (
											 SELECT 
											 OPDForm_medicalrecords.OPDFormid
                                             ,OPDForm_medicalrecords.OPDForm_medicalrecordsid   
											 ,OPDForm_medicalrecords.medicalrecordname
,OPDForm_medicalrecords.medicalrecordfile
 
											  
											 FROM OPDForm_medicalrecords
											 WHERE 
											 OPDForm_medicalrecords.OPDFormid=OPDForm.OPDFormid
                                             
                                             ORDER BY record_order DESC
											) J) as medicalrecords
,(SELECT json_agg(J) FROM (
											 SELECT 
											 OPDForm_appointmentpreferences.OPDFormid
                                             ,OPDForm_appointmentpreferences.OPDForm_appointmentpreferencesid   
											 ,OPDForm_appointmentpreferences.preferreddate
,OPDForm_appointmentpreferences.slotpreference
 
											  
											 FROM OPDForm_appointmentpreferences
											 WHERE 
											 OPDForm_appointmentpreferences.OPDFormid=OPDForm.OPDFormid
                                             
                                             ORDER BY record_order ASC
											) J) as appointmentpreferences
   
			  FROM OPDForm
			  WHERE CAST(OPDForm.OPDFormid AS Varchar)=pvar_OPDFormid
                       ;

					 	
			  END
              
$BODY$;
