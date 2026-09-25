
			  CREATE OR REPLACE FUNCTION  "Add_Actions"
			  (
				  pvar_Actionsid uuid
,pvar_tenantid uuid
,
pvar_actiontype  uuid
,
pvar_actionname Varchar(128)
,
pvar_description Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:16:42*/
		

			  
                                                                                    if pvar_Actionsid is null then
                                                                                    pvar_Actionsid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Actions', 'create') THEN
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Actions(
				 actiontype
,actionname
,description

				 ,createduser
				 ,Actionsid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_actiontype
,pvar_actionname
,pvar_description

				 ,pvar_createduser
				 ,pvar_Actionsid
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
																,'Add_Actions'
																,'Authorization Failed Add_Actions'
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
						,'Add_Actions'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Actions - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

