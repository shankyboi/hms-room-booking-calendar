
			  CREATE OR REPLACE FUNCTION  "otplogs_List"
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:45*/
			  		

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
                    FROM  otplogs 

                    WHERE otplogs.isdeleted=false 
 AND ( ((pvar_searchterm is null) or CAST(otplogs.username AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(otplogs.otpcode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(otplogs.expirytime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    otplogs.otplogsid
,otplogs.username
,otplogs.otpcode
,CAST(COALESCE(to_char(otplogs.expirytime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as expirytime
,CAST(case when otplogs.isused=true then 'Yes' else 'No' End AS Varchar)as isused

                    
                    ,otplogs.createduser,otplogs.createddate,otplogs.modifieduser,otplogs.modifieddate
                    FROM  otplogs 

                    WHERE otplogs.isdeleted=false 

                     AND ( ((pvar_searchterm is null) or CAST(otplogs.username AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(otplogs.otpcode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(otplogs.expirytime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'username' THEN otplogs.username::TEXT
WHEN 'otpcode' THEN otplogs.otpcode::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'expirytime' THEN otplogs.expirytime
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'isused' THEN otplogs.isused
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'otpcode' THEN otplogs.otpcode::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'username' THEN otplogs.username::TEXT
WHEN 'otpcode' THEN otplogs.otpcode::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'expirytime' THEN otplogs.expirytime
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'isused' THEN otplogs.isused
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'otpcode' THEN otplogs.otpcode::NUMERIC
			
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

