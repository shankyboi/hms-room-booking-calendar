
								CREATE OR REPLACE FUNCTION  "lookup_PatientVisit_opdnumber"
								(
                                pvar_tenantid Varchar=null
,pvar_patientname Varchar(50)=null

                                ,pvar_searchterm character varying='',
		pvar_pagesize integer=50,
	pvar_pagenumber integer=0
                                )
								RETURNS TABLE("OPDFormid" Varchar
,patientname Varchar
) 
						 		AS $BODY$
                                declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:59*/
							    
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
								CAST(OPDForm.OPDFormid AS Varchar) as OPDFormid,CAST(OPDForm.patientname AS Varchar) as patientname
								FROM OPDForm
								 WHERE COALESCE(cast(OPDForm.tenantid as varchar),'') = Any(lvar_tenantid) AND OPDForm.isdeleted=false
AND (pvar_patientname IS NOT NULL AND CAST( OPDForm.patientname AS Varchar) = pvar_patientname)
 AND OPDForm.verifiedstatus='Approved'
 AND  ((pvar_searchterm is null)  or OPDForm.patientname::varchar ilike pvar_searchterm or  CAST(OPDForm.OPDFormid AS Varchar) ilike pvar_searchterm)
 ORDER BY OPDForm.patientname ASC
                                 limit pvar_pagesize
offset pvar_pagenumber * pvar_pagesize;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

