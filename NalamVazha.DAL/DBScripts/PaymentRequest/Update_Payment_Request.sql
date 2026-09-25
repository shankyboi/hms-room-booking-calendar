
CREATE OR REPLACE FUNCTION public."Update_Payment_Request"(
	pvar_paymentrequestid uuid,
	pvar_tenantid uuid,
	pvar_paymentgateway character varying,
	pvar_requestdatetime timestamp without time zone,
	pvar_patientname uuid,
	pvar_people uuid,
	pvar_paymenttype character varying,
	pvar_merchantid character varying,
	pvar_orderid character varying,
	pvar_paymentid character varying,
	pvar_amount numeric,
	pvar_currency character varying,
	pvar_customername character varying,
	pvar_customeremail character varying,
	pvar_customerphone character varying,
	pvar_orderpaymentdesc character varying,
	pvar_returnurl character varying,
	pvar_notifyurl character varying,
	pvar_signatureorchecksum character varying,
	pvar_modifieduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 08:09:46*/
			  IF "Check_Authorization"(pvar_modifieduser, 'PaymentRequest', 'edit') THEN

			  pvar_returnMessage:='';

			  
               IF(pvar_currency is not null AND pvar_currency!='0' AND LENGTH(pvar_currency)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_currency, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='currency'
                                                                and entityname='PaymentRequest' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_currency, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'currency value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_paymentgateway is not null AND pvar_paymentgateway!='0' AND LENGTH(pvar_paymentgateway)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_paymentgateway, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='paymentgateway'
                                                                and entityname='PaymentRequest' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_paymentgateway, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'paymentgateway value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_paymenttype is not null AND pvar_paymenttype!='0' AND LENGTH(pvar_paymenttype)>0)
                                                            THEN                        
                                                                 if EXISTS (
                                                                    SELECT 1
                                                                    FROM regexp_split_to_table(pvar_paymenttype, ',') AS T1
                                                                    WHERE NOT EXISTS (
                                                                        SELECT 1
                                                                        FROM regexp_split_to_table((SELECT fielddesc
                                                                            FROM lookups
                                                                            WHERE fieldname='paymenttype'
                                                                            AND entityname='PaymentRequest'
                                                                            LIMIT 1), ',') AS T2
                                                                        WHERE lower(trim(T1.T1)) = lower(trim(T2.T2))
                                                                    )
                                                                    AND lower(trim(T1.T1)) NOT IN (
                                                                        'balance payment',
                                                                        'booking deposit',
                                                                        'ipd balance payment',
                                                                        'ipd booking deposit',
                                                                        'ipd booking advance',
                                                                        'ipd advance and deposit',
                                                                        'op follow up',
                                                                        'online op new',
                                                                        'online op follow up',
                                                                        'room booking advance'
                                                                    )
                                                                 )
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'paymenttype value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('PaymentRequest', NOW(),
(SELECT query_to_xml('SELECT * FROM PaymentRequest WHERE PaymentRequest.PaymentRequestid= '''||pvar_PaymentRequestid||'''', true, false, '')));

                    
                    UPDATE PaymentRequest SET
                    paymentgateway=pvar_paymentgateway
,requestdatetime=pvar_requestdatetime
,patientname=pvar_patientname
,people=pvar_people
,paymenttype=pvar_paymenttype
,merchantid=pvar_merchantid
,orderid=pvar_orderid
,paymentid=pvar_paymentid
,amount=pvar_amount
,currency=pvar_currency
,customername=pvar_customername
,customeremail=pvar_customeremail
,customerphone=pvar_customerphone
,orderpaymentdesc=pvar_orderpaymentdesc
,returnurl=pvar_returnurl
,notifyurl=pvar_notifyurl
,signatureorchecksum=pvar_signatureorchecksum

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE PaymentRequestid=pvar_PaymentRequestid;

                    

                    

					
							
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
																,'Update_Payment_Request'
																,'Authorization Failed Update_Payment_Request'
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
						,'Update_Payment_Request'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Payment_Request - update failed';*/
			  	
			  END
              
$BODY$;
