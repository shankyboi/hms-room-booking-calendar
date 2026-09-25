 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Medicine"
			  (
				  pvar_Medicineid Varchar
			  )
			  RETURNS TABLE(
                medicationtype uuid
,medicinename Varchar
,price decimal
,prescriptionrequired Boolean
,sideeffect text
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Medicineid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:36*/
               
              RETURN QUERY
			  SELECT 
				 Medicine.medicationtype
,Medicine.medicinename
,Medicine.price
,COALESCE(Medicine.prescriptionrequired,true) as prescriptionrequired
,Medicine.sideeffect

				 ,Medicine.createduser,Medicine.createddate,Medicine.modifieduser,Medicine.modifieddate
				 ,Medicine.tenantid
                 ,Medicine.Medicineid
                    
			  FROM Medicine
			  WHERE CAST(Medicine.Medicineid AS Varchar)=pvar_Medicineid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

