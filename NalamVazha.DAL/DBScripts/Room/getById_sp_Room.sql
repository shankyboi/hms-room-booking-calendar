 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Room"
			  (
				  pvar_Roomid Varchar
			  )
			  RETURNS TABLE(
                roomcode Varchar
,block uuid
,building uuid
,floor uuid
,roomtype uuid
,bookingdeposit decimal
,roomgroup uuid
,roomnumber Varchar
,roomimage Varchar
,occupancystatus Varchar
,maintenancestatus Varchar
,housekeepingstatus Varchar
,nextdaycheckin Varchar
,nextdaycheckout Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Roomid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:22*/
               
              RETURN QUERY
			  SELECT 
				 Room.roomcode
,Room.block
,Room.building
,Room.floor
,Room.roomtype
,Room.bookingdeposit
,Room.roomgroup
,Room.roomnumber
,Room.roomimage
,Room.occupancystatus
,Room.maintenancestatus
,Room.housekeepingstatus
,Room.nextdaycheckin
,Room.nextdaycheckout

				 ,Room.createduser,Room.createddate,Room.modifieduser,Room.modifieddate
				 ,Room.tenantid
                 ,Room.Roomid
                    
			  FROM Room
			  WHERE CAST(Room.Roomid AS Varchar)=pvar_Roomid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

