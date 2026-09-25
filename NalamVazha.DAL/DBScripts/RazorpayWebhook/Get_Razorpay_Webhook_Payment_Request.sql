CREATE OR REPLACE FUNCTION "Get_Razorpay_Webhook_Payment_Request"
(
    pvar_orderid varchar
)
RETURNS TABLE(
    paymentrequestid uuid,
    tenantid uuid,
    paymenttype varchar,
    orderid varchar,
    paymentid varchar,
    amount decimal,
    currency varchar,
    createduser uuid
)
AS $BODY$
BEGIN
    RETURN QUERY
    SELECT
        pr.paymentrequestid,
        pr.tenantid,
        pr.paymenttype,
        pr.orderid,
        pr.paymentid,
        pr.amount,
        pr.currency,
        pr.createduser
    FROM PaymentRequest pr
    WHERE COALESCE(pr.isdeleted, false) = false
      AND pr.orderid = pvar_orderid
    ORDER BY pr.createddate DESC
    LIMIT 1;
END;
$BODY$
LANGUAGE plpgsql;
