CREATE OR REPLACE FUNCTION "BillingPayment_Has_Active_Razorpay_Request"
(
    pvar_paymentmarker character varying,
    pvar_paymenttype character varying,
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
          AND (pvar_paymenttype IS NULL OR pr.paymenttype = pvar_paymenttype)
          AND pr.paymentid LIKE pvar_paymentmarker
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
