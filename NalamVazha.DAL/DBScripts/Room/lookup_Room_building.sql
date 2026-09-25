
								CREATE OR REPLACE FUNCTION  "lookup_Room_building"
								(
                                pvar_tenantid Varchar=null
,pvar_block Varchar(50)=null

                                
                                )
								RETURNS TABLE("Buildingid" Varchar
,buildingcode Varchar
,buildingname Varchar
) 
						 		AS $BODY$
                                declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:22*/
							    
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
								CAST(Building.Buildingid AS Varchar) as Buildingid,CAST(Building.buildingcode AS Varchar) as buildingcode,CAST(Building.buildingname AS Varchar) as buildingname
								FROM Building
								 WHERE COALESCE(cast(Building.tenantid as varchar),'') = Any(lvar_tenantid) AND Building.isdeleted=false
AND (pvar_block IS NOT NULL AND CAST( Building.block AS Varchar) = pvar_block)

 ORDER BY Building.buildingname ASC
                                ;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

