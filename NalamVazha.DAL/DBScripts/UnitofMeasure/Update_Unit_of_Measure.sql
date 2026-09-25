
			  CREATE OR REPLACE FUNCTION  "Update_Unit_of_Measure"
			  (
				  pvar_UnitofMeasureid uuid
,
pvar_unitofmeasurename Varchar(128)
,
pvar_unitofmeasuredesc Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:42*/
			  IF "Check_Authorization"(pvar_modifieduser, 'UnitofMeasure', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from UnitofMeasure where upper(UnitofMeasure.unitofmeasurename) = upper(pvar_unitofmeasurename)  and UnitofMeasure.UnitofMeasureid <> pvar_UnitofMeasureid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Unit of Measure Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('UnitofMeasure', NOW(),
(SELECT query_to_xml('SELECT * FROM UnitofMeasure WHERE UnitofMeasure.UnitofMeasureid= '''||pvar_UnitofMeasureid||'''', true, false, '')));

                    
                    UPDATE UnitofMeasure SET
                    unitofmeasurename=pvar_unitofmeasurename
,unitofmeasuredesc=pvar_unitofmeasuredesc

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE UnitofMeasureid=pvar_UnitofMeasureid;

                    

                    


					
							
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
																,'Update_Unit_of_Measure'
																,'Authorization Failed Update_Unit_of_Measure'
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
						,'Update_Unit_of_Measure'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Unit_of_Measure - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

