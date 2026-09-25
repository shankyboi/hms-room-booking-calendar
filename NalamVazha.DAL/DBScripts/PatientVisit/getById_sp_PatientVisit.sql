 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_PatientVisit"
			  (
				  pvar_PatientVisitid Varchar
			  )
			  RETURNS TABLE(
                visitnumber Varchar
,visitdatetime Timestamp(3)
,patientname uuid
,visittype Varchar
,ipdnumber uuid
,opdnumber uuid
,consultingdoctor uuid
,visitstatus Varchar
,notes Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,PatientVisitid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:59*/
               
              RETURN QUERY
			  SELECT 
				 PatientVisit.visitnumber
,PatientVisit.visitdatetime
,PatientVisit.patientname
,PatientVisit.visittype
,PatientVisit.ipdnumber
,PatientVisit.opdnumber
,PatientVisit.consultingdoctor
,PatientVisit.visitstatus
,PatientVisit.notes

				 ,PatientVisit.createduser,PatientVisit.createddate,PatientVisit.modifieduser,PatientVisit.modifieddate
				 ,PatientVisit.tenantid
                 ,PatientVisit.PatientVisitid
                    
			  FROM PatientVisit
			  WHERE CAST(PatientVisit.PatientVisitid AS Varchar)=pvar_PatientVisitid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

