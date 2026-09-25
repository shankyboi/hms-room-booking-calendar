 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_RoomGroup"
			  (
				  pvar_RoomGroupid Varchar
			  )
			  RETURNS TABLE(
                groupname Varchar
,groupnumber Varchar
,groupdesc Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,RoomGroupid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:13*/
               
              RETURN QUERY
			  SELECT 
				 RoomGroup.groupname
,RoomGroup.groupnumber
,RoomGroup.groupdesc

				 ,RoomGroup.createduser,RoomGroup.createddate,RoomGroup.modifieduser,RoomGroup.modifieddate
				 ,RoomGroup.tenantid
                 ,RoomGroup.RoomGroupid
                    
			  FROM RoomGroup
			  WHERE CAST(RoomGroup.RoomGroupid AS Varchar)=pvar_RoomGroupid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

