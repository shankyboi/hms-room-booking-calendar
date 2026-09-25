
			  CREATE OR REPLACE FUNCTION  "Building_List"
              (pvar_tenantid Varchar
,pvar_block Varchar(1024)
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,Buildingid uuid
,block uuid,block_master Varchar,buildingcode Varchar,buildingname Varchar,buildingdescription text,buildingimage Varchar,buildingnearbylandmark Varchar,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:07*/
			  		
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
				Building.tenantid
,tenant.businessname as _tenantName
,Building.Buildingid
,Building.block
,CAST(_Block.blockcode||' '||_Block.blockname AS VARCHAR) as block_master
,Building.buildingcode
,Building.buildingname
,Building.buildingdescription
,Building.buildingimage
,Building.buildingnearbylandmark

				
				,Building.createduser,Building.createddate,Building.modifieduser,Building.modifieddate
				FROM  Building 
 LEFT OUTER JOIN tenant ON Building.tenantid=tenant.tenantid
INNER JOIN Block _Block ON Building.block=_Block.Blockid

				WHERE (lvar_tenantid is null or COALESCE(cast(Building.tenantid as varchar), '') = Any(lvar_tenantid)) AND Building.isdeleted=false
AND (pvar_block is null or pvar_block ='0' or LENGTH(CAST(pvar_block as Varchar))=0 or CAST(Building.block as VARCHAR)=pvar_block)

				 ORDER BY Building.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

