 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_MedicalCondition"
              (
			    pvar_tenantid Varchar=null,
                pvar_searchterm character varying='',
                pvar_pagesize integer=50,
                pvar_pagenumber integer=0
              )
			  RETURNS TABLE(
                conditionname Varchar
,snomedid Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"MedicalConditionid" uuid
               )
               AS $BODY$
               BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:54*/

                if(pvar_searchterm is not null and LENGTH(CAST(pvar_searchterm as Varchar)) > 0)
                then
                pvar_searchterm := '%' || pvar_searchterm || '%';
                else
                pvar_searchterm := null;
                end if;

			 
                RETURN QUERY
                SELECT 
                MedicalCondition.conditionname
,MedicalCondition.snomedid
,MedicalCondition.description

                ,MedicalCondition.createduser,MedicalCondition.createddate,MedicalCondition.modifieduser,MedicalCondition.modifieddate
                ,MedicalCondition.MedicalConditionid
                FROM MedicalCondition
			    
                 WHERE MedicalCondition.isdeleted=false
                
                 AND (((pvar_searchterm is null) or COALESCE(MedicalCondition.MedicalConditionid::varchar,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(MedicalCondition.conditionname,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(MedicalCondition.snomedid,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(MedicalCondition.description,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

