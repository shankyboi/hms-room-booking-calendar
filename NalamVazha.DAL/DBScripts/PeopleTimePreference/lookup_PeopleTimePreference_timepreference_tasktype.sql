
CREATE OR REPLACE FUNCTION public."lookup_PeopleTimePreference_timepreference_tasktype"(
	pvar_tenantid character varying DEFAULT NULL::character varying)
    RETURNS TABLE("TaskTypeid" character varying, tasktypename character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                                        declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								        BEGIN
								        /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 14:00:30*/
							            
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
								        CAST(TaskType.TaskTypeid AS Varchar) as TaskTypeid,CAST(TaskType.tasktypename AS Varchar) as tasktypename
								        FROM TaskType
								         WHERE COALESCE(cast(TaskType.tenantid as varchar),'') = Any(lvar_tenantid) AND TaskType.isdeleted=false

 ORDER BY TaskType.tasktypename ASC
                                        ;
								
											
								        END
                                        
$BODY$;

