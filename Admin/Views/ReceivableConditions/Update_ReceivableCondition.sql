
			  CREATE OR REPLACE FUNCTION  "Update_ReceivableCondition"
			  (
				  pvar_ReceivableConditionsid uuid
,pvar_tenantid uuid
,
pvar_receivablefor  Varchar(1024)
,
pvar_amount decimal(18,2)
,
pvar_ismandatory Boolean

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/23/2026 09:15:10*/
			  IF "Check_Authorization"(pvar_modifieduser, 'ReceivableConditions', 'edit') THEN


			  pvar_returnMessage:='';

			  
               IF(pvar_receivablefor is not null AND pvar_receivablefor!='0' AND LENGTH(pvar_receivablefor)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_receivablefor, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='receivablefor'
                                                                and entityname='ReceivableConditions' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_receivablefor, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'receivablefor value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('ReceivableConditions', NOW(),
(SELECT query_to_xml('SELECT * FROM ReceivableConditions WHERE ReceivableConditions.ReceivableConditionsid= '''||pvar_ReceivableConditionsid||'''', true, false, '')));

                    
                    UPDATE ReceivableConditions SET
                    receivablefor=pvar_receivablefor
,amount=pvar_amount
,ismandatory=pvar_ismandatory

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE ReceivableConditionsid=pvar_ReceivableConditionsid;

                    

                    


					
							
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
																,'Update_ReceivableCondition'
																,'Authorization Failed Update_ReceivableCondition'
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
						,'Update_ReceivableCondition'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_ReceivableCondition - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

