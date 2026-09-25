 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_TherapyItem"
			  (
				  pvar_TherapyItemid Varchar
			  )
			  RETURNS TABLE(
                therapyitemcategory uuid
,therapyitemname Varchar
,price decimal
,therapyitemimage Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,TherapyItemid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:42*/
               
              RETURN QUERY
			  SELECT 
				 TherapyItem.therapyitemcategory
,TherapyItem.therapyitemname
,TherapyItem.price
,TherapyItem.therapyitemimage

				 ,TherapyItem.createduser,TherapyItem.createddate,TherapyItem.modifieduser,TherapyItem.modifieddate
				 ,TherapyItem.tenantid
                 ,TherapyItem.TherapyItemid
                    
			  FROM TherapyItem
			  WHERE CAST(TherapyItem.TherapyItemid AS Varchar)=pvar_TherapyItemid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

