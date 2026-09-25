
CREATE OR REPLACE FUNCTION public."getById_sp_all_PeopleTimePreference"(
	pvar_peopletimepreferenceid character varying)
    RETURNS TABLE(tenantid uuid, _tenantname character varying, "PeopleTimePreferenceid" uuid, shiftname character varying,workprofile Varchar, people character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, "automaton_PeopleTimePreference_timepreference" json) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 14:00:31*/
			  		 
              RETURN QUERY
			  SELECT  
				 PeopleTimePreference.tenantid
,tenant.businessname as _tenantname
,PeopleTimePreference.PeopleTimePreferenceid
,CAST(_Shift.shiftname AS VARCHAR) as shiftname
,CAST(__WorkProfile.workprofilename AS VARCHAR) as workprofile
,CAST(CASE WHEN ___People.lastname IS NULL OR ___People.lastname = '' THEN ___People.firstname ELSE ___People.firstname || ' ' || ___People.lastname END AS VARCHAR) as people
				 ,PeopleTimePreference.createduser,PeopleTimePreference.createddate,PeopleTimePreference.modifieduser,PeopleTimePreference.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(_TaskType.tasktypename AS VARCHAR) as "Task Type"
,CAST(__Task.taskname AS VARCHAR) as "Task Name"
,PeopleTimePreference_timepreference.availableon as "Available on"
,PeopleTimePreference_timepreference.taskstarttime as "Task Start Time"
,PeopleTimePreference_timepreference.taskendtime as "Task End Time"
,PeopleTimePreference_timepreference.taskhours as "Task Hours"
							
						FROM  PeopleTimePreference_timepreference 
LEFT OUTER JOIN TaskType _TaskType ON PeopleTimePreference_timepreference.tasktype=_TaskType.TaskTypeid
LEFT OUTER JOIN Task __Task ON PeopleTimePreference_timepreference.taskname=__Task.Taskid
						WHERE PeopleTimePreference.PeopleTimePreferenceid =PeopleTimePreference_timepreference.PeopleTimePreferenceid
) J)
						as automaton_PeopleTimePreference_timepreference
                 
			  FROM  PeopleTimePreference 
 LEFT OUTER JOIN tenant ON PeopleTimePreference.tenantid=tenant.tenantid
INNER JOIN Shift _Shift ON PeopleTimePreference.shiftname=_Shift.Shiftid
INNER JOIN WorkProfile __WorkProfile ON PeopleTimePreference.workprofile=__WorkProfile.WorkProfileid
INNER JOIN People ___People ON PeopleTimePreference.people=___People.Peopleid
			  WHERE CAST(PeopleTimePreference.PeopleTimePreferenceid AS Varchar)=pvar_PeopleTimePreferenceid;
			  
			  END
              
$BODY$;

