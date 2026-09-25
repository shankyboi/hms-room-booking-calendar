
			  CREATE OR REPLACE FUNCTION  "Update_Medication_Type"
			  (
				  pvar_MedicationTypeid uuid
,pvar_tenantid uuid
,
pvar_medicationtypename Varchar(128)
,
pvar_medicationtypedescription Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:33*/
			  IF "Check_Authorization"(pvar_modifieduser, 'MedicationType', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from MedicationType where upper(MedicationType.medicationtypename) = upper(pvar_medicationtypename) and MedicationType.tenantid=pvar_tenantid  and MedicationType.MedicationTypeid <> pvar_MedicationTypeid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Medication Type Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('MedicationType', NOW(),
(SELECT query_to_xml('SELECT * FROM MedicationType WHERE MedicationType.MedicationTypeid= '''||pvar_MedicationTypeid||'''', true, false, '')));

                    
                    UPDATE MedicationType SET
                    medicationtypename=pvar_medicationtypename
,medicationtypedescription=pvar_medicationtypedescription

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE MedicationTypeid=pvar_MedicationTypeid;

                    

                    


					
							
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
																,'Update_Medication_Type'
																,'Authorization Failed Update_Medication_Type'
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
						,'Update_Medication_Type'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Medication_Type - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

