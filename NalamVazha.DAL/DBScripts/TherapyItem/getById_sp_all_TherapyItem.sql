
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_TherapyItem"
              (
			  pvar_TherapyItemid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"TherapyItemid" uuid
,therapyitemcategory Varchar
,therapyitemname Varchar
,price decimal
,therapyitemimage Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:42*/
			  		 
              RETURN QUERY
			  SELECT  
				 TherapyItem.tenantid
,tenant.businessname as _tenantname
,TherapyItem.TherapyItemid
,CAST(_TherapyItemCategory.itemcategory AS VARCHAR) as therapyitemcategory
,TherapyItem.therapyitemname
,TherapyItem.price
,TherapyItem.therapyitemimage

				 ,TherapyItem.createduser,TherapyItem.createddate,TherapyItem.modifieduser,TherapyItem.modifieddate
                 
                 
				 
			  FROM  TherapyItem 
 LEFT OUTER JOIN tenant ON TherapyItem.tenantid=tenant.tenantid
INNER JOIN TherapyItemCategory _TherapyItemCategory ON TherapyItem.therapyitemcategory=_TherapyItemCategory.TherapyItemCategoryid

			  WHERE CAST(TherapyItem.TherapyItemid AS Varchar)=pvar_TherapyItemid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

