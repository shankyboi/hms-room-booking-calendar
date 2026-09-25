
								CREATE OR REPLACE FUNCTION "lookup_change_people_ShiftPlanning_personname"
								(
                                pvar_Peopleid Varchar(50)=null
								)
                                RETURNS TABLE("Peopleid" Varchar
,firstname Varchar
,lastname Varchar
,workprofile Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:20*/
										
                                RETURN QUERY
								SELECT  
									CAST(People.Peopleid AS Varchar) as Peopleid
,CAST(People.firstname AS Varchar) as firstname
,CAST(People.lastname AS Varchar) as lastname
,CAST (People.workprofile AS VARCHAR) as workprofile

								FROM People
							    WHERE (CAST(People.Peopleid AS VARCHAR) = pvar_Peopleid) AND COALESCE(People.status, 'Active')='Active'
;
								
											
								
								END
                                $BODY$
                                LANGUAGE plpgsql;

