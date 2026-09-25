CREATE OR REPLACE FUNCTION public."Room_Allocation_List"(
	pvar_tenantid character varying,
	pvar_pagesize integer,
	pvar_pagenumber integer,
	pvar_searchterm character varying,
	pvar_sort_fields json)
    RETURNS json
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
               declare local_sortcolumn_array text[] = (
	                select array_agg(col) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)
                );
                declare local_sortorder_array text[] = (
	                select array_agg(dir) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)	
                );          
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;  
               
          	  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:10*/
			  		
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
                    if(pvar_searchterm is not null and LENGTH(CAST(pvar_searchterm as Varchar)) > 0)
                    then
                    pvar_searchterm := '%' || pvar_searchterm || '%';
                    else
                    pvar_searchterm := null;
                    end if;
              
                    RETURN json_build_object(
                    'count'
                    ,(SELECT  
                    COUNT(*)
                    FROM  RoomAllocation 
 LEFT OUTER JOIN tenant ON RoomAllocation.tenantid=tenant.tenantid
LEFT JOIN LATERAL (
    SELECT ros.ipdno
    FROM RoomOccupancyStatus ros
    WHERE COALESCE(ros.isdeleted, false) = false
      AND ros.ipdno IS NOT NULL
      AND (
          ros.roomallocationno = RoomAllocation.RoomAllocationid::varchar
          OR ros.roomallocationno = RoomAllocation.roomallocationno
      )
    ORDER BY ros.createddate DESC NULLS LAST
    LIMIT 1
) _RoomAllocationIpd ON true
LEFT JOIN IPDApplicationForm _IPDApplicationForm ON COALESCE(NULLIF(RoomAllocation.ipdno, '00000000-0000-0000-0000-000000000000'::uuid), _RoomAllocationIpd.ipdno)=_IPDApplicationForm.IPDApplicationFormid
INNER JOIN Block __Block ON RoomAllocation.block=__Block.Blockid
INNER JOIN Building ___Building ON RoomAllocation.building=___Building.Buildingid
INNER JOIN Floor ____Floor ON RoomAllocation.floor=____Floor.Floorid
INNER JOIN Room _____Room ON RoomAllocation.room=_____Room.Roomid
                    WHERE (lvar_tenantid is null or COALESCE(cast(RoomAllocation.tenantid as varchar), '') = Any(lvar_tenantid)) AND RoomAllocation.isdeleted=false
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(RoomAllocation.roomallocationno AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_IPDApplicationForm.firstname||' '||_IPDApplicationForm.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__Block.blockname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___Building.buildingname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____Floor.floorname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_____Room.roomnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(RoomAllocation.fromdate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(RoomAllocation.todate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(RoomAllocation.status AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    RoomAllocation.tenantid
,tenant.businessname as _tenantName
,RoomAllocation.RoomAllocationid
,RoomAllocation.roomallocationno
,COALESCE(NULLIF(RoomAllocation.ipdno, '00000000-0000-0000-0000-000000000000'::uuid), _RoomAllocationIpd.ipdno) as ipdno
,_IPDApplicationForm.bookingreferencenumber
,CAST(_IPDApplicationForm.firstname||' '||_IPDApplicationForm.lastname AS VARCHAR) as ipdno_master
,RoomAllocation.block
,CAST(__Block.blockname AS VARCHAR) as block_master
,RoomAllocation.building
,CAST(___Building.buildingname AS VARCHAR) as building_master
,RoomAllocation.floor
,CAST(____Floor.floorname AS VARCHAR) as floor_master
,RoomAllocation.room
,CAST(_____Room.roomnumber AS VARCHAR) as room_master
,CAST(COALESCE(to_char(RoomAllocation.fromdate,'dd/MM/yyyy'),'') AS Varchar) as fromdate
,CAST(COALESCE(to_char(RoomAllocation.todate,'dd/MM/yyyy'),'') AS Varchar) as todate
,RoomAllocation.status
                    
                    ,RoomAllocation.createduser,RoomAllocation.createddate,RoomAllocation.modifieduser,RoomAllocation.modifieddate
                    FROM  RoomAllocation 
 LEFT OUTER JOIN tenant ON RoomAllocation.tenantid=tenant.tenantid
LEFT JOIN LATERAL (
    SELECT ros.ipdno
    FROM RoomOccupancyStatus ros
    WHERE COALESCE(ros.isdeleted, false) = false
      AND ros.ipdno IS NOT NULL
      AND (
          ros.roomallocationno = RoomAllocation.RoomAllocationid::varchar
          OR ros.roomallocationno = RoomAllocation.roomallocationno
      )
    ORDER BY ros.createddate DESC NULLS LAST
    LIMIT 1
) _RoomAllocationIpd ON true
LEFT JOIN IPDApplicationForm _IPDApplicationForm ON COALESCE(NULLIF(RoomAllocation.ipdno, '00000000-0000-0000-0000-000000000000'::uuid), _RoomAllocationIpd.ipdno)=_IPDApplicationForm.IPDApplicationFormid
INNER JOIN Block __Block ON RoomAllocation.block=__Block.Blockid
INNER JOIN Building ___Building ON RoomAllocation.building=___Building.Buildingid
INNER JOIN Floor ____Floor ON RoomAllocation.floor=____Floor.Floorid
INNER JOIN Room _____Room ON RoomAllocation.room=_____Room.Roomid
                    WHERE (lvar_tenantid is null or COALESCE(cast(RoomAllocation.tenantid as varchar), '') = Any(lvar_tenantid)) AND RoomAllocation.isdeleted=false
                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(RoomAllocation.roomallocationno AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_IPDApplicationForm.firstname||' '||_IPDApplicationForm.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__Block.blockname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___Building.buildingname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____Floor.floorname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_____Room.roomnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(RoomAllocation.fromdate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(RoomAllocation.todate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(RoomAllocation.status AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'roomallocationno' THEN RoomAllocation.roomallocationno::TEXT
WHEN 'roomallocationno' THEN RoomAllocation.roomallocationno::TEXT
WHEN 'ipdno' THEN _IPDApplicationForm.firstname||' '||_IPDApplicationForm.lastname::TEXT
WHEN 'block' THEN __Block.blockname::TEXT
WHEN 'building' THEN ___Building.buildingname::TEXT
WHEN 'floor' THEN ____Floor.floorname::TEXT
WHEN 'room' THEN _____Room.roomnumber::TEXT
WHEN 'status' THEN RoomAllocation.status::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'fromdate' THEN RoomAllocation.fromdate
WHEN 'todate' THEN RoomAllocation.todate
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'roomallocationno' THEN RoomAllocation.roomallocationno::TEXT
WHEN 'roomallocationno' THEN RoomAllocation.roomallocationno::TEXT
WHEN 'ipdno' THEN _IPDApplicationForm.firstname||' '||_IPDApplicationForm.lastname::TEXT
WHEN 'block' THEN __Block.blockname::TEXT
WHEN 'building' THEN ___Building.buildingname::TEXT
WHEN 'floor' THEN ____Floor.floorname::TEXT
WHEN 'room' THEN _____Room.roomnumber::TEXT
WHEN 'status' THEN RoomAllocation.status::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'fromdate' THEN RoomAllocation.fromdate
WHEN 'todate' THEN RoomAllocation.todate

                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    -- Default/fallback order: when no column is explicitly sorted (all the CASE
                    -- expressions above evaluate to NULL), or as a stable tiebreaker when an
                    -- explicit sort has equal values, fall back to most-recently-created first
                    -- instead of leaving the order undefined ("random").
                    RoomAllocation.createddate DESC

                    limit pvar_pagesize
                    offset pvar_pagenumber * pvar_pagesize

                    ) d));
	
			  
					 	
			  END
              
$BODY$;
ALTER FUNCTION public."Room_Allocation_List"(character varying, integer, integer, character varying, json)
    OWNER TO md_nalamvazha;
GRANT EXECUTE ON FUNCTION public."Room_Allocation_List"(character varying, integer, integer, character varying, json) TO PUBLIC;
GRANT EXECUTE ON FUNCTION public."Room_Allocation_List"(character varying, integer, integer, character varying, json) TO develop_ukan;
GRANT EXECUTE ON FUNCTION public."Room_Allocation_List"(character varying, integer, integer, character varying, json) TO md_nalamvazha;
