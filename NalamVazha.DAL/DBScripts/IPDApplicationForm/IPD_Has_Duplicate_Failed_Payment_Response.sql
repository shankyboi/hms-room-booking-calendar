CREATE OR REPLACE FUNCTION "IPD_Has_Duplicate_Failed_Payment_Response"
(
    pvar_paymentrequest uuid,
    pvar_orderid varchar,
    pvar_paymentid varchar,
    pvar_errorcode varchar,
    pvar_errorreason varchar
)
RETURNS boolean
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1
        FROM PaymentResponse
        WHERE COALESCE(isdeleted, false) = false
          AND paymentrequest = pvar_paymentrequest
          AND LOWER(COALESCE(status, '')) = 'failed'
          AND COALESCE(orderid, '') = COALESCE(pvar_orderid, '')
          AND (
              (COALESCE(pvar_paymentid, '') <> '' AND COALESCE(paymentid, '') = COALESCE(pvar_paymentid, ''))
              OR (
                  COALESCE(pvar_paymentid, '') = ''
                  AND COALESCE(paymentid, '') = ''
                  AND COALESCE(gatewayresponsecode, '') = COALESCE(pvar_errorcode, '')
                  AND COALESCE(responsesignature, '') = COALESCE(pvar_errorreason, '')
              )
          )
    );
END;
$$;
