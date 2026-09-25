
			  CREATE OR REPLACE FUNCTION  "Therapies"
              (pvar_tenantid Varchar
,pvar_therapycategory Varchar(1024)
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,Therapiesid uuid
,therapycategory uuid,therapycategory_master Varchar,therapyname Varchar,therapycost decimal,standarddurationinmins int,therapyimage Varchar,therapyvideourl Varchar,therapyinstructions text,isgrouptherapyallowed Varchar,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:30*/
			  		
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
				Therapies.tenantid
,tenant.businessname as _tenantName
,Therapies.Therapiesid
,Therapies.therapycategory
,CAST(_TherapyCategory.categoryname AS VARCHAR) as therapycategory_master
,Therapies.therapyname
,Therapies.therapycost
,Therapies.standarddurationinmins
,Therapies.therapyimage
,Therapies.therapyvideourl
,Therapies.therapyinstructions
,CAST(case when Therapies.isgrouptherapyallowed=true then 'Yes' else 'No' End AS Varchar)as isgrouptherapyallowed

				
				,Therapies.createduser,Therapies.createddate,Therapies.modifieduser,Therapies.modifieddate
				FROM  Therapies 
 LEFT OUTER JOIN tenant ON Therapies.tenantid=tenant.tenantid
INNER JOIN TherapyCategory _TherapyCategory ON Therapies.therapycategory=_TherapyCategory.TherapyCategoryid

				WHERE (lvar_tenantid is null or COALESCE(cast(Therapies.tenantid as varchar), '') = Any(lvar_tenantid)) AND Therapies.isdeleted=false
AND (pvar_therapycategory is null or pvar_therapycategory ='0' or LENGTH(CAST(pvar_therapycategory as Varchar))=0 or CAST(Therapies.therapycategory as VARCHAR)=pvar_therapycategory)

				 ORDER BY Therapies.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

