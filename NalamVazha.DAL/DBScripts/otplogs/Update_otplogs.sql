
			  CREATE OR REPLACE FUNCTION  "Update_otplogs"
			  (
				  pvar_otplogsid uuid
,
pvar_username Varchar(128)
,
pvar_otpcode int
,
pvar_expirytime Timestamp(3)
,
pvar_isused Boolean

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:45*/
			  IF "Check_Authorization"(pvar_modifieduser, 'otplogs', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('otplogs', NOW(),
(SELECT query_to_xml('SELECT * FROM otplogs WHERE otplogs.otplogsid= '''||pvar_otplogsid||'''', true, false, '')));

                    
                    UPDATE otplogs SET
                    username=pvar_username
,otpcode=pvar_otpcode
,expirytime=pvar_expirytime
,isused=pvar_isused

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE otplogsid=pvar_otplogsid;

                    

                    


					
							
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
																,'Update_otplogs'
																,'Authorization Failed Update_otplogs'
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
						,'Update_otplogs'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_otplogs - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

