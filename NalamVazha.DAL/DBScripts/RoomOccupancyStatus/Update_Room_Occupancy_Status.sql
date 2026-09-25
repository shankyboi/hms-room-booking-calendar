
			  CREATE OR REPLACE FUNCTION  "Update_Room_Occupancy_Status"
			  (
				  pvar_RoomOccupancyStatusid uuid
,pvar_tenantid uuid
,
pvar_roomallocationno Varchar(256)
,
pvar_patientvisit  uuid
,
pvar_patientname  uuid
,
pvar_ipdno  uuid
,
pvar_block  uuid
,
pvar_building  uuid
,
pvar_floor  uuid
,
pvar_room  uuid
,
pvar_bookeddate date
,
pvar_status  Varchar(1024)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:03*/
			  IF "Check_Authorization"(pvar_modifieduser, 'RoomOccupancyStatus', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from RoomOccupancyStatus where upper(RoomOccupancyStatus.roomallocationno) = upper(pvar_roomallocationno) and RoomOccupancyStatus.tenantid=pvar_tenantid  and RoomOccupancyStatus.RoomOccupancyStatusid <> pvar_RoomOccupancyStatusid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Room Allocation No Already Exists.';

																END IF;

               IF(pvar_status is not null AND pvar_status!='0' AND LENGTH(pvar_status)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_status, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='status'
                                                                and entityname='RoomOccupancyStatus' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_status, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'status value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('RoomOccupancyStatus', NOW(),
(SELECT query_to_xml('SELECT * FROM RoomOccupancyStatus WHERE RoomOccupancyStatus.RoomOccupancyStatusid= '''||pvar_RoomOccupancyStatusid||'''', true, false, '')));

                    
                    UPDATE RoomOccupancyStatus SET
                    roomallocationno=pvar_roomallocationno
,patientvisit=pvar_patientvisit
,patientname=pvar_patientname
,ipdno=pvar_ipdno
,block=pvar_block
,building=pvar_building
,floor=pvar_floor
,room=pvar_room
,bookeddate=pvar_bookeddate
,status=pvar_status

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE RoomOccupancyStatusid=pvar_RoomOccupancyStatusid;

                    

                    


					
							
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
																,'Update_Room_Occupancy_Status'
																,'Authorization Failed Update_Room_Occupancy_Status'
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
						,'Update_Room_Occupancy_Status'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Room_Occupancy_Status - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

