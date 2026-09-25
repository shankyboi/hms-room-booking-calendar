CREATE OR REPLACE FUNCTION get_cancel_ipd_payment_history
(
    pvar_ipdapplicationformid uuid
)
RETURNS TABLE(
    billingpaymentid uuid,
    receiptno varchar,
    receivableid uuid,
    receivablefor varchar,
    paymentdate timestamp,
    createddate timestamp,
    amount numeric,
    paymentmode varchar,
    paymentstatus varchar,
    collectedby_master varchar,
    refundstatus varchar,
    refundedamount numeric,
    remarks varchar,
    transactionreference varchar,
    room uuid,
    room_master varchar
)
AS $BODY$
BEGIN
    RETURN QUERY
    SELECT
        bp.billingpaymentid,
        bp.receiptno,
        NULL::uuid AS receivableid,
        COALESCE(NULLIF(linked_receivables.receivablefor, ''), bp.receivablefor, '')::varchar AS receivablefor,
        bp.paymentdate,
        bp.createddate,
        COALESCE(bp.amount, 0)::numeric AS amount,
        bp.paymentmode,
        CASE
            WHEN lower(COALESCE(bp.paymentstatus, '')) = 'refunded' THEN 'Refund Initiated'
            ELSE bp.paymentstatus
        END::varchar AS paymentstatus,
        CASE
            WHEN lower(COALESCE(bp.paymentstatus, '')) IN ('success', 'paid', 'completed')
                 AND (
                     lower(COALESCE(bp.counterid, '')) = 'through razorpay'
                     OR lower(COALESCE(bp.paymentmode, '')) LIKE '%razorpay%'
                     OR COALESCE(bp.transactionreference, '') ILIKE 'pay\_%' ESCAPE '\'
                     OR lower(COALESCE(bp.remarks, '')) LIKE '%razorpay%'
                 )
                THEN 'Through Razorpay'
            ELSE COALESCE(u.firstname || ' ' || u.lastname, '')
        END::varchar AS collectedby_master,
        CASE
            WHEN razorpay_payment.paymentid IS NOT NULL THEN 'Through Razorpay'
            ELSE COALESCE(u.firstname || ' ' || u.lastname, '')
        END::varchar AS collectedby_master,
        CASE
            WHEN lower(COALESCE(bp.refundstatus, '')) = 'refunded' THEN 'Refund Initiated'
            ELSE bp.refundstatus
        END::varchar AS refundstatus,
        COALESCE(bp.refundedamount, 0)::numeric AS refundedamount,
        bp.remarks,
        bp.transactionreference,
        bp.room,
        COALESCE(r.roomnumber, '')::varchar AS room_master
    FROM BillingPayment bp
    LEFT JOIN users u ON u.usersid = COALESCE(bp.refundedby, bp.collectedby)
    LEFT JOIN Room r ON r.Roomid = bp.room
    LEFT JOIN LATERAL (
        SELECT string_agg(DISTINCT NULLIF(TRIM(rec.receivablefor), ''), ', ')
                   AS receivablefor
        FROM receivable rec
        WHERE rec.billingpaymentid = bp.billingpaymentid
          AND COALESCE(rec.isdeleted, false) = false
    ) linked_receivables ON true
    LEFT JOIN LATERAL (
        SELECT pr.paymentid
        FROM paymentresponse pr
        INNER JOIN paymentrequest req
            ON req.paymentrequestid = pr.paymentrequest
        WHERE pr.paymentid = bp.transactionreference
          AND LOWER(COALESCE(req.paymentgateway, '')) = 'razorpay'
          AND COALESCE(pr.isdeleted, false) = false
          AND COALESCE(req.isdeleted, false) = false
        LIMIT 1
    ) razorpay_payment ON true
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
    ORDER BY bp.createddate ASC;
END;
$BODY$
LANGUAGE plpgsql;
