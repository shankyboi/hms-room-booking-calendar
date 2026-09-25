 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_MedicalCondition"
			  (
				  pvar_MedicalConditionid Varchar
			  )
			  RETURNS TABLE(
                conditionname Varchar
,snomedid Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,MedicalConditionid uuid
                ,synonyms JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:54*/
               
              RETURN QUERY
			  SELECT 
				 MedicalCondition.conditionname
,MedicalCondition.snomedid
,MedicalCondition.description

				 ,MedicalCondition.createduser,MedicalCondition.createddate,MedicalCondition.modifieduser,MedicalCondition.modifieddate
				 
                 ,MedicalCondition.MedicalConditionid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 MedicalCondition_synonyms.MedicalConditionid
                                             ,MedicalCondition_synonyms.MedicalCondition_synonymsid   
											 ,MedicalCondition_synonyms.synonymname
,MedicalCondition_synonyms.slanguage
 
											  
											 FROM MedicalCondition_synonyms
											 WHERE 
											 MedicalCondition_synonyms.MedicalConditionid=MedicalCondition.MedicalConditionid
                                             
                                             ORDER BY record_order DESC
											) J) as synonyms
   
			  FROM MedicalCondition
			  WHERE CAST(MedicalCondition.MedicalConditionid AS Varchar)=pvar_MedicalConditionid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

