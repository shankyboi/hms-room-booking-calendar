
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_WorkProfile"
              (
			  pvar_WorkProfileid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"WorkProfileid" uuid
,department Varchar
,workprofilename Varchar
,rolename Varchar
,isthisaclinicalprofile Varchar
,workprofiledescription Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:29*/
			  		 
              RETURN QUERY
			  SELECT  
				 WorkProfile.tenantid
,tenant.businessname as _tenantname
,WorkProfile.WorkProfileid
,CAST(_Department.name AS VARCHAR) as department
,WorkProfile.workprofilename
,WorkProfile.rolename
,WorkProfile.isthisaclinicalprofile
,WorkProfile.workprofiledescription

				 ,WorkProfile.createduser,WorkProfile.createddate,WorkProfile.modifieduser,WorkProfile.modifieddate
                 
                 
				 
			  FROM  WorkProfile 
 LEFT OUTER JOIN tenant ON WorkProfile.tenantid=tenant.tenantid
INNER JOIN Department _Department ON WorkProfile.department=_Department.Departmentid

			  WHERE CAST(WorkProfile.WorkProfileid AS Varchar)=pvar_WorkProfileid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

