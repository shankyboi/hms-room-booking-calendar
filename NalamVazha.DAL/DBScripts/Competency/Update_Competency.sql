
			  CREATE OR REPLACE FUNCTION  "Update_Competency"
			  (
				  pvar_Competencyid uuid
,pvar_tenantid uuid
,
pvar_competencyname Varchar(128)
,
pvar_description Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:27*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Competency', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from Competency where upper(Competency.competencyname) = upper(pvar_competencyname) and Competency.tenantid=pvar_tenantid  and Competency.Competencyid <> pvar_Competencyid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Competency Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Competency', NOW(),
(SELECT query_to_xml('SELECT * FROM Competency WHERE Competency.Competencyid= '''||pvar_Competencyid||'''', true, false, '')));

                    
                    UPDATE Competency SET
                    competencyname=pvar_competencyname
,description=pvar_description

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Competencyid=pvar_Competencyid;

                    

                    


					
							
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
																,'Update_Competency'
																,'Authorization Failed Update_Competency'
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
						,'Update_Competency'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Competency - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

