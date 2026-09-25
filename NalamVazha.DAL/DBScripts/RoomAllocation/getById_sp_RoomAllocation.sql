 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_RoomAllocation"
			  (
				  pvar_RoomAllocationid Varchar
			  )
			  RETURNS TABLE(
                roomallocationno Varchar
,ipdno uuid
,block uuid
,building uuid
,floor uuid
,room uuid
,fromdate date
,todate date
,status Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,RoomAllocationid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:10*/
               
              RETURN QUERY
			  SELECT 
				 RoomAllocation.roomallocationno
,RoomAllocation.ipdno
,RoomAllocation.block
,RoomAllocation.building
,RoomAllocation.floor
,RoomAllocation.room
,RoomAllocation.fromdate
,RoomAllocation.todate
,RoomAllocation.status

				 ,RoomAllocation.createduser,RoomAllocation.createddate,RoomAllocation.modifieduser,RoomAllocation.modifieddate
				 ,RoomAllocation.tenantid
                 ,RoomAllocation.RoomAllocationid
                    
			  FROM RoomAllocation
			  WHERE CAST(RoomAllocation.RoomAllocationid AS Varchar)=pvar_RoomAllocationid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

