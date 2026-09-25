
								CREATE OR REPLACE FUNCTION  "lookup_Finance_bookingreferencenumber"
								(
                                pvar_tenantid Varchar=null
,pvar_patientname Varchar(50)=null

                                ,pvar_searchterm character varying='',
		pvar_pagesize integer=50,
	pvar_pagenumber integer=0
                                )
								RETURNS TABLE("BillingPaymentid" Varchar
,ipdnumber Varchar
,opdnumber Varchar
) 
						 		AS $BODY$
                                declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 10:01:30*/
							    
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
								SELECT DISTINCT CAST(resolved.reference AS varchar) AS BillingPaymentid,
								CAST(resolved.reference AS varchar) AS ipdnumber, ''::varchar AS opdnumber
								FROM BillingPayment bp
								LEFT JOIN IPDApplicationForm ipd ON bp.ipdnumber = ipd.IPDApplicationFormid
								LEFT JOIN OPDForm opd ON bp.opdnumber = opd.OPDFormid
								CROSS JOIN LATERAL (SELECT COALESCE(ipd.bookingreferencenumber, opd.bookingreferencenumber)::varchar AS reference) resolved
								 WHERE COALESCE(cast(bp.tenantid as varchar),'') = Any(lvar_tenantid) AND bp.isdeleted=false
AND (COALESCE(pvar_patientname, '') = '' OR CAST(bp.patientname AS varchar) = pvar_patientname)
AND NULLIF(trim(resolved.reference), '') IS NOT NULL
AND ((pvar_searchterm is null) OR resolved.reference ilike pvar_searchterm)
 ORDER BY 2 ASC
                                 limit pvar_pagesize
offset (GREATEST(pvar_pagenumber, 1) - 1) * pvar_pagesize;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;
