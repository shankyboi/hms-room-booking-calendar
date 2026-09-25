
			  CREATE OR REPLACE FUNCTION  "Therapy_Item_List"
              (pvar_tenantid Varchar
,pvar_therapyitemcategory Varchar(1024)
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,TherapyItemid uuid
,therapyitemcategory uuid,therapyitemcategory_master Varchar,therapyitemname Varchar,price decimal,therapyitemimage Varchar,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:42*/
			  		
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
				TherapyItem.tenantid
,tenant.businessname as _tenantName
,TherapyItem.TherapyItemid
,TherapyItem.therapyitemcategory
,CAST(_TherapyItemCategory.itemcategory AS VARCHAR) as therapyitemcategory_master
,TherapyItem.therapyitemname
,TherapyItem.price
,TherapyItem.therapyitemimage

				
				,TherapyItem.createduser,TherapyItem.createddate,TherapyItem.modifieduser,TherapyItem.modifieddate
				FROM  TherapyItem 
 LEFT OUTER JOIN tenant ON TherapyItem.tenantid=tenant.tenantid
INNER JOIN TherapyItemCategory _TherapyItemCategory ON TherapyItem.therapyitemcategory=_TherapyItemCategory.TherapyItemCategoryid

				WHERE (lvar_tenantid is null or COALESCE(cast(TherapyItem.tenantid as varchar), '') = Any(lvar_tenantid)) AND TherapyItem.isdeleted=false
AND (pvar_therapyitemcategory is null or pvar_therapyitemcategory ='0' or LENGTH(CAST(pvar_therapyitemcategory as Varchar))=0 or CAST(TherapyItem.therapyitemcategory as VARCHAR)=pvar_therapyitemcategory)

				 ORDER BY TherapyItem.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

