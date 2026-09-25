
			  CREATE OR REPLACE FUNCTION  "Shift_List"
              (pvar_tenantid Varchar
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
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;  
               
          	  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:17*/
			  		
                SELECT  SPLIT_PART(pvar_tenantid, '|', 1),SPLIT_PART(pvar_tenantid, '|', 2) into lstr_usersid,pvar_tenantid;
		        
                if(pvar_tenantid is null or pvar_tenantid='' or pvar_tenantid='00000000-0000-0000-0000-000000000000')	
				then
                    SELECT STRING_TO_ARRAY(viewertenantids, ',') into lvar_tenantid
				    FROM users where users.usersid::varchar=lstr_usersid;	
                    if(lvar_tenantid is NULL)
					then 
						SELECT array_agg(tenant.tenantid) INTO lvar_tenantid FROM tenant;
               
					end if;
                else 
				  lvar_tenantid=ARRAY[pvar_tenantid];
                end if;
                lvar_tenantid := lvar_tenantid || ARRAY[''::character varying] || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];




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
                    FROM  Shift 
 LEFT OUTER JOIN tenant ON Shift.tenantid=tenant.tenantid

                    WHERE (lvar_tenantid is null or COALESCE(cast(Shift.tenantid as varchar), '') = Any(lvar_tenantid)) AND Shift.isdeleted=false
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.shiftcode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.shiftname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.shiftstarttime AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.shiftendtime AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.shifthours AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.description AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.totalbreakinmins AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.totalbreakinhrs AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.workhours AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    Shift.tenantid
,tenant.businessname as _tenantName
,Shift.Shiftid
,Shift.shiftcode
,Shift.shiftname
,Shift.shiftstarttime
,Shift.shiftendtime
,Shift.shifthours
,Shift.description
,Shift.totalbreakinmins
,Shift.totalbreakinhrs
,Shift.workhours

                    ,
                (SELECT json_agg(J) FROM (SELECT   
				 Shift_breakdurationdetails.breakname as "Break Name"
,Shift_breakdurationdetails.starttime as "Start Time"
,Shift_breakdurationdetails.endtime as "End Time"
,Shift_breakdurationdetails.durationinmin as "Duration in Min"

		 	   FROM  Shift_breakdurationdetails 

			  WHERE Shift.Shiftid =Shift_breakdurationdetails.Shiftid
) J)
			    as automaton_Shift_breakdurationdetails

                    ,Shift.createduser,Shift.createddate,Shift.modifieduser,Shift.modifieddate
                    FROM  Shift 
 LEFT OUTER JOIN tenant ON Shift.tenantid=tenant.tenantid

                    WHERE (lvar_tenantid is null or COALESCE(cast(Shift.tenantid as varchar), '') = Any(lvar_tenantid)) AND Shift.isdeleted=false

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.shiftcode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.shiftname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.shiftstarttime AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.shiftendtime AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.shifthours AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.description AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.totalbreakinmins AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.totalbreakinhrs AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Shift.workhours AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'shiftcode' THEN Shift.shiftcode::TEXT
WHEN 'shiftcode' THEN Shift.shiftcode::TEXT
WHEN 'shiftname' THEN Shift.shiftname::TEXT
WHEN 'shiftstarttime' THEN Shift.shiftstarttime::TEXT
WHEN 'shiftstarttime' THEN Shift.shiftstarttime::TEXT
WHEN 'shiftendtime' THEN Shift.shiftendtime::TEXT
WHEN 'shiftendtime' THEN Shift.shiftendtime::TEXT
WHEN 'shifthours' THEN Shift.shifthours::TEXT
WHEN 'description' THEN Shift.description::TEXT
WHEN 'totalbreakinmins' THEN Shift.totalbreakinmins::TEXT
WHEN 'totalbreakinhrs' THEN Shift.totalbreakinhrs::TEXT
WHEN 'workhours' THEN Shift.workhours::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'shifthours' THEN Shift.shifthours::NUMERIC
WHEN 'totalbreakinhrs' THEN Shift.totalbreakinhrs::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'shiftcode' THEN Shift.shiftcode::TEXT
WHEN 'shiftcode' THEN Shift.shiftcode::TEXT
WHEN 'shiftname' THEN Shift.shiftname::TEXT
WHEN 'shiftstarttime' THEN Shift.shiftstarttime::TEXT
WHEN 'shiftstarttime' THEN Shift.shiftstarttime::TEXT
WHEN 'shiftendtime' THEN Shift.shiftendtime::TEXT
WHEN 'shiftendtime' THEN Shift.shiftendtime::TEXT
WHEN 'shifthours' THEN Shift.shifthours::TEXT
WHEN 'description' THEN Shift.description::TEXT
WHEN 'totalbreakinmins' THEN Shift.totalbreakinmins::TEXT
WHEN 'totalbreakinhrs' THEN Shift.totalbreakinhrs::TEXT
WHEN 'workhours' THEN Shift.workhours::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'shifthours' THEN Shift.shifthours::NUMERIC
WHEN 'totalbreakinhrs' THEN Shift.totalbreakinhrs::NUMERIC
			
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

