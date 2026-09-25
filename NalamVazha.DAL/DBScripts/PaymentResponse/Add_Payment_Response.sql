
			  CREATE OR REPLACE FUNCTION  "Add_Payment_Response"
			  (
				  pvar_PaymentResponseid uuid
,pvar_tenantid uuid
,
pvar_paymentrequest  uuid
,
pvar_paymenttype  Varchar(1024)
,
pvar_transactiontime Timestamp(3)
,
pvar_orderid Varchar(128)
,
pvar_paymentid Varchar(128)
,
pvar_status  Varchar(1024)
,
pvar_amount decimal(18,2)
,
pvar_paymentmethod  Varchar(1024)
,
pvar_banktransactionid Varchar(256)
,
pvar_gatewayresponsecode Varchar(128)
,
pvar_gatewayresponsemessage Varchar(1024)
,
pvar_responsesignature Varchar(1024)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:53*/
		

			  
                                                                                    if pvar_PaymentResponseid is null then
                                                                                    pvar_PaymentResponseid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'PaymentResponse', 'create') THEN
			  pvar_returnMessage:='';
			  
              IF(pvar_paymentmethod is not null AND pvar_paymentmethod!='0' AND LENGTH(pvar_paymentmethod)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_paymentmethod, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='paymentmethod'
                                                                and entityname='PaymentResponse' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_paymentmethod, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'paymentmethod value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_paymenttype is not null AND pvar_paymenttype!='0' AND LENGTH(pvar_paymenttype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_paymenttype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='paymenttype'
                                                                and entityname='PaymentResponse' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_paymenttype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'paymenttype value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_status is not null AND pvar_status!='0' AND LENGTH(pvar_status)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_status, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='status'
                                                                and entityname='PaymentResponse' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_status, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'status value is invalid';


                                                                END IF;
                                                            END IF;
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO PaymentResponse(
				 paymentrequest
,paymenttype
,transactiontime
,orderid
,paymentid
,status
,amount
,paymentmethod
,banktransactionid
,gatewayresponsecode
,gatewayresponsemessage
,responsesignature

				 ,createduser
				 ,PaymentResponseid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_paymentrequest
,pvar_paymenttype
,pvar_transactiontime
,pvar_orderid
,pvar_paymentid
,pvar_status
,pvar_amount
,pvar_paymentmethod
,pvar_banktransactionid
,pvar_gatewayresponsecode
,pvar_gatewayresponsemessage
,pvar_responsesignature

				 ,pvar_createduser
				 ,pvar_PaymentResponseid
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
																,'Add_Payment_Response'
																,'Authorization Failed Add_Payment_Response'
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
						,'Add_Payment_Response'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Payment_Response - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

