CREATE OR REPLACE FUNCTION public."Get_OPD_Remaining_Refundable_Amount"(
    pvar_opdformid uuid)
RETURNS numeric
LANGUAGE sql
STABLE
AS $BODY$
    SELECT GREATEST(
        COALESCE(SUM(CASE
            WHEN COALESCE(bp.amount, 0) > 0
             AND LOWER(TRIM(COALESCE(bp.paymentstatus, ''))) IN ('paid', 'success', 'completed')
             AND LOWER(TRIM(COALESCE(bp.receivablefor, ''))) <> 'cancellation refund'
            THEN bp.amount ELSE 0 END), 0)
        - LEAST(
            COALESCE(SUM(CASE
                WHEN COALESCE(bp.amount, 0) < 0
                  OR LOWER(TRIM(COALESCE(bp.receivablefor, ''))) = 'cancellation refund'
                THEN GREATEST(ABS(COALESCE(bp.amount, 0)), COALESCE(bp.refundedamount, 0))
                ELSE 0 END), 0)
            + COALESCE(SUM(CASE
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
        0
    )
    FROM BillingPayment bp
    WHERE bp.opdnumber = pvar_opdformid
      AND COALESCE(bp.isdeleted, false) = false;
$BODY$;
