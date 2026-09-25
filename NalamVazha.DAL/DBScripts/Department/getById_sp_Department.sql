 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Department"
			  (
				  pvar_Departmentid Varchar
			  )
			  RETURNS TABLE(
                name Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Departmentid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:38*/
               
              RETURN QUERY
			  SELECT 
				 Department.name
,Department.description

				 ,Department.createduser,Department.createddate,Department.modifieduser,Department.modifieddate
				 ,Department.tenantid
                 ,Department.Departmentid
                    
			  FROM Department
			  WHERE CAST(Department.Departmentid AS Varchar)=pvar_Departmentid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

