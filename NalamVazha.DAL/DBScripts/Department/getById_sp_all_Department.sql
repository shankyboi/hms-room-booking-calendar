
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Department"
              (
			  pvar_Departmentid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Departmentid" uuid
,name Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:38*/
			  		 
              RETURN QUERY
			  SELECT  
				 Department.tenantid
,tenant.businessname as _tenantname
,Department.Departmentid
,Department.name
,Department.description

				 ,Department.createduser,Department.createddate,Department.modifieduser,Department.modifieddate
                 
                 
				 
			  FROM  Department 
 LEFT OUTER JOIN tenant ON Department.tenantid=tenant.tenantid

			  WHERE CAST(Department.Departmentid AS Varchar)=pvar_Departmentid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

