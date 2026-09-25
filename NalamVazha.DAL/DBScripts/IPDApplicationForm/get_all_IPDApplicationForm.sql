
CREATE OR REPLACE FUNCTION public."get_all_IPDApplicationForm"(
	pvar_tenantid character varying DEFAULT NULL::character varying,
	pvar_searchterm character varying DEFAULT ''::character varying,
	pvar_pagesize integer DEFAULT 50,
	pvar_pagenumber integer DEFAULT 0)
    RETURNS TABLE(bookingreferencenumber character varying, patientname uuid, firstname character varying, lastname character varying, gender character varying, mobilenumber character varying, whatsappnumber character varying, countryoforigin uuid, generalcondition character varying, bookingstatus character varying, packagename uuid, verifiedstatus character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, tenantid uuid, "IPDApplicationFormid" uuid) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
               BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:01*/
               
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

				 IPDApplicationForm.bookingreferencenumber
,IPDApplicationForm.patientname
,IPDApplicationForm.firstname
,IPDApplicationForm.lastname
,IPDApplicationForm.gender
,IPDApplicationForm.mobilenumber
,IPDApplicationForm.whatsappnumber
,IPDApplicationForm.countryoforigin
,IPDApplicationForm.generalcondition
,IPDApplicationForm.bookingstatus
,IPDApplicationForm.packagename
,IPDApplicationForm.verifiedstatus

				 ,IPDApplicationForm.createduser,IPDApplicationForm.createddate,IPDApplicationForm.modifieduser,IPDApplicationForm.modifieddate
				,IPDApplicationForm.tenantid 
                ,IPDApplicationForm.IPDApplicationFormid
				 
			  FROM IPDApplicationForm
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(IPDApplicationForm.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND IPDApplicationForm.isdeleted=false
			   AND IPDApplicationForm.verifiedstatus='Approved'
		        AND (((pvar_searchterm is null) or COALESCE(IPDApplicationForm.IPDApplicationFormid::varchar,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.bookingreferencenumber,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.firstname,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.lastname,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.groupleadersbookingreferencenumber,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.paddressline1,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.paddressline2,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.ptown,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.pcityordistrict,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.pstatename,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.caddressline1,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.caddressline2,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.ctown,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.ccityordistrict,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.cstatename,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.passportnumber,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.visanumber,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.admissionreason,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.pickupfrom,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(IPDApplicationForm.specialrequest,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

