
			  CREATE OR REPLACE FUNCTION  "Update_Appoinment"
			  (
				  pvar_Appoinmentid uuid
,pvar_tenantid uuid
,
pvar_patient  uuid
,
pvar_origin  uuid
,
pvar_bookingreferencenumber  uuid
,
pvar_doctor  uuid
,
pvar_appointmentdate  uuid
,
pvar_task  uuid
,
pvar_duration  uuid
,
pvar_status  uuid

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:54:47*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Appoinment', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Appoinment', NOW(),
(SELECT query_to_xml('SELECT * FROM Appoinment WHERE Appoinment.Appoinmentid= '''||pvar_Appoinmentid||'''', true, false, '')));

                    
                    UPDATE Appoinment SET
                    patient=pvar_patient
,origin=pvar_origin
,bookingreferencenumber=pvar_bookingreferencenumber
,doctor=pvar_doctor
,appointmentdate=pvar_appointmentdate
,task=pvar_task
,duration=pvar_duration
,status=pvar_status

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Appoinmentid=pvar_Appoinmentid;

                    

                    


					
							
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
																,'Update_Appoinment'
																,'Authorization Failed Update_Appoinment'
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
						,'Update_Appoinment'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Appoinment - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

