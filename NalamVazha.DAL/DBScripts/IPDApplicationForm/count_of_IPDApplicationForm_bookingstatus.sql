 CREATE OR REPLACE FUNCTION  "count_of_IPDApplicationForm_bookingstatus"
              (pvar_tenantid Varchar
,pvar_patientname Varchar(1024)
,pvar_bookingstatus Varchar(1024)
)
			  RETURNS TABLE(count bigint,bookingstatus varchar)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/14/2026 05:16:22*/
			  		
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



              
              RETURN QUERY
			  SELECT  
			  Count(*),IPDApplicationForm.bookingstatus::varchar as bookingstatus
			  FROM IPDApplicationForm
			  WHERE (lvar_tenantid is null or COALESCE(cast(IPDApplicationForm.tenantid as varchar), '') = Any(lvar_tenantid)) AND IPDApplicationForm.isdeleted=false
AND (pvar_patientname is null or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(IPDApplicationForm.patientname as VARCHAR)=pvar_patientname)

               GROUP BY IPDApplicationForm.bookingstatus;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;
