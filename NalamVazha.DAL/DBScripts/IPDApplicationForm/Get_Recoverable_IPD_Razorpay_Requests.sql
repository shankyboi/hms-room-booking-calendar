CREATE OR REPLACE FUNCTION "Get_Recoverable_IPD_Razorpay_Requests"
(
    pvar_ipdapplicationformid uuid
)
RETURNS TABLE
(
    paymentrequestid uuid,
    tenantid uuid,
    orderid varchar,
    paymentid varchar,
    amount numeric,
    currency varchar,
    createduser uuid
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        pr.paymentrequestid,
        pr.tenantid,
        pr.orderid,
        pr.paymentid,
        pr.amount,
        pr.currency,
        pr.createduser
    FROM PaymentRequest pr
    WHERE COALESCE(pr.isdeleted, false) = false
      AND pr.paymentgateway = 'RazorPay'
      AND pr.paymenttype = 'Booking Deposit'
      AND pr.paymentid LIKE ('IPD:' || pvar_ipdapplicationformid::varchar || ':%')
      AND NOT EXISTS (
          SELECT 1
          FROM PaymentResponse resp
          WHERE COALESCE(resp.isdeleted, false) = false
            AND resp.paymentrequest = pr.PaymentRequestid
            AND LOWER(COALESCE(resp.status, '')) IN ('paid', 'success')
      )
    ORDER BY pr.requestdatetime DESC, pr.createddate DESC
    LIMIT 5;
END;
$$;
