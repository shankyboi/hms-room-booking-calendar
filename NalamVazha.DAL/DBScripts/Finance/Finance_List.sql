CREATE OR REPLACE FUNCTION "Finance_List"
(
    pvar_tenantid varchar, pvar_paymentdate varchar(1024), pvar_paymentmode varchar(1024),
    pvar_patient varchar(1024), pvar_bookingreferencenumber varchar(1024),
    pvar_paymentstatus varchar(1024), pvar_pagesize integer, pvar_pagenumber integer,
    pvar_searchterm varchar, pvar_sort_fields json
)
RETURNS json AS $BODY$
DECLARE
    local_sortcolumn text;
    local_sortorder text;
    lvar_tenantid varchar[];
    lstr_usersid varchar;
BEGIN
    SELECT split_part(pvar_tenantid, '|', 1), split_part(pvar_tenantid, '|', 2)
      INTO lstr_usersid, pvar_tenantid;

    IF pvar_tenantid IS NULL OR pvar_tenantid = ''
       OR pvar_tenantid = '00000000-0000-0000-0000-000000000000' THEN
        SELECT string_to_array(viewertenantids, ',') INTO lvar_tenantid
          FROM users WHERE usersid::varchar = lstr_usersid;
        IF lvar_tenantid IS NULL THEN
            SELECT array_agg(tenantid) INTO lvar_tenantid FROM tenant;
        END IF;
    ELSE
        lvar_tenantid := ARRAY[pvar_tenantid];
    END IF;

    lvar_tenantid := lvar_tenantid || ARRAY[''::varchar, '00000000-0000-0000-0000-000000000000'::varchar];
    pvar_searchterm := CASE WHEN length(COALESCE(pvar_searchterm, '')) > 0 THEN '%' || pvar_searchterm || '%' ELSE NULL END;

    SELECT col, dir INTO local_sortcolumn, local_sortorder
      FROM json_to_recordset(COALESCE(pvar_sort_fields, '[]'::json)) AS x(col text, dir text)
     LIMIT 1;

    RETURN (
        WITH payment_rows AS (
            SELECT bp.tenantid, t.businessname AS _tenantname,
                   bp.billingpaymentid AS financeid,
                   bp.billingpaymentid AS paymentdate,
                   to_char(COALESCE(NULLIF(bp.paymentdate, '0001-01-01'::date), bp.createddate::date), 'DD/MM/YYYY')::varchar AS paymentdate_master,
                   bp.billingpaymentid AS paymentmode, bp.paymentmode::varchar AS paymentmode_master,
                   bp.billingpaymentid AS receiptnumber, bp.receiptno::varchar AS receiptnumber_master,
                   bp.patientname AS patient,
                   NULLIF(btrim(concat_ws(' ', NULLIF(pp.firstname, ''), NULLIF(pp.lastname, ''), NULLIF(pp.mobilenumber, ''))), '')::varchar AS patient_master,
                   bp.billingpaymentid AS receivablefor, bp.receivablefor::varchar AS receivablefor_master,
                   bp.billingpaymentid AS bookingreferencenumber,
                   COALESCE(ipd.bookingreferencenumber, opd.bookingreferencenumber)::varchar AS bookingreferencenumber_master,
                   bp.billingpaymentid AS billedamount,
                   COALESCE(receivable_totals.billedamount, bp.amount, 0)::varchar AS billedamount_master,
                   bp.billingpaymentid AS receivedamount, COALESCE(bp.receivedamount, 0)::varchar AS receivedamount_master,
                   COALESCE(
                       receivable_totals.pendingamount,
                       GREATEST(COALESCE(bp.amount, 0) - COALESCE(bp.receivedamount, 0), 0)
                   ) AS pendingamount,
                   bp.billingpaymentid AS paymentstatus, bp.paymentstatus::varchar AS paymentstatus_master,
                   bp.billingpaymentid AS collectedby,
                   NULLIF(btrim(concat_ws(' ', NULLIF(cu.firstname, ''), NULLIF(cu.lastname, ''))), '')::varchar AS collectedby_master,
                   bp.billingpaymentid AS refundmode, bp.refundmode::varchar AS refundmode_master,
                   bp.billingpaymentid AS refundedamount, COALESCE(bp.refundedamount, 0)::varchar AS refundedamount_master,
                   bp.billingpaymentid AS refundedby,
                   NULLIF(btrim(concat_ws(' ', NULLIF(ru.firstname, ''), NULLIF(ru.lastname, ''))), '')::varchar AS refundedby_master,
                   bp.billingpaymentid AS remarks, bp.remarks::varchar AS remarks_master,
                   COALESCE(NULLIF(bp.paymentdate, '0001-01-01'::date), bp.createddate::date) AS effective_paymentdate,
                   bp.ipdnumber, bp.opdnumber, bp.createduser, bp.createddate, bp.modifieduser, bp.modifieddate
              FROM billingpayment bp
              LEFT JOIN tenant t ON bp.tenantid = t.tenantid
              LEFT JOIN patientprofile pp ON bp.patientname = pp.patientprofileid
              LEFT JOIN ipdapplicationform ipd ON bp.ipdnumber = ipd.ipdapplicationformid
              LEFT JOIN opdform opd ON bp.opdnumber = opd.opdformid
              LEFT JOIN users cu ON bp.collectedby = cu.usersid
              LEFT JOIN users ru ON bp.refundedby = ru.usersid
              LEFT JOIN LATERAL (
                  SELECT SUM(COALESCE(r.amount, 0)) AS billedamount,
                         SUM(GREATEST(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0), 0)) AS pendingamount
                    FROM receivable r
                   WHERE COALESCE(r.isdeleted, false) = false
                     AND (
                         (bp.ipdnumber IS NOT NULL AND r.ipdnumber = bp.ipdnumber)
                         OR (bp.opdnumber IS NOT NULL AND r.opdnumber = bp.opdnumber)
                         OR (bp.ipdnumber IS NULL AND bp.opdnumber IS NULL
                             AND r.billingpaymentid = bp.billingpaymentid)
                     )
              ) receivable_totals ON true
             WHERE COALESCE(bp.isdeleted, false) = false
        ), filtered AS (
            SELECT * FROM payment_rows x
             WHERE (lvar_tenantid IS NULL OR COALESCE(x.tenantid::varchar, '') = ANY(lvar_tenantid))
               AND (COALESCE(trim(pvar_paymentdate), '') IN ('', '0')
                    OR x.effective_paymentdate = to_date(left(trim(pvar_paymentdate), 10), 'DD/MM/YYYY'))
               AND (COALESCE(pvar_paymentmode, '') IN ('', '0')
                    OR x.paymentmode::varchar = pvar_paymentmode OR lower(x.paymentmode_master) = lower(pvar_paymentmode))
               AND (COALESCE(pvar_patient, '') IN ('', '0') OR x.patient::varchar = pvar_patient)
               AND (COALESCE(pvar_bookingreferencenumber, '') IN ('', '0')
                    OR x.bookingreferencenumber::varchar = pvar_bookingreferencenumber
                    OR x.ipdnumber::varchar = pvar_bookingreferencenumber
                    OR x.opdnumber::varchar = pvar_bookingreferencenumber
                    OR x.bookingreferencenumber_master = pvar_bookingreferencenumber)
               AND (COALESCE(pvar_paymentstatus, '') IN ('', '0')
                    OR x.paymentstatus::varchar = pvar_paymentstatus OR lower(x.paymentstatus_master) = lower(pvar_paymentstatus))
               AND (pvar_searchterm IS NULL
                    OR COALESCE(x._tenantname, '') ILIKE pvar_searchterm
                    OR COALESCE(x.paymentdate_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.paymentmode_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.receiptnumber_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.patient_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.receivablefor_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.bookingreferencenumber_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.paymentstatus_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.remarks_master, '') ILIKE pvar_searchterm)
        )
        SELECT json_build_object(
            'count', (SELECT count(*) FROM filtered),
            'detail', (
                SELECT json_agg(row_to_json(q)) FROM (
                    SELECT * FROM filtered
                     ORDER BY
                        CASE WHEN local_sortorder = 'asc' THEN CASE local_sortcolumn
                            WHEN 'paymentdate' THEN effective_paymentdate::text WHEN 'paymentmode' THEN paymentmode_master
                            WHEN 'receiptnumber' THEN receiptnumber_master WHEN 'patient' THEN patient_master
                            WHEN 'receivablefor' THEN receivablefor_master WHEN 'bookingreferencenumber' THEN bookingreferencenumber_master
                            WHEN 'paymentstatus' THEN paymentstatus_master WHEN 'collectedby' THEN collectedby_master
                            WHEN 'refundmode' THEN refundmode_master WHEN 'refundedby' THEN refundedby_master WHEN 'remarks' THEN remarks_master END END ASC,
                        CASE WHEN local_sortorder = 'desc' THEN CASE local_sortcolumn
                            WHEN 'paymentdate' THEN effective_paymentdate::text WHEN 'paymentmode' THEN paymentmode_master
                            WHEN 'receiptnumber' THEN receiptnumber_master WHEN 'patient' THEN patient_master
                            WHEN 'receivablefor' THEN receivablefor_master WHEN 'bookingreferencenumber' THEN bookingreferencenumber_master
                            WHEN 'paymentstatus' THEN paymentstatus_master WHEN 'collectedby' THEN collectedby_master
                            WHEN 'refundmode' THEN refundmode_master WHEN 'refundedby' THEN refundedby_master WHEN 'remarks' THEN remarks_master END END DESC,
                        effective_paymentdate DESC, createddate DESC
                     LIMIT COALESCE(pvar_pagesize, 50) OFFSET COALESCE(pvar_pagenumber, 0) * COALESCE(pvar_pagesize, 50)
                ) q
            )
        )
    );
END
$BODY$ LANGUAGE plpgsql;
