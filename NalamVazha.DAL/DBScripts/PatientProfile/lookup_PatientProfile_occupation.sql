
								CREATE OR REPLACE FUNCTION  "lookup_PatientProfile_occupation"
								(
                                
                                
                                )
								RETURNS TABLE("Occupationid" Varchar
,occupationname Varchar
) 
						 		AS $BODY$
                                
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:57*/
							    
                                
                                RETURN QUERY        
								SELECT  
								CAST(Occupation.Occupationid AS Varchar) as Occupationid,CAST(Occupation.occupationname AS Varchar) as occupationname
								FROM Occupation
								 WHERE Occupation.isdeleted=false 

 ORDER BY Occupation.occupationname ASC
                                ;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

