
			  CREATE OR REPLACE FUNCTION  "Medical_Condition_List"
              (pvar_pagesize integer
,pvar_pagenumber integer
,pvar_searchterm varchar
,pvar_sort_fields json



                )
			  RETURNS json
			  AS $BODY$
               declare local_sortcolumn_array text[] = (
	                select array_agg(col) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)
                );
                declare local_sortorder_array text[] = (
	                select array_agg(dir) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)	
                );          
                
               
          	  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:54*/
			  		

                    if(pvar_searchterm is not null and LENGTH(CAST(pvar_searchterm as Varchar)) > 0)
                    then
                    pvar_searchterm := '%' || pvar_searchterm || '%';
                    else
                    pvar_searchterm := null;
                    end if;
              
                    RETURN json_build_object(
                    'count'
                    ,(SELECT  
                    COUNT(*)
                    FROM  MedicalCondition 

                    WHERE MedicalCondition.isdeleted=false 
 AND ( ((pvar_searchterm is null) or CAST(MedicalCondition.conditionname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MedicalCondition.snomedid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MedicalCondition.description AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    MedicalCondition.MedicalConditionid
,MedicalCondition.conditionname
,MedicalCondition.snomedid
,MedicalCondition.description

                    ,
                (SELECT json_agg(J) FROM (SELECT   
				 MedicalCondition_synonyms.synonymname as "Synonym Name"
,MedicalCondition_synonyms.slanguage as "Language"

		 	   FROM  MedicalCondition_synonyms 

			  WHERE MedicalCondition.MedicalConditionid =MedicalCondition_synonyms.MedicalConditionid
) J)
			    as automaton_MedicalCondition_synonyms

                    ,MedicalCondition.createduser,MedicalCondition.createddate,MedicalCondition.modifieduser,MedicalCondition.modifieddate
                    FROM  MedicalCondition 

                    WHERE MedicalCondition.isdeleted=false 

                     AND ( ((pvar_searchterm is null) or CAST(MedicalCondition.conditionname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MedicalCondition.snomedid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MedicalCondition.description AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'conditionname' THEN MedicalCondition.conditionname::TEXT
WHEN 'snomedid' THEN MedicalCondition.snomedid::TEXT
WHEN 'description' THEN MedicalCondition.description::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'conditionname' THEN MedicalCondition.conditionname::TEXT
WHEN 'snomedid' THEN MedicalCondition.snomedid::TEXT
WHEN 'description' THEN MedicalCondition.description::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC

                    limit pvar_pagesize
                    offset pvar_pagenumber * pvar_pagesize			 	
			 	
                    ) d));
	
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

