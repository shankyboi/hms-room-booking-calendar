
			  CREATE OR REPLACE FUNCTION  "Add_Discharge"
			  (
				  pvar_Dischargeid uuid
,pvar_tenantid uuid
,
pvar_ipdnumber  uuid
,
pvar_patient  uuid
,
pvar_room  uuid
,
pvar_discharge  uuid
,
pvar_daysofstay int
,
pvar_pendingamount  uuid
,
pvar_paymentstatus  uuid
,
pvar_refundamount  uuid
,
pvar_refundstatus  uuid
,
pvar_feedbackstatus Varchar(128)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:43:40*/
		

			  
                                                                                    if pvar_Dischargeid is null then
                                                                                    pvar_Dischargeid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Discharge', 'create') THEN
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Discharge(
				 ipdnumber
,patient
,room
,discharge
,daysofstay
,pendingamount
,paymentstatus
,refundamount
,refundstatus
,feedbackstatus

				 ,createduser
				 ,Dischargeid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_ipdnumber
,pvar_patient
,pvar_room
,pvar_discharge
,pvar_daysofstay
,pvar_pendingamount
,pvar_paymentstatus
,pvar_refundamount
,pvar_refundstatus
,pvar_feedbackstatus

				 ,pvar_createduser
				 ,pvar_Dischargeid
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
																,'Add_Discharge'
																,'Authorization Failed Add_Discharge'
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
						,'Add_Discharge'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Discharge - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

