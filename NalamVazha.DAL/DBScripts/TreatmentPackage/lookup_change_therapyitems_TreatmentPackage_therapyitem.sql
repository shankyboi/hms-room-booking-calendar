
								CREATE OR REPLACE FUNCTION "lookup_change_therapyitems_TreatmentPackage_therapyitem"
								(
                                pvar_TherapyItemid Varchar(50)=null
								)
                                RETURNS TABLE("TherapyItemid" Varchar
,therapyitemname Varchar
,price Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:49*/
										
                                RETURN QUERY
								SELECT  
									CAST(TherapyItem.TherapyItemid AS Varchar) as TherapyItemid
,CAST(TherapyItem.therapyitemname AS Varchar) as therapyitemname
,CAST (TherapyItem.price AS VARCHAR) as price

								FROM TherapyItem
							    WHERE (CAST(TherapyItem.TherapyItemid AS VARCHAR) = pvar_TherapyItemid)
;
								
											
								
								END
                                $BODY$
                                LANGUAGE plpgsql;

