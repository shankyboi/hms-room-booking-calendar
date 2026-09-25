
			  CREATE OR REPLACE FUNCTION  "Add_Medical_Condition"
			  (
				  pvar_MedicalConditionid uuid
,
pvar_conditionname Varchar(128)
,
pvar_snomedid Varchar(128)
,
pvar_description Varchar(256)
,pvar_synonyms json
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:54*/
		

			  
                                                                                    if pvar_MedicalConditionid is null then
                                                                                    pvar_MedicalConditionid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'MedicalCondition', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from MedicalCondition where upper(MedicalCondition.conditionname::varchar) = upper(pvar_conditionname::varchar))
																THEN

																pvar_returnMessage := pvar_returnMessage||'Condition Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO MedicalCondition(
				 conditionname
,snomedid
,description

				 ,createduser
				 ,MedicalConditionid
				 
                
			  )
			  VALUES (
 				 pvar_conditionname
,pvar_snomedid
,pvar_description

				 ,pvar_createduser
				 ,pvar_MedicalConditionid
				 
                   
			  );
			   
               

			  


			  
								
								
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
																,'Add_Medical_Condition'
																,'Authorization Failed Add_Medical_Condition'
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
						,'Add_Medical_Condition'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Medical_Condition - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

