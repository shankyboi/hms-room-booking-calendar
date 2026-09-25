 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Designation"
			  (
				  pvar_Designationid Varchar
			  )
			  RETURNS TABLE(
                workprofile uuid
,designation Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Designationid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:33*/
               
              RETURN QUERY
			  SELECT 
				 Designation.workprofile
,Designation.designation

				 ,Designation.createduser,Designation.createddate,Designation.modifieduser,Designation.modifieddate
				 ,Designation.tenantid
                 ,Designation.Designationid
                    
			  FROM Designation
			  WHERE CAST(Designation.Designationid AS Varchar)=pvar_Designationid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

