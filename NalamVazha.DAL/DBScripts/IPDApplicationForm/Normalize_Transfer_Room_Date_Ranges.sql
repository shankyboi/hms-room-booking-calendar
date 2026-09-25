CREATE OR REPLACE FUNCTION public."Normalize_Transfer_Room_Date_Ranges"(
    pvar_ipdapplicationformid uuid,
    pvar_allottedto character varying,
    pvar_newroomid uuid,
    pvar_newfromdate date,
    pvar_newtodate date,
    pvar_modifieduser uuid)
    RETURNS character varying
    LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
    lvar_oldtodate date := pvar_newfromdate - 1;
    lvar_message varchar;
BEGIN
    WITH new_room AS (
        SELECT IPDApplicationForm_roomid
        FROM IPDApplicationForm_room
        WHERE IPDApplicationFormid = pvar_ipdapplicationformid
          AND COALESCE(isdeleted, false) = false
          AND EXISTS (
              SELECT 1 FROM unnest(string_to_array(lower(coalesce(allottedto,'')), ',')) AS part
              WHERE trim(part) = lower(coalesce(pvar_allottedto,''))
          )
          AND roomnumber = pvar_newroomid
        ORDER BY action_date DESC NULLS LAST, record_order DESC NULLS LAST, fromdate DESC NULLS LAST
        LIMIT 1
    ),
    normalized_new AS (
        UPDATE IPDApplicationForm_room r
        SET fromdate    = pvar_newfromdate,
            todate      = pvar_newtodate,
            action      = 'Transfer',
            action_by   = pvar_modifieduser,
            action_date = NOW()
        FROM new_room nr
        WHERE r.IPDApplicationForm_roomid = nr.IPDApplicationForm_roomid
        RETURNING r.IPDApplicationForm_roomid
    )
    UPDATE IPDApplicationForm_room oldroom
    SET todate      = lvar_oldtodate,
        action      = 'Transfer',
        action_by   = pvar_modifieduser,
        action_date = NOW()
    WHERE oldroom.IPDApplicationFormid = pvar_ipdapplicationformid
      AND COALESCE(oldroom.isdeleted, false) = false
      AND EXISTS (
          SELECT 1 FROM unnest(string_to_array(lower(coalesce(oldroom.allottedto,'')), ',')) AS part
          WHERE trim(part) = lower(coalesce(pvar_allottedto,''))
      )
      AND oldroom.IPDApplicationForm_roomid NOT IN (SELECT IPDApplicationForm_roomid FROM normalized_new)
      AND oldroom.fromdate::date < pvar_newfromdate
      AND oldroom.todate::date >= pvar_newfromdate;

    SELECT "Validate_Room_Date_Ranges_Do_Not_Overlap"(pvar_ipdapplicationformid, pvar_allottedto)
    INTO lvar_message;

    RETURN lvar_message;
EXCEPTION WHEN OTHERS THEN
    RETURN SQLERRM;
END
$BODY$;