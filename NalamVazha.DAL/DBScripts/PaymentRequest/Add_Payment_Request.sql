
			  CREATE OR REPLACE FUNCTION  "Add_Payment_Request"
			  (
				  pvar_PaymentRequestid uuid
,pvar_tenantid uuid
,
pvar_paymentgateway  Varchar(1024)
,
pvar_requestdatetime Timestamp(3)
,
pvar_patientname  uuid
,
pvar_people  uuid
,
pvar_paymenttype  Varchar(1024)
,
pvar_merchantid Varchar(128)
,
pvar_orderid Varchar(128)
,
pvar_paymentid Varchar(128)
,
pvar_amount decimal(18,2)
,
pvar_currency  Varchar(1024)
,
pvar_customername Varchar(128)
,
pvar_customeremail Varchar(128)
,
pvar_customerphone Varchar(10)
,
pvar_orderpaymentdesc Varchar(256)
,
pvar_returnurl Varchar(256)
,
pvar_notifyurl Varchar(256)
,
pvar_signatureorchecksum Varchar(128)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:48*/
		

			  
                                                                                    if pvar_PaymentRequestid is null then
                                                                                    pvar_PaymentRequestid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'PaymentRequest', 'create') THEN
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
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_paymenttype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='paymenttype'
                                                                and entityname='PaymentRequest' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_paymenttype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'paymenttype value is invalid';


                                                                END IF;
                                                            END IF;
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO PaymentRequest(
				 paymentgateway
,requestdatetime
,patientname
,people
,paymenttype
,merchantid
,orderid
,paymentid
,amount
,currency
,customername
,customeremail
,customerphone
,orderpaymentdesc
,returnurl
,notifyurl
,signatureorchecksum

				 ,createduser
				 ,PaymentRequestid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_paymentgateway
,pvar_requestdatetime
,pvar_patientname
,pvar_people
,pvar_paymenttype
,pvar_merchantid
,pvar_orderid
,pvar_paymentid
,pvar_amount
,pvar_currency
,pvar_customername
,pvar_customeremail
,pvar_customerphone
,pvar_orderpaymentdesc
,pvar_returnurl
,pvar_notifyurl
,pvar_signatureorchecksum

				 ,pvar_createduser
				 ,pvar_PaymentRequestid
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
																,'Add_Payment_Request'
																,'Authorization Failed Add_Payment_Request'
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
						,'Add_Payment_Request'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Payment_Request - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

