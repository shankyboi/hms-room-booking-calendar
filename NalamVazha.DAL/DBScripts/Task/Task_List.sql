-- FUNCTION: public.Task_List(character varying, character varying)

-- DROP FUNCTION IF EXISTS public."Task_List"(character varying, character varying);

CREATE OR REPLACE FUNCTION public."Task_List"(
	pvar_tenantid character varying,
	pvar_tasktype character varying)
    RETURNS TABLE(tenantid uuid, _tenantname character varying, taskid uuid, tasktype uuid, tasktype_master character varying, taskname character varying, taskdesc character varying, allowpatienttoselectappointmentslot character varying, taskfeestype character varying, taskcharges numeric, slainhrs double precision, "automaton_Task_screeningtemplates" json, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/30/2026 16:03:22*/
			  		
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
				Task.tenantid
,tenant.businessname as _tenantName
,Task.Taskid
,Task.tasktype
,CAST(_TaskType.tasktypename AS VARCHAR) as tasktype_master
,Task.taskname
,Task.taskdesc
,Task.allowpatienttoselectappointmentslot
,Task.taskfeestype
,Task.taskcharges
,Task.slainhrs

				,
                (SELECT json_agg(J) FROM (SELECT   
				 CAST(_AssessmentTemplate.templatename AS VARCHAR) as "Screening Template"
,Task_screeningtemplates.isdefaulttemplate as "Is Default Template ?"

		 	   FROM  Task_screeningtemplates 
LEFT OUTER JOIN AssessmentTemplate _AssessmentTemplate ON Task_screeningtemplates.screeningtemplate=_AssessmentTemplate.AssessmentTemplateid

			  WHERE Task.Taskid =Task_screeningtemplates.Taskid
) J)
			    as automaton_Task_screeningtemplates

				,Task.createduser,Task.createddate,Task.modifieduser,Task.modifieddate
				FROM  Task 
 LEFT OUTER JOIN tenant ON Task.tenantid=tenant.tenantid
INNER JOIN TaskType _TaskType ON Task.tasktype=_TaskType.TaskTypeid

				WHERE (lvar_tenantid is null or COALESCE(cast(Task.tenantid as varchar), '') = Any(lvar_tenantid)) AND Task.isdeleted=false
AND (pvar_tasktype is null or pvar_tasktype ='0' or LENGTH(CAST(pvar_tasktype as Varchar))=0 or CAST(Task.tasktype as VARCHAR)=pvar_tasktype)
 AND Task.taskname NOT IN ('Default Template')
				 ORDER BY Task.createddate DESC;
			  
					 	
			  END
              
$BODY$;

ALTER FUNCTION public."Task_List"(character varying, character varying)
    OWNER TO md_nalamvazha;

GRANT EXECUTE ON FUNCTION public."Task_List"(character varying, character varying) TO PUBLIC;

GRANT EXECUTE ON FUNCTION public."Task_List"(character varying, character varying) TO md_nalamvazha;

