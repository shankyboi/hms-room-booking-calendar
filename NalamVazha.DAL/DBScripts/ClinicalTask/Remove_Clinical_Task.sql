
			  CREATE OR REPLACE FUNCTION  "Remove_Clinical_Task"
			  (
				  pvar_ClinicalTaskid Varchar(50)
				  ,pvar_modifieduser  Varchar(50)
				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:36*/
			  IF "Check_Authorization"(pvar_modifieduser::uuid, 'ClinicalTask', 'delete') THEN
			  
            INSERT INTO history
VALUES('ClinicalTask', NOW(),
(SELECT query_to_xml('SELECT * FROM ClinicalTask WHERE CAST(ClinicalTask.ClinicalTaskid AS VARCHAR)= '''||pvar_ClinicalTaskid||'''', true, false, '')));

			 INSERT INTO history
VALUES('ClinicalTask_taskduration', NOW(),
(SELECT query_to_xml('SELECT * FROM ClinicalTask_taskduration WHERE ClinicalTask_taskduration.ClinicalTaskid= '''||pvar_ClinicalTaskid||'''', true, false, '')));


			 UPDATE ClinicalTask SET
													isdeleted=true ,modifieduser=CAST(pvar_modifieduser AS UUID),modifieddate=NOW()
													WHERE CAST(ClinicalTask.ClinicalTaskid AS VARCHAR)=pvar_ClinicalTaskid;
					 
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
																,'Remove_Clinical_Task'
																,'Authorization Failed Remove_Clinical_Task'
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
						,'Remove_Clinical_Task'
						,'user delete failed'
						);				 
					    pvar_returnMessage := 'user delete failed';*/
				  
			  
 			  END
              $BODY$
              LANGUAGE plpgsql;

