CREATE OR REPLACE FUNCTION public."CreateApprovedConcessionReceivables"(
    pvar_concessionformid character varying,
    pvar_createduser uuid)
RETURNS integer
LANGUAGE plpgsql
AS $function$
DECLARE
    inserted_rows integer := 0;
BEGIN
    -- Serializes receivable-number generation for automatic concessions created
    -- on the same day. The lock is released with the caller's transaction.
    PERFORM pg_advisory_xact_lock(
        hashtext('auto-concession-receivable:' || CURRENT_DATE::text));

    WITH approved_forms AS (
        SELECT cf.concessionformid,
               cf.tenantid,
               cf.patientname,
               cf.bookingreferencenumber AS ipdnumber,
               cf.approvedconcessionamount,
               'Auto:ConcessionForm:' || cf.concessionformid::text AS auto_remark
        FROM concessionform cf
        WHERE cf.concessionformid = ANY(
                  string_to_array(pvar_concessionformid, ',')::uuid[])
          AND LOWER(TRIM(COALESCE(cf.verifiedstatus, ''))) = 'approved'
          AND COALESCE(cf.approvedconcessionamount, 0) < 0
          AND COALESCE(cf.isdeleted, false) = false
    ), max_sequence AS (
        SELECT COALESCE(MAX(
            CASE
                WHEN RIGHT(r.receivableno, 5) ~ '^[0-9]+$'
                    THEN RIGHT(r.receivableno, 5)::integer
                ELSE 0
            END), 0) AS sequence
        FROM receivable r
        WHERE LEFT(r.receivableno, 8) = to_char(CURRENT_DATE, 'YYYYMMDD')
          AND r.receivableno NOT LIKE '%/%'
    ), rows_to_insert AS (
        SELECT af.*,
               row_number() OVER (ORDER BY af.concessionformid) AS row_number
        FROM approved_forms af
        WHERE NOT EXISTS (
            SELECT 1
            FROM receivable r
            WHERE r.ipdnumber = af.ipdnumber
              AND r.remarks = af.auto_remark
              AND COALESCE(r.isdeleted, false) = false
        )
    )
    INSERT INTO receivable (
        receivableid,
        tenantid,
        receivableno,
        receivabledate,
        patientname,
        ipdnumber,
        receivablefor,
        amount,
        paidamount,
        paymentstatus,
        remarks,
        createduser,
        createddate,
        isdeleted)
    SELECT gen_random_uuid(),
           rti.tenantid,
           to_char(CURRENT_DATE, 'YYYYMMDD') || '-' ||
               to_char((ms.sequence + rti.row_number)::integer, 'fm00000'),
           CURRENT_DATE,
           rti.patientname,
           rti.ipdnumber,
           'Concession',
           rti.approvedconcessionamount,
           0,
           'Pending',
           rti.auto_remark,
           pvar_createduser,
           NOW(),
           false
    FROM rows_to_insert rti
    CROSS JOIN max_sequence ms;

    GET DIAGNOSTICS inserted_rows = ROW_COUNT;
    RETURN inserted_rows;
END;
$function$;
