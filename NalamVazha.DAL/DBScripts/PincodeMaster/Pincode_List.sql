
			  CREATE OR REPLACE FUNCTION  "Pincode_List"
              (pvar_pincode Varchar(1024)
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/16/2026 09:42:32*/
			  		

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
                    FROM  PincodeMaster 

                    WHERE PincodeMaster.isdeleted=false 
AND (pvar_pincode is null or pvar_pincode ='0' or LENGTH(CAST(pvar_pincode as Varchar))=0 or CAST(PincodeMaster.pincode as VARCHAR)=pvar_pincode)
 AND ( ((pvar_searchterm is null) or CAST(PincodeMaster.circlename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.regionname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.divisionname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.officename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.pincode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.officetype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.delivery AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.district AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.statename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.latitude AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.longitude AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    PincodeMaster.PincodeMasterid
,PincodeMaster.circlename
,PincodeMaster.regionname
,PincodeMaster.divisionname
,PincodeMaster.officename
,PincodeMaster.pincode
,PincodeMaster.officetype
,PincodeMaster.delivery
,PincodeMaster.district
,PincodeMaster.statename
,PincodeMaster.latitude
,PincodeMaster.longitude

                    
                    ,PincodeMaster.createduser,PincodeMaster.createddate,PincodeMaster.modifieduser,PincodeMaster.modifieddate
                    FROM  PincodeMaster 

                    WHERE PincodeMaster.isdeleted=false 
AND (pvar_pincode is null or pvar_pincode ='0' or LENGTH(CAST(pvar_pincode as Varchar))=0 or CAST(PincodeMaster.pincode as VARCHAR)=pvar_pincode)

                     AND ( ((pvar_searchterm is null) or CAST(PincodeMaster.circlename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.regionname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.divisionname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.officename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.pincode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.officetype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.delivery AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.district AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.statename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.latitude AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PincodeMaster.longitude AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'circlename' THEN PincodeMaster.circlename::TEXT
WHEN 'regionname' THEN PincodeMaster.regionname::TEXT
WHEN 'divisionname' THEN PincodeMaster.divisionname::TEXT
WHEN 'officename' THEN PincodeMaster.officename::TEXT
WHEN 'pincode' THEN PincodeMaster.pincode::TEXT
WHEN 'officetype' THEN PincodeMaster.officetype::TEXT
WHEN 'delivery' THEN PincodeMaster.delivery::TEXT
WHEN 'district' THEN PincodeMaster.district::TEXT
WHEN 'statename' THEN PincodeMaster.statename::TEXT
WHEN 'latitude' THEN PincodeMaster.latitude::TEXT
WHEN 'longitude' THEN PincodeMaster.longitude::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'circlename' THEN PincodeMaster.circlename::TEXT
WHEN 'regionname' THEN PincodeMaster.regionname::TEXT
WHEN 'divisionname' THEN PincodeMaster.divisionname::TEXT
WHEN 'officename' THEN PincodeMaster.officename::TEXT
WHEN 'pincode' THEN PincodeMaster.pincode::TEXT
WHEN 'officetype' THEN PincodeMaster.officetype::TEXT
WHEN 'delivery' THEN PincodeMaster.delivery::TEXT
WHEN 'district' THEN PincodeMaster.district::TEXT
WHEN 'statename' THEN PincodeMaster.statename::TEXT
WHEN 'latitude' THEN PincodeMaster.latitude::TEXT
WHEN 'longitude' THEN PincodeMaster.longitude::TEXT
			
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

