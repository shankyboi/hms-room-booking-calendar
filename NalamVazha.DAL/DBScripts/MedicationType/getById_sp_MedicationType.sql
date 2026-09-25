 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_MedicationType"
			  (
				  pvar_MedicationTypeid Varchar
			  )
			  RETURNS TABLE(
                medicationtypename Varchar
,medicationtypedescription Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,MedicationTypeid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:33*/
               
              RETURN QUERY
			  SELECT 
				 MedicationType.medicationtypename
,MedicationType.medicationtypedescription

				 ,MedicationType.createduser,MedicationType.createddate,MedicationType.modifieduser,MedicationType.modifieddate
				 ,MedicationType.tenantid
                 ,MedicationType.MedicationTypeid
                    
			  FROM MedicationType
			  WHERE CAST(MedicationType.MedicationTypeid AS Varchar)=pvar_MedicationTypeid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

