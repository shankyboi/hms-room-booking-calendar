
CREATE OR REPLACE FUNCTION public."prefill_PeopleTimePreference_timepreference"(
	pvar_people character varying DEFAULT NULL::character varying)
    RETURNS TABLE(tasktype character varying, taskname character varying, availableon character varying, clinicaltask character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                                    
								BEGIN
                                /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 14:00:31*/
										

                                RETURN QUERY
								SELECT  
									CAST(people_clinicaltaskinfo.tasktype AS Varchar) as tasktype,CAST(people_clinicaltaskinfo.taskname AS Varchar) as taskname,CAST(people_clinicaltaskinfo.availableon AS Varchar) as availableon,CAST(people_clinicaltaskinfo.people_clinicaltaskinfoid AS Varchar) as clinicaltaskid
								FROM people_clinicaltaskinfo INNER JOIN People ON people_clinicaltaskinfo.peopleid=People.peopleid
								 WHERE (CAST(People.Peopleid AS Varchar) like pvar_people)
;
								
											
								END
                                
$BODY$;

