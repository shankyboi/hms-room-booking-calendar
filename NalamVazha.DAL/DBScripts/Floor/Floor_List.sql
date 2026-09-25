
			  CREATE OR REPLACE FUNCTION  "Floor_List"
              (pvar_tenantid Varchar
,pvar_block Varchar(1024)
,pvar_building Varchar(1024)
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,Floorid uuid
,block uuid,block_master Varchar,building uuid,building_master Varchar,floorname Varchar,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:10*/
			  		
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
				Floor.tenantid
,tenant.businessname as _tenantName
,Floor.Floorid
,Floor.block
,CAST(_Block.blockcode||' '||_Block.blockname AS VARCHAR) as block_master
,Floor.building
,CAST(__Building.buildingcode||' '||__Building.buildingname AS VARCHAR) as building_master
,Floor.floorname

				
				,Floor.createduser,Floor.createddate,Floor.modifieduser,Floor.modifieddate
				FROM  Floor 
 LEFT OUTER JOIN tenant ON Floor.tenantid=tenant.tenantid
INNER JOIN Block _Block ON Floor.block=_Block.Blockid
INNER JOIN Building __Building ON Floor.building=__Building.Buildingid

				WHERE (lvar_tenantid is null or COALESCE(cast(Floor.tenantid as varchar), '') = Any(lvar_tenantid)) AND Floor.isdeleted=false
AND (pvar_block is null or pvar_block ='0' or LENGTH(CAST(pvar_block as Varchar))=0 or CAST(Floor.block as VARCHAR)=pvar_block)
AND (pvar_building is null or pvar_building ='0' or LENGTH(CAST(pvar_building as Varchar))=0 or CAST(Floor.building as VARCHAR)=pvar_building)

				 ORDER BY Floor.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

