
CREATE OR REPLACE FUNCTION public."Get_OPD_Consultation_Fee"(
	pvar_opdformid uuid)
    RETURNS TABLE("Receivableid" uuid, receivableno character varying, receivabledate character varying, tenantid uuid, bookingreferencenumber character varying, patientname uuid, patientname_master character varying, mobilenumber character varying, patientvisit uuid, visitnumber character varying, preferreddoctor uuid, preferreddoctor_master character varying, receivablefor character varying, amount numeric, paidamount numeric, paymentstatus character varying, remarks character varying, createddate timestamp without time zone) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT
        r.Receivableid,
        r.receivableno,
        CAST(COALESCE(to_char(r.receivabledate, 'dd/MM/yyyy'), '') AS varchar)  AS receivabledate,
        o.tenantid,
        CAST(COALESCE(o.bookingreferencenumber, '') AS varchar)                 AS bookingreferencenumber,
        pp.PatientProfileid                                                      AS patientname,
        CAST(CONCAT(pp.firstname, ' ', pp.lastname) AS varchar) AS patientname_master,
        CAST(COALESCE(pp.mobilenumber, '') AS varchar)                           AS mobilenumber,
        pv.PatientVisitid                                                        AS patientvisit,
        CAST(COALESCE(pv.visitnumber, '') AS varchar)                            AS visitnumber,
        COALESCE(o.preferreddoctor, pv.consultingdoctor, ca.actualpractitioner, ca.practitioner) AS preferreddoctor,
        CAST(NULLIF(btrim(concat_ws(' ', NULLIF(pe.firstname, ''), NULLIF(pe.lastname, ''))), '') AS varchar) AS preferreddoctor_master,
        r.receivablefor,
        COALESCE(r.amount, 0)                                                    AS amount,
        COALESCE(r.paidamount, 0)                                                AS paidamount,
        COALESCE(r.paymentstatus, '')                                            AS paymentstatus,
        COALESCE(r.remarks, '')                                                  AS remarks,
        r.createddate
    FROM   Receivable r
    INNER JOIN OPDForm        o   ON o.OPDFormid         = r.opdnumber
                                 AND COALESCE(o.isdeleted, false) = false
    INNER JOIN PatientProfile pp  ON pp.PatientProfileid  = r.patientname
    LEFT  JOIN PatientVisit   pv  ON pv.PatientVisitid    = r.patientvisit
                                 AND COALESCE(pv.isdeleted, false) = false
    LEFT  JOIN LATERAL (
        SELECT latest_appt.actualpractitioner, latest_appt.practitioner
        FROM ClinicalAppointment latest_appt
        WHERE latest_appt.bookingid = CAST(o.OPDFormid AS varchar)
          AND COALESCE(latest_appt.isdeleted, false) = false
        ORDER BY latest_appt.createddate DESC
        LIMIT 1
    ) ca ON true
    LEFT  JOIN People         pe  ON pe.Peopleid = COALESCE(o.preferreddoctor, pv.consultingdoctor, ca.actualpractitioner, ca.practitioner)
    WHERE  r.opdnumber = pvar_opdformid
      AND  r.remarks   = 'Auto:Approved:ConsultationFee'
      AND  COALESCE(r.isdeleted, false) = false
    ORDER BY r.createddate DESC;
END;
$BODY$;
