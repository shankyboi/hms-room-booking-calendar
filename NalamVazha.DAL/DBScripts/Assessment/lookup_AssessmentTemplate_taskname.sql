-- FUNCTION: public.lookup_AssessmentTemplate_taskname(character varying)

-- DROP FUNCTION IF EXISTS public."lookup_AssessmentTemplate_taskname"(character varying);

CREATE OR REPLACE FUNCTION public."lookup_AssessmentTemplate_taskname"(
	pvar_tenantid character varying DEFAULT NULL::character varying)
    RETURNS TABLE("Taskid" character varying, taskname character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                                declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/20/2026 05:22:36*/
							    
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
								SELECT DISTINCT ON (Task.taskname)
								CAST(Task.Taskid AS Varchar) as Taskid,CAST(Task.taskname AS Varchar) as taskname
								FROM Task
								 WHERE COALESCE(cast(Task.tenantid as varchar),'') = Any(lvar_tenantid) AND Task.isdeleted=false
 AND Task.taskname NOT IN ('Default Template')
 ORDER BY Task.taskname ASC
                                ;
								
											
								END
                                
$BODY$;

ALTER FUNCTION public."lookup_AssessmentTemplate_taskname"(character varying)
    OWNER TO md_nalamvazha;

GRANT EXECUTE ON FUNCTION public."lookup_AssessmentTemplate_taskname"(character varying) TO PUBLIC;

GRANT EXECUTE ON FUNCTION public."lookup_AssessmentTemplate_taskname"(character varying) TO md_nalamvazha;

