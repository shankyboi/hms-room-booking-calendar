 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Floor"
			  (
				  pvar_Floorid Varchar
			  )
			  RETURNS TABLE(
                block uuid
,building uuid
,floorname Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Floorid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:10*/
               
              RETURN QUERY
			  SELECT 
				 Floor.block
,Floor.building
,Floor.floorname

				 ,Floor.createduser,Floor.createddate,Floor.modifieduser,Floor.modifieddate
				 ,Floor.tenantid
                 ,Floor.Floorid
                    
			  FROM Floor
			  WHERE CAST(Floor.Floorid AS Varchar)=pvar_Floorid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

