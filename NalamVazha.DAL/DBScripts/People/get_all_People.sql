 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_People"
              (
			  pvar_tenantid Varchar=null,
                pvar_searchterm character varying='',
                pvar_pagesize integer=50,
                pvar_pagenumber integer=0
         
              )
			 RETURNS TABLE(
                practitionerid Varchar
,firstname Varchar
,lastname Varchar
,workprofile uuid
,competencylevel uuid
,designation uuid
,contactnumber Varchar
,whatsappnumber Varchar
,emailid Varchar
,gender Varchar
,dob date
,employmentstatus Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,"Peopleid" uuid
                 
               )
               AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
               BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 11:34:27*/
               
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

				 People.practitionerid
,People.firstname
,People.lastname
,People.workprofile
,People.competencylevel
,People.designation
,People.contactnumber
,People.whatsappnumber
,People.emailid
,People.gender
,People.dob
,People.employmentstatus

				 ,People.createduser,People.createddate,People.modifieduser,People.modifieddate
				,People.tenantid 
                ,People.Peopleid
				 
			  FROM People
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(People.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND People.isdeleted=false AND COALESCE(People.status, 'Active')='Active'
			  
		        AND (((pvar_searchterm is null) or COALESCE(People.Peopleid::varchar,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.practitionerid,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.firstname,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.lastname,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.emailid,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.idnumber,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.paddressline1,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.paddressline2,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.ptown,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.pcityordistrict,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.pstatename,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.caddressline1,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.caddressline2,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.ctown,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.ccityordistrict,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.cstatename,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.registrationnumber,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.issuingauthority,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(People.bio,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

