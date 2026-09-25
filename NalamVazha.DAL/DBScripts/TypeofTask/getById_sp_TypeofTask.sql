 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_TypeofTask"
			  (
				  pvar_TypeofTaskid Varchar
			  )
			  RETURNS TABLE(
                tasktype Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,TypeofTaskid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:38:47*/
               
              RETURN QUERY
			  SELECT 
				 TypeofTask.tasktype
,TypeofTask.description

				 ,TypeofTask.createduser,TypeofTask.createddate,TypeofTask.modifieduser,TypeofTask.modifieddate
				 ,TypeofTask.tenantid
                 ,TypeofTask.TypeofTaskid
                    
			  FROM TypeofTask
			  WHERE CAST(TypeofTask.TypeofTaskid AS Varchar)=pvar_TypeofTaskid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

