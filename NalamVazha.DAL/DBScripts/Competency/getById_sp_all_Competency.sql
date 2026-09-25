
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Competency"
              (
			  pvar_Competencyid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Competencyid" uuid
,competencyname Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:27*/
			  		 
              RETURN QUERY
			  SELECT  
				 Competency.tenantid
,tenant.businessname as _tenantname
,Competency.Competencyid
,Competency.competencyname
,Competency.description

				 ,Competency.createduser,Competency.createddate,Competency.modifieduser,Competency.modifieddate
                 
                 
				 
			  FROM  Competency 
 LEFT OUTER JOIN tenant ON Competency.tenantid=tenant.tenantid

			  WHERE CAST(Competency.Competencyid AS Varchar)=pvar_Competencyid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

