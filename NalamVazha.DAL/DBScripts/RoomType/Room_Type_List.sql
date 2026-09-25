
			  -- The filter panel on the list page (Concession Eligibility / Suitability for VIP /
			  -- Gender Suitability) was not actually filtering anything: the old 1-parameter
			  -- version of this function ignored those fields entirely. Drop it so the new
			  -- 4-parameter overload below is the only one in play.
			  DROP FUNCTION IF EXISTS "Room_Type_List"(Varchar);

			  CREATE OR REPLACE FUNCTION  "Room_Type_List"
              (pvar_tenantid Varchar
,pvar_concessoneligibility Varchar DEFAULT NULL
,pvar_suitabilityforvip Varchar DEFAULT NULL
,pvar_gendersuitability Varchar DEFAULT NULL
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,RoomTypeid uuid
,name Varchar,prebookingdaylimit int,minbookingdays int,maxbookingdays int,concessoneligibility Varchar,suitabilityforvip Varchar,gendersuitability Varchar,roomtypeicon Varchar,deposittype Varchar,costperday decimal,advanceperday decimal,bookingdeposit decimal,variableofbookingdays decimal,attendantcostperday decimal,attendantadvanceperday decimal,attendantbookingdeposit decimal,attendantvariableofbookingdays decimal,hourlychargesapplicable Varchar,chargeperhour decimal,billingwaiverfordelayedstart Varchar,waiverpercentage decimal,roomtransfercost Varchar,description text,"automaton_RoomType_refundpolicy" json,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:17*/
			  		
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

                -- Empty string from the UI ("Choose" placeholder / cleared select2) means
                -- "no filter", same convention used by the other *_List functions.
                pvar_concessoneligibility := NULLIF(pvar_concessoneligibility, '');
                pvar_suitabilityforvip := NULLIF(pvar_suitabilityforvip, '');
                pvar_gendersuitability := NULLIF(pvar_gendersuitability, '');


                RETURN QUERY
				SELECT  
				RoomType.tenantid
,tenant.businessname as _tenantName
,RoomType.RoomTypeid
,RoomType.name
,RoomType.prebookingdaylimit
,RoomType.minbookingdays
,RoomType.maxbookingdays
,RoomType.concessoneligibility
,RoomType.suitabilityforvip
,RoomType.gendersuitability
,RoomType.roomtypeicon
,RoomType.deposittype
,RoomType.costperday
,RoomType.advanceperday
,RoomType.bookingdeposit
,RoomType.variableofbookingdays
,RoomType.attendantcostperday
,RoomType.attendantadvanceperday
,RoomType.attendantbookingdeposit
,RoomType.attendantvariableofbookingdays
,CAST(case when RoomType.hourlychargesapplicable=true then 'Yes' else 'No' End AS Varchar)as hourlychargesapplicable
,RoomType.chargeperhour
,RoomType.billingwaiverfordelayedstart
,RoomType.waiverpercentage
,RoomType.roomtransfercost
,RoomType.description

				,
                (SELECT json_agg(J) FROM (SELECT   
				 RoomType_refundpolicy.refundtype as "Refund Type"
,RoomType_refundpolicy.cancellationby as "Cancellation By"
,RoomType_refundpolicy.cancellationwindowdays as "Cancellation Window (Days)"
,RoomType_refundpolicy.refundpercentage as "Refund Percentage"

		 	   FROM  RoomType_refundpolicy 

			  WHERE RoomType.RoomTypeid =RoomType_refundpolicy.RoomTypeid
) J)
			    as automaton_RoomType_refundpolicy

				,RoomType.createduser,RoomType.createddate,RoomType.modifieduser,RoomType.modifieddate
				FROM  RoomType 
 LEFT OUTER JOIN tenant ON RoomType.tenantid=tenant.tenantid

				WHERE (lvar_tenantid is null or COALESCE(cast(RoomType.tenantid as varchar), '') = Any(lvar_tenantid)) AND RoomType.isdeleted=false
 AND ((pvar_concessoneligibility is null) or CAST(RoomType.concessoneligibility AS VARCHAR) = pvar_concessoneligibility)
 AND ((pvar_suitabilityforvip is null) or CAST(RoomType.suitabilityforvip AS VARCHAR) = pvar_suitabilityforvip)
 AND (
        pvar_gendersuitability is null
        OR EXISTS (
            -- Selected checkboxes (e.g. "Male,Female") vs. the row's own comma-separated
            -- gendersuitability column (e.g. "Female,Other") -- match if there is any overlap.
            SELECT 1
            FROM unnest(string_to_array(pvar_gendersuitability, ',')) AS sel(val)
            WHERE TRIM(sel.val) <> ''
              AND EXISTS (
                  SELECT 1
                  FROM unnest(string_to_array(COALESCE(RoomType.gendersuitability, ''), ',')) AS col(val)
                  WHERE LOWER(TRIM(col.val)) = LOWER(TRIM(sel.val))
              )
        )
    )

				 ORDER BY RoomType.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

