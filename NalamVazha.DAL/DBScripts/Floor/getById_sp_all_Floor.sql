
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Floor"
              (
			  pvar_Floorid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Floorid" uuid
,block Varchar
,building Varchar
,floorname Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:10*/
			  		 
              RETURN QUERY
			  SELECT  
				 Floor.tenantid
,tenant.businessname as _tenantname
,Floor.Floorid
,CAST(_Block.blockcode||' '||_Block.blockname AS VARCHAR) as block
,CAST(__Building.buildingcode||' '||__Building.buildingname AS VARCHAR) as building
,Floor.floorname

				 ,Floor.createduser,Floor.createddate,Floor.modifieduser,Floor.modifieddate
                 
                 
				 
			  FROM  Floor 
 LEFT OUTER JOIN tenant ON Floor.tenantid=tenant.tenantid
INNER JOIN Block _Block ON Floor.block=_Block.Blockid
INNER JOIN Building __Building ON Floor.building=__Building.Buildingid

			  WHERE CAST(Floor.Floorid AS Varchar)=pvar_Floorid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

