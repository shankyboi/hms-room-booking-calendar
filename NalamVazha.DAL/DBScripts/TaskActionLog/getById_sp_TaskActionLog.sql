 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_TaskActionLog"
			  (
				  pvar_TaskActionLogid Varchar
			  )
			  RETURNS TABLE(
                taskname uuid
,tasktype uuid
,actiondate Timestamp(3)
,actionby uuid
,comments Varchar
,assignto uuid
,escalateto uuid
,summary Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,TaskActionLogid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:40:35*/
               
              RETURN QUERY
			  SELECT 
				 TaskActionLog.taskname
,TaskActionLog.tasktype
,TaskActionLog.actiondate
,TaskActionLog.actionby
,TaskActionLog.comments
,TaskActionLog.assignto
,TaskActionLog.escalateto
,TaskActionLog.summary
,TaskActionLog.description

				 ,TaskActionLog.createduser,TaskActionLog.createddate,TaskActionLog.modifieduser,TaskActionLog.modifieddate
				 ,TaskActionLog.tenantid
                 ,TaskActionLog.TaskActionLogid
                    
			  FROM TaskActionLog
			  WHERE CAST(TaskActionLog.TaskActionLogid AS Varchar)=pvar_TaskActionLogid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

