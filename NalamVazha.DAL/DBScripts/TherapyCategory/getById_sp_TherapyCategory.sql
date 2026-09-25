 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_TherapyCategory"
			  (
				  pvar_TherapyCategoryid Varchar
			  )
			  RETURNS TABLE(
                categoryname Varchar
,categorydescription Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,TherapyCategoryid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:28*/
               
              RETURN QUERY
			  SELECT 
				 TherapyCategory.categoryname
,TherapyCategory.categorydescription

				 ,TherapyCategory.createduser,TherapyCategory.createddate,TherapyCategory.modifieduser,TherapyCategory.modifieddate
				 ,TherapyCategory.tenantid
                 ,TherapyCategory.TherapyCategoryid
                    
			  FROM TherapyCategory
			  WHERE CAST(TherapyCategory.TherapyCategoryid AS Varchar)=pvar_TherapyCategoryid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

