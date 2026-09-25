CREATE OR REPLACE FUNCTION "Get_OPD_Razorpay_Paid_Billings"
(
    pvar_opdnumber uuid
)
RETURNS TABLE(
    billingpaymentid uuid,
    amount decimal
)
AS $BODY$
BEGIN
    RETURN QUERY
    SELECT
        bp.BillingPaymentid,
        COALESCE(bp.amount, 0) AS amount
    FROM BillingPayment bp
    WHERE bp.opdnumber = pvar_opdnumber
      AND COALESCE(bp.isdeleted, false) = false
      AND COALESCE(bp.amount, 0) > 0
      AND LOWER(COALESCE(bp.paymentstatus, '')) IN ('paid', 'success', 'completed')
      AND (
          LOWER(COALESCE(bp.paymentmode, '')) LIKE '%razor%'
          OR LOWER(COALESCE(bp.paymentmode, '')) LIKE '%online%'
          OR COALESCE(bp.transactionreference, '') LIKE 'pay_%'
      )
    ORDER BY bp.paymentdate, bp.createddate, bp.BillingPaymentid;
END;
$BODY$
LANGUAGE plpgsql;
