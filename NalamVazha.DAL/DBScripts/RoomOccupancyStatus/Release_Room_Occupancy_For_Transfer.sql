DROP FUNCTION IF EXISTS public."Release_Room_Occupancy_For_Transfer"
    (uuid, date, uuid, date, varchar);

CREATE OR REPLACE FUNCTION public."Release_Room_Occupancy_For_Transfer"
(
    pvar_ipdno        uuid,
    pvar_fromdate     date,
    pvar_modifieduser uuid,
    pvar_todate       date,
    pvar_allottedto   varchar,
    OUT pvar_returnmessage varchar(4000)
)
RETURNS varchar(4000)
LANGUAGE plpgsql
AS $BODY$
BEGIN
    IF pvar_fromdate IS NULL OR pvar_todate IS NULL OR pvar_fromdate > pvar_todate THEN
        pvar_returnmessage := 'Invalid room transfer date range.';
        RETURN;
    END IF;

    /*
      The new-room occupancy is inserted only after this function succeeds.
      Therefore every matching active row in this range belongs to the old room.
    */
    UPDATE roomoccupancystatus occupancy
    SET    isdeleted    = true,
           status       = 'Available',
           modifieduser = pvar_modifieduser,
           modifieddate = NOW()
    WHERE  occupancy.ipdno = pvar_ipdno
      AND  COALESCE(occupancy.isdeleted, false) = false
      AND  occupancy.bookeddate::date BETWEEN pvar_fromdate AND pvar_todate
      AND
      (
          NULLIF(BTRIM(COALESCE(pvar_allottedto, '')), '') IS NULL
          OR NULLIF(BTRIM(COALESCE(occupancy.bookedfor, '')), '') IS NULL
          OR
          (
              LOWER(pvar_allottedto) LIKE '%patient%'
              AND LOWER(occupancy.bookedfor) LIKE '%patient%'
          )
          OR
          (
              LOWER(pvar_allottedto) LIKE '%attendant%'
              AND LOWER(occupancy.bookedfor) LIKE '%attendant%'
          )
      );

    pvar_returnmessage := '201.1';
EXCEPTION WHEN OTHERS THEN
    pvar_returnmessage := SQLERRM;
END
$BODY$;
