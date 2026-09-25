
			  CREATE OR REPLACE FUNCTION  "Task_Template_List"
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:27:26*/
			  		
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
                    FROM  TaskTemplate 
 LEFT OUTER JOIN tenant ON TaskTemplate.tenantid=tenant.tenantid
LEFT OUTER JOIN TypeofTask _TypeofTask ON TaskTemplate.tasktype=_TypeofTask.TypeofTaskid

                    WHERE (lvar_tenantid is null or COALESCE(cast(TaskTemplate.tenantid as varchar), '') = Any(lvar_tenantid)) AND TaskTemplate.isdeleted=false
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_TypeofTask.tasktype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(TaskTemplate.taskname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(TaskTemplate.priority AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(TaskTemplate.duration AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    TaskTemplate.tenantid
,tenant.businessname as _tenantName
,TaskTemplate.TaskTemplateid
,TaskTemplate.tasktype
,CAST(_TypeofTask.tasktype AS VARCHAR) as tasktype_master
,TaskTemplate.taskname
,TaskTemplate.priority
,TaskTemplate.duration

                    ,
                (SELECT json_agg(J) FROM (SELECT   
				 TaskTemplate_escalationdetails.priority as "Priority"
,CAST(_users.firstname||' '||_users.lastname AS VARCHAR) as "Notify To "
,TaskTemplate_escalationdetails.emailid as "Email ID"
,TaskTemplate_escalationdetails.mobilenumber as "Mobile Number"

		 	   FROM  TaskTemplate_escalationdetails 
LEFT OUTER JOIN users _users ON TaskTemplate_escalationdetails.notifyto=_users.usersid

			  WHERE TaskTemplate.TaskTemplateid =TaskTemplate_escalationdetails.TaskTemplateid
) J)
			    as automaton_TaskTemplate_escalationdetails
,
                (SELECT json_agg(J) FROM (SELECT   
				 CAST(_ActionType.actiontype AS VARCHAR) as "Action Type"
,CAST(__Actions.actionname AS VARCHAR) as "Action Name"

		 	   FROM  TaskTemplate_nextactiondetails 
LEFT OUTER JOIN ActionType _ActionType ON TaskTemplate_nextactiondetails.actiontype=_ActionType.ActionTypeid
LEFT OUTER JOIN Actions __Actions ON TaskTemplate_nextactiondetails.actionname=__Actions.Actionsid

			  WHERE TaskTemplate.TaskTemplateid =TaskTemplate_nextactiondetails.TaskTemplateid
) J)
			    as automaton_TaskTemplate_nextactiondetails

                    ,TaskTemplate.createduser,TaskTemplate.createddate,TaskTemplate.modifieduser,TaskTemplate.modifieddate
                    FROM  TaskTemplate 
 LEFT OUTER JOIN tenant ON TaskTemplate.tenantid=tenant.tenantid
LEFT OUTER JOIN TypeofTask _TypeofTask ON TaskTemplate.tasktype=_TypeofTask.TypeofTaskid

                    WHERE (lvar_tenantid is null or COALESCE(cast(TaskTemplate.tenantid as varchar), '') = Any(lvar_tenantid)) AND TaskTemplate.isdeleted=false

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_TypeofTask.tasktype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(TaskTemplate.taskname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(TaskTemplate.priority AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(TaskTemplate.duration AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'tasktype' THEN _TypeofTask.tasktype::TEXT
WHEN 'taskname' THEN TaskTemplate.taskname::TEXT
WHEN 'priority' THEN TaskTemplate.priority::TEXT
WHEN 'duration' THEN TaskTemplate.duration::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'duration' THEN TaskTemplate.duration::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'tasktype' THEN _TypeofTask.tasktype::TEXT
WHEN 'taskname' THEN TaskTemplate.taskname::TEXT
WHEN 'priority' THEN TaskTemplate.priority::TEXT
WHEN 'duration' THEN TaskTemplate.duration::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'duration' THEN TaskTemplate.duration::NUMERIC
			
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

