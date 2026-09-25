
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_MedicalCondition"
              (
			  pvar_MedicalConditionid Varchar
			  )
              RETURNS TABLE(
                "MedicalConditionid" uuid
,conditionname Varchar
,snomedid Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"automaton_MedicalCondition_synonyms" json
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:54*/
			  		 
              RETURN QUERY
			  SELECT  
				 MedicalCondition.MedicalConditionid
,MedicalCondition.conditionname
,MedicalCondition.snomedid
,MedicalCondition.description

				 ,MedicalCondition.createduser,MedicalCondition.createddate,MedicalCondition.modifieduser,MedicalCondition.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						MedicalCondition_synonyms.synonymname as "Synonym Name"
,MedicalCondition_synonyms.slanguage as "Language"

							
						FROM  MedicalCondition_synonyms 

						WHERE MedicalCondition.MedicalConditionid =MedicalCondition_synonyms.MedicalConditionid
) J)
						as automaton_MedicalCondition_synonyms

                 
				 
			  FROM  MedicalCondition 

			  WHERE CAST(MedicalCondition.MedicalConditionid AS Varchar)=pvar_MedicalConditionid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

