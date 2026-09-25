
			  CREATE OR REPLACE FUNCTION  "Update_Blacklist_reason"
			  (
				  pvar_Blacklistreasonid uuid
,pvar_tenantid uuid
,
pvar_reason Varchar(128)
,
pvar_reasondesc Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:47*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Blacklistreason', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Blacklistreason', NOW(),
(SELECT query_to_xml('SELECT * FROM Blacklistreason WHERE Blacklistreason.Blacklistreasonid= '''||pvar_Blacklistreasonid||'''', true, false, '')));

                    
                    UPDATE Blacklistreason SET
                    reason=pvar_reason
,reasondesc=pvar_reasondesc

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Blacklistreasonid=pvar_Blacklistreasonid;

                    

                    


					
							
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
																,'Update_Blacklist_reason'
																,'Authorization Failed Update_Blacklist_reason'
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
						,'Update_Blacklist_reason'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Blacklist_reason - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

