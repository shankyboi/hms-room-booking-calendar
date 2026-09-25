
								CREATE OR REPLACE FUNCTION "lookup_change_therapy_TreatmentPackage_therapyname"
								(
                                pvar_Therapiesid Varchar(50)=null
								)
                                RETURNS TABLE("Therapiesid" Varchar
,therapyname Varchar
,therapycost Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:49*/
										
                                RETURN QUERY
								SELECT  
									CAST(Therapies.Therapiesid AS Varchar) as Therapiesid
,CAST(Therapies.therapyname AS Varchar) as therapyname
,CAST (Therapies.therapycost AS VARCHAR) as therapycost

								FROM Therapies
							    WHERE (CAST(Therapies.Therapiesid AS VARCHAR) = pvar_Therapiesid)
;
								
											
								
								END
                                $BODY$
                                LANGUAGE plpgsql;

