CREATE OR REPLACE FUNCTION "Discharge_List"
(
    pvar_tenantid varchar, pvar_ipdnumber varchar(1024), pvar_patient varchar(1024),
    pvar_room varchar(1024), pvar_discharge varchar(1024), pvar_paymentstatus varchar(1024),
    pvar_refundstatus varchar(1024), pvar_feedbackstatus varchar(1024), pvar_pagesize integer,
    pvar_pagenumber integer, pvar_searchterm varchar, pvar_sort_fields json
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
    IF pvar_tenantid IS NULL OR pvar_tenantid = '' OR pvar_tenantid = '00000000-0000-0000-0000-000000000000' THEN
        SELECT string_to_array(viewertenantids, ',') INTO lvar_tenantid
          FROM users WHERE usersid::varchar = lstr_usersid;
        IF lvar_tenantid IS NULL THEN SELECT array_agg(tenantid) INTO lvar_tenantid FROM tenant; END IF;
    ELSE
        lvar_tenantid := ARRAY[pvar_tenantid];
    END IF;
    lvar_tenantid := lvar_tenantid || ARRAY[''::varchar, '00000000-0000-0000-0000-000000000000'::varchar];
    pvar_searchterm := CASE WHEN length(COALESCE(pvar_searchterm, '')) > 0 THEN '%' || pvar_searchterm || '%' ELSE NULL END;
    SELECT col, dir INTO local_sortcolumn, local_sortorder
      FROM json_to_recordset(pvar_sort_fields) AS x(col text, dir text) LIMIT 1;

    RETURN (
        WITH discharge_rows AS (
            SELECT d.tenantid, t.businessname AS _tenantname, d.dischargeid, d.ipdnumber,
                   ipd.bookingreferencenumber::varchar AS ipdnumber_master, d.patient,
                   concat_ws(' ', pp.firstname, pp.lastname, pp.mobilenumber)::varchar AS patient_master,
                   d.room, room.roomnumber::varchar AS room_master, d.discharge,
                   ra.todate AS effective_discharge, to_char(ra.todate, 'DD/MM/YYYY')::varchar AS discharge_master,
                   d.daysofstay, d.pendingamount, pending.amount::varchar AS pendingamount_master,
                   d.paymentstatus, payment.paymentstatus::varchar AS paymentstatus_master,
                   d.refundamount, refund.refundedamount::varchar AS refundamount_master,
                   d.refundstatus, refund_status.refundstatus::varchar AS refundstatus_master,
                   d.feedbackstatus, d.createduser, d.createddate, d.modifieduser, d.modifieddate,
                   false AS issyntheticdischarge
              FROM discharge d
              LEFT JOIN tenant t ON d.tenantid = t.tenantid
              LEFT JOIN ipdapplicationform ipd ON d.ipdnumber = ipd.ipdapplicationformid
              LEFT JOIN patientprofile pp ON d.patient = pp.patientprofileid
              LEFT JOIN room ON d.room = room.roomid
              LEFT JOIN roomallocation ra ON d.discharge = ra.roomallocationid
              LEFT JOIN billingpayment pending ON d.pendingamount = pending.billingpaymentid
              LEFT JOIN billingpayment payment ON d.paymentstatus = payment.billingpaymentid
              LEFT JOIN billingpayment refund ON d.refundamount = refund.billingpaymentid
              LEFT JOIN billingpayment refund_status ON d.refundstatus = refund_status.billingpaymentid
             WHERE d.isdeleted = false

            UNION ALL

            SELECT ipd.tenantid, t.businessname, NULL::uuid, ipd.ipdapplicationformid,
                   ipd.bookingreferencenumber::varchar, ipd.patientname,
                   concat_ws(' ', pp.firstname, pp.lastname, pp.mobilenumber)::varchar,
                   stay.roomnumber, room.roomnumber::varchar, NULL::uuid,
                   CASE WHEN lower(trim(COALESCE(ipd.bookingstatus,'')))='discharged'
                        THEN COALESCE(historical_discharge.todate, ipd.bookingstatusdate, stay.todate) ELSE stay.todate END,
                   to_char(
                       CASE WHEN lower(trim(COALESCE(ipd.bookingstatus,'')))='discharged'
                            THEN COALESCE(historical_discharge.todate, ipd.bookingstatusdate, stay.todate)
                            ELSE stay.todate
                       END,
                       'DD/MM/YYYY'
                   )::varchar,
                   GREATEST((stay.todate::date - stay.fromdate::date) + 1, 1),
                   NULL::uuid, receivables.pendingamount::varchar,
                   NULL::uuid, receivables.paymentstatus::varchar,
                   NULL::uuid, NULL::varchar, NULL::uuid, NULL::varchar, NULL::varchar,
                   ipd.createduser, ipd.createddate, ipd.modifieduser, ipd.modifieddate, true
              FROM ipdapplicationform ipd
              LEFT JOIN tenant t ON ipd.tenantid = t.tenantid
              LEFT JOIN patientprofile pp ON ipd.patientname = pp.patientprofileid
              JOIN LATERAL (
                  SELECT candidate.roomnumber, candidate.fromdate, candidate.todate
                    FROM (
                        SELECT ir.roomnumber, ir.fromdate, ir.todate, 1 AS source_priority, ir.record_order
                          FROM ipdapplicationform_room ir
                         WHERE ir.ipdapplicationformid = ipd.ipdapplicationformid
                           AND ir.isdeleted = false
                           AND lower(trim(COALESCE(ir.allottedto, ''))) = 'patient'
                           AND ir.todate IS NOT NULL
                        UNION ALL
                        SELECT ra.room, ra.fromdate::timestamp, ra.todate::timestamp, 2, 0
                          FROM roomallocation ra
                         WHERE ra.ipdno = ipd.ipdapplicationformid
                           AND ra.isdeleted = false
                           AND ra.todate IS NOT NULL
                        UNION ALL
                        SELECT NULL::uuid, pd.dateofarrival::timestamp, pd.dateofdeparture::timestamp, 3, pd.record_order
                          FROM ipdapplicationform_preferreddatesofadmission pd
                         WHERE pd.ipdapplicationformid = ipd.ipdapplicationformid
                           AND pd.isdeleted = false
                    ) candidate
                   ORDER BY candidate.source_priority, candidate.todate DESC, candidate.record_order DESC
                   LIMIT 1
              ) stay ON true
              LEFT JOIN LATERAL (
                  SELECT MAX(ra.todate)::timestamp AS todate
                    FROM roomallocation ra
                   WHERE ra.ipdno=ipd.ipdapplicationformid AND ra.todate IS NOT NULL
              ) historical_discharge ON true
              LEFT JOIN room ON stay.roomnumber = room.roomid
              LEFT JOIN LATERAL (
                  SELECT COALESCE(SUM(GREATEST(COALESCE(r.amount,0)-COALESCE(r.paidamount,0),0)),0) AS pendingamount,
                         CASE WHEN COUNT(*) > 0 AND SUM(GREATEST(COALESCE(r.amount,0)-COALESCE(r.paidamount,0),0)) <= 0 THEN 'Paid'
                              WHEN SUM(COALESCE(r.paidamount,0)) > 0 THEN 'Partially Paid' ELSE 'Pending' END AS paymentstatus
                    FROM receivable r
                   WHERE r.ipdnumber = ipd.ipdapplicationformid AND COALESCE(r.isdeleted,false)=false
                     AND COALESCE(r.remarks,'') NOT ILIKE '%inactive%'
              ) receivables ON true
             WHERE ipd.isdeleted = false
               AND lower(trim(COALESCE(ipd.bookingstatus,''))) IN ('admitted','discharged')
               AND NOT EXISTS (SELECT 1 FROM discharge d WHERE d.ipdnumber=ipd.ipdapplicationformid AND d.isdeleted=false)
        ), filtered AS (
            SELECT * FROM discharge_rows x
             WHERE (lvar_tenantid IS NULL OR COALESCE(x.tenantid::varchar,'')=ANY(lvar_tenantid))
               AND (COALESCE(pvar_ipdnumber,'') IN ('','0') OR x.ipdnumber::varchar=pvar_ipdnumber)
               AND (COALESCE(pvar_patient,'') IN ('','0') OR x.patient::varchar=pvar_patient)
               AND (COALESCE(pvar_room,'') IN ('','0') OR x.room::varchar=pvar_room)
               AND (COALESCE(trim(pvar_discharge),'') IN ('','0')
                    OR x.effective_discharge::date=to_date(left(trim(pvar_discharge),10),'DD/MM/YYYY')
                    OR EXISTS (
                        SELECT 1 FROM ipdapplicationform status_ipd
                         WHERE status_ipd.ipdapplicationformid=x.ipdnumber
                           AND lower(trim(COALESCE(status_ipd.bookingstatus,'')))='discharged'
                           AND status_ipd.bookingstatusdate::date=to_date(left(trim(pvar_discharge),10),'DD/MM/YYYY')
                    )
                    OR EXISTS (
                        SELECT 1 FROM ipdapplicationform_room ir
                         WHERE ir.ipdapplicationformid=x.ipdnumber AND ir.isdeleted=false
                           AND ir.todate::date=to_date(left(trim(pvar_discharge),10),'DD/MM/YYYY')
                    )
                    OR EXISTS (
                        SELECT 1 FROM roomallocation ra
                         WHERE ra.ipdno=x.ipdnumber
                           AND (ra.isdeleted=false OR EXISTS (
                               SELECT 1 FROM ipdapplicationform discharged_ipd
                                WHERE discharged_ipd.ipdapplicationformid=x.ipdnumber
                                  AND lower(trim(COALESCE(discharged_ipd.bookingstatus,'')))='discharged'))
                           AND ra.todate::date=to_date(left(trim(pvar_discharge),10),'DD/MM/YYYY')
                    )
                    OR EXISTS (
                        SELECT 1 FROM ipdapplicationform_preferreddatesofadmission pd
                         WHERE pd.ipdapplicationformid=x.ipdnumber AND pd.isdeleted=false
                           AND pd.dateofdeparture=to_date(left(trim(pvar_discharge),10),'DD/MM/YYYY')
                    ))
               AND (COALESCE(pvar_paymentstatus,'') IN ('','0') OR x.paymentstatus::varchar=pvar_paymentstatus OR lower(x.paymentstatus_master)=lower(pvar_paymentstatus))
               AND (COALESCE(pvar_refundstatus,'') IN ('','0') OR x.refundstatus::varchar=pvar_refundstatus OR lower(x.refundstatus_master)=lower(pvar_refundstatus))
               AND (COALESCE(pvar_feedbackstatus,'') IN ('','0') OR x.feedbackstatus=pvar_feedbackstatus)
               AND (pvar_searchterm IS NULL OR COALESCE(x._tenantname,'') ILIKE pvar_searchterm OR COALESCE(x.ipdnumber_master,'') ILIKE pvar_searchterm
                    OR COALESCE(x.patient_master,'') ILIKE pvar_searchterm OR COALESCE(x.room_master,'') ILIKE pvar_searchterm
                    OR COALESCE(x.discharge_master,'') ILIKE pvar_searchterm OR COALESCE(x.paymentstatus_master,'') ILIKE pvar_searchterm)
        )
        SELECT json_build_object(
            'count',(SELECT count(*) FROM filtered),
            'summary',(
                SELECT json_build_object(
                    'totalDischarges', count(*),
                    'pendingAmount', COALESCE(sum(
                        COALESCE(NULLIF(regexp_replace(COALESCE(pendingamount_master,'0'),'[^0-9.-]','','g'),'')::numeric,0)
                    ),0),
                    'pendingRefundAmount', COALESCE(sum(
                        CASE
                            WHEN COALESCE(NULLIF(regexp_replace(COALESCE(refundamount_master,'0'),'[^0-9.-]','','g'),'')::numeric,0) > 0
                             AND lower(trim(COALESCE(refundstatus_master,''))) NOT IN ('refunded','completed','processed','success','successful')
                            THEN COALESCE(NULLIF(regexp_replace(COALESCE(refundamount_master,'0'),'[^0-9.-]','','g'),'')::numeric,0)
                            ELSE 0
                        END
                    ),0),
                    'feedbackPending', count(*) FILTER (
                        WHERE lower(trim(COALESCE(feedbackstatus,''))) NOT IN ('completed','submitted','received')
                    )
                ) FROM filtered
            ),
            'detail',(
            SELECT json_agg(row_to_json(q)) FROM (
                SELECT * FROM filtered ORDER BY
                    CASE WHEN local_sortorder='asc' THEN CASE local_sortcolumn WHEN 'ipdnumber' THEN ipdnumber_master WHEN 'patient' THEN patient_master WHEN 'room' THEN room_master WHEN 'discharge' THEN effective_discharge::text WHEN 'paymentstatus' THEN paymentstatus_master END END ASC,
                    CASE WHEN local_sortorder='desc' THEN CASE local_sortcolumn WHEN 'ipdnumber' THEN ipdnumber_master WHEN 'patient' THEN patient_master WHEN 'room' THEN room_master WHEN 'discharge' THEN effective_discharge::text WHEN 'paymentstatus' THEN paymentstatus_master END END DESC,
                    effective_discharge ASC NULLS LAST, createddate DESC
                LIMIT pvar_pagesize OFFSET pvar_pagenumber*pvar_pagesize
            ) q
        ))
    );
END $BODY$ LANGUAGE plpgsql;
