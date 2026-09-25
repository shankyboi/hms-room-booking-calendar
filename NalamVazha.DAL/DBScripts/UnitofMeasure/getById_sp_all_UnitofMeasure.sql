
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_UnitofMeasure"
              (
			  pvar_UnitofMeasureid Varchar
			  )
              RETURNS TABLE(
                "UnitofMeasureid" uuid
,unitofmeasurename Varchar
,unitofmeasuredesc Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:42*/
			  		 
              RETURN QUERY
			  SELECT  
				 UnitofMeasure.UnitofMeasureid
,UnitofMeasure.unitofmeasurename
,UnitofMeasure.unitofmeasuredesc

				 ,UnitofMeasure.createduser,UnitofMeasure.createddate,UnitofMeasure.modifieduser,UnitofMeasure.modifieddate
                 
                 
				 
			  FROM  UnitofMeasure 

			  WHERE CAST(UnitofMeasure.UnitofMeasureid AS Varchar)=pvar_UnitofMeasureid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

