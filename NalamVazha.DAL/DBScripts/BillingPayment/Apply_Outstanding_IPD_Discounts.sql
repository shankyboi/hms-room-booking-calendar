CREATE OR REPLACE FUNCTION "Apply_Outstanding_IPD_Discounts"(
    pvar_ipdid uuid
)
RETURNS void
LANGUAGE plpgsql
AS $$
DECLARE
    discount_row record;
    charge_row record;
    discount_remaining numeric;
    applying numeric;
    new_discount_paid numeric;
    new_charge_paid numeric;
BEGIN
    IF pvar_ipdid IS NULL THEN
        RETURN;
    END IF;

    LOCK TABLE receivable IN EXCLUSIVE MODE;

    FOR discount_row IN
        SELECT
            r.receivableid,
            COALESCE(r.amount, 0) AS amount,
            COALESCE(r.paidamount, 0) AS paidamount
        FROM receivable r
        WHERE r.ipdnumber = pvar_ipdid
          AND COALESCE(r.isdeleted, false) = false
          -- Room-transfer balance rows are already generated net of their
          -- credit adjustment.  Only explicit discounts/concessions must be
          -- applied here; applying room-transfer credits again halves the net
          -- room bill and leaves a false outstanding balance.
          AND LOWER(TRIM(COALESCE(r.receivablefor, ''))) IN ('discount', 'concession')
          AND COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0) < -0.009
        ORDER BY r.receivabledate, r.receivableid
        FOR UPDATE
    LOOP
        discount_remaining := discount_row.paidamount - discount_row.amount;
        new_discount_paid := discount_row.paidamount;

        FOR charge_row IN
            SELECT
                r.receivableid,
                COALESCE(r.amount, 0) AS amount,
                COALESCE(r.paidamount, 0) AS paidamount
            FROM receivable r
            WHERE r.ipdnumber = pvar_ipdid
              AND COALESCE(r.isdeleted, false) = false
              AND LOWER(TRIM(COALESCE(r.receivablefor, ''))) NOT IN ('discount', 'concession')
              -- Deposits are refundable security/advance amounts, not service
              -- charges. A concession must reduce room/clinical charges and
              -- must never make an unpaid deposit disappear from collection.
              AND LOWER(TRIM(COALESCE(r.receivablefor, ''))) NOT LIKE '%deposit%'
              AND COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0) > 0.009
            ORDER BY r.receivabledate, r.receivableid
            FOR UPDATE
        LOOP
            EXIT WHEN discount_remaining <= 0.009;

            applying := LEAST(discount_remaining, charge_row.amount - charge_row.paidamount);
            new_charge_paid := charge_row.paidamount + applying;

            UPDATE receivable
            SET paidamount = new_charge_paid,
                paymentstatus = CASE
                    WHEN new_charge_paid + 0.009 >= COALESCE(amount, 0) THEN 'Paid'
                    ELSE 'Partially Paid'
                END,
                modifieddate = NOW()
            WHERE receivableid = charge_row.receivableid;

            new_discount_paid := new_discount_paid - applying;
            discount_remaining := discount_remaining - applying;
        END LOOP;

        IF new_discount_paid <> discount_row.paidamount THEN
            UPDATE receivable
            SET paidamount = new_discount_paid,
                paymentstatus = CASE
                    WHEN new_discount_paid <= COALESCE(amount, 0) + 0.009 THEN 'Paid'
                    ELSE 'Partially Paid'
                END,
                modifieddate = NOW()
            WHERE receivableid = discount_row.receivableid;
        END IF;
    END LOOP;
END;
$$;
