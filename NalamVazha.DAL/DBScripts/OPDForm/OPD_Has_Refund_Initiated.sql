CREATE OR REPLACE FUNCTION "OPD_Has_Refund_Initiated"
(
    pvar_opdnumber uuid
)
RETURNS boolean
AS $BODY$
BEGIN
    RETURN EXISTS (
        SELECT 1
        FROM BillingPayment bp
        WHERE COALESCE(bp.isdeleted, false) = false
          AND bp.opdnumber = pvar_opdnumber
          AND (
              LOWER(COALESCE(bp.receivablefor, '')) = 'cancellation refund'
              OR LOWER(COALESCE(bp.paymentstatus, '')) LIKE 'refund%'
              OR LOWER(COALESCE(bp.refundstatus, '')) LIKE 'refund%'
              OR COALESCE(bp.amount, 0) < 0
          )
    );
END;
$BODY$
LANGUAGE plpgsql;
