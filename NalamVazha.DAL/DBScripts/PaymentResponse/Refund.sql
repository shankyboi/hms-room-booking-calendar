
			  CREATE OR REPLACE FUNCTION  "Refund"
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
pvar_refundedamount decimal(18,2)
,
pvar_refundreason Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:53*/
			  IF "Check_Authorization"(pvar_modifieduser, 'PaymentResponse', 'edit') THEN


			  pvar_returnMessage:='';

			  
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
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('PaymentResponse', NOW(),
(SELECT query_to_xml('SELECT * FROM PaymentResponse WHERE PaymentResponse.PaymentResponseid= '''||pvar_PaymentResponseid||'''', true, false, '')));

                    
                    UPDATE PaymentResponse SET
                    paymentrequest=pvar_paymentrequest
,paymenttype=pvar_paymenttype
,transactiontime=pvar_transactiontime
,orderid=pvar_orderid
,paymentid=pvar_paymentid
,refundedamount=pvar_refundedamount
,refundreason=pvar_refundreason

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE PaymentResponseid=pvar_PaymentResponseid;

                    

                    


					
							
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
																,'Refund'
																,'Authorization Failed Refund'
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
						,'Refund'
						,'update failed'
						);
                        pvar_returnMessage := 'Refund - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

