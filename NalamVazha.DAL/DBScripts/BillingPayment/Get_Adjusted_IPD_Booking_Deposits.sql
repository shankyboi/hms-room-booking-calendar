CREATE OR REPLACE FUNCTION public."Get_Adjusted_IPD_Booking_Deposits"(
    pvar_patientid uuid,
    pvar_ipdid uuid
)
RETURNS TABLE (
    receivableid uuid,
    receivabledate text,
    receivablefor text,
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
        COALESCE(r.receivablefor, 'IPD Booking Deposit') AS receivablefor,
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
      AND COALESCE(r.isdeleted, false) = false
      AND COALESCE(r.amount, 0) > 0.009
      AND LOWER(TRIM(COALESCE(r.receivablefor, ''))) LIKE '%booking deposit%'
      -- The normal pending-receivables query already returns deposits that
      -- still have a balance.  Restore only a deposit that reached zero when
      -- Apply_Outstanding_IPD_Discounts adjusted it; do not surface deposits
      -- that were paid through the regular payment flow.
      AND ABS(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0)) <= 0.009
      AND r.billingpaymentid IS NULL
      AND EXISTS (
          SELECT 1
          FROM receivable discount_row
          WHERE discount_row.ipdnumber = r.ipdnumber
            AND COALESCE(discount_row.isdeleted, false) = false
            AND LOWER(TRIM(COALESCE(discount_row.receivablefor, ''))) IN ('discount', 'concession')
            AND COALESCE(discount_row.amount, 0) < -0.009
            AND ABS(COALESCE(discount_row.amount, 0) - COALESCE(discount_row.paidamount, 0)) <= 0.009
      )
    ORDER BY r.receivabledate, r.receivableid;
$BODY$;
