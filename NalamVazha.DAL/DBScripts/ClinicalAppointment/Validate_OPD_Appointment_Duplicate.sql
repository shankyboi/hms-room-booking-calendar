CREATE OR REPLACE FUNCTION public."Validate_OPD_Appointment_Duplicate"(
    pvar_tenantid uuid,
    pvar_patient uuid,
    pvar_appointmentdate date)
RETURNS varchar
LANGUAGE plpgsql
AS $BODY$
BEGIN
    IF pvar_tenantid IS NULL
       OR pvar_patient IS NULL
       OR pvar_appointmentdate IS NULL THEN
        RETURN 'Unable to validate the OPD appointment.';
    END IF;

    -- Keep this lock until the caller commits the appointment insert. This
    -- prevents concurrent requests for the same patient/day from both passing.
    PERFORM pg_advisory_xact_lock(
        hashtextextended(
            'opd-appointment:'
            || pvar_tenantid::text || ':'
            || pvar_patient::text || ':'
            || pvar_appointmentdate::text,
            0));

    IF EXISTS (
        SELECT 1
        FROM ClinicalAppointment ca
        WHERE ca.tenantid = pvar_tenantid
          AND ca.patient = pvar_patient
          AND ca.appointmentdate::date = pvar_appointmentdate
          AND UPPER(BTRIM(COALESCE(ca.origin, ''))) = 'OPD'
          AND COALESCE(ca.isdeleted, false) = false
          AND LOWER(BTRIM(COALESCE(ca.status, ''))) NOT IN (
              'cancelled',
              'canceled',
              'admittedcancelled',
              'rescheduled',
              'resheduled')
    ) THEN
        RETURN 'Appointment already booked/exists.';
    END IF;

    RETURN '201.1';
END;
$BODY$;
