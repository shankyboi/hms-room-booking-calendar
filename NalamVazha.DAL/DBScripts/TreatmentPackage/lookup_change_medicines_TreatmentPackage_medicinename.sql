
								CREATE OR REPLACE FUNCTION "lookup_change_medicines_TreatmentPackage_medicinename"
								(
                                pvar_Medicineid Varchar(50)=null
								)
                                RETURNS TABLE("Medicineid" Varchar
,medicinename Varchar
,price Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:49*/
										
                                RETURN QUERY
								SELECT  
									CAST(Medicine.Medicineid AS Varchar) as Medicineid
,CAST(Medicine.medicinename AS Varchar) as medicinename
,CAST (Medicine.price AS VARCHAR) as price

								FROM Medicine
							    WHERE (CAST(Medicine.Medicineid AS VARCHAR) = pvar_Medicineid)
;
								
											
								
								END
                                $BODY$
                                LANGUAGE plpgsql;

