 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_TherapyItemCategory"
			  (
				  pvar_TherapyItemCategoryid Varchar
			  )
			  RETURNS TABLE(
                itemcategory Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,TherapyItemCategoryid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:39*/
               
              RETURN QUERY
			  SELECT 
				 TherapyItemCategory.itemcategory
,TherapyItemCategory.description

				 ,TherapyItemCategory.createduser,TherapyItemCategory.createddate,TherapyItemCategory.modifieduser,TherapyItemCategory.modifieddate
				 ,TherapyItemCategory.tenantid
                 ,TherapyItemCategory.TherapyItemCategoryid
                    
			  FROM TherapyItemCategory
			  WHERE CAST(TherapyItemCategory.TherapyItemCategoryid AS Varchar)=pvar_TherapyItemCategoryid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

