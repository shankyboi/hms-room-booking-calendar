
			  CREATE OR REPLACE FUNCTION  "Add_Arrival"
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
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 08:07:23*/
		

			  
                                                                                    if pvar_Arrivalid is null then
                                                                                    pvar_Arrivalid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Arrival', 'create') THEN
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Arrival(
				 ipdnumber
,patient
,room
,estimatedarrival
,bookingstatus
,travelarrangement
,pickupfrom
,wheelchairassistance
,requireddinner
,specialrequest
,paymentstatus
,pendingamount

				 ,createduser
				 ,Arrivalid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_ipdnumber
,pvar_patient
,pvar_room
,pvar_estimatedarrival
,pvar_bookingstatus
,pvar_travelarrangement
,pvar_pickupfrom
,pvar_wheelchairassistance
,pvar_requireddinner
,pvar_specialrequest
,pvar_paymentstatus
,pvar_pendingamount

				 ,pvar_createduser
				 ,pvar_Arrivalid
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
																,'Add_Arrival'
																,'Authorization Failed Add_Arrival'
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
						,'Add_Arrival'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Arrival - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

