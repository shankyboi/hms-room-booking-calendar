CREATE OR REPLACE FUNCTION "Razorpay_Webhook_Has_Successful_OPD_Payment"
(
    pvar_opdnumber uuid
)
RETURNS boolean
AS $BODY$
BEGIN
    RETURN EXISTS (
        SELECT 1
        FROM BillingPayment bp
        WHERE COALESCE(bp.isdeleted, false) = false
          AND bp.opdnumber = pvar_opdnumber
          AND LOWER(COALESCE(bp.paymentstatus, '')) IN ('paid', 'success')
          AND COALESCE(bp.amount, 0) > 0
    );
END;
$BODY$
LANGUAGE plpgsql;
