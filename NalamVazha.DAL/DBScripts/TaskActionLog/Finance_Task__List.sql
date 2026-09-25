
			  CREATE OR REPLACE FUNCTION  "Finance_Task__List"
              (pvar_tenantid Varchar
,pvar_pagesize integer
,pvar_pagenumber integer
,pvar_searchterm varchar
,pvar_sort_fields json



                )
			  RETURNS json
			  AS $BODY$
               declare local_sortcolumn_array text[] = (
	                select array_agg(col) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)
                );
                declare local_sortorder_array text[] = (
	                select array_agg(dir) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)	
                );          
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;  
               
          	  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:40:35*/
			  		
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




                    if(pvar_searchterm is not null and LENGTH(CAST(pvar_searchterm as Varchar)) > 0)
                    then
                    pvar_searchterm := '%' || pvar_searchterm || '%';
                    else
                    pvar_searchterm := null;
                    end if;
              
                    RETURN json_build_object(
                    'count'
                    ,(SELECT  
                    COUNT(*)
                    FROM  TaskActionLog 
 LEFT OUTER JOIN tenant ON TaskActionLog.tenantid=tenant.tenantid
LEFT OUTER JOIN DailyTask _DailyTask ON TaskActionLog.taskname=_DailyTask.DailyTaskid
LEFT OUTER JOIN TaskTemplate _TaskTemplate ON _DailyTask.taskname=_TaskTemplate.TaskTemplateid
LEFT OUTER JOIN TypeofTask __TypeofTask ON TaskActionLog.tasktype=__TypeofTask.TypeofTaskid
LEFT OUTER JOIN users ___users ON TaskActionLog.actionby=___users.usersid
LEFT OUTER JOIN users ____users ON TaskActionLog.assignto=____users.usersid
LEFT OUTER JOIN users _____users ON TaskActionLog.escalateto=_____users.usersid

                    WHERE (lvar_tenantid is null or COALESCE(cast(TaskActionLog.tenantid as varchar), '') = Any(lvar_tenantid)) AND TaskActionLog.isdeleted=false
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_TaskTemplate.taskname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__TypeofTask.tasktype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(TaskActionLog.actiondate,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___users.firstname||' '||___users.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(TaskActionLog.comments AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____users.firstname||' '||____users.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_____users.firstname||' '||_____users.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(TaskActionLog.summary AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(TaskActionLog.description AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    TaskActionLog.tenantid
,tenant.businessname as _tenantName
,TaskActionLog.TaskActionLogid
,TaskActionLog.taskname
,CAST(_TaskTemplate.taskname AS VARCHAR) as taskname_master
,TaskActionLog.tasktype
,CAST(__TypeofTask.tasktype AS VARCHAR) as tasktype_master
,CAST(COALESCE(to_char(TaskActionLog.actiondate,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as actiondate
,TaskActionLog.actionby
,CAST(___users.firstname||' '||___users.lastname AS VARCHAR) as actionby_master
,TaskActionLog.comments
,TaskActionLog.assignto
,CAST(____users.firstname||' '||____users.lastname AS VARCHAR) as assignto_master
,TaskActionLog.escalateto
,CAST(_____users.firstname||' '||_____users.lastname AS VARCHAR) as escalateto_master
,TaskActionLog.summary
,TaskActionLog.description

                    
                    ,TaskActionLog.createduser,TaskActionLog.createddate,TaskActionLog.modifieduser,TaskActionLog.modifieddate
                    FROM  TaskActionLog 
 LEFT OUTER JOIN tenant ON TaskActionLog.tenantid=tenant.tenantid
LEFT OUTER JOIN DailyTask _DailyTask ON TaskActionLog.taskname=_DailyTask.DailyTaskid
LEFT OUTER JOIN TaskTemplate _TaskTemplate ON _DailyTask.taskname=_TaskTemplate.TaskTemplateid
LEFT OUTER JOIN TypeofTask __TypeofTask ON TaskActionLog.tasktype=__TypeofTask.TypeofTaskid
LEFT OUTER JOIN users ___users ON TaskActionLog.actionby=___users.usersid
LEFT OUTER JOIN users ____users ON TaskActionLog.assignto=____users.usersid
LEFT OUTER JOIN users _____users ON TaskActionLog.escalateto=_____users.usersid

                    WHERE (lvar_tenantid is null or COALESCE(cast(TaskActionLog.tenantid as varchar), '') = Any(lvar_tenantid)) AND TaskActionLog.isdeleted=false

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_TaskTemplate.taskname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__TypeofTask.tasktype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(TaskActionLog.actiondate,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___users.firstname||' '||___users.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(TaskActionLog.comments AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____users.firstname||' '||____users.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_____users.firstname||' '||_____users.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(TaskActionLog.summary AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(TaskActionLog.description AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'taskname' THEN _TaskTemplate.taskname::TEXT
WHEN 'tasktype' THEN __TypeofTask.tasktype::TEXT
WHEN 'actionby' THEN ___users.firstname||' '||___users.lastname::TEXT
WHEN 'comments' THEN TaskActionLog.comments::TEXT
WHEN 'assignto' THEN ____users.firstname||' '||____users.lastname::TEXT
WHEN 'escalateto' THEN _____users.firstname||' '||_____users.lastname::TEXT
WHEN 'summary' THEN TaskActionLog.summary::TEXT
WHEN 'description' THEN TaskActionLog.description::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'actiondate' THEN TaskActionLog.actiondate
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'taskname' THEN _TaskTemplate.taskname::TEXT
WHEN 'tasktype' THEN __TypeofTask.tasktype::TEXT
WHEN 'actionby' THEN ___users.firstname||' '||___users.lastname::TEXT
WHEN 'comments' THEN TaskActionLog.comments::TEXT
WHEN 'assignto' THEN ____users.firstname||' '||____users.lastname::TEXT
WHEN 'escalateto' THEN _____users.firstname||' '||_____users.lastname::TEXT
WHEN 'summary' THEN TaskActionLog.summary::TEXT
WHEN 'description' THEN TaskActionLog.description::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'actiondate' THEN TaskActionLog.actiondate
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC

                    limit pvar_pagesize
                    offset pvar_pagenumber * pvar_pagesize			 	
			 	
                    ) d));
	
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;
