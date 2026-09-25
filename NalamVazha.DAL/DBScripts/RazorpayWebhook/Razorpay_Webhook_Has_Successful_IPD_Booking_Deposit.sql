CREATE OR REPLACE FUNCTION "Razorpay_Webhook_Has_Successful_IPD_Booking_Deposit"
(
    pvar_ipdnumber uuid
)
RETURNS boolean
AS $BODY$
BEGIN
    RETURN EXISTS (
        SELECT 1
        FROM BillingPayment bp
        WHERE COALESCE(bp.isdeleted, false) = false
          AND bp.ipdnumber = pvar_ipdnumber
          AND LOWER(COALESCE(bp.receivablefor, '')) = 'ipd booking deposit'
          AND LOWER(COALESCE(bp.paymentstatus, '')) IN ('success', 'paid')
    );
END;
$BODY$
LANGUAGE plpgsql;
