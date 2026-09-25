CREATE OR REPLACE FUNCTION public."Get_IPD_Remaining_Overpayment_Amount"(
    pvar_ipdapplicationformid uuid)
RETURNS numeric
LANGUAGE sql
STABLE
AS $BODY$
    WITH ipd_receivables AS (
        SELECT
            COALESCE(SUM(COALESCE(r.amount, 0)), 0) AS statement_total,
            COALESCE(SUM(COALESCE(r.paidamount, 0)), 0) AS receivable_paid
        FROM Receivable r
        WHERE r.ipdnumber = pvar_ipdapplicationformid
          AND COALESCE(r.isdeleted, false) = false
    ),
    ipd_payments AS (
        SELECT
            COALESCE(SUM(CASE
                WHEN COALESCE(bp.amount, 0) > 0
                 AND LOWER(TRIM(COALESCE(bp.paymentstatus, ''))) IN ('paid', 'success', 'completed')
                 AND LOWER(TRIM(COALESCE(bp.receivablefor, ''))) <> 'cancellation refund'
                THEN bp.amount ELSE 0 END), 0) AS positive_paid,
            COALESCE(SUM(CASE
                WHEN COALESCE(bp.amount, 0) < 0
                  OR LOWER(TRIM(COALESCE(bp.receivablefor, ''))) = 'cancellation refund'
                THEN GREATEST(ABS(COALESCE(bp.amount, 0)), COALESCE(bp.refundedamount, 0))
                ELSE 0 END), 0) AS refund_rows,
            COALESCE(SUM(CASE
                WHEN COALESCE(bp.amount, 0) > 0
                 AND COALESCE(bp.refundedamount, 0) > 0
                 AND LOWER(TRIM(COALESCE(bp.refundstatus, ''))) NOT IN ('failed', 'cancelled', 'canceled')
                THEN bp.refundedamount ELSE 0 END), 0) AS updated_refunds
        FROM BillingPayment bp
        WHERE COALESCE(bp.isdeleted, false) = false
          AND (
              bp.ipdnumber = pvar_ipdapplicationformid
              OR bp.patientvisit IN (
                  SELECT pv.patientvisitid
                  FROM PatientVisit pv
                  WHERE pv.ipdnumber = pvar_ipdapplicationformid
                    AND COALESCE(pv.isdeleted, false) = false
              )
          )
    )
    SELECT GREATEST(
        GREATEST(
            CASE
                WHEN p.positive_paid > 0
                    THEN p.positive_paid - LEAST(p.positive_paid, GREATEST(p.refund_rows, p.updated_refunds))
                ELSE r.receivable_paid
            END,
            0
        ) - r.statement_total,
        0
    )
    FROM ipd_receivables r
    CROSS JOIN ipd_payments p;
$BODY$;
