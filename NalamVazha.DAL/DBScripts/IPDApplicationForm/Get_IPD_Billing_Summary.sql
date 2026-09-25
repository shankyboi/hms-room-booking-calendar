CREATE OR REPLACE FUNCTION get_billing_payments_for_ipd(pvar_ipdid TEXT)
RETURNS TABLE (
    billingpaymentid BIGINT,
    paymentfor TEXT,
    amount NUMERIC,
    paymentmode TEXT,
    transactionreference TEXT,
    paymentstatus TEXT,
    remarks TEXT,
    createddate TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        bp.billingpaymentid,
        bp.receivablefor AS paymentfor,
        bp.amount,
        bp.paymentmode,
        bp.transactionreference,
        bp.paymentstatus,
        bp.remarks,
        bp.createddate
    FROM billingpayment bp
    WHERE COALESCE(bp.isdeleted, false) = false
      AND (
            bp.patientvisit::text = pvar_ipdid
            OR bp.patientvisit IN (
                SELECT pv.patientvisitid
                FROM patientvisit pv
                WHERE pv.ipdnumber::text = pvar_ipdid
                  AND COALESCE(pv.isdeleted, false) = false
            )
        )
    ORDER BY bp.createddate ASC;
END;
$$;
