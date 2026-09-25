CREATE OR REPLACE FUNCTION public."SyncOPDPatientVisitStatus"(
    pvar_clinicalappointmentid uuid,
    pvar_visitstatus character varying,
    pvar_modifieduser uuid,
    OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
BEGIN
    UPDATE PatientVisit pv
    SET    visitstatus  = pvar_visitstatus,
           modifieduser = pvar_modifieduser,
           modifieddate = NOW()
    FROM   ClinicalAppointment ca
    WHERE  ca.ClinicalAppointmentid = pvar_clinicalappointmentid
      AND  COALESCE(ca.isdeleted, false) = false
      AND  COALESCE(UPPER(ca.origin), '') = 'OPD'
      AND  COALESCE(pv.isdeleted, false) = false
      AND  pv.opdnumber::varchar = ca.bookingid;

    pvar_returnmessage := '201.1';
END;
$BODY$;
