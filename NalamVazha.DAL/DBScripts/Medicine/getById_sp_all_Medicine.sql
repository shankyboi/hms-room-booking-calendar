
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Medicine"
              (
			  pvar_Medicineid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Medicineid" uuid
,medicationtype Varchar
,medicinename Varchar
,price decimal
,prescriptionrequired Varchar
,sideeffect text
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:36*/
			  		 
              RETURN QUERY
			  SELECT  
				 Medicine.tenantid
,tenant.businessname as _tenantname
,Medicine.Medicineid
,CAST(_MedicationType.medicationtypename AS VARCHAR) as medicationtype
,Medicine.medicinename
,Medicine.price
,CAST(case when Medicine.prescriptionrequired=true then 'Yes' else 'No' End AS Varchar) as prescriptionrequired
,Medicine.sideeffect

				 ,Medicine.createduser,Medicine.createddate,Medicine.modifieduser,Medicine.modifieddate
                 
                 
				 
			  FROM  Medicine 
 LEFT OUTER JOIN tenant ON Medicine.tenantid=tenant.tenantid
INNER JOIN MedicationType _MedicationType ON Medicine.medicationtype=_MedicationType.MedicationTypeid

			  WHERE CAST(Medicine.Medicineid AS Varchar)=pvar_Medicineid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

