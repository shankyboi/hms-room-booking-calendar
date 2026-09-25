CREATE OR REPLACE FUNCTION "Billing_Payment_List"
(
    pvar_tenantid varchar,
    pvar_paymentdate_automatonfrom varchar,
    pvar_paymentdate_automatonto varchar,
    pvar_patientname varchar,
    pvar_receivablefor varchar,
    pvar_pagesize integer,
    pvar_pagenumber integer,
    pvar_searchterm varchar,
    pvar_sort_fields json
)
RETURNS json
AS $BODY$
DECLARE
    local_sortcolumn_array text[] = (
        SELECT array_agg(col) FROM json_to_recordset(COALESCE(pvar_sort_fields, '[]'::json)) AS x(col text, dir text)
    );
    local_sortorder_array text[] = (
        SELECT array_agg(dir) FROM json_to_recordset(COALESCE(pvar_sort_fields, '[]'::json)) AS x(col text, dir text)
    );
    lvar_tenantid varchar[];
    lstr_usersid varchar;
    lvar_paymentdate_from date;
    lvar_paymentdate_to date;
BEGIN
    SELECT SPLIT_PART(pvar_tenantid, '|', 1), SPLIT_PART(pvar_tenantid, '|', 2)
    INTO lstr_usersid, pvar_tenantid;

    IF (pvar_tenantid IS NULL OR pvar_tenantid = '' OR pvar_tenantid = '00000000-0000-0000-0000-000000000000') THEN
        SELECT STRING_TO_ARRAY(viewertenantids, ',')
        INTO lvar_tenantid
        FROM users
        WHERE users.usersid::varchar = lstr_usersid;

        IF (lvar_tenantid IS NULL) THEN
            SELECT array_agg(tenant.tenantid) INTO lvar_tenantid FROM tenant;
        END IF;
    ELSE
        lvar_tenantid = ARRAY[pvar_tenantid];
    END IF;

    lvar_tenantid := lvar_tenantid
        || ARRAY[''::character varying]
        || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];

    lvar_paymentdate_from := NULLIF(pvar_paymentdate_automatonfrom, '')::date;
    lvar_paymentdate_to := NULLIF(pvar_paymentdate_automatonto, '')::date;

    IF (pvar_searchterm IS NOT NULL AND LENGTH(CAST(pvar_searchterm AS varchar)) > 0) THEN
        pvar_searchterm := '%' || pvar_searchterm || '%';
    ELSE
        pvar_searchterm := NULL;
    END IF;

    RETURN json_build_object(
        'count',
        (
            SELECT COUNT(*)
            FROM BillingPayment
            LEFT OUTER JOIN tenant ON BillingPayment.tenantid = tenant.tenantid
            LEFT OUTER JOIN PatientVisit _PatientVisit ON BillingPayment.patientvisit = _PatientVisit.PatientVisitid
            LEFT OUTER JOIN PatientProfile __PatientProfile ON BillingPayment.patientname = __PatientProfile.PatientProfileid
            LEFT OUTER JOIN IPDApplicationForm ___IPDApplicationForm ON BillingPayment.ipdnumber = ___IPDApplicationForm.IPDApplicationFormid
            LEFT OUTER JOIN OPDForm ____OPDForm ON BillingPayment.opdnumber = ____OPDForm.OPDFormid
            LEFT OUTER JOIN Therapies _____Therapies ON BillingPayment.therapy = _____Therapies.Therapiesid
            LEFT OUTER JOIN TherapyKit ______TherapyKit ON BillingPayment.therapykit = ______TherapyKit.TherapyKitid
            LEFT OUTER JOIN Medicine _______Medicine ON BillingPayment.medicine = _______Medicine.Medicineid
            LEFT OUTER JOIN Room ________Room ON BillingPayment.room = ________Room.Roomid
            LEFT OUTER JOIN users _________collectedby ON BillingPayment.collectedby = _________collectedby.usersid
            LEFT OUTER JOIN users __________refundedby ON BillingPayment.refundedby = __________refundedby.usersid
            WHERE (lvar_tenantid IS NULL OR COALESCE(CAST(BillingPayment.tenantid AS varchar), '') = ANY(lvar_tenantid))
              AND BillingPayment.isdeleted = false
              AND (lvar_paymentdate_from IS NULL OR COALESCE(NULLIF(BillingPayment.paymentdate, '0001-01-01'::date), BillingPayment.createddate::date) >= lvar_paymentdate_from)
              AND (lvar_paymentdate_to IS NULL OR COALESCE(NULLIF(BillingPayment.paymentdate, '0001-01-01'::date), BillingPayment.createddate::date) <= lvar_paymentdate_to)
              AND (pvar_patientname IS NULL OR pvar_patientname = '' OR CAST(BillingPayment.patientname AS varchar) = pvar_patientname)
              AND (pvar_receivablefor IS NULL OR pvar_receivablefor = '' OR BillingPayment.receivablefor = pvar_receivablefor)
              AND (
                    pvar_searchterm IS NULL
                    OR COALESCE(tenant.businessname::varchar, '') ILIKE pvar_searchterm
                    OR CAST(BillingPayment.paymentdate AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.receiptno AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.receivablefor AS varchar) ILIKE pvar_searchterm
                    OR CAST(_PatientVisit.visitnumber AS varchar) ILIKE pvar_searchterm
                    OR CAST(___IPDApplicationForm.bookingreferencenumber AS varchar) ILIKE pvar_searchterm
                    OR CAST(____OPDForm.bookingreferencenumber AS varchar) ILIKE pvar_searchterm
                    OR CAST(NULLIF(btrim(concat_ws(' ', NULLIF(__PatientProfile.firstname, ''), NULLIF(__PatientProfile.lastname, ''), NULLIF(__PatientProfile.mobilenumber, ''))), '') AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.amount AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.paymentmode AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.transactionreference AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.paymentstatus AS varchar) ILIKE pvar_searchterm
                    OR CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_________collectedby.firstname, ''), NULLIF(_________collectedby.lastname, ''))), '') AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.refundmode AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.refundedamount AS varchar) ILIKE pvar_searchterm
                    OR CAST(NULLIF(btrim(concat_ws(' ', NULLIF(__________refundedby.firstname, ''), NULLIF(__________refundedby.lastname, ''))), '') AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.refundreferencenumber AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.refundbankname AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.refundreason AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.refundstatus AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.counterid AS varchar) ILIKE pvar_searchterm
                    OR CAST(BillingPayment.remarks AS varchar) ILIKE pvar_searchterm
              )
        ),
        'detail',
        (
            SELECT json_agg(row_to_json(d))
            FROM (
                SELECT
                    BillingPayment.tenantid,
                    tenant.businessname AS _tenantName,
                    BillingPayment.BillingPaymentid,
                    to_char(COALESCE(NULLIF(BillingPayment.paymentdate, '0001-01-01'::date), BillingPayment.createddate::date), 'DD/MM/YYYY')::varchar AS paymentdate,
                    BillingPayment.receiptno,
                    BillingPayment.patientvisit,
                    CAST(_PatientVisit.visitnumber AS varchar) AS patientvisit_master,
                    BillingPayment.patientname,
                    CAST(NULLIF(btrim(concat_ws(' ', NULLIF(__PatientProfile.firstname, ''), NULLIF(__PatientProfile.lastname, ''), NULLIF(__PatientProfile.mobilenumber, ''))), '') AS varchar) AS patientname_master,
                    BillingPayment.ipdnumber,
                    CAST(___IPDApplicationForm.bookingreferencenumber AS varchar) AS ipdnumber_master,
                    BillingPayment.opdnumber,
                    CAST(____OPDForm.bookingreferencenumber AS varchar) AS opdnumber_master,
                    BillingPayment.receivablefor,
                    BillingPayment.therapy,
                    CAST(_____Therapies.therapyname AS varchar) AS therapy_master,
                    BillingPayment.therapycost,
                    BillingPayment.therapykit,
                    CAST(______TherapyKit.therapykitname AS varchar) AS therapykit_master,
                    BillingPayment.kitprice,
                    BillingPayment.medicine,
                    CAST(_______Medicine.medicinename AS varchar) AS medicine_master,
                    BillingPayment.price,
                    BillingPayment.room,
                    CAST(________Room.roomnumber AS varchar) AS room_master,
                    BillingPayment.receivedamount,
                    BillingPayment.currency,
                    BillingPayment.conversionrate,
                    BillingPayment.amount,
                    BillingPayment.remarks,
                    BillingPayment.paymentmode,
                    BillingPayment.transactionreference,
                    BillingPayment.bankname,
                    BillingPayment.chequedddate,
                    BillingPayment.paymentstatus,
                    BillingPayment.collectedby,
                    CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_________collectedby.firstname, ''), NULLIF(_________collectedby.lastname, ''))), '') AS varchar) AS collectedby_master,
                    BillingPayment.refundmode,
                    BillingPayment.refundedamount,
                    BillingPayment.refundedby,
                    CAST(NULLIF(btrim(concat_ws(' ', NULLIF(__________refundedby.firstname, ''), NULLIF(__________refundedby.lastname, ''))), '') AS varchar) AS refundedby_master,
                    BillingPayment.refundreferencenumber,
                    BillingPayment.refundbankname,
                    BillingPayment.refundreason,
                    BillingPayment.refundstatus,
                    BillingPayment.counterid,
                    BillingPayment.createduser,
                    BillingPayment.createddate,
                    BillingPayment.modifieduser,
                    BillingPayment.modifieddate
                FROM BillingPayment
                LEFT OUTER JOIN tenant ON BillingPayment.tenantid = tenant.tenantid
                LEFT OUTER JOIN PatientVisit _PatientVisit ON BillingPayment.patientvisit = _PatientVisit.PatientVisitid
                LEFT OUTER JOIN PatientProfile __PatientProfile ON BillingPayment.patientname = __PatientProfile.PatientProfileid
                LEFT OUTER JOIN IPDApplicationForm ___IPDApplicationForm ON BillingPayment.ipdnumber = ___IPDApplicationForm.IPDApplicationFormid
                LEFT OUTER JOIN OPDForm ____OPDForm ON BillingPayment.opdnumber = ____OPDForm.OPDFormid
                LEFT OUTER JOIN Therapies _____Therapies ON BillingPayment.therapy = _____Therapies.Therapiesid
                LEFT OUTER JOIN TherapyKit ______TherapyKit ON BillingPayment.therapykit = ______TherapyKit.TherapyKitid
                LEFT OUTER JOIN Medicine _______Medicine ON BillingPayment.medicine = _______Medicine.Medicineid
                LEFT OUTER JOIN Room ________Room ON BillingPayment.room = ________Room.Roomid
                LEFT OUTER JOIN users _________collectedby ON BillingPayment.collectedby = _________collectedby.usersid
                LEFT OUTER JOIN users __________refundedby ON BillingPayment.refundedby = __________refundedby.usersid
                WHERE (lvar_tenantid IS NULL OR COALESCE(CAST(BillingPayment.tenantid AS varchar), '') = ANY(lvar_tenantid))
                  AND BillingPayment.isdeleted = false
                  AND (lvar_paymentdate_from IS NULL OR COALESCE(NULLIF(BillingPayment.paymentdate, '0001-01-01'::date), BillingPayment.createddate::date) >= lvar_paymentdate_from)
                  AND (lvar_paymentdate_to IS NULL OR COALESCE(NULLIF(BillingPayment.paymentdate, '0001-01-01'::date), BillingPayment.createddate::date) <= lvar_paymentdate_to)
                  AND (pvar_patientname IS NULL OR pvar_patientname = '' OR CAST(BillingPayment.patientname AS varchar) = pvar_patientname)
                  AND (pvar_receivablefor IS NULL OR pvar_receivablefor = '' OR BillingPayment.receivablefor = pvar_receivablefor)
                  AND (
                        pvar_searchterm IS NULL
                        OR COALESCE(tenant.businessname::varchar, '') ILIKE pvar_searchterm
                        OR CAST(BillingPayment.paymentdate AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.receiptno AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.receivablefor AS varchar) ILIKE pvar_searchterm
                        OR CAST(_PatientVisit.visitnumber AS varchar) ILIKE pvar_searchterm
                        OR CAST(___IPDApplicationForm.bookingreferencenumber AS varchar) ILIKE pvar_searchterm
                        OR CAST(____OPDForm.bookingreferencenumber AS varchar) ILIKE pvar_searchterm
                        OR CAST(NULLIF(btrim(concat_ws(' ', NULLIF(__PatientProfile.firstname, ''), NULLIF(__PatientProfile.lastname, ''), NULLIF(__PatientProfile.mobilenumber, ''))), '') AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.amount AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.paymentmode AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.transactionreference AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.paymentstatus AS varchar) ILIKE pvar_searchterm
                        OR CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_________collectedby.firstname, ''), NULLIF(_________collectedby.lastname, ''))), '') AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.refundmode AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.refundedamount AS varchar) ILIKE pvar_searchterm
                        OR CAST(NULLIF(btrim(concat_ws(' ', NULLIF(__________refundedby.firstname, ''), NULLIF(__________refundedby.lastname, ''))), '') AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.refundreferencenumber AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.refundbankname AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.refundreason AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.refundstatus AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.counterid AS varchar) ILIKE pvar_searchterm
                        OR CAST(BillingPayment.remarks AS varchar) ILIKE pvar_searchterm
                  )
                ORDER BY
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                        CASE local_sortcolumn_array[1]
                            WHEN 'paymentdate' THEN COALESCE(NULLIF(BillingPayment.paymentdate, '0001-01-01'::date), BillingPayment.createddate::date)::text
                            WHEN 'receiptno' THEN BillingPayment.receiptno::text
                            WHEN 'receivablefor' THEN BillingPayment.receivablefor::text
                            WHEN 'patientvisit' THEN _PatientVisit.visitnumber::text
                            WHEN 'patientname' THEN NULLIF(btrim(concat_ws(' ', NULLIF(__PatientProfile.firstname, ''), NULLIF(__PatientProfile.lastname, ''), NULLIF(__PatientProfile.mobilenumber, ''))), '')::text
                            WHEN 'ipdnumber' THEN ___IPDApplicationForm.bookingreferencenumber::text
                            WHEN 'opdnumber' THEN ____OPDForm.bookingreferencenumber::text
                            WHEN 'paymentmode' THEN BillingPayment.paymentmode::text
                            WHEN 'transactionreference' THEN BillingPayment.transactionreference::text
                            WHEN 'paymentstatus' THEN BillingPayment.paymentstatus::text
                            WHEN 'collectedby' THEN NULLIF(btrim(concat_ws(' ', NULLIF(_________collectedby.firstname, ''), NULLIF(_________collectedby.lastname, ''))), '')::text
                            WHEN 'refundmode' THEN BillingPayment.refundmode::text
                            WHEN 'refundedby' THEN NULLIF(btrim(concat_ws(' ', NULLIF(__________refundedby.firstname, ''), NULLIF(__________refundedby.lastname, ''))), '')::text
                            WHEN 'refundstatus' THEN BillingPayment.refundstatus::text
                            WHEN 'counterid' THEN BillingPayment.counterid::text
                            WHEN 'remarks' THEN BillingPayment.remarks::text
                            ELSE NULL
                        END
                    ELSE NULL END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                        CASE local_sortcolumn_array[1]
                            WHEN 'amount' THEN BillingPayment.amount::numeric
                            WHEN 'refundedamount' THEN BillingPayment.refundedamount::numeric
                            ELSE NULL
                        END
                    ELSE NULL END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                        CASE local_sortcolumn_array[1]
                            WHEN 'paymentdate' THEN COALESCE(NULLIF(BillingPayment.paymentdate, '0001-01-01'::date), BillingPayment.createddate::date)::text
                            WHEN 'receiptno' THEN BillingPayment.receiptno::text
                            WHEN 'receivablefor' THEN BillingPayment.receivablefor::text
                            WHEN 'patientvisit' THEN _PatientVisit.visitnumber::text
                            WHEN 'patientname' THEN NULLIF(btrim(concat_ws(' ', NULLIF(__PatientProfile.firstname, ''), NULLIF(__PatientProfile.lastname, ''), NULLIF(__PatientProfile.mobilenumber, ''))), '')::text
                            WHEN 'ipdnumber' THEN ___IPDApplicationForm.bookingreferencenumber::text
                            WHEN 'opdnumber' THEN ____OPDForm.bookingreferencenumber::text
                            WHEN 'paymentmode' THEN BillingPayment.paymentmode::text
                            WHEN 'transactionreference' THEN BillingPayment.transactionreference::text
                            WHEN 'paymentstatus' THEN BillingPayment.paymentstatus::text
                            WHEN 'collectedby' THEN NULLIF(btrim(concat_ws(' ', NULLIF(_________collectedby.firstname, ''), NULLIF(_________collectedby.lastname, ''))), '')::text
                            WHEN 'refundmode' THEN BillingPayment.refundmode::text
                            WHEN 'refundedby' THEN NULLIF(btrim(concat_ws(' ', NULLIF(__________refundedby.firstname, ''), NULLIF(__________refundedby.lastname, ''))), '')::text
                            WHEN 'refundstatus' THEN BillingPayment.refundstatus::text
                            WHEN 'counterid' THEN BillingPayment.counterid::text
                            WHEN 'remarks' THEN BillingPayment.remarks::text
                            ELSE NULL
                        END
                    ELSE NULL END DESC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                        CASE local_sortcolumn_array[1]
                            WHEN 'amount' THEN BillingPayment.amount::numeric
                            WHEN 'refundedamount' THEN BillingPayment.refundedamount::numeric
                            ELSE NULL
                        END
                    ELSE NULL END DESC,
                    BillingPayment.createddate DESC
                LIMIT COALESCE(pvar_pagesize, 1000)
                OFFSET COALESCE(pvar_pagenumber, 0) * COALESCE(pvar_pagesize, 1000)
            ) d
        )
    );
END
$BODY$
LANGUAGE plpgsql;
