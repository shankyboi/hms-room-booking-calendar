CREATE OR REPLACE FUNCTION "OPD_Has_Paid_Payment_Response"
(
    pvar_paymentid varchar
)
RETURNS boolean
AS $BODY$
BEGIN
    RETURN EXISTS (
        SELECT 1
        FROM PaymentResponse
        WHERE COALESCE(isdeleted, false) = false
          AND paymentid = pvar_paymentid
          AND LOWER(COALESCE(status, '')) IN ('paid', 'success')
    );
END;
$BODY$
LANGUAGE plpgsql;
