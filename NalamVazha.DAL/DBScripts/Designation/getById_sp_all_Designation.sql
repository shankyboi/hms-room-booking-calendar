
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Designation"
              (
			  pvar_Designationid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Designationid" uuid
,workprofile Varchar
,designation Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:33*/
			  		 
              RETURN QUERY
			  SELECT  
				 Designation.tenantid
,tenant.businessname as _tenantname
,Designation.Designationid
,CAST(_WorkProfile.workprofilename AS VARCHAR) as workprofile
,Designation.designation

				 ,Designation.createduser,Designation.createddate,Designation.modifieduser,Designation.modifieddate
                 
                 
				 
			  FROM  Designation 
 LEFT OUTER JOIN tenant ON Designation.tenantid=tenant.tenantid
INNER JOIN WorkProfile _WorkProfile ON Designation.workprofile=_WorkProfile.WorkProfileid

			  WHERE CAST(Designation.Designationid AS Varchar)=pvar_Designationid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

