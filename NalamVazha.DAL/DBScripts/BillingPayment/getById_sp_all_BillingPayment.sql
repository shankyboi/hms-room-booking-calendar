
DROP FUNCTION IF EXISTS public."getById_sp_all_BillingPayment"(character varying);

CREATE OR REPLACE FUNCTION public."getById_sp_all_BillingPayment"(
	pvar_billingpaymentid character varying)
    RETURNS TABLE(tenantid uuid, _tenantname character varying, "BillingPaymentid" uuid, paymentdate character varying, patientname character varying, patientvisit character varying, ipdnumber character varying, opdnumber character varying, receivablefor character varying, therapy character varying, therapycost numeric, therapykit character varying, kitprice character varying, medicine character varying, price numeric, room character varying, receivedamount numeric, currency character varying, conversionrate numeric, amount numeric, remarks character varying, paymentmode character varying, transactionreference character varying, bankname character varying, chequedddate character varying, paymentstatus character varying, collectedby character varying, refundmode character varying, refundedamount numeric, refundedby character varying, refundreferencenumber character varying, refundbankname character varying, refundreason character varying, refundstatus character varying, counterid character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone)
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN

RETURN QUERY
SELECT
    bp.tenantid,
    t.businessname,
    bp.billingpaymentid,
    COALESCE(to_char(bp.paymentdate,'dd/MM/yyyy'),'')::varchar,
    NULLIF(btrim(concat_ws(' ', pp.firstname, pp.lastname, pp.mobilenumber)), '')::varchar,
    NULLIF(btrim(concat_ws(' ', pv.visitnumber, pv.ipdnumber, pv.opdnumber)), '')::varchar,
    NULLIF(btrim(concat_ws(' ', ipd.firstname, ipd.lastname, ipd.bookingreferencenumber)), '')::varchar,
    opd.bookingreferencenumber::varchar,
    bp.receivablefor,
    th.therapyname,
    bp.therapycost,
    tk.therapykitname,
	bp.kitprice::varchar,
    md.medicinename,
    bp.price,
    rm.roomnumber::varchar,
    bp.receivedamount,
    bp.currency,
    bp.conversionrate,
    bp.amount,
    bp.remarks,
    bp.paymentmode,
    bp.transactionreference,
    bp.bankname,
    COALESCE(to_char(bp.chequedddate,'dd/MM/yyyy'),'')::varchar,
    bp.paymentstatus,
    NULLIF(btrim(concat_ws(' ', u1.firstname, u1.lastname)), '')::varchar,
    bp.refundmode,
    bp.refundedamount,
    NULLIF(btrim(concat_ws(' ', u2.firstname, u2.lastname)), '')::varchar,
    bp.refundreferencenumber,  -- ✅ matches VARCHAR
    bp.refundbankname,
    bp.refundreason,
    bp.refundstatus,
    bp.counterid,
    bp.createduser,
    bp.createddate,
    bp.modifieduser,
    bp.modifieddate

FROM billingpayment bp
LEFT JOIN tenant t ON bp.tenantid = t.tenantid
INNER JOIN patientprofile pp ON bp.patientname = pp.patientprofileid
LEFT JOIN patientvisit pv ON bp.patientvisit = pv.patientvisitid
LEFT JOIN ipdapplicationform ipd ON bp.ipdnumber = ipd.ipdapplicationformid
LEFT JOIN opdform opd ON bp.opdnumber = opd.opdformid
LEFT JOIN therapies th ON bp.therapy = th.therapiesid
LEFT JOIN therapykit tk ON bp.therapykit = tk.therapykitid
LEFT JOIN medicine md ON bp.medicine = md.medicineid
LEFT JOIN room rm ON bp.room = rm.roomid
LEFT JOIN users u1 ON bp.collectedby = u1.usersid
LEFT JOIN users u2 ON bp.refundedby = u2.usersid

WHERE bp.billingpaymentid::VARCHAR = pvar_BillingPaymentid;

END;
$BODY$;
