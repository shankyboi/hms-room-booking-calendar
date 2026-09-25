 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_PatientConsent"
			  (
				  pvar_PatientConsentid Varchar
			  )
			  RETURNS TABLE(
                consenttype Varchar
,consentlanguage Varchar
,consentfile Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,PatientConsentid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:52*/
               
              RETURN QUERY
			  SELECT 
PatientConsent.consenttype
,PatientConsent.consentlanguage
,PatientConsent.consentfile

				 ,PatientConsent.createduser,PatientConsent.createddate,PatientConsent.modifieduser,PatientConsent.modifieddate
				 ,PatientConsent.tenantid
                 ,PatientConsent.PatientConsentid
                    
			  FROM PatientConsent
			  WHERE CAST(PatientConsent.PatientConsentid AS Varchar)=pvar_PatientConsentid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

