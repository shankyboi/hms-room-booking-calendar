
CREATE OR REPLACE FUNCTION public."People_Time_Preference_List"(
	pvar_tenantid character varying,
	pvar_shiftname character varying,
	pvar_people character varying)
    RETURNS TABLE(tenantid uuid, _tenantname character varying, peopletimepreferenceid uuid, shiftname uuid, shiftname_master character varying,workprofile uuid,workprofile_master Varchar, people uuid, people_master character varying, "automaton_PeopleTimePreference_timepreference" json, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 14:00:31*/
			  		
                SELECT  SPLIT_PART(pvar_tenantid, '|', 1),SPLIT_PART(pvar_tenantid, '|', 2) into lstr_usersid,pvar_tenantid;
		        
                if(pvar_tenantid is null or pvar_tenantid='' or pvar_tenantid='00000000-0000-0000-0000-000000000000')	
				then
                    SELECT STRING_TO_ARRAY(viewertenantids, ',') into lvar_tenantid
				    FROM users where users.usersid::varchar=lstr_usersid;	
                    if(lvar_tenantid is NULL)
					then 
						SELECT array_agg(tenant.tenantid) INTO lvar_tenantid FROM tenant;
               
					end if;
                else 
				  lvar_tenantid=ARRAY[pvar_tenantid];
                end if;
                lvar_tenantid := lvar_tenantid || ARRAY[''::character varying] || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];

              
                RETURN QUERY
				SELECT  
				PeopleTimePreference.tenantid
,tenant.businessname as _tenantName
,PeopleTimePreference.PeopleTimePreferenceid
,PeopleTimePreference.shiftname
,CAST(_Shift.shiftname AS VARCHAR) as shiftname_master
,PeopleTimePreference.workprofile
,CAST(__WorkProfile.workprofilename AS VARCHAR) as workprofile_master
,PeopleTimePreference.people
,CAST(CASE WHEN ___People.lastname IS NULL OR ___People.lastname = '' THEN ___People.firstname ELSE ___People.firstname || ' ' || ___People.lastname END AS VARCHAR) as people_master

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

				,PeopleTimePreference.createduser,PeopleTimePreference.createddate,PeopleTimePreference.modifieduser,PeopleTimePreference.modifieddate
				FROM  PeopleTimePreference 
 LEFT OUTER JOIN tenant ON PeopleTimePreference.tenantid=tenant.tenantid
INNER JOIN Shift _Shift ON PeopleTimePreference.shiftname=_Shift.Shiftid
INNER JOIN WorkProfile __WorkProfile ON PeopleTimePreference.workprofile=__WorkProfile.WorkProfileid
INNER JOIN People ___People ON PeopleTimePreference.people=___People.Peopleid

				WHERE (lvar_tenantid is null or COALESCE(cast(PeopleTimePreference.tenantid as varchar), '') = Any(lvar_tenantid)) AND PeopleTimePreference.isdeleted=false
AND (pvar_shiftname is null or pvar_shiftname ='0' or LENGTH(CAST(pvar_shiftname as Varchar))=0 or CAST(PeopleTimePreference.shiftname as VARCHAR)=pvar_shiftname)
AND (pvar_people is null or pvar_people ='0' or LENGTH(CAST(pvar_people as Varchar))=0 or CAST(PeopleTimePreference.people as VARCHAR)=pvar_people)

				 ORDER BY PeopleTimePreference.createddate DESC;
			  
			  END
              
$BODY$;

