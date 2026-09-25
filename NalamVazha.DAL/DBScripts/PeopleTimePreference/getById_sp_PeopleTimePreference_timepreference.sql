
CREATE OR REPLACE FUNCTION public."getById_sp_PeopleTimePreference_timepreference"(
	pvar_peopletimepreferenceid character varying)
    RETURNS TABLE("PeopleTimePreferenceid" uuid, "PeopleTimePreference_timepreferenceid" uuid, clinicaltask character varying, tasktype uuid, taskname uuid, availableon character varying, taskstarttime character varying, taskendtime character varying, taskhours numeric) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 PeopleTimePreference_timepreference.PeopleTimePreferenceid
                                             ,PeopleTimePreference_timepreference.PeopleTimePreference_timepreferenceid   
											 ,PeopleTimePreference_timepreference.clinicaltask
,PeopleTimePreference_timepreference.tasktype
,PeopleTimePreference_timepreference.taskname
,PeopleTimePreference_timepreference.availableon
,PeopleTimePreference_timepreference.taskstarttime
,PeopleTimePreference_timepreference.taskendtime
,PeopleTimePreference_timepreference.taskhours
 
											 
											 FROM PeopleTimePreference_timepreference
											 WHERE 
											 CAST(PeopleTimePreference_timepreference.PeopleTimePreferenceid AS VARCHAR)=pvar_PeopleTimePreferenceid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             
$BODY$;


