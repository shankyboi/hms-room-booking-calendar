
			  -- shifthours widened from int to decimal(18,2) to match Shift.shifthours, which
			  -- was widened to support fractional-hour shifts (e.g. "8.00", "1.50"). Drop the
			  -- old int-typed overload so this replaces it cleanly.
			  DROP FUNCTION IF EXISTS "Add_Staff_Attendance"(uuid, uuid, date, uuid, character varying, character varying, int, uuid, uuid, timestamp, decimal, decimal, decimal, int, uuid);

			  CREATE OR REPLACE FUNCTION  "Add_Staff_Attendance"
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
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:24*/
		

			  
                                                                                    if pvar_StaffAttendanceid is null then
                                                                                    pvar_StaffAttendanceid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'StaffAttendance', 'create') THEN
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO StaffAttendance(
				 shiftdate
,shift
,shiftstarttime
,shiftendtime
,shifthours
,workprofile
,peoplename
,punchdateandtime
,earlyinmin
,earlyoutmin
,latemin
,workhours

				 ,createduser
				 ,StaffAttendanceid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_shiftdate
,pvar_shift
,pvar_shiftstarttime
,pvar_shiftendtime
,pvar_shifthours
,pvar_workprofile
,pvar_peoplename
,pvar_punchdateandtime
,pvar_earlyinmin
,pvar_earlyoutmin
,pvar_latemin
,pvar_workhours

				 ,pvar_createduser
				 ,pvar_StaffAttendanceid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
					 
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
																,'Add_Staff_Attendance'
																,'Authorization Failed Add_Staff_Attendance'
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
						,'Add_Staff_Attendance'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Staff_Attendance - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

