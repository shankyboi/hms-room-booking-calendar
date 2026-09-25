
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_TaskActionLog"
              (
			  pvar_TaskActionLogid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"TaskActionLogid" uuid
,taskname Varchar
,tasktype Varchar
,actiondate Varchar
,actionby Varchar
,comments Varchar
,assignto Varchar
,escalateto Varchar
,summary Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:40:35*/
			  		 
              RETURN QUERY
			  SELECT  
				 TaskActionLog.tenantid
,tenant.businessname as _tenantname
,TaskActionLog.TaskActionLogid
,CAST(_TaskTemplate.taskname AS VARCHAR) as taskname
,CAST(__TypeofTask.tasktype AS VARCHAR) as tasktype
,CAST(COALESCE(to_char(TaskActionLog.actiondate,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as actiondate
,CAST(___users.firstname||' '||___users.lastname AS VARCHAR) as actionby
,TaskActionLog.comments
,CAST(____users.firstname||' '||____users.lastname AS VARCHAR) as assignto
,CAST(_____users.firstname||' '||_____users.lastname AS VARCHAR) as escalateto
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

			  WHERE CAST(TaskActionLog.TaskActionLogid AS Varchar)=pvar_TaskActionLogid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;
