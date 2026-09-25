CREATE OR REPLACE FUNCTION "Get_OPD_Missing_Razorpay_Billing_Source"
(
    pvar_opdnumber uuid
)
RETURNS TABLE(
    tenantid uuid,
    patientname uuid,
    patientvisit uuid,
    receivablefor varchar,
    createduser uuid,
    amount decimal,
    paymentid varchar,
    orderid varchar
)
AS $BODY$
BEGIN
    RETURN QUERY
    SELECT
        r.tenantid,
        r.patientname,
        r.patientvisit,
        r.receivablefor,
        pr.createduser,
        resp.amount,
        resp.paymentid,
        resp.orderid
    FROM PaymentRequest pr
    INNER JOIN PaymentResponse resp
        ON resp.paymentrequest = pr.PaymentRequestid
       AND COALESCE(resp.isdeleted, false) = false
    INNER JOIN Receivable r
        ON r.opdnumber = pvar_opdnumber
       AND COALESCE(r.isdeleted, false) = false
    WHERE COALESCE(pr.isdeleted, false) = false
      AND pr.paymentgateway = 'RazorPay'
      AND pr.paymentid LIKE ('OPD:' || pvar_opdnumber::varchar || ':%')
      AND LOWER(COALESCE(resp.status, '')) IN ('paid', 'success')
      AND COALESCE(resp.amount, 0) > 0
      AND NOT EXISTS (
          SELECT 1
          FROM BillingPayment bp
          WHERE COALESCE(bp.isdeleted, false) = false
            AND bp.opdnumber = pvar_opdnumber
            AND bp.transactionreference = resp.paymentid
      )
    ORDER BY resp.transactiontime DESC, resp.createddate DESC
    LIMIT 1;
END;
$BODY$
LANGUAGE plpgsql;
