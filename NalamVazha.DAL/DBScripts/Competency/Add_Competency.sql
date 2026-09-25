
			  CREATE OR REPLACE FUNCTION  "Add_Competency"
			  (
				  pvar_Competencyid uuid
,pvar_tenantid uuid
,
pvar_competencyname Varchar(128)
,
pvar_description Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:27*/
		

			  
                                                                                    if pvar_Competencyid is null then
                                                                                    pvar_Competencyid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Competency', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from Competency where upper(Competency.competencyname::varchar) = upper(pvar_competencyname::varchar) and Competency.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Competency Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Competency(
				 competencyname
,description

				 ,createduser
				 ,Competencyid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_competencyname
,pvar_description

				 ,pvar_createduser
				 ,pvar_Competencyid
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
																,'Add_Competency'
																,'Authorization Failed Add_Competency'
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
						,'Add_Competency'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Competency - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

