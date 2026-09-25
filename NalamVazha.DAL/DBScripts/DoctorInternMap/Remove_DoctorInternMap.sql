
			  CREATE OR REPLACE FUNCTION  "Remove_DoctorInternMap"
			  (
				  pvar_DoctorInternMapid Varchar(50)
				  ,pvar_modifieduser  Varchar(50)
				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 07/21/2026 11:46:56*/
			  IF "Check_Authorization"(pvar_modifieduser::uuid, 'DoctorInternMap', 'delete') THEN
			  
            INSERT INTO history
VALUES('DoctorInternMap', NOW(),
(SELECT query_to_xml('SELECT * FROM DoctorInternMap WHERE CAST(DoctorInternMap.DoctorInternMapid AS VARCHAR)= '''||pvar_DoctorInternMapid||'''', true, false, '')));

			 
			 UPDATE DoctorInternMap SET
													isdeleted=true ,modifieduser=CAST(pvar_modifieduser AS UUID),modifieddate=NOW()
													WHERE CAST(DoctorInternMap.DoctorInternMapid AS VARCHAR)=pvar_DoctorInternMapid;
					 
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
																,'Remove_DoctorInternMap'
																,'Authorization Failed Remove_DoctorInternMap'
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
						,'Remove_DoctorInternMap'
						,'user delete failed'
						);				 
					    pvar_returnMessage := 'user delete failed';*/
				  
			  
 			  END
              $BODY$
              LANGUAGE plpgsql;

