
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_MedicationType"
              (
			  pvar_MedicationTypeid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"MedicationTypeid" uuid
,medicationtypename Varchar
,medicationtypedescription Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:33*/
			  		 
              RETURN QUERY
			  SELECT  
				 MedicationType.tenantid
,tenant.businessname as _tenantname
,MedicationType.MedicationTypeid
,MedicationType.medicationtypename
,MedicationType.medicationtypedescription

				 ,MedicationType.createduser,MedicationType.createddate,MedicationType.modifieduser,MedicationType.modifieddate
                 
                 
				 
			  FROM  MedicationType 
 LEFT OUTER JOIN tenant ON MedicationType.tenantid=tenant.tenantid

			  WHERE CAST(MedicationType.MedicationTypeid AS Varchar)=pvar_MedicationTypeid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

