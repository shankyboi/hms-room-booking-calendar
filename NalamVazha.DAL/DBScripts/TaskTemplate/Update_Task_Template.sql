
			  CREATE OR REPLACE FUNCTION  "Update_Task_Template"
			  (
				  pvar_TaskTemplateid uuid
,pvar_tenantid uuid
,
pvar_tasktype  uuid
,
pvar_taskname Varchar(128)
,
pvar_priority  Varchar(1024)
,
pvar_duration int
,pvar_escalationdetails json
,pvar_nextactiondetails json

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:27:26*/
			  IF "Check_Authorization"(pvar_modifieduser, 'TaskTemplate', 'edit') THEN


			  pvar_returnMessage:='';

			  
               IF(pvar_priority is not null AND pvar_priority!='0' AND LENGTH(pvar_priority)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_priority, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='priority'
                                                                and entityname='TaskTemplate' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_priority, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'priority value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('TaskTemplate', NOW(),
(SELECT query_to_xml('SELECT * FROM TaskTemplate WHERE TaskTemplate.TaskTemplateid= '''||pvar_TaskTemplateid||'''', true, false, '')));

                    
                    UPDATE TaskTemplate SET
                    tasktype=pvar_tasktype
,taskname=pvar_taskname
,priority=pvar_priority
,duration=pvar_duration

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE TaskTemplateid=pvar_TaskTemplateid;

                    

                    INSERT INTO history
VALUES('TaskTemplate_escalationdetails', NOW(),
(SELECT query_to_xml('SELECT * FROM TaskTemplate_escalationdetails WHERE TaskTemplate_escalationdetails.TaskTemplateid= '''||pvar_TaskTemplateid||'''', true, false, '')));

								DELETE FROM  TaskTemplate_escalationdetails WHERE TaskTemplateid=pvar_TaskTemplateid;
								
								
								INSERT INTO TaskTemplate_escalationdetails (
									TaskTemplateid
									,TaskTemplate_escalationdetailsid 
                                    ,record_order  
									,priority
,notifyto
,emailid
,mobilenumber

									
									)
									SELECT 
									pvar_TaskTemplateid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'priority' as priority
,CAST(j->>'notifyto' AS uuid) as notifyto
,j->>'emailid' as emailid
,j->>'mobilenumber' as mobilenumber

									
                                    FROM json_array_elements(pvar_escalationdetails) as j;
									
INSERT INTO history
VALUES('TaskTemplate_nextactiondetails', NOW(),
(SELECT query_to_xml('SELECT * FROM TaskTemplate_nextactiondetails WHERE TaskTemplate_nextactiondetails.TaskTemplateid= '''||pvar_TaskTemplateid||'''', true, false, '')));

								DELETE FROM  TaskTemplate_nextactiondetails WHERE TaskTemplateid=pvar_TaskTemplateid;
								
								
								INSERT INTO TaskTemplate_nextactiondetails (
									TaskTemplateid
									,TaskTemplate_nextactiondetailsid 
                                    ,record_order  
									,actiontype
,actionname

									
									)
									SELECT 
									pvar_TaskTemplateid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'actiontype' AS uuid) as actiontype
,CAST(j->>'actionname' AS uuid) as actionname

									
                                    FROM json_array_elements(pvar_nextactiondetails) as j;
									



					
							
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
																,'Update_Task_Template'
																,'Authorization Failed Update_Task_Template'
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
						,'Update_Task_Template'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Task_Template - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

