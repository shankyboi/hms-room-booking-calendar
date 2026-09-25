
			  CREATE OR REPLACE FUNCTION  "Add_Patient_Consent"
			  (
				  pvar_PatientConsentid uuid
,pvar_tenantid uuid
,
pvar_consenttype  Varchar(1024)
,
pvar_consentlanguage  Varchar(1024)
,
pvar_consentfile Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:52*/
		

			  
                                                                                    if pvar_PatientConsentid is null then
                                                                                    pvar_PatientConsentid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'PatientConsent', 'create') THEN
			  pvar_returnMessage:='';
			  
              IF(pvar_consentlanguage is not null AND pvar_consentlanguage!='0' AND LENGTH(pvar_consentlanguage)>0)
                                                            THEN
                                                                 if(CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_consentlanguage, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select fielddesc
                                                                from lookups where fieldname='consentlanguage'
                                                                and entityname='PatientConsent' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_consentlanguage, ',') AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'consentlanguage value is invalid';
                                                                END IF;
                                                            END IF;

              IF(pvar_consenttype is not null AND pvar_consenttype!='0' AND LENGTH(pvar_consenttype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_consenttype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='consenttype'
                                                                and entityname='PatientConsent' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_consenttype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'consenttype value is invalid';


                                                                END IF;
                                                            END IF;
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO PatientConsent(
consenttype
,consentlanguage
,consentfile

				 ,createduser
				 ,PatientConsentid
				 ,tenantid
                
			  )
			  VALUES (
pvar_consenttype
,pvar_consentlanguage
,pvar_consentfile

				 ,pvar_createduser
				 ,pvar_PatientConsentid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
					 
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
																,'Add_Patient_Consent'
																,'Authorization Failed Add_Patient_Consent'
																,pvar_createduser
																);
																pvar_returnMessage := '401.1';
																
																END IF;
			  /*EXCEPTION WHEN OTHERS THEN
			 
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
						,'Store Proc Exception'
						,NOW()
						,'16'
						,'Add_Patient_Consent'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Patient_Consent - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

