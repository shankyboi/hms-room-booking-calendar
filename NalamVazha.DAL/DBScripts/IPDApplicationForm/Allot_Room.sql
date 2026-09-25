
CREATE OR REPLACE FUNCTION public."Allot_Room"(
	pvar_ipdapplicationformid uuid,
	pvar_tenantid uuid,
	pvar_bookingreferencenumber character varying,
	pvar_packagename uuid,
	pvar_verifiedstatus character varying,
	pvar_room json,
	pvar_modifieduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);
			  DECLARE lvar_roomallocationno varchar(20);
			  DECLARE lvar_block uuid;
				DECLARE lvar_building uuid;
				DECLARE lvar_floor uuid;
				DECLARE  lvar_room uuid;
				DECLARE lvar_fromdate Timestamp(3);
				DECLARE lvar_todate Timestamp(3);
				DECLARE lvar_roomallocation_id uuid;
					DECLARE lvar_patient_id uuid;
					DECLARE
    rec RECORD;
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 05:57:33*/
			  IF "Check_Authorization"(pvar_modifieduser, 'IPDApplicationForm', 'edit') THEN

			  pvar_returnMessage:='';

              IF pvar_IPDApplicationFormid IS NULL
                 OR pvar_IPDApplicationFormid = '00000000-0000-0000-0000-000000000000'::uuid
              THEN
                  pvar_returnMessage := 'IPDApplicationFormid is required';
                  RETURN;
              END IF;

			  if EXISTS (SELECT * from IPDApplicationForm where upper(IPDApplicationForm.bookingreferencenumber) = upper(pvar_bookingreferencenumber) and IPDApplicationForm.tenantid=pvar_tenantid  and IPDApplicationForm.IPDApplicationFormid <> pvar_IPDApplicationFormid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Booking Reference Number Already Exists.';

																END IF;

              

															-- 🚨 OCCUPANCY VALIDATION
/*IF EXISTS (
    SELECT *
    FROM RoomOccupancyStatus ros
    WHERE ros.room = lvar_room
    AND ros.isdeleted = false
    AND ros.status IN ('Blocked','Booked','Occupied')
    AND ros.bookeddate BETWEEN lvar_fromdate::date AND lvar_todate::date
)
THEN
    pvar_returnMessage := 
    'Room is already occupied/blocked/booked for selected dates';
    RETURN;
END IF;
*/

IF EXISTS (
    SELECT 1
    FROM json_array_elements(pvar_room) AS j
    JOIN RoomOccupancyStatus ros 
        ON ros.room = CAST(j->>'roomnumber' AS uuid)
    WHERE ros.isdeleted = false
       AND lower(trim(COALESCE(ros.status, ''))) IN ('blocked','booked','occupied')
    AND ros.bookeddate BETWEEN 
        CAST(j->>'fromdate' AS date) 
        AND CAST(j->>'todate' AS date)
		UNION ALL

    SELECT 1
    FROM json_array_elements(pvar_room) AS j
    JOIN RoomAllocation ra
        ON ra.room = CAST(j->>'roomnumber' AS uuid)
    WHERE ra.isdeleted = false
    AND lower(trim(COALESCE(ra.status, ''))) IN ('blocked','booked','occupied')
    AND CAST(j->>'fromdate' AS date) <= ra.todate::date
    AND CAST(j->>'todate' AS date) >= ra.fromdate::date
)
THEN
    pvar_returnMessage := 
    'Cannot allocate: Room already occupied/blocked/booked for selected dates';
    RETURN;
END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
                IF EXISTS (
                SELECT 1
                FROM IPDApplicationForm
                WHERE IPDApplicationFormid = pvar_IPDApplicationFormid
                  AND lower(coalesce(verifiedstatus,'')) IN ('approved','rejected')
            ) THEN
                pvar_returnMessage := 'Cannot update: Already Approved/Rejected';
                RETURN;
            END IF;
                    INSERT INTO history
VALUES('IPDApplicationForm', NOW(),
(SELECT query_to_xml('SELECT * FROM IPDApplicationForm WHERE IPDApplicationForm.IPDApplicationFormid= '''||pvar_IPDApplicationFormid||'''', true, false, '')));

                    -- Direct IPD is already verified through the direct-admission
                    -- workflow. Only normal IPD room allotment enters review.
                    IF lower(trim(COALESCE(pvar_verifiedstatus, ''))) != 'direct admission' THEN
                        pvar_verifiedstatus := 'Ready For Doctor Review';
                    END IF;
                    UPDATE IPDApplicationForm SET
                    --bookingreferencenumber=pvar_bookingreferencenumber
packagename=pvar_packagename
,verifiedstatus=CASE
     -- Direct IPD must retain its origin so it can follow the pre-admission workflow.
    WHEN lower(trim(COALESCE(pvar_verifiedstatus, ''))) = 'direct admission'
        THEN 'Direct Admission'
    ELSE 'Approved'
END
,bookingstatus='Provisional Booking'

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE IPDApplicationFormid=pvar_IPDApplicationFormid;

/*INSERT INTO roomoccupancystatus(
	roomoccupancystatusid, tenantid, roomallocationno, ipdno, 
	block, building, floor, room, bookeddate, 
	status, createduser, createddate, modifieduser, modifieddate, isdeleted)
	VALUES (gen_random_uuid(), pvar_tenantid, lvar_roomallocationno,pvar_IPDApplicationFormid,
	?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);*/

	
                    
							-- Inserts deleted data into a history.
                            INSERT INTO IPDApplicationForm_room_history
                            (
                            IPDApplicationFormid
                            ,IPDApplicationForm_roomid 
                            ,record_order  
                            ,allottedto
,roomnumber
,fromdate
,todate

                            ,action_date
                            ,action_by
                            ,action	
                            ) 
                            SELECT 
                            IPDApplicationFormid
                            ,IPDApplicationForm_roomid 
                            ,record_order  
                            ,allottedto
,roomnumber
,fromdate
,todate

                            ,NOW()
                            ,pvar_modifieduser
                            ,'Deleted'
                            FROM IPDApplicationForm_room 
                            WHERE IPDApplicationForm_roomid IN (
                            Select IPDApplicationForm_roomid from IPDApplicationForm_room
                            where IPDApplicationForm_roomid not in (
                            SELECT 
                            CAST(coalesce(j->>'IPDApplicationForm_roomid','00000000-0000-0000-0000-000000000000') as uuid) 
                            FROM json_array_elements(pvar_room) as j
                            ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);

                                -- Mark IPDApplicationForm_room data as deleted.
                                UPDATE IPDApplicationForm_room 
                                set isdeleted=true
                                WHERE IPDApplicationForm_roomid IN(
                                Select IPDApplicationForm_roomid from IPDApplicationForm_room
                                where IPDApplicationForm_roomid not in (
                                SELECT 
                                CAST(coalesce(j->>'IPDApplicationForm_roomid','00000000-0000-0000-0000-000000000000') as uuid)
                                FROM json_array_elements(pvar_room) as j
                                ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);
	                                

                                -- Insert newly added items to IPDApplicationForm_room
                                INSERT INTO IPDApplicationForm_room(
                                IPDApplicationFormid
                                ,IPDApplicationForm_roomid 
                                ,record_order  
                                ,allottedto
,roomnumber
,fromdate
,todate

                                ,action_date
                                ,action_by
                                ,action	
                                ) 
                                SELECT 
                                pvar_IPDApplicationFormid
                                ,gen_random_uuid()
                                ,CAST(coalesce(j->>'record_order','0') as INT)
                                 ,j->>'allottedto' as allottedto
,CAST(j->>'roomnumber' AS uuid) as roomnumber
,CAST(j->>'fromdate' AS Timestamp(3)) as fromdate
,CAST(j->>'todate' AS Timestamp(3)) as todate

                                 ,NOW()
                                 ,pvar_modifieduser
                                ,'Added'
                                FROM json_array_elements(pvar_room) as j
                                WHERE CAST(coalesce(j->>'IPDApplicationForm_roomid','00000000-0000-0000-0000-000000000000') as uuid)='00000000-0000-0000-0000-000000000000';
                                        
                                -- INSERT UPDATED DATA INTO HISTORY
                                INSERT INTO IPDApplicationForm_room_history(
                                IPDApplicationFormid
                                ,IPDApplicationForm_roomid 
                                ,record_order  
                                ,allottedto
,roomnumber
,fromdate
,todate

                                ,action_date
                                ,action_by
                                ,action	)
                                SELECT 
                                r.IPDApplicationFormid
                                ,r.IPDApplicationForm_roomid
                                ,r.record_order 
                                ,r.allottedto
,r.roomnumber
,r.fromdate
,r.todate

                                ,r.action_date
                                ,r.action_by
                                ,r.action
                                FROM json_array_elements(pvar_room) as j
                                JOIN IPDApplicationForm_room AS r 
                                ON r.IPDApplicationForm_roomid = CAST(j->>'IPDApplicationForm_roomid' AS uuid) 
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                WHERE 
                                r.IPDApplicationForm_roomid = CAST(j->>'IPDApplicationForm_roomid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.allottedto
,r.roomnumber
,r.fromdate
,r.todate
) 
                                IS 
                                DISTINCT FROM (
                                 j->>'allottedto'
,CAST(j->>'roomnumber' AS uuid)
,CAST(j->>'fromdate' AS Timestamp(3))
,CAST(j->>'todate' AS Timestamp(3))

                                );

                                -- UPDATE DATA IN TRANSACTION
						UPDATE IPDApplicationForm_room AS r
						SET 
						record_order=CAST(coalesce(j->>'record_order','0') as INT)
						,allottedto=j->>'allottedto'
						,roomnumber=CAST(j->>'roomnumber' AS uuid)
						,fromdate=CAST(j->>'fromdate' AS Timestamp(3))
						,todate=CAST(j->>'todate' AS Timestamp(3))
						
						
						,action='updated'
						,action_date=NOW()
						,action_by=pvar_modifieduser
						FROM json_array_elements(pvar_room) as j
						WHERE 
						r.IPDApplicationForm_roomid = CAST(j->>'IPDApplicationForm_roomid' AS uuid)
						AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
						AND 
						(r.allottedto
						,r.roomnumber
						,r.fromdate
						,r.todate
						) 
						IS 
						DISTINCT FROM (
						j->>'allottedto'
						,CAST(j->>'roomnumber' AS uuid)
						,CAST(j->>'fromdate' AS Timestamp(3))
						,CAST(j->>'todate' AS Timestamp(3))
						
						);

Select patientname into lvar_patient_id from IPDApplicationForm  WHERE IPDApplicationFormid = pvar_IPDApplicationFormid;

	 FOR rec IN 
    SELECT roomnumber, fromdate, todate, allottedto
    FROM IPDApplicationForm_room
    WHERE IPDApplicationFormid = pvar_IPDApplicationFormid
      AND COALESCE(isdeleted, false) = false
LOOP

    -- Get room details
    SELECT block, building, floor
    INTO lvar_block, lvar_building, lvar_floor
    FROM room 
    WHERE roomid = rec.roomnumber;

    -- Generate allocation no & id
    lvar_roomallocationno := generate_formatted_numbers(
        pvar_tenantid,
        'YYYY-MM-DD-99999',
        'roomallocation',
        'roomallocationno'
    );

    lvar_roomallocation_id := gen_random_uuid();

    -- INSERT into RoomAllocation
    INSERT INTO RoomAllocation(
         roomallocationno,
         ipdno,
         block,
         building,
         floor,
         room,
         fromdate,
         todate,
         status,
         bookedfor,   -- ✅ NEW COLUMN
         createduser,
         RoomAllocationid,
         tenantid
    )
    VALUES (
         lvar_roomallocationno,
         pvar_IPDApplicationFormid,
         lvar_block,
         lvar_building,
         lvar_floor,
         rec.roomnumber,
         rec.fromdate,
         rec.todate,
         'Blocked',
         rec.allottedto,  -- ✅ mapping
         pvar_modifieduser,
         lvar_roomallocation_id,
         pvar_tenantid
    );

    -- INSERT into RoomOccupancyStatus (per day)
    INSERT INTO RoomOccupancyStatus(
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
        bookedfor,  -- ✅ NEW COLUMN
        createduser,
        createddate
    )
    SELECT 
        gen_random_uuid(),
        pvar_tenantid,
        lvar_roomallocation_id,
        NULL,
        lvar_patient_id,
        pvar_IPDApplicationFormid,
        lvar_block,
        lvar_building,
        lvar_floor,
        rec.roomnumber,
        gs::date,
        'Blocked',
        rec.allottedto,  -- ✅ mapping
        pvar_modifieduser,
        NOW()
    FROM generate_series(
        rec.fromdate::date,
        rec.todate::date,
        interval '1 day'
    ) AS gs;

END LOOP;
				
/*SELECT roomnumber,fromdate,todate into lvar_room,lvar_fromdate,lvar_todate 
FROM IPDApplicationForm_room WHERE IPDApplicationFormid = pvar_IPDApplicationFormid;

Select patientname into lvar_patient_id from IPDApplicationForm  WHERE IPDApplicationFormid = pvar_IPDApplicationFormid;

 

SELECT block,
building,
floor into lvar_block,lvar_building,lvar_floor
FROM room where roomid=lvar_room;

                     
 lvar_roomallocationno := generate_formatted_numbers(
    pvar_tenantid,
    'YYYY-MM-DD-99999',
    'roomallocation',
    'roomallocationno'
);
lvar_roomallocation_id:=gen_random_uuid();

 
								
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
				 lvar_roomallocationno
				,pvar_IPDApplicationFormid
				,lvar_block
				,lvar_building
				,lvar_floor
				,lvar_room
				,lvar_fromdate
				,lvar_todate
				,'Blocked'
				
				 ,pvar_modifieduser
				 ,lvar_roomallocation_id
				 ,pvar_tenantid
				   
				);
			   

INSERT INTO RoomOccupancyStatus(
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
    createduser,
    createddate
)
SELECT 
    gen_random_uuid(),
    pvar_tenantid,
    lvar_roomallocation_id,
    NULL, -- or map PatientVisit if available
    lvar_patient_id, -- or map PatientProfile if available
    pvar_IPDApplicationFormid,
    lvar_block,
    lvar_building,
    lvar_floor,
    lvar_room,
    gs::date,  -- each day
    'Blocked',
    pvar_modifieduser,
    NOW()
FROM generate_series(
    lvar_fromdate::date,
    lvar_todate::date,
    interval '1 day'
) AS gs;*/

					
							
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
																,'Allot_Room'
																,'Authorization Failed Allot_Room'
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
						,'Allot_Room'
						,'update failed'
						);
                        pvar_returnMessage := 'Allot_Room - update failed';*/
			  	
			  END
              
$BODY$;