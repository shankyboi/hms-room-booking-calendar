
			  CREATE OR REPLACE FUNCTION  "count_of_EnquiryForm"
              (pvar_tenantid Varchar
,pvar_enquirynumber Varchar(1024)
,pvar_patientname Varchar(1024)
,pvar_phonenumber Varchar(1024)
,pvar_emailaddress Varchar(1024)
,pvar_enquirystatus Varchar(1024)
)
			  RETURNS TABLE(count bigint,verifiedstatus varchar)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:41*/
			  		
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
			  Count(*),EnquiryForm.verifiedstatus 
			  FROM EnquiryForm
			  WHERE (lvar_tenantid is null or COALESCE(cast(EnquiryForm.tenantid as varchar), '') = Any(lvar_tenantid)) AND EnquiryForm.isdeleted=false
AND (pvar_enquirynumber is null or LENGTH(CAST(pvar_enquirynumber as Varchar))=0 or CAST(EnquiryForm.enquirynumber as VARCHAR)=pvar_enquirynumber)
AND (pvar_patientname is null or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(EnquiryForm.patientname as VARCHAR)=pvar_patientname)
AND (pvar_phonenumber is null or LENGTH(CAST(pvar_phonenumber as Varchar))=0 or CAST(EnquiryForm.phonenumber as VARCHAR)=pvar_phonenumber)
AND (pvar_emailaddress is null or LENGTH(CAST(pvar_emailaddress as Varchar))=0 or CAST(EnquiryForm.emailaddress as VARCHAR)=pvar_emailaddress)
AND (pvar_enquirystatus is null or LENGTH(CAST(pvar_enquirystatus as Varchar))=0 or CAST(EnquiryForm.enquirystatus as VARCHAR)=pvar_enquirystatus)

               GROUP BY EnquiryForm.verifiedstatus;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

