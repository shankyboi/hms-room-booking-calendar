
			  CREATE OR REPLACE FUNCTION  "Add_Billing_Payment"
			  (
				  pvar_BillingPaymentid uuid
,pvar_tenantid uuid
,
pvar_paymentdate date
,
pvar_patientname  uuid
,
pvar_patientvisit  uuid
,
pvar_ipdnumber  uuid
,
pvar_opdnumber  uuid
,
pvar_receivablefor   Varchar(1024)
,
pvar_therapy  uuid
,
pvar_therapycost decimal(18,2)
,
pvar_therapykit  uuid
,
pvar_kitprice Varchar(256)
,
pvar_medicine  uuid
,
pvar_price decimal(18,2)
,
pvar_room  uuid
,
pvar_receivedamount decimal(18,2)
,
pvar_currency  Varchar(1024)
,
pvar_conversionrate decimal(18,2)
,
pvar_amount decimal(18,2)
,
pvar_remarks Varchar(256)
,
pvar_paymentmode   Varchar(1024)
,
pvar_transactionreference Varchar(128)
,
pvar_bankname Varchar(128)
,
pvar_chequedddate date
,
pvar_paymentstatus   Varchar(1024)
,
pvar_collectedby  uuid
,
pvar_refundmode  Varchar(1024)
,
pvar_refundedamount decimal(18,2)
,
pvar_refundedby  uuid
,
pvar_refundreferencenumber Varchar(128)
,
pvar_refundbankname Varchar(128)
,
pvar_refundreason Varchar(128)
,
pvar_refundstatus  Varchar(1024)
,
pvar_counterid  Varchar(1024)

				  ,pvar_createduser  uuid

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000)
              AS $BODY$
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/20/2026 16:46:05*/



                                                                                    if pvar_BillingPaymentid is null then
                                                                                    pvar_BillingPaymentid:=gen_random_uuid();
                                                                                    end if;


			  IF "Check_Authorization"(pvar_createduser, 'BillingPayment', 'create') THEN
			  pvar_returnMessage:='';

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
IF(pvar_refundmode is not null AND pvar_refundmode!='0' AND LENGTH(pvar_refundmode)>0)
                                                            THEN
                                                                 if(CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_refundmode, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc
                                                                from lookups  where fieldname='refundmode'
                                                                and entityname='BillingPayment' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_refundmode, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'refundmode value is invalid';


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
IF(pvar_refundstatus is not null AND pvar_refundstatus!='0' AND LENGTH(pvar_refundstatus)>0)
                                                            THEN
                                                                 if(CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_refundstatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc
                                                                from lookups  where fieldname='refundstatus'
                                                                and entityname='BillingPayment' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_refundstatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'refundstatus value is invalid';


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
IF(pvar_currency is not null AND pvar_currency!='0' AND LENGTH(pvar_currency)>0)
                                                            THEN
                                                                 if(CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_currency, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc
                                                                from lookups  where fieldname='currency'
                                                                and entityname='BillingPayment' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_currency, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'currency value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_receivablefor is not null AND pvar_receivablefor!='0' AND LENGTH(pvar_receivablefor)>0)
                                                            THEN
                                                                 if(CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_receivablefor, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc
                                                                from lookups  where fieldname='receivablefor'
                                                                and entityname='BillingPayment' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_receivablefor, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'receivablefor value is invalid';


                                                                END IF;
                                                            END IF;

			  if(pvar_returnMessage='')
			  THEN



			  INSERT INTO BillingPayment(
				 paymentdate
,patientname
,patientvisit
,ipdnumber
,opdnumber
,receivablefor
,therapy
,therapycost
,therapykit
,kitprice
,medicine
,price
,room
,receivedamount
,currency
,conversionrate
,amount
,remarks
,paymentmode
,transactionreference
,bankname
,chequedddate
,paymentstatus
,collectedby
,refundmode
,refundedamount
,refundedby
,refundreferencenumber
,refundbankname
,refundreason
,refundstatus
,counterid

				 ,createduser
				 ,BillingPaymentid
				 ,tenantid

			  )
			  VALUES (
						 pvar_paymentdate
,pvar_patientname
,pvar_patientvisit
,pvar_ipdnumber
,pvar_opdnumber
,pvar_receivablefor
,pvar_therapy
,pvar_therapycost
,pvar_therapykit
,pvar_kitprice
,pvar_medicine
,pvar_price
,pvar_room
,pvar_receivedamount
,pvar_currency
,pvar_conversionrate
,pvar_amount
,pvar_remarks
,pvar_paymentmode
,pvar_transactionreference
,pvar_bankname
,pvar_chequedddate
,pvar_paymentstatus
,pvar_collectedby
,pvar_refundmode
,pvar_refundedamount
,pvar_refundedby
,pvar_refundreferencenumber
,pvar_refundbankname
,pvar_refundreason
,pvar_refundstatus
,pvar_counterid

				 ,pvar_createduser
				 ,pvar_BillingPaymentid
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
																,'Add_Billing_Payment'
																,'Authorization Failed Add_Billing_Payment'
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
						,'Add_Billing_Payment'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Billing_Payment - Insert failed';*/

			  END
              $BODY$
              LANGUAGE plpgsql;
