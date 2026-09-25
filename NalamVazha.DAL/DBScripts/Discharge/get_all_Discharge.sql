 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_Discharge"
              (
			  pvar_tenantid Varchar=null,
                pvar_searchterm character varying='',
                pvar_pagesize integer=50,
                pvar_pagenumber integer=0
         
              )
			 RETURNS TABLE(
                ipdnumber uuid
,patient uuid
,room uuid
,discharge uuid
,daysofstay int
,pendingamount uuid
,paymentstatus uuid
,refundamount uuid
,refundstatus uuid
,feedbackstatus Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,"Dischargeid" uuid
                 
               )
               AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
               BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:43:40*/
               
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

				 Discharge.ipdnumber
,Discharge.patient
,Discharge.room
,Discharge.discharge
,Discharge.daysofstay
,Discharge.pendingamount
,Discharge.paymentstatus
,Discharge.refundamount
,Discharge.refundstatus
,Discharge.feedbackstatus

				 ,Discharge.createduser,Discharge.createddate,Discharge.modifieduser,Discharge.modifieddate
				,Discharge.tenantid 
                ,Discharge.Dischargeid
				 
			  FROM Discharge
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(Discharge.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND Discharge.isdeleted=false
			  
		        AND (((pvar_searchterm is null) or COALESCE(Discharge.Dischargeid::varchar,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(Discharge.feedbackstatus,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

