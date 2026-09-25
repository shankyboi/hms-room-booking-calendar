
			  CREATE OR REPLACE FUNCTION  "Update_Department"
			  (
				  pvar_Departmentid uuid
,pvar_tenantid uuid
,
pvar_name Varchar(128)
,
pvar_description Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:38*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Department', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from Department where upper(Department.name) = upper(pvar_name) and Department.tenantid=pvar_tenantid  and Department.Departmentid <> pvar_Departmentid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Department', NOW(),
(SELECT query_to_xml('SELECT * FROM Department WHERE Department.Departmentid= '''||pvar_Departmentid||'''', true, false, '')));

                    
                    UPDATE Department SET
                    name=pvar_name
,description=pvar_description

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Departmentid=pvar_Departmentid;

                    

                    


					
							
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
																,'Update_Department'
																,'Authorization Failed Update_Department'
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
						,'Update_Department'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Department - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

