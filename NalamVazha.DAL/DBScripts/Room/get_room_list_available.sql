
CREATE OR REPLACE FUNCTION public.get_room_list_available(
	pvar_tenantid uuid,
	pvar_roomtype uuid,
	pvar_fromdate date,
	pvar_todate date)
    RETURNS TABLE(tenantid uuid, _tenantname character varying, roomid uuid, roomcode character varying, block uuid, block_master character varying, building uuid, building_master character varying, floor uuid, floor_master character varying, roomtype uuid, roomtype_master character varying, bookingdeposit numeric, roomgroup uuid, roomgroup_master character varying, roomnumber character varying, roomimage text, occupancystatus character varying, maintenancestatus character varying, housekeepingstatus character varying, nextdaycheckin character varying, nextdaycheckout character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, costperday numeric, advanceperday numeric, attendantcostperday numeric, attendantadvanceperday numeric, attendantbookingdeposit numeric, easeofaccess character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN

-- Validation
IF pvar_tenantid IS NULL THEN
    RAISE EXCEPTION 'Tenant id is required';
END IF;

IF pvar_fromdate IS NULL OR pvar_todate IS NULL THEN
    RAISE EXCEPTION 'From date and To date are required';
END IF;

IF pvar_fromdate > pvar_todate THEN
    RAISE EXCEPTION 'From date cannot be greater than To date';
END IF;

RETURN QUERY

SELECT  
    r.tenantid,
    t.businessname AS _tenantName,
    r.Roomid,
    r.roomcode,
    r.block,
    CAST(b.blockcode || ' ' || b.blockname AS VARCHAR),
    r.building,
    CAST(bu.buildingcode || ' ' || bu.buildingname AS VARCHAR),
    r.floor,
    CAST(f.floorname AS VARCHAR),
    r.roomtype,
    CAST(rt.name AS VARCHAR),
    r.bookingdeposit,
    r.roomgroup,
    CAST(rg.groupname || ' ' || rg.groupnumber AS VARCHAR),
    r.roomnumber,
    r.roomimage::text as roomimage,
    'Available'::varchar as occupancystatus,
    'Available'::varchar as  maintenancestatus,
    'Available'::varchar as housekeepingstatus,
    r.nextdaycheckin,
    r.nextdaycheckout,
    r.createduser,
    r.createddate,
    r.modifieduser,
    r.modifieddate,
 
	r.costperday,
r.advanceperday,
 
r.attendantcostperday,
r.attendantadvanceperday,
r.attendantbookingdeposit,
COALESCE(r.easeofaccess,'')::varchar as easeofaccess
	

FROM Room r

LEFT JOIN tenant t ON r.tenantid = t.tenantid
INNER JOIN Block b ON r.block = b.Blockid
INNER JOIN Building bu ON r.building = bu.Buildingid
INNER JOIN Floor f ON r.floor = f.Floorid
INNER JOIN RoomType rt ON r.roomtype = rt.RoomTypeid
INNER JOIN RoomGroup rg ON r.roomgroup = rg.RoomGroupid

WHERE r.isdeleted = false
AND r.tenantid = pvar_tenantid

-- Room Type Filter
AND (
    pvar_roomtype IS NULL 
    OR pvar_roomtype = '00000000-0000-0000-0000-000000000000'
    OR r.roomtype = pvar_roomtype
)

-- Strict availability check
AND NOT EXISTS (
    SELECT 1
    FROM RoomOccupancyStatus ros
    WHERE ros.room = r.Roomid
    AND ros.isdeleted = false
    AND lower(trim(COALESCE(ros.status, ''))) IN ('blocked','booked','occupied')
    AND ros.bookeddate BETWEEN pvar_fromdate AND pvar_todate
)

-- An active allocation can exist even when per-day occupancy rows are missing.
AND NOT EXISTS (
    SELECT 1
    FROM RoomAllocation ra
    WHERE ra.room = r.Roomid
    AND ra.isdeleted = false
    AND lower(trim(COALESCE(ra.status, ''))) IN ('blocked','booked','occupied')
    AND pvar_fromdate <= ra.todate::date
    AND pvar_todate >= ra.fromdate::date
)

ORDER BY r.roomnumber;

END;
$BODY$;
