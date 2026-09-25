CREATE OR REPLACE FUNCTION "Get_HealthSeeker_Payments"(
    pvar_tenantid uuid,
    pvar_patientid uuid,
    pvar_paymentdate_automatonfrom varchar,
    pvar_paymentdate_automatonto varchar,
    pvar_pagesize integer,
    pvar_pagenumber integer
)
RETURNS TABLE(
    billingpaymentid uuid,
    receiptno varchar,
    receivablefor varchar,
    amount numeric,
    paymentmode varchar,
    transactionreference varchar,
    paymentstatus varchar,
    refundstatus varchar,
    refundedamount numeric,
    paymentdate varchar,
    createddate timestamp without time zone,
    ipdnumber uuid,
    ipdnumber_master varchar,
    opdnumber uuid,
    opdnumber_master varchar,
    remarks varchar
)
LANGUAGE plpgsql
AS $$
DECLARE
    from_date date;
    to_date date;
BEGIN
    from_date := NULLIF(pvar_paymentdate_automatonfrom, '')::date;
    to_date := NULLIF(pvar_paymentdate_automatonto, '')::date;

    RETURN QUERY
    SELECT
        bp.billingpaymentid,
        bp.receiptno,
        bp.receivablefor,
        COALESCE(bp.amount, 0) AS amount,
        bp.paymentmode,
        bp.transactionreference,
        bp.paymentstatus,
        bp.refundstatus,
        COALESCE(bp.refundedamount, 0) AS refundedamount,
        to_char(COALESCE(NULLIF(bp.paymentdate, '0001-01-01'::date), bp.createddate::date), 'DD Mon YYYY')::varchar AS paymentdate,
        bp.createddate,
        bp.ipdnumber,
        COALESCE(ipd.bookingreferencenumber, '')::varchar AS ipdnumber_master,
        bp.opdnumber,
        COALESCE(opd.bookingreferencenumber, '')::varchar AS opdnumber_master,
        bp.remarks
    FROM billingpayment bp
    LEFT JOIN ipdapplicationform ipd ON bp.ipdnumber = ipd.ipdapplicationformid
    LEFT JOIN opdform opd ON bp.opdnumber = opd.opdformid
    WHERE COALESCE(bp.isdeleted, false) = false
      AND (pvar_tenantid IS NULL OR bp.tenantid = pvar_tenantid)
      AND bp.patientname = pvar_patientid
      AND (from_date IS NULL OR COALESCE(NULLIF(bp.paymentdate, '0001-01-01'::date), bp.createddate::date) >= from_date)
      AND (to_date IS NULL OR COALESCE(NULLIF(bp.paymentdate, '0001-01-01'::date), bp.createddate::date) <= to_date)
    ORDER BY COALESCE(NULLIF(bp.paymentdate, '0001-01-01'::date), bp.createddate::date) DESC, bp.createddate DESC
    LIMIT COALESCE(pvar_pagesize, 200)
    OFFSET COALESCE(pvar_pagenumber, 0) * COALESCE(pvar_pagesize, 200);
END;
$$;
