
			  -- shifthours widened from int to decimal(18,2) to support fractional-hour shifts.
			  -- Changing a RETURNS TABLE column type requires dropping the function first --
			  -- CREATE OR REPLACE cannot alter the return type in place.
			  DROP FUNCTION IF EXISTS "getById_sp_Shift"(Varchar);

			  CREATE OR REPLACE FUNCTION  "getById_sp_Shift"
			  (
				  pvar_Shiftid Varchar
			  )
			  RETURNS TABLE(
                shiftcode Varchar
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
,tenantid uuid

                ,Shiftid uuid
                ,breakdurationdetails JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:17*/
               
              RETURN QUERY
			  SELECT 
				 Shift.shiftcode
,Shift.shiftname
,Shift.shiftstarttime
,Shift.shiftendtime
,Shift.shifthours
,Shift.description
,Shift.totalbreakinmins
,Shift.totalbreakinhrs
,Shift.workhours

				 ,Shift.createduser,Shift.createddate,Shift.modifieduser,Shift.modifieddate
				 ,Shift.tenantid
                 ,Shift.Shiftid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 Shift_breakdurationdetails.Shiftid
                                             ,Shift_breakdurationdetails.Shift_breakdurationdetailsid   
											 ,Shift_breakdurationdetails.breakname
,Shift_breakdurationdetails.starttime
,Shift_breakdurationdetails.endtime
,Shift_breakdurationdetails.durationinmin
 
											  
											 FROM Shift_breakdurationdetails
											 WHERE 
											 Shift_breakdurationdetails.Shiftid=Shift.Shiftid
                                             
                                             ORDER BY record_order DESC
											) J) as breakdurationdetails
   
			  FROM Shift
			  WHERE CAST(Shift.Shiftid AS Varchar)=pvar_Shiftid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

