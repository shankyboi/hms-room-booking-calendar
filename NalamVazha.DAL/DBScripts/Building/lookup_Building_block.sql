
								CREATE OR REPLACE FUNCTION  "lookup_Building_block"
								(
                                pvar_tenantid Varchar=null

                                
                                )
								RETURNS TABLE("Blockid" Varchar
,blockcode Varchar
,blockname Varchar
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
								CAST(Block.Blockid AS Varchar) as Blockid,CAST(Block.blockcode AS Varchar) as blockcode,CAST(Block.blockname AS Varchar) as blockname
								FROM Block
								 WHERE COALESCE(cast(Block.tenantid as varchar),'') = Any(lvar_tenantid) AND Block.isdeleted=false

 ORDER BY Block.blockname ASC
                                ;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

