
			  CREATE OR REPLACE FUNCTION  "Add_Medicine"
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
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:36*/
		

			  
                                                                                    if pvar_Medicineid is null then
                                                                                    pvar_Medicineid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Medicine', 'create') THEN
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Medicine(
				 medicationtype
,medicinename
,price
,prescriptionrequired
,sideeffect

				 ,createduser
				 ,Medicineid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_medicationtype
,pvar_medicinename
,pvar_price
,pvar_prescriptionrequired
,pvar_sideeffect

				 ,pvar_createduser
				 ,pvar_Medicineid
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
																,'Add_Medicine'
																,'Authorization Failed Add_Medicine'
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
						,'Add_Medicine'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Medicine - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

