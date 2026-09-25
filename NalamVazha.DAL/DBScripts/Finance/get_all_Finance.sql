 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_Finance"
              (
			  pvar_tenantid Varchar=null,
                pvar_searchterm character varying='',
                pvar_pagesize integer=50,
                pvar_pagenumber integer=0
         
              )
			 RETURNS TABLE(
                paymentdate uuid
,paymentmode uuid
,receiptnumber uuid
,patient uuid
,receivablefor uuid
,bookingreferencenumber uuid
,billedamount uuid
,receivedamount uuid
,pendingamount int
,paymentstatus uuid
,collectedby uuid
,refundmode uuid
,refundedamount uuid
,refundedby uuid
,remarks uuid
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,"Financeid" uuid
                 
               )
               AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
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
			  SELECT 

				 Finance.paymentdate
,Finance.paymentmode
,Finance.receiptnumber
,Finance.patient
,Finance.receivablefor
,Finance.bookingreferencenumber
,Finance.billedamount
,Finance.receivedamount
,Finance.pendingamount
,Finance.paymentstatus
,Finance.collectedby
,Finance.refundmode
,Finance.refundedamount
,Finance.refundedby
,Finance.remarks

				 ,Finance.createduser,Finance.createddate,Finance.modifieduser,Finance.modifieddate
				,Finance.tenantid 
                ,Finance.Financeid
				 
			  FROM Finance
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(Finance.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND Finance.isdeleted=false
			  
		        AND (((pvar_searchterm is null) or COALESCE(Finance.Financeid::varchar,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

