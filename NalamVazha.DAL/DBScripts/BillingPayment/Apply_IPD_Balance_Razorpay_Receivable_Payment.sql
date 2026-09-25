CREATE OR REPLACE FUNCTION "Apply_IPD_Balance_Razorpay_Receivable_Payment"(
    p_ipdid uuid,
    p_paid numeric,
    p_billingpaymentid uuid,
    p_modifieduser uuid
)
RETURNS void
LANGUAGE plpgsql
AS $$
DECLARE
    rec record;
    remaining numeric := COALESCE(p_paid, 0);
    receivable_balance numeric;
    applying numeric;
    new_paid numeric;
BEGIN
    IF remaining <= 0 THEN
        RETURN;
    END IF;

    LOCK TABLE receivable IN EXCLUSIVE MODE;

    FOR rec IN
        SELECT
            receivableid,
            COALESCE(amount, 0) AS amount,
            COALESCE(paidamount, 0) AS paidamount
        FROM receivable
        WHERE ipdnumber = p_ipdid
          AND COALESCE(isdeleted, false) = false
          AND COALESCE(amount, 0) > COALESCE(paidamount, 0)
          AND COALESCE(paymentstatus, '') NOT IN ('Paid', 'Cancelled', 'Refunded', 'Refund Pending')
        ORDER BY receivabledate, createddate, receivableno
    LOOP
        EXIT WHEN remaining <= 0;

        receivable_balance := rec.amount - rec.paidamount;
        IF receivable_balance <= 0 THEN
            CONTINUE;
        END IF;

        applying := LEAST(remaining, receivable_balance);
        new_paid := rec.paidamount + applying;

        UPDATE receivable
        SET
            paidamount = new_paid,
            paymentstatus = CASE
                WHEN new_paid + 0.009 >= rec.amount THEN 'Paid'
                ELSE 'Partially Paid'
            END,
            billingpaymentid = p_billingpaymentid,
            modifieduser = p_modifieduser,
            modifieddate = NOW()
        WHERE receivableid = rec.receivableid;

        remaining := remaining - applying;
    END LOOP;
END;
$$;
