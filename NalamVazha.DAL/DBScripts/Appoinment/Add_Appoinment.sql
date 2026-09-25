
			  CREATE OR REPLACE FUNCTION  "Add_Appoinment"
			  (
				  pvar_Appoinmentid uuid
,pvar_tenantid uuid
,
pvar_patient  uuid
,
pvar_origin  uuid
,
pvar_bookingreferencenumber  uuid
,
pvar_doctor  uuid
,
pvar_appointmentdate  uuid
,
pvar_task  uuid
,
pvar_duration  uuid
,
pvar_status  uuid
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:54:47*/
		

			  
                                                                                    if pvar_Appoinmentid is null then
                                                                                    pvar_Appoinmentid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Appoinment', 'create') THEN
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Appoinment(
				 patient
,origin
,bookingreferencenumber
,doctor
,appointmentdate
,task
,duration
,status

				 ,createduser
				 ,Appoinmentid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_patient
,pvar_origin
,pvar_bookingreferencenumber
,pvar_doctor
,pvar_appointmentdate
,pvar_task
,pvar_duration
,pvar_status

				 ,pvar_createduser
				 ,pvar_Appoinmentid
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
																,'Add_Appoinment'
																,'Authorization Failed Add_Appoinment'
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
						,'Add_Appoinment'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Appoinment - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

