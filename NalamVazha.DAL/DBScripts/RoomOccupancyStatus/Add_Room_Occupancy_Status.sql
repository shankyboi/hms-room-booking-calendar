
			  CREATE OR REPLACE FUNCTION  "Add_Room_Occupancy_Status"
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
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);lvar_curday_roomallocationno Varchar(10);lvar_val_roomallocationno int;
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:03*/
		

			  
                                                                                    if pvar_RoomOccupancyStatusid is null then
                                                                                    pvar_RoomOccupancyStatusid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
select cast(to_char(NOW(),'yyyy') as Varchar(4)) ||'-'|| RIGHT('00' ||cast(to_char(NOW(),'MM') as Varchar(2)),2) 
                                          INTO lvar_curday_roomallocationno;
                                        select COALESCE(max(RIGHT(RoomOccupancyStatus.roomallocationno,4)),'0') INTO lvar_val_roomallocationno from
                                        RoomOccupancyStatus where substring(RoomOccupancyStatus.roomallocationno,1,7) = lvar_curday_roomallocationno and (RoomOccupancyStatus.roomallocationno) NOT LIKE '%/%';
                                        lvar_val_roomallocationno:=lvar_val_roomallocationno + 1;
                                        pvar_roomallocationno:= (lvar_curday_roomallocationno||'-'|| cast(to_char(lvar_val_roomallocationno,'fm0000') as Varchar(4)));

			  IF "Check_Authorization"(pvar_createduser, 'RoomOccupancyStatus', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from RoomOccupancyStatus where upper(RoomOccupancyStatus.roomallocationno::varchar) = upper(pvar_roomallocationno::varchar) and RoomOccupancyStatus.tenantid=pvar_tenantid)
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
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO RoomOccupancyStatus(
				 roomallocationno
,patientvisit
,patientname
,ipdno
,block
,building
,floor
,room
,bookeddate
,status

				 ,createduser
				 ,RoomOccupancyStatusid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_roomallocationno
,pvar_patientvisit
,pvar_patientname
,pvar_ipdno
,pvar_block
,pvar_building
,pvar_floor
,pvar_room
,pvar_bookeddate
,pvar_status

				 ,pvar_createduser
				 ,pvar_RoomOccupancyStatusid
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
																,'Add_Room_Occupancy_Status'
																,'Authorization Failed Add_Room_Occupancy_Status'
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
						,'Add_Room_Occupancy_Status'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Room_Occupancy_Status - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

