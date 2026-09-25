 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Competency"
			  (
				  pvar_Competencyid Varchar
			  )
			  RETURNS TABLE(
                competencyname Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Competencyid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:27*/
               
              RETURN QUERY
			  SELECT 
				 Competency.competencyname
,Competency.description

				 ,Competency.createduser,Competency.createddate,Competency.modifieduser,Competency.modifieddate
				 ,Competency.tenantid
                 ,Competency.Competencyid
                    
			  FROM Competency
			  WHERE CAST(Competency.Competencyid AS Varchar)=pvar_Competencyid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

