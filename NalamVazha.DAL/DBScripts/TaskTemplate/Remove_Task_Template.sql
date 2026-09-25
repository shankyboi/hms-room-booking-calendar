
			  CREATE OR REPLACE FUNCTION  "Remove_Task_Template"
			  (
				  pvar_TaskTemplateid Varchar(50)
				  ,pvar_modifieduser  Varchar(50)
				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:27:26*/
			  IF "Check_Authorization"(pvar_modifieduser::uuid, 'TaskTemplate', 'delete') THEN
			  
            INSERT INTO history
VALUES('TaskTemplate', NOW(),
(SELECT query_to_xml('SELECT * FROM TaskTemplate WHERE CAST(TaskTemplate.TaskTemplateid AS VARCHAR)= '''||pvar_TaskTemplateid||'''', true, false, '')));

			 INSERT INTO history
VALUES('TaskTemplate_escalationdetails', NOW(),
(SELECT query_to_xml('SELECT * FROM TaskTemplate_escalationdetails WHERE TaskTemplate_escalationdetails.TaskTemplateid= '''||pvar_TaskTemplateid||'''', true, false, '')));

INSERT INTO history
VALUES('TaskTemplate_nextactiondetails', NOW(),
(SELECT query_to_xml('SELECT * FROM TaskTemplate_nextactiondetails WHERE TaskTemplate_nextactiondetails.TaskTemplateid= '''||pvar_TaskTemplateid||'''', true, false, '')));


			 UPDATE TaskTemplate SET
													isdeleted=true ,modifieduser=CAST(pvar_modifieduser AS UUID),modifieddate=NOW()
													WHERE CAST(TaskTemplate.TaskTemplateid AS VARCHAR)=pvar_TaskTemplateid;
					 
				  pvar_returnMessage := '201.1';
					 
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
																,'Remove_Task_Template'
																,'Authorization Failed Remove_Task_Template'
																,pvar_modifieduser
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
						,'Postgre Function Exception'
						,NOW()
						,'16'
						,'Remove_Task_Template'
						,'user delete failed'
						);				 
					    pvar_returnMessage := 'user delete failed';*/
				  
			  
 			  END
              $BODY$
              LANGUAGE plpgsql;

