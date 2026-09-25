
			  CREATE OR REPLACE FUNCTION  "Update_Therapy"
			  (
				  pvar_Therapiesid uuid
,pvar_tenantid uuid
,
pvar_therapycategory  uuid
,
pvar_therapyname Varchar(128)
,
pvar_therapycost decimal(18,2)
,
pvar_standarddurationinmins int
,
pvar_therapyimage Varchar(256)
,
pvar_therapyvideourl Varchar(256)
,
pvar_therapyinstructions text
,
pvar_isgrouptherapyallowed Boolean

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:30*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Therapies', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from Therapies where upper(Therapies.therapyname) = upper(pvar_therapyname) and Therapies.tenantid=pvar_tenantid  and Therapies.Therapiesid <> pvar_Therapiesid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Therapy Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Therapies', NOW(),
(SELECT query_to_xml('SELECT * FROM Therapies WHERE Therapies.Therapiesid= '''||pvar_Therapiesid||'''', true, false, '')));

                    
                    UPDATE Therapies SET
                    therapycategory=pvar_therapycategory
,therapyname=pvar_therapyname
,therapycost=pvar_therapycost
,standarddurationinmins=pvar_standarddurationinmins
,therapyimage=pvar_therapyimage
,therapyvideourl=pvar_therapyvideourl
,therapyinstructions=pvar_therapyinstructions
,isgrouptherapyallowed=pvar_isgrouptherapyallowed

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Therapiesid=pvar_Therapiesid;

                    

                    


					
							
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
																,'Update_Therapy'
																,'Authorization Failed Update_Therapy'
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
						,'Update_Therapy'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Therapy - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

