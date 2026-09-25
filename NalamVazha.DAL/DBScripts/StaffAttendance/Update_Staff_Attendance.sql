
			  -- shifthours widened from int to decimal(18,2) to match Shift.shifthours, which
			  -- was widened to support fractional-hour shifts (e.g. "8.00", "1.50"). Drop the
			  -- old int-typed overload so this replaces it cleanly.
			  DROP FUNCTION IF EXISTS "Update_Staff_Attendance"(uuid, uuid, date, uuid, character varying, character varying, int, uuid, uuid, timestamp, decimal, decimal, decimal, int, uuid);

			  CREATE OR REPLACE FUNCTION  "Update_Staff_Attendance"
			  (
				  pvar_StaffAttendanceid uuid
,pvar_tenantid uuid
,
pvar_shiftdate date
,
pvar_shift  uuid
,
pvar_shiftstarttime Varchar(256)
,
pvar_shiftendtime Varchar(256)
,
pvar_shifthours decimal(18,2)
,
pvar_workprofile  uuid
,
pvar_peoplename  uuid
,
pvar_punchdateandtime Timestamp(3)
,
pvar_earlyinmin decimal(18,2)
,
pvar_earlyoutmin decimal(18,2)
,
pvar_latemin decimal(18,2)
,
pvar_workhours int

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:24*/
			  IF "Check_Authorization"(pvar_modifieduser, 'StaffAttendance', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('StaffAttendance', NOW(),
(SELECT query_to_xml('SELECT * FROM StaffAttendance WHERE StaffAttendance.StaffAttendanceid= '''||pvar_StaffAttendanceid||'''', true, false, '')));

                    
                    UPDATE StaffAttendance SET
                    shiftdate=pvar_shiftdate
,shift=pvar_shift
,shiftstarttime=pvar_shiftstarttime
,shiftendtime=pvar_shiftendtime
,shifthours=pvar_shifthours
,workprofile=pvar_workprofile
,peoplename=pvar_peoplename
,punchdateandtime=pvar_punchdateandtime
,earlyinmin=pvar_earlyinmin
,earlyoutmin=pvar_earlyoutmin
,latemin=pvar_latemin
,workhours=pvar_workhours

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE StaffAttendanceid=pvar_StaffAttendanceid;

                    

                    


					
							
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
																,'Update_Staff_Attendance'
																,'Authorization Failed Update_Staff_Attendance'
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
						,'Update_Staff_Attendance'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Staff_Attendance - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

