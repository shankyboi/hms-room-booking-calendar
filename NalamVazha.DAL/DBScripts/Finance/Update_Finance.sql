
			  CREATE OR REPLACE FUNCTION  "Update_Finance"
			  (
				  pvar_Financeid uuid
,pvar_tenantid uuid
,
pvar_paymentdate  uuid
,
pvar_paymentmode  uuid
,
pvar_receiptnumber  uuid
,
pvar_patient  uuid
,
pvar_receivablefor  uuid
,
pvar_bookingreferencenumber  uuid
,
pvar_billedamount  uuid
,
pvar_receivedamount  uuid
,
pvar_pendingamount int
,
pvar_paymentstatus  uuid
,
pvar_collectedby  uuid
,
pvar_refundmode  uuid
,
pvar_refundedamount  uuid
,
pvar_refundedby  uuid
,
pvar_remarks  uuid

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 10:01:30*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Finance', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Finance', NOW(),
(SELECT query_to_xml('SELECT * FROM Finance WHERE Finance.Financeid= '''||pvar_Financeid||'''', true, false, '')));

                    
                    UPDATE Finance SET
                    paymentdate=pvar_paymentdate
,paymentmode=pvar_paymentmode
,receiptnumber=pvar_receiptnumber
,patient=pvar_patient
,receivablefor=pvar_receivablefor
,bookingreferencenumber=pvar_bookingreferencenumber
,billedamount=pvar_billedamount
,receivedamount=pvar_receivedamount
,pendingamount=pvar_pendingamount
,paymentstatus=pvar_paymentstatus
,collectedby=pvar_collectedby
,refundmode=pvar_refundmode
,refundedamount=pvar_refundedamount
,refundedby=pvar_refundedby
,remarks=pvar_remarks

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Financeid=pvar_Financeid;

                    

                    


					
							
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
																,'Update_Finance'
																,'Authorization Failed Update_Finance'
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
						,'Update_Finance'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Finance - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

