
								CREATE OR REPLACE FUNCTION "lookup_change_therapykits_TreatmentPackage_therapykitname"
								(
                                pvar_TherapyKitid Varchar(50)=null
								)
                                RETURNS TABLE("TherapyKitid" Varchar
,therapykitname Varchar
,kitprice Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:49*/
										
                                RETURN QUERY
								SELECT  
									CAST(TherapyKit.TherapyKitid AS Varchar) as TherapyKitid
,CAST(TherapyKit.therapykitname AS Varchar) as therapykitname
,CAST (TherapyKit.kitprice AS VARCHAR) as kitprice

								FROM TherapyKit
							    WHERE (CAST(TherapyKit.TherapyKitid AS VARCHAR) = pvar_TherapyKitid)
;
								
											
								
								END
                                $BODY$
                                LANGUAGE plpgsql;

