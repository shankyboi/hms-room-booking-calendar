
CREATE OR REPLACE FUNCTION public."Shift_Planning_List"(
	pvar_tenantid character varying,
	pvar_shiftname character varying,
	pvar_pagesize integer,
	pvar_pagenumber integer,
	pvar_searchterm character varying,
	pvar_sort_fields json)
    RETURNS json
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
               declare local_sortcolumn_array text[] = (
	                select array_agg(col) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)
                );
                declare local_sortorder_array text[] = (
	                select array_agg(dir) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)	
                );          
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;  
               
          	  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:20*/
			  		
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
                    FROM  ShiftPlanning 
 LEFT OUTER JOIN tenant ON ShiftPlanning.tenantid=tenant.tenantid
INNER JOIN Shift _Shift ON ShiftPlanning.shiftname=_Shift.Shiftid

                    WHERE (lvar_tenantid is null or COALESCE(cast(ShiftPlanning.tenantid as varchar), '') = Any(lvar_tenantid)) AND ShiftPlanning.isdeleted=false
AND (pvar_shiftname is null or pvar_shiftname ='0' or LENGTH(CAST(pvar_shiftname as Varchar))=0 or CAST(ShiftPlanning.shiftname as VARCHAR)=pvar_shiftname)
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_Shift.shiftname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ShiftPlanning.shiftstarttime AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ShiftPlanning.shiftendtime AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(ShiftPlanning.validfrom,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(ShiftPlanning.validto,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    ShiftPlanning.tenantid
,tenant.businessname as _tenantName
,ShiftPlanning.ShiftPlanningid
,ShiftPlanning.shiftname
,CAST(_Shift.shiftname AS VARCHAR) as shiftname_master
,ShiftPlanning.shiftstarttime
,ShiftPlanning.shiftendtime
,CAST(COALESCE(to_char(ShiftPlanning.validfrom,'dd/MM/yyyy'),'') AS Varchar) as validfrom
,CAST(COALESCE(to_char(ShiftPlanning.validto,'dd/MM/yyyy'),'') AS Varchar) as validto

                    ,
                (SELECT json_agg(J) FROM (SELECT   
				 CAST(_People.firstname||' '||_People.lastname AS VARCHAR) as "Person Name"
,CAST(__WorkProfile.workprofilename AS VARCHAR) as "Work Profile"
,ShiftPlanning_people.coveragetype as "Coverage Type "

		 	   FROM  ShiftPlanning_people 
INNER JOIN People _People ON ShiftPlanning_people.personname=_People.Peopleid
LEFT OUTER JOIN WorkProfile __WorkProfile ON ShiftPlanning_people.workprofile=__WorkProfile.WorkProfileid

			  WHERE ShiftPlanning.ShiftPlanningid =ShiftPlanning_people.ShiftPlanningid
) J)
			    as automaton_ShiftPlanning_people

                    ,ShiftPlanning.createduser,ShiftPlanning.createddate,ShiftPlanning.modifieduser,ShiftPlanning.modifieddate
                    FROM  ShiftPlanning 
 LEFT OUTER JOIN tenant ON ShiftPlanning.tenantid=tenant.tenantid
INNER JOIN Shift _Shift ON ShiftPlanning.shiftname=_Shift.Shiftid

                    WHERE (lvar_tenantid is null or COALESCE(cast(ShiftPlanning.tenantid as varchar), '') = Any(lvar_tenantid)) AND ShiftPlanning.isdeleted=false
AND (pvar_shiftname is null or pvar_shiftname ='0' or LENGTH(CAST(pvar_shiftname as Varchar))=0 or CAST(ShiftPlanning.shiftname as VARCHAR)=pvar_shiftname)

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_Shift.shiftname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ShiftPlanning.shiftstarttime AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ShiftPlanning.shiftendtime AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(ShiftPlanning.validfrom,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(ShiftPlanning.validto,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'shiftname' THEN _Shift.shiftname::TEXT
WHEN 'shiftstarttime' THEN ShiftPlanning.shiftstarttime::TEXT
WHEN 'shiftstarttime' THEN ShiftPlanning.shiftstarttime::TEXT
WHEN 'shiftendtime' THEN ShiftPlanning.shiftendtime::TEXT
WHEN 'shiftendtime' THEN ShiftPlanning.shiftendtime::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'validfrom' THEN ShiftPlanning.validfrom
WHEN 'validto' THEN ShiftPlanning.validto
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'shiftname' THEN _Shift.shiftname::TEXT
WHEN 'shiftstarttime' THEN ShiftPlanning.shiftstarttime::TEXT
WHEN 'shiftstarttime' THEN ShiftPlanning.shiftstarttime::TEXT
WHEN 'shiftendtime' THEN ShiftPlanning.shiftendtime::TEXT
WHEN 'shiftendtime' THEN ShiftPlanning.shiftendtime::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'validfrom' THEN ShiftPlanning.validfrom
WHEN 'validto' THEN ShiftPlanning.validto
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC

                    limit pvar_pagesize
                    offset pvar_pagenumber * pvar_pagesize			 	
			 	
                    ) d));
	
			  
					 	
			  END
              
$BODY$;

