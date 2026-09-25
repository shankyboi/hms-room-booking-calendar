CREATE OR REPLACE FUNCTION "Apply_Selected_IPD_Balance_Razorpay_Receivable_Payment"(
    p_receivableid uuid,
    p_paid numeric,
    p_billingpaymentid uuid,
    p_modifieduser uuid
)
RETURNS void
LANGUAGE plpgsql
AS $$
DECLARE
    target_ipd uuid;
    target_type text;
    rec record;
    remaining numeric := COALESCE(p_paid, 0);
    applying numeric;
    new_paid numeric;
BEGIN
    IF p_receivableid IS NULL OR remaining <= 0 THEN
        RETURN;
    END IF;

    SELECT ipdnumber, LOWER(TRIM(COALESCE(receivablefor, '')))
    INTO target_ipd, target_type
    FROM receivable
    WHERE receivableid = p_receivableid
      AND COALESCE(isdeleted, false) = false
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Receivable not found.';
    END IF;

    -- The dashboard consolidates multiple room receivables into one row and
    -- submits one representative id with the combined PayNow amount. Apply to
    -- that row first, then waterfall the remainder across the other room rows.
    FOR rec IN
        SELECT receivableid, COALESCE(amount, 0) AS amount,
               COALESCE(paidamount, 0) AS paidamount
        FROM receivable
        WHERE COALESCE(isdeleted, false) = false
          AND COALESCE(amount, 0) > COALESCE(paidamount, 0)
          AND (
              receivableid = p_receivableid
              OR (target_type = 'room' AND ipdnumber = target_ipd
                  AND LOWER(TRIM(COALESCE(receivablefor, ''))) = 'room')
          )
        ORDER BY CASE WHEN receivableid = p_receivableid THEN 0 ELSE 1 END,
                 receivabledate, createddate, receivableid
        FOR UPDATE
    LOOP
        EXIT WHEN remaining <= 0.009;
        applying := LEAST(remaining, rec.amount - rec.paidamount);
        new_paid := rec.paidamount + applying;

        UPDATE receivable
        SET paidamount = new_paid,
            paymentstatus = CASE WHEN new_paid + 0.009 >= rec.amount
                                 THEN 'Paid' ELSE 'Partially Paid' END,
            billingpaymentid = p_billingpaymentid,
            modifieduser = p_modifieduser,
            modifieddate = NOW()
        WHERE receivableid = rec.receivableid;

        remaining := remaining - applying;
    END LOOP;
END;
$$;
