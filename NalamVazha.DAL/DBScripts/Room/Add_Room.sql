
			  CREATE OR REPLACE FUNCTION  "Add_Room"
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

				  ,pvar_createduser  uuid

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);lvar_curday_roomcode Varchar(10);lvar_val_roomcode int;
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:22*/
		

			  
                                                                                    if pvar_Roomid is null then
                                                                                    pvar_Roomid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
select cast(to_char(NOW(),'yyyy') as Varchar(4)) ||'-'|| RIGHT('00' ||cast(to_char(NOW(),'MM') as Varchar(2)),2) 
                                          INTO lvar_curday_roomcode;
                                        select COALESCE(max(RIGHT(Room.roomcode,4)),'0') INTO lvar_val_roomcode from
                                        Room where substring(Room.roomcode,1,7) = lvar_curday_roomcode and (Room.roomcode) NOT LIKE '%/%';
                                        lvar_val_roomcode:=lvar_val_roomcode + 1;
                                        pvar_roomcode:= (lvar_curday_roomcode||'-'|| cast(to_char(lvar_val_roomcode,'fm0000') as Varchar(4)));

			  IF "Check_Authorization"(pvar_createduser, 'Room', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from Room where upper(Room.roomcode::varchar) = upper(pvar_roomcode::varchar) and Room.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Room Code Already Exists.';

																END IF;
IF EXISTS (SELECT * from Room where upper(Room.roomnumber::varchar) = upper(pvar_roomnumber::varchar) and Room.tenantid=pvar_tenantid)
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
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Room(
				 roomcode
,block
,building
,floor
,roomtype
,bookingdeposit
,roomgroup
,roomnumber
,roomimage
,occupancystatus
,maintenancestatus
,housekeepingstatus
,nextdaycheckin
,nextdaycheckout
,easeofaccess

				 ,createduser
				 ,Roomid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_roomcode
,pvar_block
,pvar_building
,pvar_floor
,pvar_roomtype
,pvar_bookingdeposit
,pvar_roomgroup
,pvar_roomnumber
,pvar_roomimage
,pvar_occupancystatus
,pvar_maintenancestatus
,pvar_housekeepingstatus
,pvar_nextdaycheckin
,pvar_nextdaycheckout
,pvar_easeofaccess

				 ,pvar_createduser
				 ,pvar_Roomid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
					 
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
																,'Add_Room'
																,'Authorization Failed Add_Room'
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
						,'Add_Room'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Room - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

