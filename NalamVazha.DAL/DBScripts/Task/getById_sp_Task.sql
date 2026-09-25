 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Task"
			  (
				  pvar_Taskid Varchar
			  )
			  RETURNS TABLE(
                tasktype uuid
,taskname Varchar
,taskdesc Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Taskid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:24*/
               
              RETURN QUERY
			  SELECT 
				 Task.tasktype
,Task.taskname
,Task.taskdesc

				 ,Task.createduser,Task.createddate,Task.modifieduser,Task.modifieddate
				 ,Task.tenantid
                 ,Task.Taskid
                    
			  FROM Task
			  WHERE CAST(Task.Taskid AS Varchar)=pvar_Taskid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

