
			  CREATE OR REPLACE FUNCTION  "Update_Pincode"
			  (
				  pvar_PincodeMasterid uuid
,
pvar_circlename Varchar(256)
,
pvar_regionname Varchar(256)
,
pvar_divisionname Varchar(256)
,
pvar_officename Varchar(256)
,
pvar_pincode Varchar(256)
,
pvar_officetype Varchar(256)
,
pvar_delivery Varchar(256)
,
pvar_district Varchar(256)
,
pvar_statename Varchar(256)
,
pvar_latitude Varchar(256)
,
pvar_longitude Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/16/2026 09:42:32*/
			  IF "Check_Authorization"(pvar_modifieduser, 'PincodeMaster', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('PincodeMaster', NOW(),
(SELECT query_to_xml('SELECT * FROM PincodeMaster WHERE PincodeMaster.PincodeMasterid= '''||pvar_PincodeMasterid||'''', true, false, '')));

                    
                    UPDATE PincodeMaster SET
                    circlename=pvar_circlename
,regionname=pvar_regionname
,divisionname=pvar_divisionname
,officename=pvar_officename
,pincode=pvar_pincode
,officetype=pvar_officetype
,delivery=pvar_delivery
,district=pvar_district
,statename=pvar_statename
,latitude=pvar_latitude
,longitude=pvar_longitude

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE PincodeMasterid=pvar_PincodeMasterid;

                    

                    


					
							
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
																,'Update_Pincode'
																,'Authorization Failed Update_Pincode'
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
						,'Update_Pincode'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Pincode - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

