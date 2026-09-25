
			  CREATE OR REPLACE FUNCTION  "Add_Medication_Type"
			  (
				  pvar_MedicationTypeid uuid
,pvar_tenantid uuid
,
pvar_medicationtypename Varchar(128)
,
pvar_medicationtypedescription Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:33*/
		

			  
                                                                                    if pvar_MedicationTypeid is null then
                                                                                    pvar_MedicationTypeid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'MedicationType', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from MedicationType where upper(MedicationType.medicationtypename::varchar) = upper(pvar_medicationtypename::varchar) and MedicationType.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Medication Type Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO MedicationType(
				 medicationtypename
,medicationtypedescription

				 ,createduser
				 ,MedicationTypeid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_medicationtypename
,pvar_medicationtypedescription

				 ,pvar_createduser
				 ,pvar_MedicationTypeid
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
																,'Add_Medication_Type'
																,'Authorization Failed Add_Medication_Type'
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
						,'Add_Medication_Type'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Medication_Type - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

