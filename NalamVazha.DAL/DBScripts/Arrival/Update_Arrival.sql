
			  CREATE OR REPLACE FUNCTION  "Update_Arrival"
			  (
				  pvar_Arrivalid uuid
,pvar_tenantid uuid
,
pvar_ipdnumber  uuid
,
pvar_patient  uuid
,
pvar_room  uuid
,
pvar_estimatedarrival  uuid
,
pvar_bookingstatus  uuid
,
pvar_travelarrangement  uuid
,
pvar_pickupfrom  uuid
,
pvar_wheelchairassistance  uuid
,
pvar_requireddinner  uuid
,
pvar_specialrequest  uuid
,
pvar_paymentstatus  uuid
,
pvar_pendingamount  uuid

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 08:07:23*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Arrival', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Arrival', NOW(),
(SELECT query_to_xml('SELECT * FROM Arrival WHERE Arrival.Arrivalid= '''||pvar_Arrivalid||'''', true, false, '')));

                    
                    UPDATE Arrival SET
                    ipdnumber=pvar_ipdnumber
,patient=pvar_patient
,room=pvar_room
,estimatedarrival=pvar_estimatedarrival
,bookingstatus=pvar_bookingstatus
,travelarrangement=pvar_travelarrangement
,pickupfrom=pvar_pickupfrom
,wheelchairassistance=pvar_wheelchairassistance
,requireddinner=pvar_requireddinner
,specialrequest=pvar_specialrequest
,paymentstatus=pvar_paymentstatus
,pendingamount=pvar_pendingamount

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Arrivalid=pvar_Arrivalid;

                    

                    


					
							
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
																,'Update_Arrival'
																,'Authorization Failed Update_Arrival'
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
						,'Update_Arrival'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Arrival - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

