CREATE OR REPLACE FUNCTION public."Validate_Room_Date_Ranges_Do_Not_Overlap"(
    pvar_ipdapplicationformid uuid,
    pvar_allottedto character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
    lvar_role varchar;
    lvar_from date;
    lvar_to date;
    lvar_prev_from date;
    lvar_prev_to date;
BEGIN
    -- Check for invalid date ranges (fromdate > todate)
    -- SKIP rows where todate = fromdate - 1 (same-day transfer close-out, these are valid)
    SELECT allottedto, fromdate::date, todate::date
    INTO lvar_role, lvar_from, lvar_to
    FROM IPDApplicationForm_room
    WHERE IPDApplicationFormid = pvar_ipdapplicationformid
      AND COALESCE(isdeleted, false) = false
      AND EXISTS
      (
          SELECT 1
          FROM unnest(string_to_array(COALESCE(pvar_allottedto, ''), ',')) AS input_role
          INNER JOIN unnest(string_to_array(COALESCE(allottedto, ''), ',')) AS saved_role
              ON LOWER(TRIM(input_role)) = LOWER(TRIM(saved_role))
      )
      AND fromdate::date > todate::date
      -- Allow todate = fromdate - 1 (valid same-day transfer closeout)
      AND NOT (todate::date = fromdate::date - INTERVAL '1 day')
    ORDER BY fromdate
    LIMIT 1;

    IF lvar_from IS NOT NULL THEN
        RETURN coalesce(lvar_role, '') || ' room allocation has invalid dates (' ||
               to_char(lvar_from, 'dd/MM/yyyy') || ' - ' ||
               to_char(lvar_to, 'dd/MM/yyyy') || ').';
    END IF;

    -- Check for overlapping date ranges
    WITH ordered_rooms AS
    (
        SELECT
            allottedto,
            fromdate::date AS from_date,
            todate::date AS to_date,
            lag(fromdate::date) OVER (ORDER BY fromdate, todate) AS prev_from_date,
            lag(todate::date)   OVER (ORDER BY fromdate, todate) AS prev_to_date
        FROM IPDApplicationForm_room
        WHERE IPDApplicationFormid = pvar_ipdapplicationformid
          AND COALESCE(isdeleted, false) = false
          AND EXISTS
          (
              SELECT 1
              FROM unnest(string_to_array(COALESCE(pvar_allottedto, ''), ',')) AS input_role
              INNER JOIN unnest(string_to_array(COALESCE(allottedto, ''), ',')) AS saved_role
                  ON LOWER(TRIM(input_role)) = LOWER(TRIM(saved_role))
          )
          -- Exclude same-day transfer closeout rows from overlap check
          AND NOT (todate::date = fromdate::date - INTERVAL '1 day')
    )
    SELECT allottedto, from_date, to_date, prev_from_date, prev_to_date
    INTO lvar_role, lvar_from, lvar_to, lvar_prev_from, lvar_prev_to
    FROM ordered_rooms
    WHERE prev_to_date IS NOT NULL
      AND from_date <= prev_to_date
    ORDER BY from_date
    LIMIT 1;

    IF lvar_from IS NOT NULL THEN
        RETURN coalesce(lvar_role, '') || ' room allocations overlap (' ||
               to_char(lvar_prev_from, 'dd/MM/yyyy') || ' - ' ||
               to_char(lvar_prev_to, 'dd/MM/yyyy') || ' and ' ||
               to_char(lvar_from, 'dd/MM/yyyy') || ' - ' ||
               to_char(lvar_to, 'dd/MM/yyyy') || ').';
    END IF;

    RETURN '201.1';

EXCEPTION WHEN OTHERS THEN
    RETURN SQLERRM;
END
$BODY$;