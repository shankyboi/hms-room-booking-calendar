
			  CREATE OR REPLACE FUNCTION  "Add_Action_Type"
			  (
				  pvar_ActionTypeid uuid
,pvar_tenantid uuid
,
pvar_actiontype Varchar(128)
,
pvar_description Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:14:11*/
		

			  
                                                                                    if pvar_ActionTypeid is null then
                                                                                    pvar_ActionTypeid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'ActionType', 'create') THEN
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO ActionType(
				 actiontype
,description

				 ,createduser
				 ,ActionTypeid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_actiontype
,pvar_description

				 ,pvar_createduser
				 ,pvar_ActionTypeid
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
																,'Add_Action_Type'
																,'Authorization Failed Add_Action_Type'
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
						,'Add_Action_Type'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Action_Type - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

