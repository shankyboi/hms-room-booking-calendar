
			  CREATE OR REPLACE FUNCTION  "MailLogs_List"
              (pvar_entityname Varchar(1024)
,pvar_mailfor Varchar(1024)
,pvar_pagesize integer
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:28*/
			  		

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
                    FROM  MailLogs 

                    WHERE MailLogs.isdeleted=false 
AND (pvar_entityname is null or pvar_entityname ='0' or LENGTH(CAST(pvar_entityname as Varchar))=0 or CAST(MailLogs.entityname as VARCHAR)=pvar_entityname)
AND (pvar_mailfor is null or pvar_mailfor ='0' or LENGTH(CAST(pvar_mailfor as Varchar))=0 or CAST(MailLogs.mailfor as VARCHAR)=pvar_mailfor)
 AND ( ((pvar_searchterm is null) or CAST(MailLogs.entityname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MailLogs.entityid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MailLogs.mailfor AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MailLogs.mailsubject AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MailLogs.mailto AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MailLogs.mailbody AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    MailLogs.MailLogsid
,MailLogs.entityname
,MailLogs.entityid
,MailLogs.mailfor
,MailLogs.mailsubject
,MailLogs.mailto
,MailLogs.mailbody
,CAST(case when MailLogs.issent=true then 'Yes' else 'No' End AS Varchar) as issent

                    
                    ,MailLogs.createduser,MailLogs.createddate,MailLogs.modifieduser,MailLogs.modifieddate
                    FROM  MailLogs 

                    WHERE MailLogs.isdeleted=false 
AND (pvar_entityname is null or pvar_entityname ='0' or LENGTH(CAST(pvar_entityname as Varchar))=0 or CAST(MailLogs.entityname as VARCHAR)=pvar_entityname)
AND (pvar_mailfor is null or pvar_mailfor ='0' or LENGTH(CAST(pvar_mailfor as Varchar))=0 or CAST(MailLogs.mailfor as VARCHAR)=pvar_mailfor)

                     AND ( ((pvar_searchterm is null) or CAST(MailLogs.entityname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MailLogs.entityid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MailLogs.mailfor AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MailLogs.mailsubject AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MailLogs.mailto AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(MailLogs.mailbody AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'entityname' THEN MailLogs.entityname::TEXT
WHEN 'entityid' THEN MailLogs.entityid::TEXT
WHEN 'mailfor' THEN MailLogs.mailfor::TEXT
WHEN 'mailsubject' THEN MailLogs.mailsubject::TEXT
WHEN 'mailto' THEN MailLogs.mailto::TEXT
WHEN 'mailbody' THEN MailLogs.mailbody::TEXT
WHEN 'mailbody' THEN MailLogs.mailbody::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'issent' THEN MailLogs.issent
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'entityname' THEN MailLogs.entityname::TEXT
WHEN 'entityid' THEN MailLogs.entityid::TEXT
WHEN 'mailfor' THEN MailLogs.mailfor::TEXT
WHEN 'mailsubject' THEN MailLogs.mailsubject::TEXT
WHEN 'mailto' THEN MailLogs.mailto::TEXT
WHEN 'mailbody' THEN MailLogs.mailbody::TEXT
WHEN 'mailbody' THEN MailLogs.mailbody::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'issent' THEN MailLogs.issent
			
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

