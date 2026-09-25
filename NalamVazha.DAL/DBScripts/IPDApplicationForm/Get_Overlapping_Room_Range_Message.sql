CREATE OR REPLACE FUNCTION public."Get_Overlapping_Room_Range_Message"(
    pvar_ipdapplicationformid uuid,
    pvar_allottedto character varying,
    pvar_excludedroomrowid uuid,
    pvar_fromdate date,
    pvar_todate date)
    RETURNS character varying
    LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
    lvar_existing_from date;
    lvar_existing_to date;
    lvar_existing_allottedto varchar;
BEGIN
    SELECT fromdate::date, todate::date, allottedto
    INTO lvar_existing_from, lvar_existing_to, lvar_existing_allottedto
    FROM IPDApplicationForm_room
    WHERE IPDApplicationFormid = pvar_ipdapplicationformid
      AND COALESCE(isdeleted, false) = false
      -- Match if allottedto contains the role (handles combined "Patient, Attendant")
      AND EXISTS (
          SELECT 1 FROM unnest(string_to_array(lower(coalesce(allottedto,'')), ',')) AS part
          WHERE trim(part) = lower(coalesce(pvar_allottedto,''))
      )
      AND (pvar_excludedroomrowid IS NULL OR IPDApplicationForm_roomid <> pvar_excludedroomrowid)
      AND fromdate::date <= pvar_todate
      AND pvar_fromdate <= todate::date
    ORDER BY fromdate
    LIMIT 1;

    IF lvar_existing_from IS NOT NULL THEN
        RETURN 'Transfer room date range overlaps an existing ' || coalesce(lvar_existing_allottedto, '') ||
               ' room allocation (' || to_char(lvar_existing_from, 'dd/MM/yyyy') ||
               ' - ' || to_char(lvar_existing_to, 'dd/MM/yyyy') || ').';
    END IF;

    RETURN '';
EXCEPTION WHEN OTHERS THEN
    RETURN SQLERRM;
END
$BODY$;