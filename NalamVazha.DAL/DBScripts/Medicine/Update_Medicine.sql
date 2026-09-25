
			  CREATE OR REPLACE FUNCTION  "Update_Medicine"
			  (
				  pvar_Medicineid uuid
,pvar_tenantid uuid
,
pvar_medicationtype  uuid
,
pvar_medicinename Varchar(128)
,
pvar_price decimal(18,2)
,
pvar_prescriptionrequired Boolean
,
pvar_sideeffect text

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:36*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Medicine', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Medicine', NOW(),
(SELECT query_to_xml('SELECT * FROM Medicine WHERE Medicine.Medicineid= '''||pvar_Medicineid||'''', true, false, '')));

                    
                    UPDATE Medicine SET
                    medicationtype=pvar_medicationtype
,medicinename=pvar_medicinename
,price=pvar_price
,prescriptionrequired=pvar_prescriptionrequired
,sideeffect=pvar_sideeffect

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Medicineid=pvar_Medicineid;

                    

                    


					
							
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
																,'Update_Medicine'
																,'Authorization Failed Update_Medicine'
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
						,'Update_Medicine'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Medicine - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

