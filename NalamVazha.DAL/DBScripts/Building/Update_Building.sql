
			  CREATE OR REPLACE FUNCTION  "Update_Building"
			  (
				  pvar_Buildingid uuid
,pvar_tenantid uuid
,
pvar_block  uuid
,
pvar_buildingcode Varchar(128)
,
pvar_buildingname Varchar(128)
,
pvar_buildingdescription text
,
pvar_buildingimage Varchar(256)
,
pvar_buildingnearbylandmark Varchar(128)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:07*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Building', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from Building where upper(Building.buildingcode) = upper(pvar_buildingcode) and Building.tenantid=pvar_tenantid  and Building.Buildingid <> pvar_Buildingid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Building Code Already Exists.';

																END IF;
if EXISTS (SELECT * from Building where upper(Building.buildingname) = upper(pvar_buildingname) and Building.tenantid=pvar_tenantid  and Building.Buildingid <> pvar_Buildingid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Building Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Building', NOW(),
(SELECT query_to_xml('SELECT * FROM Building WHERE Building.Buildingid= '''||pvar_Buildingid||'''', true, false, '')));

                    
                    UPDATE Building SET
                    block=pvar_block
,buildingcode=pvar_buildingcode
,buildingname=pvar_buildingname
,buildingdescription=pvar_buildingdescription
,buildingimage=pvar_buildingimage
,buildingnearbylandmark=pvar_buildingnearbylandmark

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Buildingid=pvar_Buildingid;

                    

                    


					
							
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
																,'Update_Building'
																,'Authorization Failed Update_Building'
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
						,'Update_Building'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Building - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

