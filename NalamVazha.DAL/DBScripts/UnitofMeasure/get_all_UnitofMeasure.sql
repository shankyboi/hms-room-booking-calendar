 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_UnitofMeasure"
              (
			  pvar_tenantid Varchar=null
              )
			  RETURNS TABLE(
                unitofmeasurename Varchar
,unitofmeasuredesc Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"UnitofMeasureid" uuid
               )
               AS $BODY$
               BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:42*/

			 
                RETURN QUERY
                SELECT 
                UnitofMeasure.unitofmeasurename
,UnitofMeasure.unitofmeasuredesc

                ,UnitofMeasure.createduser,UnitofMeasure.createddate,UnitofMeasure.modifieduser,UnitofMeasure.modifieddate
                ,UnitofMeasure.UnitofMeasureid
                FROM UnitofMeasure
			    
                 WHERE UnitofMeasure.isdeleted=false
                ;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

