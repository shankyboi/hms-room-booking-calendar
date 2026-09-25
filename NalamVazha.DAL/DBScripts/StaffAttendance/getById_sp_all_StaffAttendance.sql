
			  -- shifthours widened from int to decimal(18,2) to match Shift.shifthours.
			  -- Changing a RETURNS TABLE column type requires dropping the function first --
			  -- CREATE OR REPLACE cannot alter the return type in place.
			  DROP FUNCTION IF EXISTS "getById_sp_all_StaffAttendance"(Varchar);

			  CREATE OR REPLACE FUNCTION  "getById_sp_all_StaffAttendance"
              (
			  pvar_StaffAttendanceid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"StaffAttendanceid" uuid
,shiftdate Varchar
,shift Varchar
,shiftstarttime Varchar
,shiftendtime Varchar
,shifthours decimal(18,2)
,workprofile Varchar
,peoplename Varchar
,punchdateandtime Varchar
,earlyinmin decimal
,earlyoutmin decimal
,latemin decimal
,workhours int
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:24*/
			  		 
              RETURN QUERY
			  SELECT  
				 StaffAttendance.tenantid
,tenant.businessname as _tenantname
,StaffAttendance.StaffAttendanceid
,CAST(COALESCE(to_char(StaffAttendance.shiftdate,'dd/MM/yyyy'),'') AS Varchar) as shiftdate
,CAST(_Shift.shiftname AS VARCHAR) as shift
,StaffAttendance.shiftstarttime
,StaffAttendance.shiftendtime
,StaffAttendance.shifthours
,CAST(__WorkProfile.workprofilename AS VARCHAR) as workprofile
,CAST(___People.firstname||' '||___People.lastname AS VARCHAR) as peoplename
,CAST(COALESCE(to_char(StaffAttendance.punchdateandtime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as punchdateandtime
,StaffAttendance.earlyinmin
,StaffAttendance.earlyoutmin
,StaffAttendance.latemin
,StaffAttendance.workhours

				 ,StaffAttendance.createduser,StaffAttendance.createddate,StaffAttendance.modifieduser,StaffAttendance.modifieddate
                 
                 
				 
			  FROM  StaffAttendance 
 LEFT OUTER JOIN tenant ON StaffAttendance.tenantid=tenant.tenantid
INNER JOIN Shift _Shift ON StaffAttendance.shift=_Shift.Shiftid
INNER JOIN WorkProfile __WorkProfile ON StaffAttendance.workprofile=__WorkProfile.WorkProfileid
INNER JOIN People ___People ON StaffAttendance.peoplename=___People.Peopleid

			  WHERE CAST(StaffAttendance.StaffAttendanceid AS Varchar)=pvar_StaffAttendanceid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

