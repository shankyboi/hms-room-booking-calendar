
								CREATE OR REPLACE FUNCTION  "lookup_change_ShiftPlanning_shiftname"(
								pvar_Shiftid Varchar(50)=null
                                )
								RETURNS TABLE("Shiftid" Varchar
,shiftname Varchar
,shiftstarttime Varchar
,shiftendtime Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 07/28/2026 11:24:25*/
										
                                RETURN QUERY
								SELECT  
									CAST(Shift.Shiftid AS Varchar) as Shiftid
,CAST(Shift.shiftname AS Varchar) as shiftname
,CAST(Shift.shiftstarttime AS Varchar) as shiftstarttime
,CAST(Shift.shiftendtime AS Varchar) as shiftendtime

								FROM Shift
							   WHERE (CAST(Shift.Shiftid AS VARCHAR) = pvar_Shiftid)
;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

