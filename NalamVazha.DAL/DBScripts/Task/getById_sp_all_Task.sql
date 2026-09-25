
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Task"
              (
			  pvar_Taskid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Taskid" uuid
,tasktype Varchar
,taskname Varchar
,taskdesc Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:24*/
			  		 
              RETURN QUERY
			  SELECT  
				 Task.tenantid
,tenant.businessname as _tenantname
,Task.Taskid
,CAST(_TaskType.tasktypename AS VARCHAR) as tasktype
,Task.taskname
,Task.taskdesc

				 ,Task.createduser,Task.createddate,Task.modifieduser,Task.modifieddate
                 
                 
				 
			  FROM  Task 
 LEFT OUTER JOIN tenant ON Task.tenantid=tenant.tenantid
INNER JOIN TaskType _TaskType ON Task.tasktype=_TaskType.TaskTypeid

			  WHERE CAST(Task.Taskid AS Varchar)=pvar_Taskid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

