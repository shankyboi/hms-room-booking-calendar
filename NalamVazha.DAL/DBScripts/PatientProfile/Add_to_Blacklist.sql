
			  CREATE OR REPLACE FUNCTION  "Add_to_Blacklist"
			  (
				  pvar_PatientProfileid uuid
,pvar_tenantid uuid
,
pvar_registrationid Varchar(256)
,
pvar_blacklisted  Varchar(1024)
,
pvar_reasonforblacklisting  uuid
,
pvar_detailedremarks Varchar(256)

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

               IF(pvar_blacklisted is not null AND pvar_blacklisted!='0' AND LENGTH(pvar_blacklisted)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_blacklisted, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='blacklisted'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_blacklisted, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'blacklisted value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('PatientProfile', NOW(),
(SELECT query_to_xml('SELECT * FROM PatientProfile WHERE PatientProfile.PatientProfileid= '''||pvar_PatientProfileid||'''', true, false, '')));

                    
                    UPDATE PatientProfile SET
                    registrationid=pvar_registrationid
,blacklisted=pvar_blacklisted
,reasonforblacklisting=pvar_reasonforblacklisting
,detailedremarks=pvar_detailedremarks

                    
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
																,'Add_to_Blacklist'
																,'Authorization Failed Add_to_Blacklist'
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
						,'Add_to_Blacklist'
						,'update failed'
						);
                        pvar_returnMessage := 'Add_to_Blacklist - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

