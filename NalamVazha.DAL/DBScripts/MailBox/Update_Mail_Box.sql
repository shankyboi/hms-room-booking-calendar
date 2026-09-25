
			  CREATE OR REPLACE FUNCTION  "Update_Mail_Box"
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

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:14*/
			  IF "Check_Authorization"(pvar_modifieduser, 'MailBox', 'edit') THEN


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
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('MailBox', NOW(),
(SELECT query_to_xml('SELECT * FROM MailBox WHERE MailBox.MailBoxid= '''||pvar_MailBoxid||'''', true, false, '')));

                    
                    UPDATE MailBox SET
                    senderdisplayname=pvar_senderdisplayname
,senderemail=pvar_senderemail
,password=pvar_password
,emailhostname=pvar_emailhostname
,portnumber=pvar_portnumber
,applicableservice=coalesce(pvar_applicableservice,'')
,emailfooter=pvar_emailfooter

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE MailBoxid=pvar_MailBoxid;

                    

                    


					
							
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
																,'Update_Mail_Box'
																,'Authorization Failed Update_Mail_Box'
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
						,'Update_Mail_Box'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Mail_Box - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

