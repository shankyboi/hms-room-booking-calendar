 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_Country"
              (
			  pvar_tenantid Varchar=null
              )
			  RETURNS TABLE(
                countryname Varchar
,countrycode Varchar
,countryshortcode Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"Countryid" uuid
               )
               AS $BODY$
               BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:31*/

			 
                RETURN QUERY
                SELECT 
                Country.countryname
,Country.countrycode
,Country.countryshortcode

                ,Country.createduser,Country.createddate,Country.modifieduser,Country.modifieddate
                ,Country.Countryid
                FROM Country
			    
                 WHERE Country.isdeleted=false
                ;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

