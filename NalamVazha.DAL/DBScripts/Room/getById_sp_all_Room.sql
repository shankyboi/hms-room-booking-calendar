
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Room"
              (
			  pvar_Roomid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Roomid" uuid
,roomcode Varchar
,block Varchar
,building Varchar
,floor Varchar
,roomtype Varchar
,bookingdeposit decimal
,roomgroup Varchar
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

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:22*/
			  		 
              RETURN QUERY
			  SELECT  
				 Room.tenantid
,tenant.businessname as _tenantname
,Room.Roomid
,Room.roomcode
,CAST(_Block.blockcode||' '||_Block.blockname AS VARCHAR) as block
,CAST(__Building.buildingcode||' '||__Building.buildingname AS VARCHAR) as building
,CAST(___Floor.floorname AS VARCHAR) as floor
,CAST(____RoomType.name AS VARCHAR) as roomtype
,Room.bookingdeposit
,CAST(_____RoomGroup.groupname||' '||_____RoomGroup.groupnumber AS VARCHAR) as roomgroup
,Room.roomnumber
,Room.roomimage
,Room.occupancystatus
,Room.maintenancestatus
,Room.housekeepingstatus
,Room.nextdaycheckin
,Room.nextdaycheckout

				 ,Room.createduser,Room.createddate,Room.modifieduser,Room.modifieddate
                 
                 
				 
			  FROM  Room 
 LEFT OUTER JOIN tenant ON Room.tenantid=tenant.tenantid
INNER JOIN Block _Block ON Room.block=_Block.Blockid
INNER JOIN Building __Building ON Room.building=__Building.Buildingid
INNER JOIN Floor ___Floor ON Room.floor=___Floor.Floorid
INNER JOIN RoomType ____RoomType ON Room.roomtype=____RoomType.RoomTypeid
INNER JOIN RoomGroup _____RoomGroup ON Room.roomgroup=_____RoomGroup.RoomGroupid

			  WHERE CAST(Room.Roomid AS Varchar)=pvar_Roomid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

