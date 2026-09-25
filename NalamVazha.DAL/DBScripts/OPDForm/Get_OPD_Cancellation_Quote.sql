CREATE OR REPLACE FUNCTION public."Get_OPD_Cancellation_Quote"(
    pvar_opdformid uuid)
RETURNS TABLE(
    grosscharge numeric,
    discountamount numeric,
    netpayable numeric,
    successfulpaid numeric,
    alreadyrefunded numeric,
    effectivepaid numeric,
    balance numeric,
    refundableamount numeric,
    onlinerefundableamount numeric,
    hasrazorpaypayment boolean,
    refundinprogress boolean)
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_gross numeric := 0;
    lvar_discount numeric := 0;
    lvar_paid numeric := 0;
    lvar_refunded numeric := 0;
    lvar_onlinepaid numeric := 0;
    lvar_onlinerefunded numeric := 0;
    lvar_net numeric := 0;
    lvar_effectivepaid numeric := 0;
    lvar_refundable numeric := 0;
BEGIN
    SELECT
        COALESCE(SUM(CASE
            WHEN COALESCE(r.amount, 0) > 0
             AND LOWER(TRIM(COALESCE(r.receivablefor, ''))) NOT IN
                 ('discount', 'concession', 'cancellation refund')
             AND LOWER(TRIM(COALESCE(r.paymentstatus, ''))) NOT IN
                 ('cancelled', 'canceled', 'refunded')
            THEN r.amount ELSE 0 END), 0),
        ABS(COALESCE(SUM(CASE
            WHEN COALESCE(r.amount, 0) < 0
             AND LOWER(TRIM(COALESCE(r.receivablefor, ''))) IN ('discount', 'concession')
             AND LOWER(TRIM(COALESCE(r.paymentstatus, ''))) NOT IN
                 ('cancelled', 'canceled', 'refunded')
            THEN r.amount ELSE 0 END), 0))
    INTO lvar_gross, lvar_discount
    FROM Receivable r
    WHERE r.opdnumber = pvar_opdformid
      AND COALESCE(r.isdeleted, false) = false;

    SELECT
        COALESCE(SUM(CASE
            WHEN COALESCE(bp.amount, 0) > 0
             AND LOWER(TRIM(COALESCE(bp.paymentstatus, ''))) IN
                 ('paid', 'success', 'completed')
             AND LOWER(TRIM(COALESCE(bp.receivablefor, ''))) <> 'cancellation refund'
            THEN bp.amount ELSE 0 END), 0),
        LEAST(
            COALESCE(SUM(CASE
                WHEN COALESCE(bp.amount, 0) < 0
                  OR LOWER(TRIM(COALESCE(bp.receivablefor, ''))) = 'cancellation refund'
                THEN GREATEST(ABS(COALESCE(bp.amount, 0)), COALESCE(bp.refundedamount, 0))
                ELSE 0 END), 0)
            + COALESCE(SUM(CASE
                /* Some older refund flows update refundedamount on the original
                   positive payment instead of inserting a negative refund row. */
                WHEN COALESCE(bp.amount, 0) > 0
                 AND COALESCE(bp.refundedamount, 0) > 0
                 AND LOWER(TRIM(COALESCE(bp.refundstatus, ''))) NOT IN ('failed', 'cancelled', 'canceled')
                THEN bp.refundedamount ELSE 0 END), 0),
            COALESCE(SUM(CASE
                WHEN COALESCE(bp.amount, 0) > 0
                 AND LOWER(TRIM(COALESCE(bp.paymentstatus, ''))) IN ('paid', 'success', 'completed')
                 AND LOWER(TRIM(COALESCE(bp.receivablefor, ''))) <> 'cancellation refund'
                THEN bp.amount ELSE 0 END), 0)
        ),
        COALESCE(SUM(CASE
            WHEN COALESCE(bp.amount, 0) > 0
             AND LOWER(TRIM(COALESCE(bp.paymentstatus, ''))) IN
                 ('paid', 'success', 'completed')
             AND (
                 LOWER(COALESCE(bp.paymentmode, '')) LIKE '%razor%'
                 OR LOWER(COALESCE(bp.paymentmode, '')) LIKE '%online%'
                 OR COALESCE(bp.transactionreference, '') LIKE 'pay_%')
            THEN bp.amount ELSE 0 END), 0),
        LEAST(
            COALESCE(SUM(CASE
                WHEN (
                     COALESCE(bp.amount, 0) < 0
                     OR LOWER(TRIM(COALESCE(bp.receivablefor, ''))) = 'cancellation refund')
                 AND (
                     LOWER(COALESCE(bp.refundmode, '')) LIKE '%razor%'
                     OR LOWER(COALESCE(bp.paymentmode, '')) LIKE '%razor%')
                THEN GREATEST(ABS(COALESCE(bp.amount, 0)), COALESCE(bp.refundedamount, 0))
                ELSE 0 END), 0)
            + COALESCE(SUM(CASE
                WHEN COALESCE(bp.amount, 0) > 0
                 AND COALESCE(bp.refundedamount, 0) > 0
                 AND LOWER(TRIM(COALESCE(bp.refundstatus, ''))) NOT IN ('failed', 'cancelled', 'canceled')
                 AND (
                     LOWER(COALESCE(bp.refundmode, '')) LIKE '%razor%'
                     OR LOWER(COALESCE(bp.paymentmode, '')) LIKE '%razor%'
                     OR COALESCE(bp.transactionreference, '') LIKE 'pay_%')
                THEN bp.refundedamount ELSE 0 END), 0),
            COALESCE(SUM(CASE
                WHEN COALESCE(bp.amount, 0) > 0
                 AND LOWER(TRIM(COALESCE(bp.paymentstatus, ''))) IN ('paid', 'success', 'completed')
                 AND (
                     LOWER(COALESCE(bp.paymentmode, '')) LIKE '%razor%'
                     OR LOWER(COALESCE(bp.paymentmode, '')) LIKE '%online%'
                     OR COALESCE(bp.transactionreference, '') LIKE 'pay_%')
                THEN bp.amount ELSE 0 END), 0)
        )
    INTO lvar_paid, lvar_refunded, lvar_onlinepaid, lvar_onlinerefunded
    FROM BillingPayment bp
    WHERE bp.opdnumber = pvar_opdformid
      AND COALESCE(bp.isdeleted, false) = false;

    lvar_net := GREATEST(lvar_gross - lvar_discount, 0);
    lvar_effectivepaid := GREATEST(lvar_paid - lvar_refunded, 0);
    lvar_refundable := GREATEST(lvar_effectivepaid - lvar_net, 0);

    RETURN QUERY SELECT
        lvar_gross,
        lvar_discount,
        lvar_net,
        lvar_paid,
        lvar_refunded,
        lvar_effectivepaid,
        GREATEST(lvar_net - lvar_effectivepaid, 0),
        lvar_refundable,
        -- Online refundable balance is the unrefunded online payment. The
        -- controller applies cancellation or overpayment limits as appropriate.
        LEAST(lvar_effectivepaid, GREATEST(lvar_onlinepaid - lvar_onlinerefunded, 0)),
        lvar_onlinepaid > 0,
        public."OPD_Has_Refund_Initiated"(pvar_opdformid);
END;
$BODY$;

