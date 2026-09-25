
			  CREATE OR REPLACE FUNCTION  "Create_MailLog"
			  (
				  pvar_MailLogsid uuid
,
pvar_entityname Varchar(128)
,
pvar_entityid Varchar(128)
,
pvar_mailfor Varchar(256)
,
pvar_mailsubject Varchar(256)
,
pvar_mailto Varchar(4096)
,
pvar_mailbody text
,
pvar_issent Boolean
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:27*/
		

			  
                                                                                    if pvar_MailLogsid is null then
                                                                                    pvar_MailLogsid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO MailLogs(
				 entityname
,entityid
,mailfor
,mailsubject
,mailto
,mailbody
,issent

				 ,createduser
				 ,MailLogsid
				 
                
			  )
			  VALUES (
 				 pvar_entityname
,pvar_entityid
,pvar_mailfor
,pvar_mailsubject
,pvar_mailto
,pvar_mailbody
,pvar_issent

				 ,pvar_createduser
				 ,pvar_MailLogsid
				 
                   
			  );
			   
               

			  


			  
					 
			  pvar_returnMessage :='201.1';
               
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
						,'Create_MailLog'
						,'insert failed'
						);
                        pvar_returnMessage := 'Create_MailLog - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

