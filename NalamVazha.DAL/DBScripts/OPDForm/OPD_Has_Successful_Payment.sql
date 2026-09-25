CREATE OR REPLACE FUNCTION "OPD_Has_Successful_Payment"
(
    pvar_opdnumber uuid
)
RETURNS boolean
AS $BODY$
BEGIN
    RETURN public."Get_OPD_Remaining_Refundable_Amount"(pvar_opdnumber) > 0.009 OR (
        /* A fully waived/neutralized OPD has no amount left to collect. It is
           payment-complete for workflow purposes even without a positive
           BillingPayment row. Require at least one receivable so a booking
           with no billing setup is not accidentally treated as paid. */
        EXISTS (
            SELECT 1
            FROM Receivable r
            WHERE r.opdnumber = pvar_opdnumber
              AND COALESCE(r.isdeleted, false) = false
        )
        AND COALESCE((
            SELECT SUM(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0))
            FROM Receivable r
            WHERE r.opdnumber = pvar_opdnumber
              AND COALESCE(r.isdeleted, false) = false
        ), 0) <= 0
    );
END;
$BODY$
LANGUAGE plpgsql;

/* Backfill assessments that were saved before zero/negative net receivables
   were recognized as payment-complete. */
WITH released AS (
    UPDATE OPDForm o
    SET verifiedstatus = 'Assessment Intern Review Pending',
        verifieddate = NOW(),
        modifieddate = NOW()
    WHERE COALESCE(o.isdeleted, false) = false
      AND LOWER(COALESCE(o.verifiedstatus, '')) IN
          ('approved', 'opd approved', 'assessment pending', 'assessment in progress')
      AND LOWER(COALESCE(o.appointmentmode, '')) LIKE '%online%'
      AND EXISTS (
          SELECT 1 FROM Assessment a
          WHERE a.opdform = o.opdformid
            AND COALESCE(a.isdeleted, false) = false)
      AND "OPD_Has_Successful_Payment"(o.opdformid)
    RETURNING o.opdformid, COALESCE(o.modifieduser, o.createduser) AS actionuser
)
INSERT INTO reviewlogsOPDForm(opdformid, verifiedstatus, reviewcomments, createduser)
SELECT opdformid,
       'Assessment Intern Review Pending',
       'Online OPD payment completed by full waiver/adjustment; assessment released for review',
       actionuser
FROM released
WHERE actionuser IS NOT NULL;
