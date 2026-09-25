
			  CREATE OR REPLACE FUNCTION  "Therapy_Kit_List"
              (pvar_tenantid Varchar
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,TherapyKitid uuid
,therapykitname Varchar,kitprice Varchar,"automaton_TherapyKit_kititems" json,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:45*/
			  		
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
				TherapyKit.tenantid
,tenant.businessname as _tenantName
,TherapyKit.TherapyKitid
,TherapyKit.therapykitname
,TherapyKit.kitprice

				,
                (SELECT json_agg(J) FROM (SELECT   
				 CAST(_TherapyItem.therapyitemname AS VARCHAR) as "Therapy Item"
,TherapyKit_kititems.price as "Price"
,TherapyKit_kititems.count as "count"
,TherapyKit_kititems.linetotal as "Line Total"

		 	   FROM  TherapyKit_kititems 
INNER JOIN TherapyItem _TherapyItem ON TherapyKit_kititems.therapyitem=_TherapyItem.TherapyItemid

			  WHERE TherapyKit.TherapyKitid =TherapyKit_kititems.TherapyKitid
) J)
			    as automaton_TherapyKit_kititems

				,TherapyKit.createduser,TherapyKit.createddate,TherapyKit.modifieduser,TherapyKit.modifieddate
				FROM  TherapyKit 
 LEFT OUTER JOIN tenant ON TherapyKit.tenantid=tenant.tenantid

				WHERE (lvar_tenantid is null or COALESCE(cast(TherapyKit.tenantid as varchar), '') = Any(lvar_tenantid)) AND TherapyKit.isdeleted=false

				 ORDER BY TherapyKit.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

