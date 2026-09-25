
			  -- shifthours widened from int to decimal(18,2): the UI always posts a formatted
			  -- value like "8.00" (to support fractional-hour shifts), which an int param
			  -- rejects outright. Drop the old int-typed overload so this replaces it cleanly.
			  DROP FUNCTION IF EXISTS "Update_Shift"(uuid, uuid, character varying, character varying, character varying, character varying, int, character varying, character varying, decimal, character varying, json, uuid);

			  CREATE OR REPLACE FUNCTION  "Update_Shift"
			  (
				  pvar_Shiftid uuid
,pvar_tenantid uuid
,
pvar_shiftcode Varchar(256)
,
pvar_shiftname Varchar(128)
,
pvar_shiftstarttime Varchar(256)
,
pvar_shiftendtime Varchar(256)
,
pvar_shifthours decimal(18,2)
,
pvar_description Varchar(256)
,
pvar_totalbreakinmins Varchar(256)
,
pvar_totalbreakinhrs decimal(18,2)
,
pvar_workhours Varchar(256)
,pvar_breakdurationdetails json

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:17*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Shift', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from Shift where upper(Shift.shiftcode) = upper(pvar_shiftcode) and Shift.tenantid=pvar_tenantid  and Shift.Shiftid <> pvar_Shiftid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Shift Code Already Exists.';

																END IF;
if EXISTS (SELECT * from Shift where upper(Shift.shiftname) = upper(pvar_shiftname) and Shift.tenantid=pvar_tenantid  and Shift.Shiftid <> pvar_Shiftid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Shift Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Shift', NOW(),
(SELECT query_to_xml('SELECT * FROM Shift WHERE Shift.Shiftid= '''||pvar_Shiftid||'''', true, false, '')));

                    
                    UPDATE Shift SET
                    shiftcode=pvar_shiftcode
,shiftname=pvar_shiftname
,shiftstarttime=pvar_shiftstarttime
,shiftendtime=pvar_shiftendtime
,shifthours=pvar_shifthours
,description=pvar_description
,totalbreakinmins=pvar_totalbreakinmins
,totalbreakinhrs=pvar_totalbreakinhrs
,workhours=pvar_workhours

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Shiftid=pvar_Shiftid;

                    

                    INSERT INTO history
VALUES('Shift_breakdurationdetails', NOW(),
(SELECT query_to_xml('SELECT * FROM Shift_breakdurationdetails WHERE Shift_breakdurationdetails.Shiftid= '''||pvar_Shiftid||'''', true, false, '')));

								DELETE FROM  Shift_breakdurationdetails WHERE Shiftid=pvar_Shiftid;
								
								
								INSERT INTO Shift_breakdurationdetails (
									Shiftid
									,Shift_breakdurationdetailsid 
                                    ,record_order  
									,breakname
,starttime
,endtime
,durationinmin

									
									)
									SELECT 
									pvar_Shiftid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'breakname' as breakname
,j->>'starttime' as starttime
,j->>'endtime' as endtime
,CAST(j->>'durationinmin' AS decimal(18,2)) as durationinmin

									
                                    FROM json_array_elements(pvar_breakdurationdetails) as j;
									



					
							
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
																,'Update_Shift'
																,'Authorization Failed Update_Shift'
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
						,'Update_Shift'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Shift - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

