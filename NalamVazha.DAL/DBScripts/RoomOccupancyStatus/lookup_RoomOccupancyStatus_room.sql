
								CREATE OR REPLACE FUNCTION  "lookup_RoomOccupancyStatus_room"
								(
                                pvar_tenantid Varchar=null
,pvar_block Varchar(50)=null
,pvar_building Varchar(50)=null
,pvar_floor Varchar(50)=null

                                
                                )
								RETURNS TABLE("Roomid" Varchar
,roomnumber Varchar
) 
						 		AS $BODY$
                                declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:03*/
							    
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
								CAST(Room.Roomid AS Varchar) as Roomid,CAST(Room.roomnumber AS Varchar) as roomnumber
								FROM Room
								 WHERE COALESCE(cast(Room.tenantid as varchar),'') = Any(lvar_tenantid) AND Room.isdeleted=false
AND (pvar_block IS NOT NULL AND CAST( Room.block AS Varchar) = pvar_block)
AND (pvar_building IS NOT NULL AND CAST( Room.building AS Varchar) = pvar_building)
AND (pvar_floor IS NOT NULL AND CAST( Room.floor AS Varchar) = pvar_floor)

 ORDER BY Room.roomnumber ASC
                                ;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

