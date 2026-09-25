
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_RoomOccupancyStatus"
              (
			  pvar_RoomOccupancyStatusid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"RoomOccupancyStatusid" uuid
,roomallocationno Varchar
,patientvisit Varchar
,patientname Varchar
,ipdno Varchar
,block Varchar
,building Varchar
,floor Varchar
,room Varchar
,bookeddate Varchar
,status Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:03*/
			  		 
              RETURN QUERY
			  SELECT  
				 RoomOccupancyStatus.tenantid
,tenant.businessname as _tenantname
,RoomOccupancyStatus.RoomOccupancyStatusid
,RoomOccupancyStatus.roomallocationno
,CAST(_PatientVisit.visitnumber AS VARCHAR) as patientvisit
,CAST(__PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber AS VARCHAR) as patientname
,CAST(___IPDApplicationForm.firstname||' '||___IPDApplicationForm.lastname AS VARCHAR) as ipdno
,CAST(____Block.blockname AS VARCHAR) as block
,CAST(_____Building.buildingname AS VARCHAR) as building
,CAST(______Floor.floorname AS VARCHAR) as floor
,CAST(_______Room.roomnumber AS VARCHAR) as room
,CAST(COALESCE(to_char(RoomOccupancyStatus.bookeddate,'dd/MM/yyyy'),'') AS Varchar) as bookeddate
,RoomOccupancyStatus.status

				 ,RoomOccupancyStatus.createduser,RoomOccupancyStatus.createddate,RoomOccupancyStatus.modifieduser,RoomOccupancyStatus.modifieddate
                 
                 
				 
			  FROM  RoomOccupancyStatus 
 LEFT OUTER JOIN tenant ON RoomOccupancyStatus.tenantid=tenant.tenantid
INNER JOIN PatientVisit _PatientVisit ON RoomOccupancyStatus.patientvisit=_PatientVisit.PatientVisitid
LEFT OUTER JOIN PatientProfile __PatientProfile ON RoomOccupancyStatus.patientname=__PatientProfile.PatientProfileid
INNER JOIN IPDApplicationForm ___IPDApplicationForm ON RoomOccupancyStatus.ipdno=___IPDApplicationForm.IPDApplicationFormid
INNER JOIN Block ____Block ON RoomOccupancyStatus.block=____Block.Blockid
INNER JOIN Building _____Building ON RoomOccupancyStatus.building=_____Building.Buildingid
INNER JOIN Floor ______Floor ON RoomOccupancyStatus.floor=______Floor.Floorid
INNER JOIN Room _______Room ON RoomOccupancyStatus.room=_______Room.Roomid

			  WHERE CAST(RoomOccupancyStatus.RoomOccupancyStatusid AS Varchar)=pvar_RoomOccupancyStatusid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

