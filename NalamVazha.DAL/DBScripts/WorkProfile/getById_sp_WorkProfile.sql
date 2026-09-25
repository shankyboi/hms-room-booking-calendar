 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_WorkProfile"
			  (
				  pvar_WorkProfileid Varchar
			  )
			  RETURNS TABLE(
                department uuid
,workprofilename Varchar
,rolename Varchar
,isthisaclinicalprofile Varchar
,workprofiledescription Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,WorkProfileid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:29*/
               
              RETURN QUERY
			  SELECT 
				 WorkProfile.department
,WorkProfile.workprofilename
,WorkProfile.rolename
,WorkProfile.isthisaclinicalprofile
,WorkProfile.workprofiledescription

				 ,WorkProfile.createduser,WorkProfile.createddate,WorkProfile.modifieduser,WorkProfile.modifieddate
				 ,WorkProfile.tenantid
                 ,WorkProfile.WorkProfileid
                    
			  FROM WorkProfile
			  WHERE CAST(WorkProfile.WorkProfileid AS Varchar)=pvar_WorkProfileid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

