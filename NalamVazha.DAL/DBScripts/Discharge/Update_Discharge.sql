
			  CREATE OR REPLACE FUNCTION  "Update_Discharge"
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

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:43:40*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Discharge', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Discharge', NOW(),
(SELECT query_to_xml('SELECT * FROM Discharge WHERE Discharge.Dischargeid= '''||pvar_Dischargeid||'''', true, false, '')));

                    
                    UPDATE Discharge SET
                    ipdnumber=pvar_ipdnumber
,patient=pvar_patient
,room=pvar_room
,discharge=pvar_discharge
,daysofstay=pvar_daysofstay
,pendingamount=pvar_pendingamount
,paymentstatus=pvar_paymentstatus
,refundamount=pvar_refundamount
,refundstatus=pvar_refundstatus
,feedbackstatus=pvar_feedbackstatus

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Dischargeid=pvar_Dischargeid;

                    

                    


					
							
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
																,'Update_Discharge'
																,'Authorization Failed Update_Discharge'
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
						,'Update_Discharge'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Discharge - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

