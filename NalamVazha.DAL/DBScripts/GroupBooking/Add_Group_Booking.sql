
			  CREATE OR REPLACE FUNCTION  "Add_Group_Booking"
			  (
				  pvar_GroupBookingid uuid
,pvar_tenantid uuid
,
pvar_groupcode Varchar(128)
,
pvar_groupname Varchar(128)
,
pvar_countofmembers int
,pvar_patientinfo json
,pvar_contacts json
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 06/18/2026 05:34:25*/
		

			  
                                                                                    if pvar_GroupBookingid is null then
                                                                                    pvar_GroupBookingid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'GroupBooking', 'create') THEN
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO GroupBooking(
				 groupcode
,groupname
,countofmembers

				 ,createduser
				 ,GroupBookingid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_groupcode
,pvar_groupname
,pvar_countofmembers

				 ,pvar_createduser
				 ,pvar_GroupBookingid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
								
								
								INSERT INTO GroupBooking_patientinfo (
									GroupBookingid
									,GroupBooking_patientinfoid 
                                    ,record_order  
									,name
,emailid
,phonenumber

									
									)
									SELECT 
									pvar_GroupBookingid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'name' as name
,j->>'emailid' as emailid
,j->>'phonenumber' as phonenumber

									
                                    FROM json_array_elements(pvar_patientinfo) as j;
									

								
								
								INSERT INTO GroupBooking_contacts (
									GroupBookingid
									,GroupBooking_contactsid 
                                    ,record_order  
									,person
,mobile
,email

									
									)
									SELECT 
									pvar_GroupBookingid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'person' as person
,j->>'mobile' as mobile
,j->>'email' as email

									
                                    FROM json_array_elements(pvar_contacts) as j;
									

					 
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
																,'Add_Group_Booking'
																,'Authorization Failed Add_Group_Booking'
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
						,'Add_Group_Booking'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Group_Booking - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

