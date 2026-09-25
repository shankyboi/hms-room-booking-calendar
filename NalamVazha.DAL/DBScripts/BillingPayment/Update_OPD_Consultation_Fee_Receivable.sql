CREATE OR REPLACE FUNCTION "Update_OPD_Consultation_Fee_Receivable"
(
    p_opdformid uuid,
    p_paidamount numeric,
    p_billingpaymentid uuid
)
RETURNS void
LANGUAGE plpgsql
AS $$
DECLARE
    rec record;
    remaining numeric := COALESCE(p_paidamount, 0);
    receivable_balance numeric;
    applying numeric;
    new_paid numeric;
    pending_count integer := 0;
BEGIN
    IF remaining <= 0 THEN
        RAISE EXCEPTION 'Paid amount must be greater than zero.';
    END IF;

    FOR rec IN
        SELECT
            receivableid,
            COALESCE(amount, 0) AS amount,
            COALESCE(paidamount, 0) AS paidamount
        FROM Receivable
        WHERE opdnumber = p_opdformid
          AND COALESCE(isdeleted, false) = false
          AND COALESCE(amount, 0) > COALESCE(paidamount, 0)
        ORDER BY createddate, receivableid
        FOR UPDATE
    LOOP
        pending_count := pending_count + 1;
        EXIT WHEN remaining <= 0;

        receivable_balance := rec.amount - rec.paidamount;
        IF receivable_balance <= 0 THEN
            CONTINUE;
        END IF;

        applying := LEAST(remaining, receivable_balance);
        new_paid := rec.paidamount + applying;

        UPDATE Receivable
        SET
            paidamount = new_paid,
            paymentstatus = CASE
                WHEN new_paid + 0.009 >= rec.amount THEN 'Paid'
                ELSE 'Partially Paid'
            END,
            billingpaymentid = p_billingpaymentid
        WHERE receivableid = rec.receivableid;

        remaining := remaining - applying;
    END LOOP;

    IF pending_count = 0 THEN
        RAISE EXCEPTION 'No pending OPD receivable found to apply Razorpay payment.';
    END IF;

    IF remaining > 0.009 THEN
        RAISE EXCEPTION 'Razorpay payment amount is greater than the pending OPD receivable balance.';
    END IF;
END;
$$;
