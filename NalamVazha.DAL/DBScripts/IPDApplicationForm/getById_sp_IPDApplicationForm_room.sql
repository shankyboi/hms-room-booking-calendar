-- FUNCTION: public.getById_sp_IPDApplicationForm_room(character varying)
-- DROP FUNCTION IF EXISTS public."getById_sp_IPDApplicationForm_room"(character varying);
CREATE OR REPLACE FUNCTION public."getById_sp_IPDApplicationForm_room"(
	pvar_ipdapplicationformid character varying)
    RETURNS TABLE(
        "IPDApplicationFormid" uuid,
        "IPDApplicationForm_roomid" uuid,
        allottedto character varying,
        roomnumber uuid,
        fromdate timestamp without time zone,
        todate timestamp without time zone,
        roomtypeid uuid          -- NEW: resolved room type for this room
    )
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000
AS $BODY$
BEGIN

    RETURN QUERY
    SELECT
        ipdroom.IPDApplicationFormid
        ,ipdroom.IPDApplicationForm_roomid
        ,ipdroom.allottedto
        ,ipdroom.roomnumber
        ,ipdroom.fromdate
        ,ipdroom.todate
        ,room.RoomType as     RoomTypeid        -- NEW: pulled from the Room master table
    FROM IPDApplicationForm_room ipdroom
    LEFT JOIN Room room
        ON room.Roomid = ipdroom.roomnumber
       AND COALESCE(room.isdeleted, false) = false
    WHERE
        CAST(ipdroom.IPDApplicationFormid AS VARCHAR) = pvar_IPDApplicationFormid
        AND COALESCE(ipdroom.isdeleted, false) = false
    ORDER BY ipdroom.record_order DESC;

END
$BODY$;
