CREATE OR REPLACE FUNCTION "getById_sp_ShiftPlanning_people"(
												 pvar_ShiftPlanningid Varchar(50)
											 )
                                             RETURNS TABLE("ShiftPlanningid" uuid,"ShiftPlanning_peopleid" uuid ,personname uuid
,workprofile uuid
,coveragetype Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 ShiftPlanning_people.ShiftPlanningid
                                             ,ShiftPlanning_people.ShiftPlanning_peopleid   
											 ,ShiftPlanning_people.personname
,ShiftPlanning_people.workprofile
,ShiftPlanning_people.coveragetype
 
											 
											 FROM ShiftPlanning_people
											 WHERE 
											 CAST(ShiftPlanning_people.ShiftPlanningid AS VARCHAR)=pvar_ShiftPlanningid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

