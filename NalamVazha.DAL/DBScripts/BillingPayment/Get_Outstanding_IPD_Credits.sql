CREATE OR REPLACE FUNCTION "Get_Outstanding_IPD_Credits"(
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
AS $$
    SELECT
        r.receivableid,
        COALESCE(to_char(r.receivabledate, 'DD/MM/YYYY'), '') AS receivabledate,
        COALESCE(r.receivablefor, 'Discount') AS receivablefor,
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
      AND COALESCE(r.isdeleted, false) = false
      AND (
          -- Keep applied discounts visible as informational ledger rows even
          -- after their remaining balance has reached zero.
          (
              LOWER(TRIM(COALESCE(r.receivablefor, ''))) IN ('discount', 'concession')
              AND COALESCE(r.amount, 0) < -0.009
              AND ABS(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0)) <= 0.009
          )
          OR
          (
              LOWER(COALESCE(r.paymentstatus, '')) <> 'paid'
              AND COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0) < -0.009
          )
      )
    ORDER BY r.receivabledate, r.receivableid;
$$;
