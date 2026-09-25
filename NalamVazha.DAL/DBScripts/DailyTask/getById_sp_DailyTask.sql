 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_DailyTask"
			  (
				  pvar_DailyTaskid Varchar
			  )
			  RETURNS TABLE(
                taskno Varchar
,dateandtime Timestamp(3)
,tasktype uuid
,taskname uuid
,patientname uuid
,patientcategory Varchar
,ipdreferencenumber uuid
,opdreferencenumber uuid
,status Varchar
,amount int
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,DailyTaskid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:39:40*/
               
              RETURN QUERY
			  SELECT 
				 DailyTask.taskno
,DailyTask.dateandtime
,DailyTask.tasktype
,DailyTask.taskname
,DailyTask.patientname
,DailyTask.patientcategory
,DailyTask.ipdreferencenumber
,DailyTask.opdreferencenumber
,DailyTask.status
,DailyTask.amount

				 ,DailyTask.createduser,DailyTask.createddate,DailyTask.modifieduser,DailyTask.modifieddate
				 ,DailyTask.tenantid
                 ,DailyTask.DailyTaskid
                    
			  FROM DailyTask
			  WHERE CAST(DailyTask.DailyTaskid AS Varchar)=pvar_DailyTaskid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

