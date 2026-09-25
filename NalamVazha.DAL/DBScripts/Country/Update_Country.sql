
			  CREATE OR REPLACE FUNCTION  "Update_Country"
			  (
				  pvar_Countryid uuid
,
pvar_countryname Varchar(128)
,
pvar_countrycode Varchar(128)
,
pvar_countryshortcode Varchar(128)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:31*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Country', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Country', NOW(),
(SELECT query_to_xml('SELECT * FROM Country WHERE Country.Countryid= '''||pvar_Countryid||'''', true, false, '')));

                    
                    UPDATE Country SET
                    countryname=pvar_countryname
,countrycode=pvar_countrycode
,countryshortcode=pvar_countryshortcode

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Countryid=pvar_Countryid;

                    

                    


					
							
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
																,'Update_Country'
																,'Authorization Failed Update_Country'
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
						,'Update_Country'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Country - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

