
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Building"
              (
			  pvar_Buildingid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Buildingid" uuid
,block Varchar
,buildingcode Varchar
,buildingname Varchar
,buildingdescription text
,buildingimage Varchar
,buildingnearbylandmark Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:07*/
			  		 
              RETURN QUERY
			  SELECT  
				 Building.tenantid
,tenant.businessname as _tenantname
,Building.Buildingid
,CAST(_Block.blockcode||' '||_Block.blockname AS VARCHAR) as block
,Building.buildingcode
,Building.buildingname
,Building.buildingdescription
,Building.buildingimage
,Building.buildingnearbylandmark

				 ,Building.createduser,Building.createddate,Building.modifieduser,Building.modifieddate
                 
                 
				 
			  FROM  Building 
 LEFT OUTER JOIN tenant ON Building.tenantid=tenant.tenantid
INNER JOIN Block _Block ON Building.block=_Block.Blockid

			  WHERE CAST(Building.Buildingid AS Varchar)=pvar_Buildingid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

