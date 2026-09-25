
			  CREATE OR REPLACE FUNCTION  "Add_Clinical_Appointment"
			  (
				  pvar_ClinicalAppointmentid uuid
,pvar_tenantid uuid
,
pvar_tasktype  Varchar(1024)
,
pvar_patient  uuid
,
pvar_practitioner  uuid
,
pvar_photo Varchar(256)
,
pvar_actualpractitioner  uuid
,
pvar_appointmentdate date
,
pvar_durationfrom Varchar(256)
,
pvar_durationto Varchar(256)
,
pvar_status  Varchar(1024)
,
pvar_origin  Varchar(1024)
,
pvar_bookingid Varchar(128)
,
pvar_tokennumber Varchar(256)
,pvar_reshedulehistory json
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);lvar_curday_tokennumber Varchar(10);lvar_val_tokennumber int;
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:33*/
		

			  
                                                                                    if pvar_ClinicalAppointmentid is null then
                                                                                    pvar_ClinicalAppointmentid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
select cast(to_char(NOW(),'yyyy') as Varchar(4)) || RIGHT('00' ||cast(to_char(NOW(),'MM') as Varchar(2)),2) 
                                        || RIGHT('00' ||cast(to_char(NOW(),'dd') as Varchar(2)),2) INTO lvar_curday_tokennumber;
                                        select COALESCE(max(RIGHT(ClinicalAppointment.tokennumber,4)),'0') INTO lvar_val_tokennumber from
                                        ClinicalAppointment where substring(ClinicalAppointment.tokennumber,1,8) = lvar_curday_tokennumber and (ClinicalAppointment.tokennumber) NOT LIKE '%/%';
                                        lvar_val_tokennumber:=lvar_val_tokennumber + 1;
                                        pvar_tokennumber:= (lvar_curday_tokennumber||'-'|| cast(to_char(lvar_val_tokennumber,'fm0000') as Varchar(4)));

			  IF "Check_Authorization"(pvar_createduser, 'ClinicalAppointment', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from ClinicalAppointment where upper(ClinicalAppointment.tokennumber::varchar) = upper(pvar_tokennumber::varchar) and ClinicalAppointment.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Token Number Already Exists.';

																END IF;

              IF(pvar_origin is not null AND pvar_origin!='0' AND LENGTH(pvar_origin)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_origin, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='origin'
                                                                and entityname='ClinicalAppointment' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_origin, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'origin value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_status is not null AND pvar_status!='0' AND LENGTH(pvar_status)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_status, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='status'
                                                                and entityname='ClinicalAppointment' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_status, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'status value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_tasktype is not null AND pvar_tasktype!='0' AND LENGTH(pvar_tasktype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                    FROM regexp_split_to_table(pvar_tasktype, ',') AS T1
                                                                        INNER JOIN (Select DISTINCT TaskType.tasktypename from TaskType) AS T2 on T1.T1 = T2.tasktypename) AS int) <> CAST((SELECT Count(T1.T1)
                                                                    FROM regexp_split_to_table(pvar_tasktype, ',')  AS T1) AS int))
                                                                    THEN
                                                                         pvar_returnMessage := pvar_returnMessage || ' tasktype value is invalid';


                                                                    END IF;
                                                                    END IF;
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO ClinicalAppointment(
				 tasktype
,patient
,practitioner
,photo
,actualpractitioner
,appointmentdate
,durationfrom
,durationto
,status
,origin
,bookingid
,tokennumber

				 ,createduser
				 ,ClinicalAppointmentid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_tasktype
,pvar_patient
,pvar_practitioner
,pvar_photo
,pvar_actualpractitioner
,pvar_appointmentdate
,pvar_durationfrom
,pvar_durationto
,pvar_status
,pvar_origin
,pvar_bookingid
,pvar_tokennumber

				 ,pvar_createduser
				 ,pvar_ClinicalAppointmentid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
								
								
								INSERT INTO ClinicalAppointment_reshedulehistory (
									ClinicalAppointmentid
									,ClinicalAppointment_reshedulehistoryid 
                                    ,record_order  
									,resheduleddatetime
,reshedulereason

									
									)
									SELECT 
									pvar_ClinicalAppointmentid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'resheduleddatetime' AS Timestamp(3)) as resheduleddatetime
,j->>'reshedulereason' as reshedulereason

									
                                    FROM json_array_elements(pvar_reshedulehistory) as j;
									

					 
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
																,'Add_Clinical_Appointment'
																,'Authorization Failed Add_Clinical_Appointment'
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
						,'Add_Clinical_Appointment'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Clinical_Appointment - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

