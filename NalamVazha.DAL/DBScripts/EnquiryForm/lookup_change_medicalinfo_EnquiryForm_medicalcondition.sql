
								CREATE OR REPLACE FUNCTION "lookup_change_medicalinfo_EnquiryForm_medicalcondition"
								(
                                pvar_MedicalConditionid Varchar(50)=null
								)
                                RETURNS TABLE("MedicalConditionid" Varchar
,conditionname Varchar
,conditionname Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:41*/
										
                                RETURN QUERY
								SELECT  
									CAST(MedicalCondition.MedicalConditionid AS Varchar) as MedicalConditionid
,CAST(MedicalCondition.conditionname AS Varchar) as conditionname
,CAST (MedicalCondition.conditionname AS VARCHAR) as conditionname

								FROM MedicalCondition
							    WHERE (CAST(MedicalCondition.MedicalConditionid AS VARCHAR) = pvar_MedicalConditionid)
;
								
											
								
								END
                                $BODY$
                                LANGUAGE plpgsql;

