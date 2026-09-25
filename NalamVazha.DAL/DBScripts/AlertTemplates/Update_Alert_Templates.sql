
			  CREATE OR REPLACE FUNCTION  "Update_Alert_Templates"
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

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:25*/
			  IF "Check_Authorization"(pvar_modifieduser, 'AlertTemplates', 'edit') THEN


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
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('AlertTemplates', NOW(),
(SELECT query_to_xml('SELECT * FROM AlertTemplates WHERE AlertTemplates.AlertTemplatesid= '''||pvar_AlertTemplatesid||'''', true, false, '')));

                    
                    UPDATE AlertTemplates SET
                    entityname=pvar_entityname
,entityaction=pvar_entityaction
,sendasbatch=pvar_sendasbatch
,alerttype=pvar_alerttype
,alertsubject=pvar_alertsubject
,alertcopyto=pvar_alertcopyto
,alertcarboncopyto=pvar_alertcarboncopyto
,alertcontent=pvar_alertcontent

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE AlertTemplatesid=pvar_AlertTemplatesid;

                    

                    


					
							
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
																,'Update_Alert_Templates'
																,'Authorization Failed Update_Alert_Templates'
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
						,'Update_Alert_Templates'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Alert_Templates - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

