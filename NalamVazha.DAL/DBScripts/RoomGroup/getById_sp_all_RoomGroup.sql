
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_RoomGroup"
              (
			  pvar_RoomGroupid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"RoomGroupid" uuid
,groupname Varchar
,groupnumber Varchar
,groupdesc Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:13*/
			  		 
              RETURN QUERY
			  SELECT  
				 RoomGroup.tenantid
,tenant.businessname as _tenantname
,RoomGroup.RoomGroupid
,RoomGroup.groupname
,RoomGroup.groupnumber
,RoomGroup.groupdesc

				 ,RoomGroup.createduser,RoomGroup.createddate,RoomGroup.modifieduser,RoomGroup.modifieddate
                 
                 
				 
			  FROM  RoomGroup 
 LEFT OUTER JOIN tenant ON RoomGroup.tenantid=tenant.tenantid

			  WHERE CAST(RoomGroup.RoomGroupid AS Varchar)=pvar_RoomGroupid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

