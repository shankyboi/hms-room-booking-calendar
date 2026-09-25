
CREATE OR REPLACE FUNCTION public."Get_OPD_Billing_Summary"(
	pvar_patientvisitid uuid)
    RETURNS TABLE(patientvisitid uuid, visitnumber character varying, patientname character varying, consultingdoctor character varying, consultationcharge numeric, additionalcharge numeric, totalcharge numeric, totalpaid numeric, balance numeric)
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT
        pv.PatientVisitid,
        pv.visitnumber::varchar,

        (pp.firstname || ' ' || COALESCE(pp.lastname, ''))::varchar,
        (p.firstname  || ' ' || COALESCE(p.lastname,  ''))::varchar,

        --Consultation Charges
        COALESCE(SUM(
            CASE 
                WHEN bp.paymentfor ILIKE '%consultation%' 
                     AND bp.paymentfor NOT ILIKE '%additional%'
                THEN bp.amount 
                ELSE 0 
            END
        ), 0) AS consultationcharge,

        --Additional Charges
        COALESCE(SUM(
            CASE 
                WHEN bp.paymentfor ILIKE '%additional%' 
                THEN bp.amount 
                ELSE 0 
            END
        ), 0) AS additionalcharge,

        --Total Charge
        COALESCE(SUM(bp.amount), 0) AS totalcharge,

        --Paid Amount
        COALESCE(SUM(
            CASE 
               WHEN bp.paymentstatus IN ('Paid', 'Completed', 'Success')
                THEN bp.amount 
                ELSE 0 
            END
        ), 0) AS totalpaid,

        -- Balance
        COALESCE(SUM(bp.amount), 0)
        -
        COALESCE(SUM(
            CASE 
               WHEN bp.paymentstatus IN ('Paid', 'Completed', 'Success')
                THEN bp.amount 
                ELSE 0 
            END
        ), 0)

    FROM PatientVisit pv

    LEFT JOIN PatientProfile pp 
        ON pp.PatientProfileid = pv.patientname

    LEFT JOIN People p  
        ON p.Peopleid = pv.consultingdoctor

    LEFT JOIN BillingPayment bp 
        ON bp.patientvisit = pv.PatientVisitid
       AND COALESCE(bp.isdeleted, false) = false

    WHERE pv.PatientVisitid = pvar_patientvisitid
      AND COALESCE(pv.isdeleted, false) = false

    GROUP BY
        pv.PatientVisitid,
        pv.visitnumber,
        pp.firstname, pp.lastname,
        p.firstname,  p.lastname;

END;
$BODY$
LANGUAGE plpgsql;
