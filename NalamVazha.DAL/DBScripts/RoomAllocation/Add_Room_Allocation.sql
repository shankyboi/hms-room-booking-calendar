-- FUNCTION: public.Add_Room_Allocation(uuid, uuid, character varying, uuid, uuid, uuid, uuid, uuid, date, date, character varying, uuid)

-- DROP FUNCTION IF EXISTS public."Add_Room_Allocation"(uuid, uuid, character varying, uuid, uuid, uuid, uuid, uuid, date, date, character varying, uuid);

CREATE OR REPLACE FUNCTION public."Add_Room_Allocation"(
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
	pvar_createduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);lvar_curday_roomallocationno Varchar(10);lvar_val_roomallocationno int;
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:10*/
		

			  
                                                                                    if pvar_RoomAllocationid is null then
                                                                                    pvar_RoomAllocationid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
pvar_roomallocationno := generate_formatted_numbers(
    pvar_tenantid,
    'YYYY-MM-9999',      
    'roomallocation',    
    'roomallocationno'
);

			  IF "Check_Authorization"(pvar_createduser, 'RoomAllocation', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from RoomAllocation where upper(RoomAllocation.roomallocationno::varchar) = upper(pvar_roomallocationno::varchar) and RoomAllocation.tenantid=pvar_tenantid)
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
                        AND ROS.status IN (
                            'Booked',
                            'Occupied',
                            CASE WHEN pvar_status <> 'Available' THEN 'Blocked' ELSE NULL END
                        )
                        AND ROS.bookeddate BETWEEN pvar_fromdate AND pvar_todate
                  )
                  THEN
                      pvar_returnMessage := pvar_returnMessage || 'Room is already Booked/Blocked/Occupied for the selected date range.';
                  END IF;
              END IF;
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO RoomAllocation(
				 roomallocationno
,ipdno
,block
,building
,floor
,room
,fromdate
,todate
,status

				 ,createduser
				 ,RoomAllocationid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_roomallocationno
,pvar_ipdno
,pvar_block
,pvar_building
,pvar_floor
,pvar_room
,pvar_fromdate
,pvar_todate
,pvar_status

				 ,pvar_createduser
				 ,pvar_RoomAllocationid
				 ,pvar_tenantid
                   
			  );

DELETE FROM RoomOccupancyStatus ros
WHERE ros.tenantid = pvar_tenantid::uuid
  AND ros.room = pvar_room::uuid
  AND COALESCE(ros.ipdno::text, '') = COALESCE(NULLIF(pvar_ipdno::text, ''), '')
  AND ros.bookeddate::date BETWEEN pvar_fromdate::date AND pvar_todate::date;

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
    pvar_tenantid::uuid,
    pvar_RoomAllocationid::uuid,
    NULL,
    NULL,
    NULLIF(pvar_ipdno::text, '')::uuid,
    NULLIF(pvar_block::text, '')::uuid,
    NULLIF(pvar_building::text, '')::uuid,
    NULLIF(pvar_floor::text, '')::uuid,
    NULLIF(pvar_room::text, '')::uuid,
    gs::date,
    pvar_status,
    NULL,
    pvar_createduser::uuid,
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
																,'Add_Room_Allocation'
																,'Authorization Failed Add_Room_Allocation'
																,pvar_createduser
																);
																pvar_returnMessage := '401.1';
																
																END IF;
			  /*EXCEPTION WHEN OTHERS THEN
			 
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
						,'Store Proc Exception'
						,NOW()
						,'16'
						,'Add_Room_Allocation'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Room_Allocation - Insert failed';*/
			  	
			  END
              
$BODY$;

ALTER FUNCTION public."Add_Room_Allocation"(uuid, uuid, character varying, uuid, uuid, uuid, uuid, uuid, date, date, character varying, uuid)
    OWNER TO md_nalamvazha;

GRANT EXECUTE ON FUNCTION public."Add_Room_Allocation"(uuid, uuid, character varying, uuid, uuid, uuid, uuid, uuid, date, date, character varying, uuid) TO PUBLIC;

GRANT EXECUTE ON FUNCTION public."Add_Room_Allocation"(uuid, uuid, character varying, uuid, uuid, uuid, uuid, uuid, date, date, character varying, uuid) TO develop_ukan;

GRANT EXECUTE ON FUNCTION public."Add_Room_Allocation"(uuid, uuid, character varying, uuid, uuid, uuid, uuid, uuid, date, date, character varying, uuid) TO md_nalamvazha;
