
			  -- shifthours widened from int to decimal(18,2) to support fractional-hour shifts.
			  -- Changing a RETURNS TABLE column type requires dropping the function first --
			  -- CREATE OR REPLACE cannot alter the return type in place.
			  DROP FUNCTION IF EXISTS "getById_sp_all_Shift"(Varchar);

			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Shift"
              (
			  pvar_Shiftid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Shiftid" uuid
,shiftcode Varchar
,shiftname Varchar
,shiftstarttime Varchar
,shiftendtime Varchar
,shifthours decimal(18,2)
,description Varchar
,totalbreakinmins Varchar
,totalbreakinhrs decimal
,workhours Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"automaton_Shift_breakdurationdetails" json
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:17*/
			  		 
              RETURN QUERY
			  SELECT  
				 Shift.tenantid
,tenant.businessname as _tenantname
,Shift.Shiftid
,Shift.shiftcode
,Shift.shiftname
,Shift.shiftstarttime
,Shift.shiftendtime
,Shift.shifthours
,Shift.description
,Shift.totalbreakinmins
,Shift.totalbreakinhrs
,Shift.workhours

				 ,Shift.createduser,Shift.createddate,Shift.modifieduser,Shift.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						Shift_breakdurationdetails.breakname as "Break Name"
,Shift_breakdurationdetails.starttime as "Start Time"
,Shift_breakdurationdetails.endtime as "End Time"
,Shift_breakdurationdetails.durationinmin as "Duration in Min"

							
						FROM  Shift_breakdurationdetails 

						WHERE Shift.Shiftid =Shift_breakdurationdetails.Shiftid
) J)
						as automaton_Shift_breakdurationdetails

                 
				 
			  FROM  Shift 
 LEFT OUTER JOIN tenant ON Shift.tenantid=tenant.tenantid

			  WHERE CAST(Shift.Shiftid AS Varchar)=pvar_Shiftid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

