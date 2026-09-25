CREATE OR REPLACE FUNCTION "IPD_Has_Cancellation_Or_Refund"
(
    pvar_ipdapplicationformid uuid
)
RETURNS boolean
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1
        FROM BillingPayment
        WHERE COALESCE(isdeleted, false) = false
          AND ipdnumber = pvar_ipdapplicationformid
          AND (
              LOWER(COALESCE(receivablefor, '')) = 'cancellation refund'
              OR LOWER(COALESCE(paymentstatus, '')) LIKE 'refund%'
              OR LOWER(COALESCE(refundstatus, '')) LIKE 'refund%'
          )
    );
END;
$$;
