
			  CREATE OR REPLACE FUNCTION  "Group_Booking_List"
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
              
                    RETURN json_build_object(
                    'count'
                    ,(SELECT  
                    COUNT(*)
                    FROM  GroupBooking 
 LEFT OUTER JOIN tenant ON GroupBooking.tenantid=tenant.tenantid

                    WHERE (lvar_tenantid is null or COALESCE(cast(GroupBooking.tenantid as varchar), '') = Any(lvar_tenantid)) AND GroupBooking.isdeleted=false
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(GroupBooking.groupcode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(GroupBooking.groupname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(GroupBooking.countofmembers AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    GroupBooking.tenantid
,tenant.businessname as _tenantName
,GroupBooking.GroupBookingid
,GroupBooking.groupcode
,GroupBooking.groupname
,GroupBooking.countofmembers

                    ,
                (SELECT json_agg(J) FROM (SELECT   
				 GroupBooking_patientinfo.name as "Name"
,GroupBooking_patientinfo.emailid as "Email ID"
,GroupBooking_patientinfo.phonenumber as "Phone Number"

		 	   FROM  GroupBooking_patientinfo 

			  WHERE GroupBooking.GroupBookingid =GroupBooking_patientinfo.GroupBookingid
) J)
			    as automaton_GroupBooking_patientinfo
,
                (SELECT json_agg(J) FROM (SELECT   
				 GroupBooking_contacts.person as "Person"
,GroupBooking_contacts.mobile as "Mobile"
,GroupBooking_contacts.email as "Email"

		 	   FROM  GroupBooking_contacts 

			  WHERE GroupBooking.GroupBookingid =GroupBooking_contacts.GroupBookingid
) J)
			    as automaton_GroupBooking_contacts

                    ,GroupBooking.createduser,GroupBooking.createddate,GroupBooking.modifieduser,GroupBooking.modifieddate
                    FROM  GroupBooking 
 LEFT OUTER JOIN tenant ON GroupBooking.tenantid=tenant.tenantid

                    WHERE (lvar_tenantid is null or COALESCE(cast(GroupBooking.tenantid as varchar), '') = Any(lvar_tenantid)) AND GroupBooking.isdeleted=false

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(GroupBooking.groupcode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(GroupBooking.groupname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(GroupBooking.countofmembers AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'groupcode' THEN GroupBooking.groupcode::TEXT
WHEN 'groupname' THEN GroupBooking.groupname::TEXT
WHEN 'countofmembers' THEN GroupBooking.countofmembers::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'countofmembers' THEN GroupBooking.countofmembers::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'groupcode' THEN GroupBooking.groupcode::TEXT
WHEN 'groupname' THEN GroupBooking.groupname::TEXT
WHEN 'countofmembers' THEN GroupBooking.countofmembers::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'countofmembers' THEN GroupBooking.countofmembers::NUMERIC
			
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

