
			  CREATE OR REPLACE FUNCTION  "Add_Finance"
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
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 10:01:30*/
		

			  
                                                                                    if pvar_Financeid is null then
                                                                                    pvar_Financeid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Finance', 'create') THEN
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Finance(
				 paymentdate
,paymentmode
,receiptnumber
,patient
,receivablefor
,bookingreferencenumber
,billedamount
,receivedamount
,pendingamount
,paymentstatus
,collectedby
,refundmode
,refundedamount
,refundedby
,remarks

				 ,createduser
				 ,Financeid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_paymentdate
,pvar_paymentmode
,pvar_receiptnumber
,pvar_patient
,pvar_receivablefor
,pvar_bookingreferencenumber
,pvar_billedamount
,pvar_receivedamount
,pvar_pendingamount
,pvar_paymentstatus
,pvar_collectedby
,pvar_refundmode
,pvar_refundedamount
,pvar_refundedby
,pvar_remarks

				 ,pvar_createduser
				 ,pvar_Financeid
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
																,'Add_Finance'
																,'Authorization Failed Add_Finance'
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
						,'Add_Finance'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Finance - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

