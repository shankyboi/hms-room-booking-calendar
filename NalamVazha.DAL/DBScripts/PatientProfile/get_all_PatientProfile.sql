 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_PatientProfile"
              (
			  pvar_tenantid Varchar=null,
                pvar_searchterm character varying='',
                pvar_pagesize integer=50,
                pvar_pagenumber integer=0
         
              )
			 RETURNS TABLE(
                registrationid Varchar
,firstname Varchar
,lastname Varchar
,gender Varchar
,dateofbirth date
,age int
,emailaddress Varchar
,mobilenumber Varchar
,whatsappnumber Varchar
,photo Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,"PatientProfileid" uuid
                 
               )
               AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
               BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:57*/
               
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

				 PatientProfile.registrationid
,PatientProfile.firstname
,PatientProfile.lastname
,PatientProfile.gender
,PatientProfile.dateofbirth
,PatientProfile.age
,PatientProfile.emailaddress
,PatientProfile.mobilenumber
,PatientProfile.whatsappnumber
,PatientProfile.photo

				 ,PatientProfile.createduser,PatientProfile.createddate,PatientProfile.modifieduser,PatientProfile.modifieddate
				,PatientProfile.tenantid 
                ,PatientProfile.PatientProfileid
				 
			  FROM PatientProfile
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(PatientProfile.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND PatientProfile.isdeleted=false
			  
		        AND (((pvar_searchterm is null) or COALESCE(PatientProfile.PatientProfileid::varchar,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.registrationid,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.firstname,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.lastname,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.emailaddress,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.paddressline1,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.paddressline2,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.ptown,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.pcityordistrict,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.ppstatename,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.caddressline1,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.caddressline2,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.ctown,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.ccityordistrict,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.cstatename,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.idproofnumber,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.otherlanguages,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.typeofpractice,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.othercreativeactivities,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.insurancecompany,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.policynumber,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.referrername,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.magazinename,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.otherreferral,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.detailedremarks,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PatientProfile.causeofdeath,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

