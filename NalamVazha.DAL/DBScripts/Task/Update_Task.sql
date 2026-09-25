
			  CREATE OR REPLACE FUNCTION  "Update_Task"
			  (
				  pvar_Taskid uuid
,pvar_tenantid uuid
,
pvar_tasktype  uuid
,
pvar_taskname Varchar(128)
,
pvar_taskdesc Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:24*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Task', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from Task where upper(Task.taskname) = upper(pvar_taskname) and Task.tenantid=pvar_tenantid  and Task.Taskid <> pvar_Taskid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Task Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Task', NOW(),
(SELECT query_to_xml('SELECT * FROM Task WHERE Task.Taskid= '''||pvar_Taskid||'''', true, false, '')));

                    
                    UPDATE Task SET
                    tasktype=pvar_tasktype
,taskname=pvar_taskname
,taskdesc=pvar_taskdesc

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Taskid=pvar_Taskid;

                    

                    


					
							
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
																,'Update_Task'
																,'Authorization Failed Update_Task'
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
						,'Update_Task'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Task - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

