CREATE OR REPLACE FUNCTION public."Get_Outstanding_IPD_Other_Receivables"(
    pvar_patientid uuid,
    pvar_ipdid uuid
)
RETURNS TABLE (
    receivableid uuid,
    receivabledate text,
    displayreceivablefor text,
    remarks text,
    amount numeric,
    paidamount numeric,
    balance numeric,
    ismandatory boolean
)
LANGUAGE sql
STABLE
AS $BODY$
    SELECT
        r.receivableid,
        COALESCE(to_char(r.receivabledate, 'DD/MM/YYYY'), '') AS receivabledate,
        CASE
            WHEN NULLIF(TRIM(COALESCE(r.specifyothers, '')), '') IS NOT NULL
                THEN 'Others - ' || TRIM(r.specifyothers)
            ELSE 'Others'
        END AS displayreceivablefor,
        COALESCE(r.remarks, '') AS remarks,
        COALESCE(r.amount, 0) AS amount,
        COALESCE(r.paidamount, 0) AS paidamount,
        COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0) AS balance,
        EXISTS (
            SELECT 1
            FROM receivableconditions rc
            WHERE LOWER(TRIM(rc.receivablefor)) = LOWER(TRIM(r.receivablefor))
              AND COALESCE(rc.isdeleted, false) = false
              AND COALESCE(rc.ismandatory, false) = true
        ) AS ismandatory
    FROM receivable r
    WHERE r.ipdnumber = pvar_ipdid
      AND r.patientname = pvar_patientid
      AND LOWER(TRIM(COALESCE(r.receivablefor, ''))) = 'others'
      AND COALESCE(r.isdeleted, false) = false
      AND COALESCE(r.remarks, '') NOT ILIKE '%inactive%'
      -- Return the charge even when a concession/discount has fully adjusted
      -- it, so billing screens can show the line with a zero balance instead
      -- of making it appear to be missing.
      AND COALESCE(r.amount, 0) > 0.009
    ORDER BY r.receivabledate, r.receivableid;
$BODY$;
