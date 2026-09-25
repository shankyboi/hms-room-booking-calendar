CREATE OR REPLACE FUNCTION "IPD_Has_Successful_Booking_Deposit_Payment"
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
          AND LOWER(COALESCE(receivablefor, '')) = 'ipd booking deposit'
          AND LOWER(COALESCE(paymentstatus, '')) IN ('success', 'paid')
    );
END;
$$;
