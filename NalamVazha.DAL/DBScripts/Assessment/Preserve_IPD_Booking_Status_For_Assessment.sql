DROP FUNCTION IF EXISTS public."Get_IPD_Booking_Status_For_Assessment"(uuid);

CREATE OR REPLACE FUNCTION public."Get_IPD_Booking_Status_For_Assessment"(
    pvar_ipdapplicationformid uuid)
RETURNS TABLE(bookingstatus varchar, bookingstatusdate timestamp without time zone)
LANGUAGE plpgsql
VOLATILE
AS $BODY$
BEGIN
    RETURN QUERY
    SELECT ipd.bookingstatus, ipd.bookingstatusdate
      FROM ipdapplicationform ipd
     WHERE ipd.ipdapplicationformid = pvar_ipdapplicationformid
     FOR UPDATE;
END;
$BODY$;

DROP FUNCTION IF EXISTS public."Restore_IPD_Booking_Status_For_Assessment"(
    uuid, varchar, timestamp without time zone);

CREATE OR REPLACE FUNCTION public."Restore_IPD_Booking_Status_For_Assessment"(
    pvar_ipdapplicationformid uuid,
    pvar_bookingstatus varchar,
    pvar_bookingstatusdate timestamp without time zone)
RETURNS void
LANGUAGE plpgsql
VOLATILE
AS $BODY$
BEGIN
    UPDATE ipdapplicationform
       SET bookingstatus = pvar_bookingstatus,
           bookingstatusdate = pvar_bookingstatusdate
     WHERE ipdapplicationformid = pvar_ipdapplicationformid;
END;
$BODY$;
