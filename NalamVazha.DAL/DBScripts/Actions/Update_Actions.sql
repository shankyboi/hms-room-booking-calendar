
			  CREATE OR REPLACE FUNCTION  "Update_Actions"
			  (
				  pvar_Actionsid uuid
,pvar_tenantid uuid
,
pvar_actiontype  uuid
,
pvar_actionname Varchar(128)
,
pvar_description Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:16:42*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Actions', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Actions', NOW(),
(SELECT query_to_xml('SELECT * FROM Actions WHERE Actions.Actionsid= '''||pvar_Actionsid||'''', true, false, '')));

                    
                    UPDATE Actions SET
                    actiontype=pvar_actiontype
,actionname=pvar_actionname
,description=pvar_description

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Actionsid=pvar_Actionsid;

                    

                    


					
							
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
																,'Update_Actions'
																,'Authorization Failed Update_Actions'
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
						,'Update_Actions'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Actions - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

