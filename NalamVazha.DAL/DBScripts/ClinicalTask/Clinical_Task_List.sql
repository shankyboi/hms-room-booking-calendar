
			  CREATE OR REPLACE FUNCTION  "Clinical_Task_List"
              (pvar_tenantid Varchar
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,ClinicalTaskid uuid
,workprofile uuid,workprofile_master Varchar,competency uuid,competency_master Varchar,"automaton_ClinicalTask_taskduration" json,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:36*/
			  		
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
				ClinicalTask.tenantid
,tenant.businessname as _tenantName
,ClinicalTask.ClinicalTaskid
,ClinicalTask.workprofile
,CAST(_WorkProfile.workprofilename AS VARCHAR) as workprofile_master
,ClinicalTask.competency
,CAST(__Competency.competencyname AS VARCHAR) as competency_master

				,
                (SELECT json_agg(J) FROM (SELECT   
				 CAST(_WorkProfile.workprofilename AS VARCHAR) as "Work Profile"
,CAST(__TaskType.tasktypename AS VARCHAR) as "Task Type"
,CAST(___Task.taskname AS VARCHAR) as "Task Name"
,ClinicalTask_taskduration.durationinminutes as "Duration in Minutes"
,ClinicalTask_taskduration.overbookingcount as "Over Booking Count"

		 	   FROM  ClinicalTask_taskduration 
INNER JOIN WorkProfile _WorkProfile ON ClinicalTask_taskduration.workprofile=_WorkProfile.WorkProfileid
INNER JOIN TaskType __TaskType ON ClinicalTask_taskduration.tasktype=__TaskType.TaskTypeid
INNER JOIN Task ___Task ON ClinicalTask_taskduration.taskname=___Task.Taskid

			  WHERE ClinicalTask.ClinicalTaskid =ClinicalTask_taskduration.ClinicalTaskid
) J)
			    as automaton_ClinicalTask_taskduration

				,ClinicalTask.createduser,ClinicalTask.createddate,ClinicalTask.modifieduser,ClinicalTask.modifieddate
				FROM  ClinicalTask 
 LEFT OUTER JOIN tenant ON ClinicalTask.tenantid=tenant.tenantid
INNER JOIN WorkProfile _WorkProfile ON ClinicalTask.workprofile=_WorkProfile.WorkProfileid
INNER JOIN Competency __Competency ON ClinicalTask.competency=__Competency.Competencyid

				WHERE (lvar_tenantid is null or COALESCE(cast(ClinicalTask.tenantid as varchar), '') = Any(lvar_tenantid)) AND ClinicalTask.isdeleted=false

				 ORDER BY ClinicalTask.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

