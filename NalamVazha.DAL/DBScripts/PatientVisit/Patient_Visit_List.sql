
			  CREATE OR REPLACE FUNCTION  "Patient_Visit_List"
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
              
                    RETURN json_build_object(
                    'count'
                    ,(SELECT  
                    COUNT(*)
                    FROM  PatientVisit 
 LEFT OUTER JOIN tenant ON PatientVisit.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON PatientVisit.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN IPDApplicationForm __IPDApplicationForm ON PatientVisit.ipdnumber=__IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN OPDForm ___OPDForm ON PatientVisit.opdnumber=___OPDForm.OPDFormid
LEFT OUTER JOIN People ____People ON PatientVisit.consultingdoctor=____People.Peopleid

                    WHERE (lvar_tenantid is null or COALESCE(cast(PatientVisit.tenantid as varchar), '') = Any(lvar_tenantid)) AND PatientVisit.isdeleted=false
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientVisit.visitnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(PatientVisit.visitdatetime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_PatientProfile.firstname||' '||_PatientProfile.lastname||' '||_PatientProfile.mobilenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientVisit.visittype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__IPDApplicationForm.firstname||' '||__IPDApplicationForm.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___OPDForm.patientname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____People.firstname||' '||____People.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientVisit.visitstatus AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientVisit.notes AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    PatientVisit.tenantid
,tenant.businessname as _tenantName
,PatientVisit.PatientVisitid
,PatientVisit.visitnumber
,CAST(COALESCE(to_char(PatientVisit.visitdatetime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as visitdatetime
,PatientVisit.patientname
,CAST(_PatientProfile.firstname||' '||_PatientProfile.lastname||' '||_PatientProfile.mobilenumber AS VARCHAR) as patientname_master
,PatientVisit.visittype
,PatientVisit.ipdnumber
,CAST(__IPDApplicationForm.firstname||' '||__IPDApplicationForm.lastname AS VARCHAR) as ipdnumber_master
,PatientVisit.opdnumber
,CAST(___OPDForm.patientname AS VARCHAR) as opdnumber_master
,PatientVisit.consultingdoctor
,CAST(____People.firstname||' '||____People.lastname AS VARCHAR) as consultingdoctor_master
,PatientVisit.visitstatus
,PatientVisit.notes

                    
                    ,PatientVisit.createduser,PatientVisit.createddate,PatientVisit.modifieduser,PatientVisit.modifieddate
                    FROM  PatientVisit 
 LEFT OUTER JOIN tenant ON PatientVisit.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON PatientVisit.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN IPDApplicationForm __IPDApplicationForm ON PatientVisit.ipdnumber=__IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN OPDForm ___OPDForm ON PatientVisit.opdnumber=___OPDForm.OPDFormid
LEFT OUTER JOIN People ____People ON PatientVisit.consultingdoctor=____People.Peopleid

                    WHERE (lvar_tenantid is null or COALESCE(cast(PatientVisit.tenantid as varchar), '') = Any(lvar_tenantid)) AND PatientVisit.isdeleted=false

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientVisit.visitnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(PatientVisit.visitdatetime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_PatientProfile.firstname||' '||_PatientProfile.lastname||' '||_PatientProfile.mobilenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientVisit.visittype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__IPDApplicationForm.firstname||' '||__IPDApplicationForm.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___OPDForm.patientname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____People.firstname||' '||____People.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientVisit.visitstatus AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientVisit.notes AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'visitnumber' THEN PatientVisit.visitnumber::TEXT
WHEN 'visitnumber' THEN PatientVisit.visitnumber::TEXT
WHEN 'patientname' THEN _PatientProfile.firstname||' '||_PatientProfile.lastname||' '||_PatientProfile.mobilenumber::TEXT
WHEN 'visittype' THEN PatientVisit.visittype::TEXT
WHEN 'ipdnumber' THEN __IPDApplicationForm.firstname||' '||__IPDApplicationForm.lastname::TEXT
WHEN 'opdnumber' THEN ___OPDForm.patientname::TEXT
WHEN 'consultingdoctor' THEN ____People.firstname||' '||____People.lastname::TEXT
WHEN 'visitstatus' THEN PatientVisit.visitstatus::TEXT
WHEN 'notes' THEN PatientVisit.notes::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'visitdatetime' THEN PatientVisit.visitdatetime
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'visitnumber' THEN PatientVisit.visitnumber::TEXT
WHEN 'visitnumber' THEN PatientVisit.visitnumber::TEXT
WHEN 'patientname' THEN _PatientProfile.firstname||' '||_PatientProfile.lastname||' '||_PatientProfile.mobilenumber::TEXT
WHEN 'visittype' THEN PatientVisit.visittype::TEXT
WHEN 'ipdnumber' THEN __IPDApplicationForm.firstname||' '||__IPDApplicationForm.lastname::TEXT
WHEN 'opdnumber' THEN ___OPDForm.patientname::TEXT
WHEN 'consultingdoctor' THEN ____People.firstname||' '||____People.lastname::TEXT
WHEN 'visitstatus' THEN PatientVisit.visitstatus::TEXT
WHEN 'notes' THEN PatientVisit.notes::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'visitdatetime' THEN PatientVisit.visitdatetime
			
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

