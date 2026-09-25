
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_TaskType"
              (
			  pvar_TaskTypeid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"TaskTypeid" uuid
,tasktypename Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:21*/
			  		 
              RETURN QUERY
			  SELECT  
				 TaskType.tenantid
,tenant.businessname as _tenantname
,TaskType.TaskTypeid
,TaskType.tasktypename
,TaskType.description

				 ,TaskType.createduser,TaskType.createddate,TaskType.modifieduser,TaskType.modifieddate
                 
                 
				 
			  FROM  TaskType 
 LEFT OUTER JOIN tenant ON TaskType.tenantid=tenant.tenantid

			  WHERE CAST(TaskType.TaskTypeid AS Varchar)=pvar_TaskTypeid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

