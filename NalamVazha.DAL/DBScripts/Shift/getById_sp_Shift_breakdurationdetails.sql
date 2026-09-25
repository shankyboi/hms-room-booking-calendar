CREATE OR REPLACE FUNCTION "getById_sp_Shift_breakdurationdetails"(
												 pvar_Shiftid Varchar(50)
											 )
                                             RETURNS TABLE("Shiftid" uuid,"Shift_breakdurationdetailsid" uuid ,breakname Varchar
,starttime Varchar
,endtime Varchar
,durationinmin decimal
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 Shift_breakdurationdetails.Shiftid
                                             ,Shift_breakdurationdetails.Shift_breakdurationdetailsid   
											 ,Shift_breakdurationdetails.breakname
,Shift_breakdurationdetails.starttime
,Shift_breakdurationdetails.endtime
,Shift_breakdurationdetails.durationinmin
 
											 
											 FROM Shift_breakdurationdetails
											 WHERE 
											 CAST(Shift_breakdurationdetails.Shiftid AS VARCHAR)=pvar_Shiftid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

