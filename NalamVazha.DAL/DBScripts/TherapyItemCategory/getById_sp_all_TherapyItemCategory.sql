
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_TherapyItemCategory"
              (
			  pvar_TherapyItemCategoryid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"TherapyItemCategoryid" uuid
,itemcategory Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:39*/
			  		 
              RETURN QUERY
			  SELECT  
				 TherapyItemCategory.tenantid
,tenant.businessname as _tenantname
,TherapyItemCategory.TherapyItemCategoryid
,TherapyItemCategory.itemcategory
,TherapyItemCategory.description

				 ,TherapyItemCategory.createduser,TherapyItemCategory.createddate,TherapyItemCategory.modifieduser,TherapyItemCategory.modifieddate
                 
                 
				 
			  FROM  TherapyItemCategory 
 LEFT OUTER JOIN tenant ON TherapyItemCategory.tenantid=tenant.tenantid

			  WHERE CAST(TherapyItemCategory.TherapyItemCategoryid AS Varchar)=pvar_TherapyItemCategoryid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

