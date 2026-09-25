
			  CREATE OR REPLACE FUNCTION  "Update_Patient_Visit"
			  (
				  pvar_PatientVisitid uuid
,pvar_tenantid uuid
,
pvar_visitnumber Varchar(256)
,
pvar_visitdatetime Timestamp(3)
,
pvar_patientname  uuid
,
pvar_visittype   Varchar(1024)
,
pvar_ipdnumber  uuid
,
pvar_opdnumber  uuid
,
pvar_consultingdoctor  uuid
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

			  if EXISTS (SELECT * from PatientVisit where upper(PatientVisit.visitnumber) = upper(pvar_visitnumber) and PatientVisit.tenantid=pvar_tenantid  and PatientVisit.PatientVisitid <> pvar_PatientVisitid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Visit Number Already Exists.';

																END IF;

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
IF(pvar_visittype is not null AND pvar_visittype!='0' AND LENGTH(pvar_visittype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_visittype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='visittype'
                                                                and entityname='PatientVisit' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_visittype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'visittype value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('PatientVisit', NOW(),
(SELECT query_to_xml('SELECT * FROM PatientVisit WHERE PatientVisit.PatientVisitid= '''||pvar_PatientVisitid||'''', true, false, '')));

                    
                    UPDATE PatientVisit SET
                    visitnumber=pvar_visitnumber
,visitdatetime=pvar_visitdatetime
,patientname=pvar_patientname
,visittype=pvar_visittype
,ipdnumber=pvar_ipdnumber
,opdnumber=pvar_opdnumber
,consultingdoctor=pvar_consultingdoctor
,visitstatus=pvar_visitstatus
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
																,'Update_Patient_Visit'
																,'Authorization Failed Update_Patient_Visit'
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
						,'Update_Patient_Visit'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Patient_Visit - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

