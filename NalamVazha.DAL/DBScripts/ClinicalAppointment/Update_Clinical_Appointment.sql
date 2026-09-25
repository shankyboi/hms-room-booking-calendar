
			  CREATE OR REPLACE FUNCTION  "Update_Clinical_Appointment"
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

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:34*/
			  IF "Check_Authorization"(pvar_modifieduser, 'ClinicalAppointment', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from ClinicalAppointment where upper(ClinicalAppointment.tokennumber) = upper(pvar_tokennumber) and ClinicalAppointment.tenantid=pvar_tenantid  and ClinicalAppointment.ClinicalAppointmentid <> pvar_ClinicalAppointmentid)
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
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('ClinicalAppointment', NOW(),
(SELECT query_to_xml('SELECT * FROM ClinicalAppointment WHERE ClinicalAppointment.ClinicalAppointmentid= '''||pvar_ClinicalAppointmentid||'''', true, false, '')));

                    
                    UPDATE ClinicalAppointment SET
                    tasktype=pvar_tasktype
,patient=pvar_patient
,practitioner=pvar_practitioner
,photo=pvar_photo
,actualpractitioner=pvar_actualpractitioner
,appointmentdate=pvar_appointmentdate
,durationfrom=pvar_durationfrom
,durationto=pvar_durationto
,status=pvar_status
,origin=pvar_origin
,bookingid=pvar_bookingid
,tokennumber=pvar_tokennumber

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE ClinicalAppointmentid=pvar_ClinicalAppointmentid;

                    

                    INSERT INTO history
VALUES('ClinicalAppointment_reshedulehistory', NOW(),
(SELECT query_to_xml('SELECT * FROM ClinicalAppointment_reshedulehistory WHERE ClinicalAppointment_reshedulehistory.ClinicalAppointmentid= '''||pvar_ClinicalAppointmentid||'''', true, false, '')));

								DELETE FROM  ClinicalAppointment_reshedulehistory WHERE ClinicalAppointmentid=pvar_ClinicalAppointmentid;
								
								
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
																,'Update_Clinical_Appointment'
																,'Authorization Failed Update_Clinical_Appointment'
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
						,'Update_Clinical_Appointment'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Clinical_Appointment - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

