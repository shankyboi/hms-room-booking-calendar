
			  CREATE OR REPLACE FUNCTION  "Add_Payment_Config"
			  (
				  pvar_PaymentConfigid uuid
,pvar_tenantid uuid
,
pvar_paymentgatewayprovider  Varchar(1024)
,pvar_keyinfo json
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:16*/
		

			  
                                                                                    if pvar_PaymentConfigid is null then
                                                                                    pvar_PaymentConfigid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'PaymentConfig', 'create') THEN
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
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO PaymentConfig(
				 paymentgatewayprovider

				 ,createduser
				 ,PaymentConfigid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_paymentgatewayprovider

				 ,pvar_createduser
				 ,pvar_PaymentConfigid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
								
								
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
																,'Add_Payment_Config'
																,'Authorization Failed Add_Payment_Config'
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
						,'Add_Payment_Config'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Payment_Config - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

