
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_RoomAllocation"
              (
			  pvar_RoomAllocationid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"RoomAllocationid" uuid
,roomallocationno Varchar
,ipdno Varchar
,block Varchar
,building Varchar
,floor Varchar
,room Varchar
,fromdate Varchar
,todate Varchar
,status Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:10*/
			  		 
              RETURN QUERY
			  SELECT  
				 RoomAllocation.tenantid
,tenant.businessname as _tenantname
,RoomAllocation.RoomAllocationid
,RoomAllocation.roomallocationno
,CAST(_IPDApplicationForm.firstname||' '||_IPDApplicationForm.lastname AS VARCHAR) as ipdno
,CAST(__Block.blockname AS VARCHAR) as block
,CAST(___Building.buildingname AS VARCHAR) as building
,CAST(____Floor.floorname AS VARCHAR) as floor
,CAST(_____Room.roomnumber AS VARCHAR) as room
,CAST(COALESCE(to_char(RoomAllocation.fromdate,'dd/MM/yyyy'),'') AS Varchar) as fromdate
,CAST(COALESCE(to_char(RoomAllocation.todate,'dd/MM/yyyy'),'') AS Varchar) as todate
,RoomAllocation.status

				 ,RoomAllocation.createduser,RoomAllocation.createddate,RoomAllocation.modifieduser,RoomAllocation.modifieddate
                 
                 
				 
			  FROM  RoomAllocation 
 LEFT OUTER JOIN tenant ON RoomAllocation.tenantid=tenant.tenantid
INNER JOIN IPDApplicationForm _IPDApplicationForm ON RoomAllocation.ipdno=_IPDApplicationForm.IPDApplicationFormid
INNER JOIN Block __Block ON RoomAllocation.block=__Block.Blockid
INNER JOIN Building ___Building ON RoomAllocation.building=___Building.Buildingid
INNER JOIN Floor ____Floor ON RoomAllocation.floor=____Floor.Floorid
INNER JOIN Room _____Room ON RoomAllocation.room=_____Room.Roomid

			  WHERE CAST(RoomAllocation.RoomAllocationid AS Varchar)=pvar_RoomAllocationid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

