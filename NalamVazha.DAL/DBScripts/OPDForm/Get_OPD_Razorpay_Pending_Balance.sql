CREATE OR REPLACE FUNCTION "Get_OPD_Razorpay_Pending_Balance"
(
    pvar_opdnumber uuid
)
RETURNS decimal
AS $BODY$
BEGIN
    RETURN (
        SELECT COALESCE(SUM(GREATEST(COALESCE(amount, 0) - COALESCE(paidamount, 0), 0)), 0)
        FROM Receivable
        WHERE opdnumber = pvar_opdnumber
          AND COALESCE(isdeleted, false) = false
    );
END;
$BODY$
LANGUAGE plpgsql;
