
			  CREATE OR REPLACE FUNCTION  "Update_Room"
			  (
				  pvar_Roomid uuid
,pvar_tenantid uuid
,
pvar_roomcode Varchar(256)
,
pvar_block  uuid
,
pvar_building  uuid
,
pvar_floor  uuid
,
pvar_roomtype  uuid
,
pvar_bookingdeposit decimal(18,2)
,
pvar_roomgroup  uuid
,
pvar_roomnumber Varchar(128)
,
pvar_roomimage Varchar(256)
,
pvar_occupancystatus  Varchar(1024)
,
pvar_maintenancestatus  Varchar(1024)
,
pvar_housekeepingstatus  Varchar(1024)
,
pvar_nextdaycheckin  Varchar(1024)
,
pvar_nextdaycheckout  Varchar(1024)

,pvar_easeofaccess  Varchar(1024)

				  ,pvar_modifieduser  uuid

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:22*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Room', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from Room where upper(Room.roomcode) = upper(pvar_roomcode) and Room.tenantid=pvar_tenantid  and Room.Roomid <> pvar_Roomid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Room Code Already Exists.';

																END IF;
if EXISTS (SELECT * from Room where upper(Room.roomnumber) = upper(pvar_roomnumber) and Room.tenantid=pvar_tenantid  and Room.Roomid <> pvar_Roomid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Room Number Already Exists.';

																END IF;

               IF(pvar_housekeepingstatus is not null AND pvar_housekeepingstatus!='0' AND LENGTH(pvar_housekeepingstatus)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_housekeepingstatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='housekeepingstatus'
                                                                and entityname='Room' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_housekeepingstatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'housekeepingstatus value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_maintenancestatus is not null AND pvar_maintenancestatus!='0' AND LENGTH(pvar_maintenancestatus)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_maintenancestatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='maintenancestatus'
                                                                and entityname='Room' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_maintenancestatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'maintenancestatus value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_nextdaycheckin is not null AND pvar_nextdaycheckin!='0' AND LENGTH(pvar_nextdaycheckin)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_nextdaycheckin, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='nextdaycheckin'
                                                                and entityname='Room' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_nextdaycheckin, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'nextdaycheckin value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_nextdaycheckout is not null AND pvar_nextdaycheckout!='0' AND LENGTH(pvar_nextdaycheckout)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_nextdaycheckout, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='nextdaycheckout'
                                                                and entityname='Room' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_nextdaycheckout, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'nextdaycheckout value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_occupancystatus is not null AND pvar_occupancystatus!='0' AND LENGTH(pvar_occupancystatus)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_occupancystatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='occupancystatus'
                                                                and entityname='Room' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_occupancystatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'occupancystatus value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Room', NOW(),
(SELECT query_to_xml('SELECT * FROM Room WHERE Room.Roomid= '''||pvar_Roomid||'''', true, false, '')));

                    
                    UPDATE Room SET
                    roomcode=pvar_roomcode
,block=pvar_block
,building=pvar_building
,floor=pvar_floor
,roomtype=pvar_roomtype
,bookingdeposit=pvar_bookingdeposit
,roomgroup=pvar_roomgroup
,roomnumber=pvar_roomnumber
,roomimage=pvar_roomimage
,occupancystatus=pvar_occupancystatus
,maintenancestatus=pvar_maintenancestatus
,housekeepingstatus=pvar_housekeepingstatus
,nextdaycheckin=pvar_nextdaycheckin
,nextdaycheckout=pvar_nextdaycheckout
,easeofaccess=pvar_easeofaccess


                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Roomid=pvar_Roomid;

                    

                    


					
							
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
																,'Update_Room'
																,'Authorization Failed Update_Room'
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
						,'Update_Room'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Room - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

