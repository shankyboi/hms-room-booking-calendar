CREATE OR REPLACE FUNCTION "getById_sp_MedicalCondition_synonyms"(
												 pvar_MedicalConditionid Varchar(50)
											 )
                                             RETURNS TABLE("MedicalConditionid" uuid,"MedicalCondition_synonymsid" uuid ,synonymname Varchar
,slanguage Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 MedicalCondition_synonyms.MedicalConditionid
                                             ,MedicalCondition_synonyms.MedicalCondition_synonymsid   
											 ,MedicalCondition_synonyms.synonymname
,MedicalCondition_synonyms.slanguage
 
											 
											 FROM MedicalCondition_synonyms
											 WHERE 
											 CAST(MedicalCondition_synonyms.MedicalConditionid AS VARCHAR)=pvar_MedicalConditionid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

