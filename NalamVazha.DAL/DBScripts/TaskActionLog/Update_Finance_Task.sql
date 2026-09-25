
			  CREATE OR REPLACE FUNCTION  "Update_Finance_Task"
			  (
				  pvar_TaskActionLogid uuid
,pvar_tenantid uuid
,
pvar_taskname  uuid
,
pvar_tasktype  uuid
,
pvar_summary Varchar(128)
,
pvar_description Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:40:35*/
			  IF "Check_Authorization"(pvar_modifieduser, 'TaskActionLog', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('TaskActionLog', NOW(),
(SELECT query_to_xml('SELECT * FROM TaskActionLog WHERE TaskActionLog.TaskActionLogid= '''||pvar_TaskActionLogid||'''', true, false, '')));

                    
                    UPDATE TaskActionLog SET
                    taskname=pvar_taskname
,tasktype=pvar_tasktype
,summary=pvar_summary
,description=pvar_description

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE TaskActionLogid=pvar_TaskActionLogid;

                    

                    


					
							
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
																,'Update_Finance_Task'
																,'Authorization Failed Update_Finance_Task'
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
						,'Update_Finance_Task'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Finance_Task - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

