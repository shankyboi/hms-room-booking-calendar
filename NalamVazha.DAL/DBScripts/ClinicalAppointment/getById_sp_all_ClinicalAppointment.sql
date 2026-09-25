-- FUNCTION: public.getById_sp_all_ClinicalAppointment(character varying)

-- DROP FUNCTION IF EXISTS public."getById_sp_all_ClinicalAppointment"(character varying);

CREATE OR REPLACE FUNCTION public."getById_sp_all_ClinicalAppointment"(
	pvar_clinicalappointmentid character varying)
    RETURNS TABLE(tenantid uuid, _tenantname character varying, "ClinicalAppointmentid" uuid, tasktype character varying, patient character varying, practitioner character varying, photo character varying, actualpractitioner character varying, appointmentdate character varying, durationfrom character varying, durationto character varying, status character varying, origin character varying, bookingid character varying, tokennumber character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, "automaton_ClinicalAppointment_reshedulehistory" json) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:33*/
			  		 
              RETURN QUERY
			  SELECT  
				 ClinicalAppointment.tenantid
,tenant.businessname as _tenantname
,ClinicalAppointment.ClinicalAppointmentid
,ClinicalAppointment.tasktype
,CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''))), '') AS VARCHAR) as patient
,CAST(NULLIF(BTRIM(CONCAT_WS(' ', COALESCE(NULLIF(__People.firstname, ''), ''), COALESCE(NULLIF(__People.lastname, ''), ''))), '') AS VARCHAR) AS practitioner
,ClinicalAppointment.photo
,CAST(NULLIF(btrim(concat_ws(' ', NULLIF(___People.firstname, ''), NULLIF(___People.lastname, ''))), '') AS VARCHAR) as actualpractitioner
,CAST(COALESCE(to_char(ClinicalAppointment.appointmentdate,'dd/MM/yyyy'),'') AS Varchar) as appointmentdate
,ClinicalAppointment.durationfrom
,ClinicalAppointment.durationto
,ClinicalAppointment.status
,ClinicalAppointment.origin
,ClinicalAppointment.bookingid
,ClinicalAppointment.tokennumber

				 ,ClinicalAppointment.createduser,ClinicalAppointment.createddate,ClinicalAppointment.modifieduser,ClinicalAppointment.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						to_char(ClinicalAppointment_reshedulehistory.resheduleddatetime, 'dd/MM/yyyy HH24:MI') as "Resheduled Date Time"
,ClinicalAppointment_reshedulehistory.reshedulereason as "Reshedule Reason"

							
						FROM  ClinicalAppointment_reshedulehistory 

						WHERE ClinicalAppointment.ClinicalAppointmentid =ClinicalAppointment_reshedulehistory.ClinicalAppointmentid
) J)
						as automaton_ClinicalAppointment_reshedulehistory

                 
				 
			  FROM  ClinicalAppointment 
 LEFT OUTER JOIN tenant ON ClinicalAppointment.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON ClinicalAppointment.patient=_PatientProfile.PatientProfileid
LEFT OUTER JOIN People __People ON ClinicalAppointment.practitioner=__People.Peopleid
LEFT OUTER JOIN People ___People ON ClinicalAppointment.actualpractitioner=___People.Peopleid

			  WHERE CAST(ClinicalAppointment.ClinicalAppointmentid AS Varchar)=pvar_ClinicalAppointmentid ;
			  
					 	
			  END
              
$BODY$;

ALTER FUNCTION public."getById_sp_all_ClinicalAppointment"(character varying)
    OWNER TO md_nalamvazha;
