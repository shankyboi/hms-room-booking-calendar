
								        CREATE OR REPLACE FUNCTION  "lookup_IPDApplicationForm_medicalinfo_medicalconditionname"
								        (
                                        
                                        pvar_searchterm character varying='',
		pvar_pagesize integer=50,
	pvar_pagenumber integer=0
                                        )
								        RETURNS TABLE("MedicalConditionid" Varchar
,conditionname Varchar
) 
						 		        AS $BODY$
                                        
								        BEGIN
								        /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:00*/
							            
                                        	if(pvar_searchterm is not null and LENGTH(CAST(pvar_searchterm as Varchar)) > 0)
	then
		pvar_searchterm := '%' || pvar_searchterm || '%';
	else
		pvar_searchterm := null;
	end if;
                                        RETURN QUERY        
								        SELECT  
								        CAST(MedicalCondition.MedicalConditionid AS Varchar) as MedicalConditionid,CAST(MedicalCondition.conditionname AS Varchar) as conditionname
								        FROM MedicalCondition
								         WHERE MedicalCondition.isdeleted=false 
 AND  ((pvar_searchterm is null)  or MedicalCondition.conditionname::varchar ilike pvar_searchterm or  CAST(MedicalCondition.MedicalConditionid AS Varchar) ilike pvar_searchterm)
 ORDER BY MedicalCondition.conditionname ASC
                                         limit pvar_pagesize
offset pvar_pagenumber * pvar_pagesize;
								
											
								        END
                                        $BODY$
                                        LANGUAGE plpgsql;

