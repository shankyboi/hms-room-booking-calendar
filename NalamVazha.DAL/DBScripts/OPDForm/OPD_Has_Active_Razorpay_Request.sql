CREATE OR REPLACE FUNCTION "OPD_Has_Active_Razorpay_Request"
(
    pvar_opdnumber uuid,
    pvar_activeafter timestamp
)
RETURNS boolean
AS $BODY$
BEGIN
    RETURN EXISTS (
        SELECT 1
        FROM PaymentRequest pr
        WHERE COALESCE(pr.isdeleted, false) = false
          AND pr.paymentgateway = 'RazorPay'
          AND pr.paymentid LIKE ('OPD:' || pvar_opdnumber::varchar || ':%')
          AND pr.requestdatetime >= pvar_activeafter
          AND NOT EXISTS (
              SELECT 1
              FROM PaymentResponse resp
              WHERE COALESCE(resp.isdeleted, false) = false
                AND resp.paymentrequest = pr.PaymentRequestid
                AND LOWER(COALESCE(resp.status, '')) IN ('paid', 'success', 'failed')
          )
    );
END;
$BODY$
LANGUAGE plpgsql;
