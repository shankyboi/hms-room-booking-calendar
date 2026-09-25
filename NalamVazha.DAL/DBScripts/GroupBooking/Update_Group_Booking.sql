
			  CREATE OR REPLACE FUNCTION  "Update_Group_Booking"
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

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 06/18/2026 05:34:25*/
			  IF "Check_Authorization"(pvar_modifieduser, 'GroupBooking', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('GroupBooking', NOW(),
(SELECT query_to_xml('SELECT * FROM GroupBooking WHERE GroupBooking.GroupBookingid= '''||pvar_GroupBookingid||'''', true, false, '')));

                    
                    UPDATE GroupBooking SET
                    groupcode=pvar_groupcode
,groupname=pvar_groupname
,countofmembers=pvar_countofmembers

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE GroupBookingid=pvar_GroupBookingid;

                    

                    INSERT INTO history
VALUES('GroupBooking_patientinfo', NOW(),
(SELECT query_to_xml('SELECT * FROM GroupBooking_patientinfo WHERE GroupBooking_patientinfo.GroupBookingid= '''||pvar_GroupBookingid||'''', true, false, '')));

								DELETE FROM  GroupBooking_patientinfo WHERE GroupBookingid=pvar_GroupBookingid;
								
								
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
									
INSERT INTO history
VALUES('GroupBooking_contacts', NOW(),
(SELECT query_to_xml('SELECT * FROM GroupBooking_contacts WHERE GroupBooking_contacts.GroupBookingid= '''||pvar_GroupBookingid||'''', true, false, '')));

								DELETE FROM  GroupBooking_contacts WHERE GroupBookingid=pvar_GroupBookingid;
								
								
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
																,'Update_Group_Booking'
																,'Authorization Failed Update_Group_Booking'
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
						,'Update_Group_Booking'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Group_Booking - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

