
CREATE OR REPLACE FUNCTION public."getById_sp_PeopleTimePreference"(
	pvar_peopletimepreferenceid character varying)
    RETURNS TABLE(shiftname uuid,workprofile uuid, people uuid, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, tenantid uuid, peopletimepreferenceid uuid, timepreference json) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 14:00:31*/
               
              RETURN QUERY
			  SELECT 
				 PeopleTimePreference.shiftname
				 ,PeopleTimePreference.workprofile
,PeopleTimePreference.people

				 ,PeopleTimePreference.createduser,PeopleTimePreference.createddate,PeopleTimePreference.modifieduser,PeopleTimePreference.modifieddate
				 ,PeopleTimePreference.tenantid
                 ,PeopleTimePreference.PeopleTimePreferenceid
                 ,(SELECT json_agg(J) FROM (
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
											 PeopleTimePreference_timepreference.PeopleTimePreferenceid=PeopleTimePreference.PeopleTimePreferenceid
                                             
                                             ORDER BY record_order DESC
											) J) as timepreference
   
			  FROM PeopleTimePreference
			  WHERE CAST(PeopleTimePreference.PeopleTimePreferenceid AS Varchar)=pvar_PeopleTimePreferenceid
                       ;

					 	
			  END
              
$BODY$;

