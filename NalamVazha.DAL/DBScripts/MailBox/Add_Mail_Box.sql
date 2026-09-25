
			  CREATE OR REPLACE FUNCTION  "Add_Mail_Box"
			  (
				  pvar_MailBoxid uuid
,pvar_tenantid uuid
,
pvar_senderdisplayname Varchar(128)
,
pvar_senderemail Varchar(128)
,
pvar_password Varchar(128)
,
pvar_emailhostname Varchar(128)
,
pvar_portnumber int
,
pvar_applicableservice Varchar(256)
,
pvar_emailfooter text
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:14*/
		

			  
                                                                                    if pvar_MailBoxid is null then
                                                                                    pvar_MailBoxid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'MailBox', 'create') THEN
			  pvar_returnMessage:='';
			  
              IF(pvar_applicableservice is not null AND pvar_applicableservice!='0' AND LENGTH(pvar_applicableservice)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_applicableservice, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='applicableservice'
                                                                and entityname='MailBox' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_applicableservice, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'applicableservice value is invalid';


                                                                END IF;
                                                            END IF;
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO MailBox(
				 senderdisplayname
,senderemail
,password
,emailhostname
,portnumber
,applicableservice
,emailfooter

				 ,createduser
				 ,MailBoxid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_senderdisplayname
,pvar_senderemail
,pvar_password
,pvar_emailhostname
,pvar_portnumber
,coalesce(pvar_applicableservice,'')
,pvar_emailfooter

				 ,pvar_createduser
				 ,pvar_MailBoxid
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
																,'Add_Mail_Box'
																,'Authorization Failed Add_Mail_Box'
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
						,'Add_Mail_Box'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Mail_Box - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

