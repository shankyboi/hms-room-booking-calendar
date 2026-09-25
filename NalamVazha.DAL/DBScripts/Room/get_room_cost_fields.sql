CREATE OR REPLACE FUNCTION public.get_room_cost_fields(
    p_roomid uuid)
    RETURNS TABLE(
        costperday              numeric(18,2),
        attendantcostperday     numeric(18,2),
        bookingdeposit          numeric(18,2),
        attendantbookingdeposit numeric(18,2)
    )
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
BEGIN
    RETURN QUERY
    SELECT
        r.costperday,
        r.attendantcostperday,
        r.bookingdeposit,
        r.attendantbookingdeposit
    FROM Room r
    WHERE r.Roomid = p_roomid
      AND COALESCE(r.isdeleted, false) = false
    LIMIT 1;
END;
$BODY$;

ALTER FUNCTION public.get_room_cost_fields(uuid)
    OWNER TO md_nalamvazha;
