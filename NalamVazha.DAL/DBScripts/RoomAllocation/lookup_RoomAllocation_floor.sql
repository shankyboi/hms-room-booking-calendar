
								CREATE OR REPLACE FUNCTION  "lookup_RoomAllocation_floor"
								(
                                pvar_tenantid Varchar=null
,pvar_block Varchar(50)=null
,pvar_building Varchar(50)=null

                                
                                )
								RETURNS TABLE("Floorid" Varchar
,floorname Varchar
) 
						 		AS $BODY$
                                declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:09*/
							    
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
								CAST(Floor.Floorid AS Varchar) as Floorid,CAST(Floor.floorname AS Varchar) as floorname
								FROM Floor
								 WHERE COALESCE(cast(Floor.tenantid as varchar),'') = Any(lvar_tenantid) AND Floor.isdeleted=false
AND (pvar_block IS NOT NULL AND CAST( Floor.block AS Varchar) = pvar_block)
AND (pvar_building IS NOT NULL AND CAST( Floor.building AS Varchar) = pvar_building)

 ORDER BY Floor.floorname ASC
                                ;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

