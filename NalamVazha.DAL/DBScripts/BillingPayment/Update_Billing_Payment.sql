
			  CREATE OR REPLACE FUNCTION  "Update_Billing_Payment"
			  (
				  pvar_BillingPaymentid uuid
,pvar_tenantid uuid
,
pvar_receiptno Varchar(256)
,
pvar_paymentfor   Varchar(1024)
,
pvar_patientvisit  uuid
,
pvar_patientname  uuid
,
pvar_amount decimal(18,2)
,
pvar_paymentmode   Varchar(1024)
,
pvar_transactionreference Varchar(128)
,
pvar_paymentstatus   Varchar(1024)
,
pvar_collectedby  uuid
,
pvar_counterid  Varchar(1024)
,
pvar_remarks Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:29*/
			  IF "Check_Authorization"(pvar_modifieduser, 'BillingPayment', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from BillingPayment where upper(BillingPayment.receiptno) = upper(pvar_receiptno) and BillingPayment.tenantid=pvar_tenantid  and BillingPayment.BillingPaymentid <> pvar_BillingPaymentid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Receipt No Already Exists.';

																END IF;

               IF(pvar_counterid is not null AND pvar_counterid!='0' AND LENGTH(pvar_counterid)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_counterid, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='counterid'
                                                                and entityname='BillingPayment' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_counterid, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'counterid value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_paymentfor is not null AND pvar_paymentfor!='0' AND LENGTH(pvar_paymentfor)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_paymentfor, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='paymentfor'
                                                                and entityname='BillingPayment' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_paymentfor, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'paymentfor value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_paymentmode is not null AND pvar_paymentmode!='0' AND LENGTH(pvar_paymentmode)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_paymentmode, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='paymentmode'
                                                                and entityname='BillingPayment' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_paymentmode, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'paymentmode value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_paymentstatus is not null AND pvar_paymentstatus!='0' AND LENGTH(pvar_paymentstatus)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_paymentstatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='paymentstatus'
                                                                and entityname='BillingPayment' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_paymentstatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'paymentstatus value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('BillingPayment', NOW(),
(SELECT query_to_xml('SELECT * FROM BillingPayment WHERE BillingPayment.BillingPaymentid= '''||pvar_BillingPaymentid||'''', true, false, '')));

                    
                    UPDATE BillingPayment SET
                    receiptno=pvar_receiptno
,paymentfor=pvar_paymentfor
,patientvisit=pvar_patientvisit
,patientname=pvar_patientname
,amount=pvar_amount
,paymentmode=pvar_paymentmode
,transactionreference=pvar_transactionreference
,paymentstatus=pvar_paymentstatus
,collectedby=pvar_collectedby
,counterid=pvar_counterid
,remarks=pvar_remarks

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE BillingPaymentid=pvar_BillingPaymentid;

                    

                    


					
							
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
																,'Update_Billing_Payment'
																,'Authorization Failed Update_Billing_Payment'
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
						,'Update_Billing_Payment'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Billing_Payment - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

