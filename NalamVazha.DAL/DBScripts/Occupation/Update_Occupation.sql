
			  CREATE OR REPLACE FUNCTION  "Update_Occupation"
			  (
				  pvar_Occupationid uuid
,
pvar_occupationname Varchar(128)
,
pvar_occupationdesc Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:36*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Occupation', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from Occupation where upper(Occupation.occupationname) = upper(pvar_occupationname)  and Occupation.Occupationid <> pvar_Occupationid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Occupation Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Occupation', NOW(),
(SELECT query_to_xml('SELECT * FROM Occupation WHERE Occupation.Occupationid= '''||pvar_Occupationid||'''', true, false, '')));

                    
                    UPDATE Occupation SET
                    occupationname=pvar_occupationname
,occupationdesc=pvar_occupationdesc

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Occupationid=pvar_Occupationid;

                    

                    


					
							
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
																,'Update_Occupation'
																,'Authorization Failed Update_Occupation'
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
						,'Update_Occupation'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Occupation - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

