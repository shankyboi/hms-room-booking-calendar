
CREATE OR REPLACE FUNCTION public."lookup_PeopleTimePreference_shiftname"(
	pvar_tenantid character varying DEFAULT NULL::character varying,
	pvar_searchterm character varying DEFAULT ''::character varying,
	pvar_pagesize integer DEFAULT 50,
	pvar_pagenumber integer DEFAULT 0)
    RETURNS TABLE("Shiftid" character varying, shiftname character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                                declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 14:00:30*/
							    
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
                                RETURN QUERY        
								SELECT  
								CAST(Shift.Shiftid AS Varchar) as Shiftid,CAST(Shift.shiftname AS Varchar) as shiftname
								FROM Shift
								 WHERE COALESCE(cast(Shift.tenantid as varchar),'') = Any(lvar_tenantid) AND Shift.isdeleted=false
 AND  ((pvar_searchterm is null)  or Shift.shiftname::varchar ilike pvar_searchterm or  CAST(Shift.Shiftid AS Varchar) ilike pvar_searchterm)
 ORDER BY Shift.shiftname ASC
                                 limit pvar_pagesize
offset pvar_pagenumber * pvar_pagesize;
								
											
								END
                                
$BODY$;

