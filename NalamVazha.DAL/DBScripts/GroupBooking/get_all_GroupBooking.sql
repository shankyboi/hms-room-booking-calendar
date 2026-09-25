 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_GroupBooking"
              (
			  pvar_tenantid Varchar=null,
                pvar_searchterm character varying='',
                pvar_pagesize integer=50,
                pvar_pagenumber integer=0
         
              )
			 RETURNS TABLE(
                groupcode Varchar
,groupname Varchar
,countofmembers int
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,"GroupBookingid" uuid
                 
               )
               AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
               BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 06/18/2026 05:34:25*/
               
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

				 GroupBooking.groupcode
,GroupBooking.groupname
,GroupBooking.countofmembers

				 ,GroupBooking.createduser,GroupBooking.createddate,GroupBooking.modifieduser,GroupBooking.modifieddate
				,GroupBooking.tenantid 
                ,GroupBooking.GroupBookingid
				 
			  FROM GroupBooking
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(GroupBooking.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND GroupBooking.isdeleted=false
			  
		        AND (((pvar_searchterm is null) or COALESCE(GroupBooking.GroupBookingid::varchar,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(GroupBooking.groupcode,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(GroupBooking.groupname,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

