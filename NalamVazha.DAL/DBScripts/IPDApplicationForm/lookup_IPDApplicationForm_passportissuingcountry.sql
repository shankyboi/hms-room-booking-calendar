
								CREATE OR REPLACE FUNCTION  "lookup_IPDApplicationForm_passportissuingcountry"
								(
                                
                                
                                )
								RETURNS TABLE("Countryid" Varchar
,countryname Varchar
) 
						 		AS $BODY$
                                
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:00*/
							    
                                
                                RETURN QUERY        
								SELECT  
								CAST(Country.Countryid AS Varchar) as Countryid,CAST(Country.countryname AS Varchar) as countryname
								FROM Country
								 WHERE Country.isdeleted=false 

 ORDER BY Country.countryname ASC
                                ;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

