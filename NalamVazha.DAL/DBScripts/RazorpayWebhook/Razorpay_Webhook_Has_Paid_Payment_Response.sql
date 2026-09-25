CREATE OR REPLACE FUNCTION "Razorpay_Webhook_Has_Paid_Payment_Response"
(
    pvar_paymentid varchar
)
RETURNS boolean
AS $BODY$
BEGIN
    RETURN EXISTS (
        SELECT 1
        FROM PaymentResponse pr
        WHERE COALESCE(pr.isdeleted, false) = false
          AND pr.paymentid = pvar_paymentid
          AND LOWER(COALESCE(pr.status, '')) IN ('paid', 'success')
    );
END;
$BODY$
LANGUAGE plpgsql;
