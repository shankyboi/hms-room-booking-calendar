
								CREATE OR REPLACE FUNCTION  "lookup_DoctorInternMap_seniordoctor"
								(
                                pvar_tenantid Varchar=null

                                ,pvar_searchterm character varying='',
		pvar_pagesize integer=50,
	pvar_pagenumber integer=0
                                )
								RETURNS TABLE("Peopleid" Varchar
,firstname Varchar
,lastname Varchar
) 
						 		AS $BODY$
                                declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 07/21/2026 11:46:53*/
							    
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
								CAST(People.Peopleid AS Varchar) as Peopleid,CAST(People.firstname AS Varchar) as firstname,CAST(People.lastname AS Varchar) as lastname
								FROM People
								INNER JOIN WorkProfile ON People.workprofile = WorkProfile.WorkProfileid
								 WHERE COALESCE(cast(People.tenantid as varchar),'') = Any(lvar_tenantid) AND People.isdeleted=false AND COALESCE(People.status, 'Active')='Active'
								 AND WorkProfile.isdeleted=false
								 AND LOWER(TRIM(WorkProfile.rolename)) = 'doctor'
 AND  ((pvar_searchterm is null)  or People.firstname::varchar ilike pvar_searchterm or People.lastname::varchar ilike pvar_searchterm or  CAST(People.Peopleid AS Varchar) ilike pvar_searchterm)
 ORDER BY People.lastname ASC
                                 limit pvar_pagesize
offset pvar_pagenumber * pvar_pagesize;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;
