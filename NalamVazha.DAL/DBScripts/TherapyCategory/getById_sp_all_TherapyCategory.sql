
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_TherapyCategory"
              (
			  pvar_TherapyCategoryid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"TherapyCategoryid" uuid
,categoryname Varchar
,categorydescription Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:28*/
			  		 
              RETURN QUERY
			  SELECT  
				 TherapyCategory.tenantid
,tenant.businessname as _tenantname
,TherapyCategory.TherapyCategoryid
,TherapyCategory.categoryname
,TherapyCategory.categorydescription

				 ,TherapyCategory.createduser,TherapyCategory.createddate,TherapyCategory.modifieduser,TherapyCategory.modifieddate
                 
                 
				 
			  FROM  TherapyCategory 
 LEFT OUTER JOIN tenant ON TherapyCategory.tenantid=tenant.tenantid

			  WHERE CAST(TherapyCategory.TherapyCategoryid AS Varchar)=pvar_TherapyCategoryid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

