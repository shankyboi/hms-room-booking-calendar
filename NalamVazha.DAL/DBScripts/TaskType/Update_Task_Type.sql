
			  CREATE OR REPLACE FUNCTION  "Update_Task_Type"
			  (
				  pvar_TaskTypeid uuid
,pvar_tenantid uuid
,
pvar_tasktypename Varchar(128)
,
pvar_description Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:21*/
			  IF "Check_Authorization"(pvar_modifieduser, 'TaskType', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from TaskType where upper(TaskType.tasktypename) = upper(pvar_tasktypename) and TaskType.tenantid=pvar_tenantid  and TaskType.TaskTypeid <> pvar_TaskTypeid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Task Type Name  Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('TaskType', NOW(),
(SELECT query_to_xml('SELECT * FROM TaskType WHERE TaskType.TaskTypeid= '''||pvar_TaskTypeid||'''', true, false, '')));

                    
                    UPDATE TaskType SET
                    tasktypename=pvar_tasktypename
,description=pvar_description

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE TaskTypeid=pvar_TaskTypeid;

                    

                    


					
							
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
																,'Update_Task_Type'
																,'Authorization Failed Update_Task_Type'
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
						,'Update_Task_Type'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Task_Type - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

