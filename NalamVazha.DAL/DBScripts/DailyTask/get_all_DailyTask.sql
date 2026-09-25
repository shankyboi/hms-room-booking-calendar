 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_DailyTask"
              (
			  pvar_tenantid Varchar=null,
                pvar_searchterm character varying='',
                pvar_pagesize integer=50,
                pvar_pagenumber integer=0
         
              )
			 RETURNS TABLE(
                tasktype uuid
,taskname uuid
,patientname uuid
,ipdreferencenumber uuid
,opdreferencenumber uuid
,status Varchar
,amount int
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,"DailyTaskid" uuid
                 
               )
               AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
               BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:39:40*/
               
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

			    
                RETURN QUERY
			  SELECT 

				 DailyTask.tasktype
,DailyTask.taskname
,DailyTask.patientname
,DailyTask.ipdreferencenumber
,DailyTask.opdreferencenumber
,DailyTask.status
,DailyTask.amount

				 ,DailyTask.createduser,DailyTask.createddate,DailyTask.modifieduser,DailyTask.modifieddate
				,DailyTask.tenantid 
                ,DailyTask.DailyTaskid
				 
			  FROM DailyTask
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(DailyTask.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND DailyTask.isdeleted=false
			  
		        AND (((pvar_searchterm is null) or COALESCE(DailyTask.DailyTaskid::varchar,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(DailyTask.taskno,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(DailyTask.patientcategory,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(DailyTask.status,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

