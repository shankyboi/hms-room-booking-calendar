CREATE OR REPLACE FUNCTION "IPD_Has_Active_Razorpay_Request"
(
    pvar_ipdapplicationformid uuid,
    pvar_activeafter timestamp without time zone
)
RETURNS boolean
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1
        FROM PaymentRequest pr
        WHERE COALESCE(pr.isdeleted, false) = false
          AND pr.paymentgateway = 'RazorPay'
          AND pr.paymenttype = 'Booking Deposit'
          AND pr.paymentid LIKE ('IPD:' || pvar_ipdapplicationformid::varchar || ':%')
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
$$;
