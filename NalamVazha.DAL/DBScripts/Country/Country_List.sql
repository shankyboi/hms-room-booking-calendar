
			  CREATE OR REPLACE FUNCTION  "Country_List"
              ()
			  RETURNS TABLE(Countryid uuid
,countryname Varchar,countrycode Varchar,countryshortcode Varchar,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:31*/
			  		
              
                RETURN QUERY
				SELECT  
				Country.Countryid
,Country.countryname
,Country.countrycode
,Country.countryshortcode

				
				,Country.createduser,Country.createddate,Country.modifieduser,Country.modifieddate
				FROM  Country 

				WHERE Country.isdeleted=false 

				 ORDER BY Country.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

