
								CREATE OR REPLACE FUNCTION  "lookup_PaymentRequest_patientname"
								(
                                pvar_tenantid Varchar=null

                                ,pvar_searchterm character varying='',
		pvar_pagesize integer=50,
	pvar_pagenumber integer=0
                                )
								RETURNS TABLE("PatientProfileid" Varchar
,firstname Varchar
,lastname Varchar
,mobilenumber Varchar
) 
						 		AS $BODY$
                                declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:48*/
							    
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
								CAST(PatientProfile.PatientProfileid AS Varchar) as PatientProfileid,CAST(PatientProfile.firstname AS Varchar) as firstname,CAST(PatientProfile.lastname AS Varchar) as lastname,CAST(PatientProfile.mobilenumber AS Varchar) as mobilenumber
								FROM PatientProfile
								 WHERE COALESCE(cast(PatientProfile.tenantid as varchar),'') = Any(lvar_tenantid) AND PatientProfile.isdeleted=false
 AND  ((pvar_searchterm is null)  or PatientProfile.firstname::varchar ilike pvar_searchterm or PatientProfile.lastname::varchar ilike pvar_searchterm or PatientProfile.mobilenumber::varchar ilike pvar_searchterm or  CAST(PatientProfile.PatientProfileid AS Varchar) ilike pvar_searchterm)
 ORDER BY PatientProfile.mobilenumber ASC
                                 limit pvar_pagesize
offset pvar_pagenumber * pvar_pagesize;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

