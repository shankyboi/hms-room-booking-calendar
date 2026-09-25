
			  CREATE OR REPLACE FUNCTION  "Change_Status"
			  (
				  pvar_PatientVisitid uuid
,pvar_tenantid uuid
,
pvar_visitstatus  Varchar(1024)
,
pvar_notes Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:59*/
			  IF "Check_Authorization"(pvar_modifieduser, 'PatientVisit', 'edit') THEN


			  pvar_returnMessage:='';

			  
               IF(pvar_visitstatus is not null AND pvar_visitstatus!='0' AND LENGTH(pvar_visitstatus)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_visitstatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='visitstatus'
                                                                and entityname='PatientVisit' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_visitstatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'visitstatus value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('PatientVisit', NOW(),
(SELECT query_to_xml('SELECT * FROM PatientVisit WHERE PatientVisit.PatientVisitid= '''||pvar_PatientVisitid||'''', true, false, '')));

                    
                    UPDATE PatientVisit SET
                    visitstatus=pvar_visitstatus
,notes=pvar_notes

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE PatientVisitid=pvar_PatientVisitid;

                    

                    


					
							
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
																,'Change_Status'
																,'Authorization Failed Change_Status'
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
						,'Change_Status'
						,'update failed'
						);
                        pvar_returnMessage := 'Change_Status - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

