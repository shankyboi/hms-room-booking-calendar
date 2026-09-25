 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_TaskType"
			  (
				  pvar_TaskTypeid Varchar
			  )
			  RETURNS TABLE(
                tasktypename Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,TaskTypeid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:21*/
               
              RETURN QUERY
			  SELECT 
				 TaskType.tasktypename
,TaskType.description

				 ,TaskType.createduser,TaskType.createddate,TaskType.modifieduser,TaskType.modifieddate
				 ,TaskType.tenantid
                 ,TaskType.TaskTypeid
                    
			  FROM TaskType
			  WHERE CAST(TaskType.TaskTypeid AS Varchar)=pvar_TaskTypeid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

