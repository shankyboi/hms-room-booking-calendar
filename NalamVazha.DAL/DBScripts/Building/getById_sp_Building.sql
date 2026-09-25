 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Building"
			  (
				  pvar_Buildingid Varchar
			  )
			  RETURNS TABLE(
                block uuid
,buildingcode Varchar
,buildingname Varchar
,buildingdescription text
,buildingimage Varchar
,buildingnearbylandmark Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Buildingid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:07*/
               
              RETURN QUERY
			  SELECT 
				 Building.block
,Building.buildingcode
,Building.buildingname
,Building.buildingdescription
,Building.buildingimage
,Building.buildingnearbylandmark

				 ,Building.createduser,Building.createddate,Building.modifieduser,Building.modifieddate
				 ,Building.tenantid
                 ,Building.Buildingid
                    
			  FROM Building
			  WHERE CAST(Building.Buildingid AS Varchar)=pvar_Buildingid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

