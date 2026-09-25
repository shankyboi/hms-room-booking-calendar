
			  CREATE OR REPLACE FUNCTION  "Create_Alert_Templates"
			  (
				  pvar_AlertTemplatesid uuid
,
pvar_entityname Varchar(128)
,
pvar_entityaction Varchar(128)
,
pvar_sendasbatch Boolean
,
pvar_alerttype  Varchar(1024)
,
pvar_alertsubject Varchar(256)
,
pvar_alertcopyto Varchar(50)
,
pvar_alertcarboncopyto Varchar(50)
,
pvar_alertcontent text
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:25*/
		

			  
                                                                                    if pvar_AlertTemplatesid is null then
                                                                                    pvar_AlertTemplatesid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'AlertTemplates', 'create') THEN
			  pvar_returnMessage:='';
			  
              IF(pvar_alerttype is not null AND pvar_alerttype!='0' AND LENGTH(pvar_alerttype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_alerttype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='alerttype'
                                                                and entityname='AlertTemplates' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_alerttype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'alerttype value is invalid';


                                                                END IF;
                                                            END IF;
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO AlertTemplates(
				 entityname
,entityaction
,sendasbatch
,alerttype
,alertsubject
,alertcopyto
,alertcarboncopyto
,alertcontent

				 ,createduser
				 ,AlertTemplatesid
				 
                
			  )
			  VALUES (
 				 pvar_entityname
,pvar_entityaction
,pvar_sendasbatch
,pvar_alerttype
,pvar_alertsubject
,pvar_alertcopyto
,pvar_alertcarboncopyto
,pvar_alertcontent

				 ,pvar_createduser
				 ,pvar_AlertTemplatesid
				 
                   
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
																,'Create_Alert_Templates'
																,'Authorization Failed Create_Alert_Templates'
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
						,'Create_Alert_Templates'
						,'insert failed'
						);
                        pvar_returnMessage := 'Create_Alert_Templates - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

