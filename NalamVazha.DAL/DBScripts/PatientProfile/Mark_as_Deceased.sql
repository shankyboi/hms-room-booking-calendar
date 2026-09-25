
			  CREATE OR REPLACE FUNCTION  "Mark_as_Deceased"
			  (
				  pvar_PatientProfileid uuid
,pvar_tenantid uuid
,
pvar_registrationid Varchar(256)
,
pvar_deceased Boolean
,
pvar_causeofdeath Varchar(128)
,
pvar_dateandtimeofdeath Timestamp(3)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:57*/
			  IF "Check_Authorization"(pvar_modifieduser, 'PatientProfile', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from PatientProfile where upper(PatientProfile.registrationid) = upper(pvar_registrationid) and PatientProfile.tenantid=pvar_tenantid  and PatientProfile.PatientProfileid <> pvar_PatientProfileid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Registration ID Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('PatientProfile', NOW(),
(SELECT query_to_xml('SELECT * FROM PatientProfile WHERE PatientProfile.PatientProfileid= '''||pvar_PatientProfileid||'''', true, false, '')));

                    
                    UPDATE PatientProfile SET
                    registrationid=pvar_registrationid
,deceased=pvar_deceased
,causeofdeath=pvar_causeofdeath
,dateandtimeofdeath=pvar_dateandtimeofdeath

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE PatientProfileid=pvar_PatientProfileid;

                    

                    


					
							
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
																,'Mark_as_Deceased'
																,'Authorization Failed Mark_as_Deceased'
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
						,'Mark_as_Deceased'
						,'update failed'
						);
                        pvar_returnMessage := 'Mark_as_Deceased - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

