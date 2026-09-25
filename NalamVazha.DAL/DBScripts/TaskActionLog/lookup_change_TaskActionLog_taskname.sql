
								CREATE OR REPLACE FUNCTION  "lookup_change_TaskActionLog_taskname"(
								pvar_DailyTaskid Varchar(50)=null
                                )
								RETURNS TABLE("DailyTaskid" Varchar
,taskname Varchar
,tasktype Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:40:35*/
										
                                RETURN QUERY
								SELECT  
									CAST(DailyTask.DailyTaskid AS Varchar) as DailyTaskid
,CAST(TaskTemplate.taskname AS Varchar) as taskname
,CAST(DailyTask.tasktype AS Varchar) as tasktype

								FROM DailyTask
								LEFT OUTER JOIN TaskTemplate ON DailyTask.taskname=TaskTemplate.TaskTemplateid
							   WHERE (CAST(DailyTask.DailyTaskid AS VARCHAR) = pvar_DailyTaskid)
;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;
