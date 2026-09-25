
			  CREATE OR REPLACE FUNCTION  "Update_Medical_Condition"
			  (
				  pvar_MedicalConditionid uuid
,
pvar_conditionname Varchar(128)
,
pvar_snomedid Varchar(128)
,
pvar_description Varchar(256)
,pvar_synonyms json

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:54*/
			  IF "Check_Authorization"(pvar_modifieduser, 'MedicalCondition', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from MedicalCondition where upper(MedicalCondition.conditionname) = upper(pvar_conditionname)  and MedicalCondition.MedicalConditionid <> pvar_MedicalConditionid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Condition Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('MedicalCondition', NOW(),
(SELECT query_to_xml('SELECT * FROM MedicalCondition WHERE MedicalCondition.MedicalConditionid= '''||pvar_MedicalConditionid||'''', true, false, '')));

                    
                    UPDATE MedicalCondition SET
                    conditionname=pvar_conditionname
,snomedid=pvar_snomedid
,description=pvar_description

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE MedicalConditionid=pvar_MedicalConditionid;

                    

                    INSERT INTO history
VALUES('MedicalCondition_synonyms', NOW(),
(SELECT query_to_xml('SELECT * FROM MedicalCondition_synonyms WHERE MedicalCondition_synonyms.MedicalConditionid= '''||pvar_MedicalConditionid||'''', true, false, '')));

								DELETE FROM  MedicalCondition_synonyms WHERE MedicalConditionid=pvar_MedicalConditionid;
								
								
								INSERT INTO MedicalCondition_synonyms (
									MedicalConditionid
									,MedicalCondition_synonymsid 
                                    ,record_order  
									,synonymname
,slanguage

									
									)
									SELECT 
									pvar_MedicalConditionid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'synonymname' as synonymname
,j->>'slanguage' as slanguage

									
                                    FROM json_array_elements(pvar_synonyms) as j;
									



					
							
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
																,'Update_Medical_Condition'
																,'Authorization Failed Update_Medical_Condition'
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
						,'Update_Medical_Condition'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Medical_Condition - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

