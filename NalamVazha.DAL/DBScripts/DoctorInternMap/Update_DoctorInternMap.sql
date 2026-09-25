
			  CREATE OR REPLACE FUNCTION  "Update_DoctorInternMap"
			  (
				  pvar_DoctorInternMapid uuid
,pvar_tenantid uuid
,
pvar_seniordoctor  uuid
,
pvar_interndoctor  uuid

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 07/21/2026 11:46:56*/
			  IF "Check_Authorization"(pvar_modifieduser, 'DoctorInternMap', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('DoctorInternMap', NOW(),
(SELECT query_to_xml('SELECT * FROM DoctorInternMap WHERE DoctorInternMap.DoctorInternMapid= '''||pvar_DoctorInternMapid||'''', true, false, '')));

                    
                    UPDATE DoctorInternMap SET
                    seniordoctor=pvar_seniordoctor
,interndoctor=pvar_interndoctor

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE DoctorInternMapid=pvar_DoctorInternMapid;

                    

                    


					
							
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
																,'Update_DoctorInternMap'
																,'Authorization Failed Update_DoctorInternMap'
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
						,'Update_DoctorInternMap'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_DoctorInternMap - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

