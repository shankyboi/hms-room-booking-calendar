-- FUNCTION: public.InvalidateRoomReceivablesForTransfer(uuid, date, boolean, uuid)

-- DROP FUNCTION IF EXISTS public."InvalidateRoomReceivablesForTransfer"(uuid, date, boolean, uuid);

CREATE OR REPLACE FUNCTION public."InvalidateRoomReceivablesForTransfer"(
	p_ipdformid uuid,
	p_fromdate date,
	p_isattendant boolean,
	p_modifiedby uuid)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    affected_count integer;
BEGIN
    UPDATE Receivable
    SET isdeleted = true,
        modifieduser = p_modifiedby,
        modifieddate = NOW()
    WHERE ipdnumber = p_ipdformid
      AND receivablefor = 'Room'
      AND receivabledate >= p_fromdate
      AND COALESCE(isdeleted, false) = false
      AND (
          (p_isattendant = true AND COALESCE(remarks, '') ILIKE '%Attendant%')
          OR (p_isattendant = false AND COALESCE(remarks, '') NOT ILIKE '%Attendant%')
      );

    GET DIAGNOSTICS affected_count = ROW_COUNT;
    RETURN affected_count;
END;
$BODY$;

ALTER FUNCTION public."InvalidateRoomReceivablesForTransfer"(uuid, date, boolean, uuid)
    OWNER TO md_nalamvazha;

