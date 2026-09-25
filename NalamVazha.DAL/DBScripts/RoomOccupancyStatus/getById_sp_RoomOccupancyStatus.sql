 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_RoomOccupancyStatus"
			  (
				  pvar_RoomOccupancyStatusid Varchar
			  )
			  RETURNS TABLE(
                roomallocationno Varchar
,patientvisit uuid
,patientname uuid
,ipdno uuid
,block uuid
,building uuid
,floor uuid
,room uuid
,bookeddate date
,status Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,RoomOccupancyStatusid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:03*/
               
              RETURN QUERY
			  SELECT 
				 RoomOccupancyStatus.roomallocationno
,RoomOccupancyStatus.patientvisit
,RoomOccupancyStatus.patientname
,RoomOccupancyStatus.ipdno
,RoomOccupancyStatus.block
,RoomOccupancyStatus.building
,RoomOccupancyStatus.floor
,RoomOccupancyStatus.room
,RoomOccupancyStatus.bookeddate
,RoomOccupancyStatus.status

				 ,RoomOccupancyStatus.createduser,RoomOccupancyStatus.createddate,RoomOccupancyStatus.modifieduser,RoomOccupancyStatus.modifieddate
				 ,RoomOccupancyStatus.tenantid
                 ,RoomOccupancyStatus.RoomOccupancyStatusid
                    
			  FROM RoomOccupancyStatus
			  WHERE CAST(RoomOccupancyStatus.RoomOccupancyStatusid AS Varchar)=pvar_RoomOccupancyStatusid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

