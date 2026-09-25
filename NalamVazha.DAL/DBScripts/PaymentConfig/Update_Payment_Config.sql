
			  CREATE OR REPLACE FUNCTION  "Update_Payment_Config"
			  (
				  pvar_PaymentConfigid uuid
,pvar_tenantid uuid
,
pvar_paymentgatewayprovider  Varchar(1024)
,pvar_keyinfo json

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:16*/
			  IF "Check_Authorization"(pvar_modifieduser, 'PaymentConfig', 'edit') THEN


			  pvar_returnMessage:='';

			  
               IF(pvar_paymentgatewayprovider is not null AND pvar_paymentgatewayprovider!='0' AND LENGTH(pvar_paymentgatewayprovider)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_paymentgatewayprovider, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='paymentgatewayprovider'
                                                                and entityname='PaymentConfig' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_paymentgatewayprovider, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'paymentgatewayprovider value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('PaymentConfig', NOW(),
(SELECT query_to_xml('SELECT * FROM PaymentConfig WHERE PaymentConfig.PaymentConfigid= '''||pvar_PaymentConfigid||'''', true, false, '')));

                    
                    UPDATE PaymentConfig SET
                    paymentgatewayprovider=pvar_paymentgatewayprovider

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE PaymentConfigid=pvar_PaymentConfigid;

                    

                    INSERT INTO history
VALUES('PaymentConfig_keyinfo', NOW(),
(SELECT query_to_xml('SELECT * FROM PaymentConfig_keyinfo WHERE PaymentConfig_keyinfo.PaymentConfigid= '''||pvar_PaymentConfigid||'''', true, false, '')));

								DELETE FROM  PaymentConfig_keyinfo WHERE PaymentConfigid=pvar_PaymentConfigid;
								
								
								INSERT INTO PaymentConfig_keyinfo (
									PaymentConfigid
									,PaymentConfig_keyinfoid 
                                    ,record_order  
									,keytype
,keyid
,keysecret
,returnurl
,notifyurl
,merchantid
,status

									
									)
									SELECT 
									pvar_PaymentConfigid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'keytype' as keytype
,j->>'keyid' as keyid
,j->>'keysecret' as keysecret
,j->>'returnurl' as returnurl
,j->>'notifyurl' as notifyurl
,j->>'merchantid' as merchantid
,j->>'status' as status

									
                                    FROM json_array_elements(pvar_keyinfo) as j;
									



					
							
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
																,'Update_Payment_Config'
																,'Authorization Failed Update_Payment_Config'
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
						,'Update_Payment_Config'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Payment_Config - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

