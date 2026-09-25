-- FUNCTION: public.Update_Room_Allocation(uuid, uuid, character varying, uuid, uuid, uuid, uuid, uuid, date, date, character varying, uuid)

-- DROP FUNCTION IF EXISTS public."Update_Room_Allocation"(uuid, uuid, character varying, uuid, uuid, uuid, uuid, uuid, date, date, character varying, uuid);

CREATE OR REPLACE FUNCTION public."Update_Room_Allocation"(
	pvar_roomallocationid uuid,
	pvar_tenantid uuid,
	pvar_roomallocationno character varying,
	pvar_ipdno uuid,
	pvar_block uuid,
	pvar_building uuid,
	pvar_floor uuid,
	pvar_room uuid,
	pvar_fromdate date,
	pvar_todate date,
	pvar_status character varying,
	pvar_modifieduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:10*/
			  IF "Check_Authorization"(pvar_modifieduser, 'RoomAllocation', 'edit') THEN

			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from RoomAllocation where upper(RoomAllocation.roomallocationno) = upper(pvar_roomallocationno) and RoomAllocation.tenantid=pvar_tenantid  and RoomAllocation.RoomAllocationid <> pvar_RoomAllocationid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Room Allocation No Already Exists.';

																END IF;

               IF(pvar_status is not null AND pvar_status!='0' AND LENGTH(pvar_status)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_status, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='status'
                                                                and entityname='RoomAllocation' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_status, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'status value is invalid';

                                                                END IF;
                                                            END IF;


 IF(pvar_returnMessage='')
               THEN
                   IF EXISTS (
                       SELECT 1
                       FROM RoomOccupancyStatus ROS
                       WHERE ROS.tenantid = pvar_tenantid
                         AND ROS.room = pvar_room
                         AND ROS.status IN ('Booked','Blocked','Occupied')
                         AND ROS.roomallocationno <> pvar_RoomAllocationid::varchar
                         AND ROS.bookeddate BETWEEN pvar_fromdate AND pvar_todate
                   )
                   THEN
                       pvar_returnMessage := pvar_returnMessage || 'Room is already Booked/Blocked/Occupied for the selected date range.';
                   END IF;
               END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('RoomAllocation', NOW(),
(SELECT query_to_xml('SELECT * FROM RoomAllocation WHERE RoomAllocation.RoomAllocationid= '''||pvar_RoomAllocationid||'''', true, false, '')));

                    
                    UPDATE RoomAllocation SET
                    roomallocationno=pvar_roomallocationno
,ipdno=pvar_ipdno
,block=pvar_block
,building=pvar_building
,floor=pvar_floor
,room=pvar_room
,fromdate=pvar_fromdate
,todate=pvar_todate
,status=pvar_status

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE RoomAllocationid=pvar_RoomAllocationid;

                    
DELETE FROM RoomOccupancyStatus
WHERE roomallocationno =  pvar_RoomAllocationid::varchar
AND tenantid = pvar_tenantid;

-- Re-insert occupancy records for the updated date range
INSERT INTO RoomOccupancyStatus
(
    RoomOccupancyStatusid,
    tenantid,
    roomallocationno,
    patientvisit,
    patientname,
    ipdno,
    block,
    building,
    floor,
    room,
    bookeddate,
    status,
    bookedfor,
    createduser,
    createddate
)
SELECT
    gen_random_uuid(),
    pvar_tenantid,
     pvar_RoomAllocationid::varchar,
    NULL,
    NULL,
    pvar_ipdno,
    pvar_block,
    pvar_building,
    pvar_floor,
    pvar_room,
    gs::date,
    pvar_status,
    null,
    pvar_modifieduser,
    NOW()
FROM generate_series
(
    pvar_fromdate::date,
    pvar_todate::date,
    interval '1 day'
) gs;
                    

					
							
					pvar_returnMessage :='201.1';
			
			  END IF;

			  
																ELSE
																

															
																INSERT INTO system_logging
																(
																Log_code
																,system_logging_guid
																,log_application
																,log_date
																,log_level
																,log_logger
																,log_message
																,log_user_name
																)
																VALUES
																('401.1'
																,gen_random_uuid()
																,'Store Proc Authorization Check'
																,NOW()
																,'Critical'
																,'Update_Room_Allocation'
																,'Authorization Failed Update_Room_Allocation'
																,pvar_modifieduser
																);
																pvar_returnMessage = '401.1';
																
																END IF;

			  			 /* EXCEPTION WHEN OTHERS THEN
			 
						INSERT INTO system_logging
						(
						Log_code
						,system_logging_guid
						,log_application
						,log_date
						,log_level
						,log_logger
						,log_message
						)
						VALUES
						('16'
						,gen_random_uuid()
						,'Postgre Function Exception'
						,NOW()
						,'16'
						,'Update_Room_Allocation'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Room_Allocation - update failed';*/
			  	
			  END
              
$BODY$;

ALTER FUNCTION public."Update_Room_Allocation"(uuid, uuid, character varying, uuid, uuid, uuid, uuid, uuid, date, date, character varying, uuid)
    OWNER TO md_nalamvazha;

GRANT EXECUTE ON FUNCTION public."Update_Room_Allocation"(uuid, uuid, character varying, uuid, uuid, uuid, uuid, uuid, date, date, character varying, uuid) TO PUBLIC;

GRANT EXECUTE ON FUNCTION public."Update_Room_Allocation"(uuid, uuid, character varying, uuid, uuid, uuid, uuid, uuid, date, date, character varying, uuid) TO develop_ukan;

GRANT EXECUTE ON FUNCTION public."Update_Room_Allocation"(uuid, uuid, character varying, uuid, uuid, uuid, uuid, uuid, date, date, character varying, uuid) TO md_nalamvazha;

