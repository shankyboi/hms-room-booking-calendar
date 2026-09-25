
			  CREATE OR REPLACE FUNCTION  "Medicine_List"
              (pvar_tenantid Varchar
,pvar_medicationtype Varchar(1024)
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:36*/
			  		
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
                    FROM  Medicine 
 LEFT OUTER JOIN tenant ON Medicine.tenantid=tenant.tenantid
INNER JOIN MedicationType _MedicationType ON Medicine.medicationtype=_MedicationType.MedicationTypeid

                    WHERE (lvar_tenantid is null or COALESCE(cast(Medicine.tenantid as varchar), '') = Any(lvar_tenantid)) AND Medicine.isdeleted=false
AND (pvar_medicationtype is null or pvar_medicationtype ='0' or LENGTH(CAST(pvar_medicationtype as Varchar))=0 or CAST(Medicine.medicationtype as VARCHAR)=pvar_medicationtype)
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_MedicationType.medicationtypename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Medicine.medicinename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Medicine.price AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Medicine.sideeffect AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    Medicine.tenantid
,tenant.businessname as _tenantName
,Medicine.Medicineid
,Medicine.medicationtype
,CAST(_MedicationType.medicationtypename AS VARCHAR) as medicationtype_master
,Medicine.medicinename
,Medicine.price
,CAST(case when Medicine.prescriptionrequired=true then 'Yes' else 'No' End AS Varchar)as prescriptionrequired
,Medicine.sideeffect

                    
                    ,Medicine.createduser,Medicine.createddate,Medicine.modifieduser,Medicine.modifieddate
                    FROM  Medicine 
 LEFT OUTER JOIN tenant ON Medicine.tenantid=tenant.tenantid
INNER JOIN MedicationType _MedicationType ON Medicine.medicationtype=_MedicationType.MedicationTypeid

                    WHERE (lvar_tenantid is null or COALESCE(cast(Medicine.tenantid as varchar), '') = Any(lvar_tenantid)) AND Medicine.isdeleted=false
AND (pvar_medicationtype is null or pvar_medicationtype ='0' or LENGTH(CAST(pvar_medicationtype as Varchar))=0 or CAST(Medicine.medicationtype as VARCHAR)=pvar_medicationtype)

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_MedicationType.medicationtypename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Medicine.medicinename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Medicine.price AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Medicine.sideeffect AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'medicationtype' THEN _MedicationType.medicationtypename::TEXT
WHEN 'medicinename' THEN Medicine.medicinename::TEXT
WHEN 'price' THEN Medicine.price::TEXT
WHEN 'sideeffect' THEN Medicine.sideeffect::TEXT
WHEN 'sideeffect' THEN Medicine.sideeffect::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'prescriptionrequired' THEN Medicine.prescriptionrequired
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'price' THEN Medicine.price::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'medicationtype' THEN _MedicationType.medicationtypename::TEXT
WHEN 'medicinename' THEN Medicine.medicinename::TEXT
WHEN 'price' THEN Medicine.price::TEXT
WHEN 'sideeffect' THEN Medicine.sideeffect::TEXT
WHEN 'sideeffect' THEN Medicine.sideeffect::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'prescriptionrequired' THEN Medicine.prescriptionrequired
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'price' THEN Medicine.price::NUMERIC
			
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

