
			  CREATE OR REPLACE FUNCTION  "Add_otplogs"
			  (
				  pvar_otplogsid uuid
,
pvar_username Varchar(128)
,
pvar_otpcode int
,
pvar_expirytime Timestamp(3)
,
pvar_isused Boolean
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:45*/
		

			  
                                                                                    if pvar_otplogsid is null then
                                                                                    pvar_otplogsid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'otplogs', 'create') THEN
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO otplogs(
				 username
,otpcode
,expirytime
,isused

				 ,createduser
				 ,otplogsid
				 
                
			  )
			  VALUES (
 				 pvar_username
,pvar_otpcode
,pvar_expirytime
,pvar_isused

				 ,pvar_createduser
				 ,pvar_otplogsid
				 
                   
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
																,'Add_otplogs'
																,'Authorization Failed Add_otplogs'
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
						,'Add_otplogs'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_otplogs - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

