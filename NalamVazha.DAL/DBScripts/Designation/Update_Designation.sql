
			  CREATE OR REPLACE FUNCTION  "Update_Designation"
			  (
				  pvar_Designationid uuid
,pvar_tenantid uuid
,
pvar_workprofile  uuid
,
pvar_designation Varchar(128)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:33*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Designation', 'edit') THEN


			  pvar_returnMessage:='';

			  IF "DesignationAlreadyExists"(pvar_tenantid, pvar_workprofile, pvar_designation, pvar_Designationid) THEN
				  pvar_returnMessage := 'Designation already exists for selected Work Profile';
			  END IF;

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Designation', NOW(),
(SELECT query_to_xml('SELECT * FROM Designation WHERE Designation.Designationid= '''||pvar_Designationid||'''', true, false, '')));

                    
                    UPDATE Designation SET
                    workprofile=pvar_workprofile
,designation=pvar_designation

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Designationid=pvar_Designationid;

                    

                    


					
							
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
																,'Update_Designation'
																,'Authorization Failed Update_Designation'
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
						,'Update_Designation'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Designation - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

