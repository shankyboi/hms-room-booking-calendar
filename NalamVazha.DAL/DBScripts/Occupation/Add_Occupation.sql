
			  CREATE OR REPLACE FUNCTION  "Add_Occupation"
			  (
				  pvar_Occupationid uuid
,
pvar_occupationname Varchar(128)
,
pvar_occupationdesc Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:35*/
		

			  
                                                                                    if pvar_Occupationid is null then
                                                                                    pvar_Occupationid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Occupation', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from Occupation where upper(Occupation.occupationname::varchar) = upper(pvar_occupationname::varchar))
																THEN

																pvar_returnMessage := pvar_returnMessage||'Occupation Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Occupation(
				 occupationname
,occupationdesc

				 ,createduser
				 ,Occupationid
				 
                
			  )
			  VALUES (
 				 pvar_occupationname
,pvar_occupationdesc

				 ,pvar_createduser
				 ,pvar_Occupationid
				 
                   
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
																,'Add_Occupation'
																,'Authorization Failed Add_Occupation'
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
						,'Add_Occupation'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Occupation - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

