
			  -- shifthours widened from int to decimal(18,2) to match Shift.shifthours.
			  -- Changing a RETURNS TABLE column type requires dropping the function first --
			  -- CREATE OR REPLACE cannot alter the return type in place.
			  DROP FUNCTION IF EXISTS "getById_sp_StaffAttendance"(Varchar);

			  CREATE OR REPLACE FUNCTION  "getById_sp_StaffAttendance"
			  (
				  pvar_StaffAttendanceid Varchar
			  )
			  RETURNS TABLE(
                shiftdate date
,shift uuid
,shiftstarttime Varchar
,shiftendtime Varchar
,shifthours decimal(18,2)
,workprofile uuid
,peoplename uuid
,punchdateandtime Timestamp(3)
,earlyinmin decimal
,earlyoutmin decimal
,latemin decimal
,workhours int
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,StaffAttendanceid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:24*/
               
              RETURN QUERY
			  SELECT 
				 StaffAttendance.shiftdate
,StaffAttendance.shift
,StaffAttendance.shiftstarttime
,StaffAttendance.shiftendtime
,StaffAttendance.shifthours
,StaffAttendance.workprofile
,StaffAttendance.peoplename
,StaffAttendance.punchdateandtime
,StaffAttendance.earlyinmin
,StaffAttendance.earlyoutmin
,StaffAttendance.latemin
,StaffAttendance.workhours

				 ,StaffAttendance.createduser,StaffAttendance.createddate,StaffAttendance.modifieduser,StaffAttendance.modifieddate
				 ,StaffAttendance.tenantid
                 ,StaffAttendance.StaffAttendanceid
                    
			  FROM StaffAttendance
			  WHERE CAST(StaffAttendance.StaffAttendanceid AS Varchar)=pvar_StaffAttendanceid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

