CREATE OR REPLACE FUNCTION "Arrival_List"
(
    pvar_tenantid varchar,
    pvar_ipdnumber varchar(1024),
    pvar_patient varchar(1024),
    pvar_room varchar(1024),
    pvar_estimatedarrival varchar(1024),
    pvar_pagesize integer,
    pvar_pagenumber integer,
    pvar_searchterm varchar,
    pvar_sort_fields json
)
RETURNS json
AS $BODY$
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
        SELECT string_to_array(viewertenantids, ',')
          INTO lvar_tenantid
          FROM users
         WHERE usersid::varchar = lstr_usersid;

        IF lvar_tenantid IS NULL THEN
            SELECT array_agg(tenantid) INTO lvar_tenantid FROM tenant;
        END IF;
    ELSE
        lvar_tenantid := ARRAY[pvar_tenantid];
    END IF;

    lvar_tenantid := lvar_tenantid
        || ARRAY[''::varchar]
        || ARRAY['00000000-0000-0000-0000-000000000000'::varchar];

    IF pvar_searchterm IS NOT NULL AND length(pvar_searchterm) > 0 THEN
        pvar_searchterm := '%' || pvar_searchterm || '%';
    ELSE
        pvar_searchterm := NULL;
    END IF;

    SELECT col, dir
      INTO local_sortcolumn, local_sortorder
      FROM json_to_recordset(pvar_sort_fields) AS x(col text, dir text)
     LIMIT 1;

    RETURN (
        WITH arrival_rows AS
        (
            /* Normal arrival records. */
            SELECT
                a.tenantid,
                t.businessname AS _tenantname,
                a.arrivalid,
                a.ipdnumber,
                ipd.bookingreferencenumber::varchar AS ipdnumber_master,
                a.patient,
                (pp.firstname || ' ' || pp.lastname || ' ' || pp.mobilenumber)::varchar AS patient_master,
                a.room,
                r.roomnumber::varchar AS room_master,
                a.estimatedarrival,
                COALESCE(estimated_ipd.estimatedarrival, ipd.estimatedarrival, normal_dates.dateofarrival, normal_dates.fromdate) AS effective_arrival,
                COALESCE(estimated_ipd.estimatedarrival, ipd.estimatedarrival, normal_dates.dateofarrival, normal_dates.fromdate)::varchar AS estimatedarrival_master,
                a.bookingstatus,
                status_ipd.bookingstatus::varchar AS bookingstatus_master,
                a.travelarrangement,
                (travel_ipd.travelarrangement || ' ' || travel_ipd.typeoftravelrequired)::varchar AS travelarrangement_master,
                a.pickupfrom,
                pickup_ipd.pickupfrom::varchar AS pickupfrom_master,
                a.wheelchairassistance,
                wheelchair_ipd.wheelchairassistance::varchar AS wheelchairassistance_master,
                a.requireddinner,
                dinner_ipd.requireddinner::varchar AS requireddinner_master,
                a.specialrequest,
                request_ipd.specialrequest::varchar AS specialrequest_master,
                a.paymentstatus,
                bp.paymentstatus::varchar AS paymentstatus_master,
                a.pendingamount,
                pending.amount::varchar AS pendingamount_master,
                a.createduser,
                a.createddate,
                a.modifieduser,
                a.modifieddate,
                false AS isdirectadmission
            FROM arrival a
            LEFT JOIN tenant t ON a.tenantid = t.tenantid
            LEFT JOIN ipdapplicationform ipd ON a.ipdnumber = ipd.ipdapplicationformid
            LEFT JOIN patientprofile pp ON a.patient = pp.patientprofileid
            LEFT JOIN room r ON a.room = r.roomid
            LEFT JOIN ipdapplicationform estimated_ipd ON a.estimatedarrival = estimated_ipd.ipdapplicationformid
            LEFT JOIN LATERAL
            (
                SELECT
                    (SELECT pd.dateofarrival
                       FROM ipdapplicationform_preferreddatesofadmission pd
                      WHERE pd.ipdapplicationformid = ipd.ipdapplicationformid
                        AND pd.isdeleted = false
                      ORDER BY pd.record_order, pd.dateofarrival
                      LIMIT 1) AS dateofarrival,
                    (SELECT ir.fromdate
                       FROM ipdapplicationform_room ir
                      WHERE ir.ipdapplicationformid = ipd.ipdapplicationformid
                        AND ir.isdeleted = false
                        AND lower(trim(COALESCE(ir.allottedto, ''))) = 'patient'
                      ORDER BY ir.record_order, ir.fromdate
                      LIMIT 1) AS fromdate
            ) normal_dates ON true
            LEFT JOIN ipdapplicationform status_ipd ON a.bookingstatus = status_ipd.ipdapplicationformid
            LEFT JOIN ipdapplicationform travel_ipd ON a.travelarrangement = travel_ipd.ipdapplicationformid
            LEFT JOIN ipdapplicationform pickup_ipd ON a.pickupfrom = pickup_ipd.ipdapplicationformid
            LEFT JOIN ipdapplicationform wheelchair_ipd ON a.wheelchairassistance = wheelchair_ipd.ipdapplicationformid
            LEFT JOIN ipdapplicationform dinner_ipd ON a.requireddinner = dinner_ipd.ipdapplicationformid
            LEFT JOIN ipdapplicationform request_ipd ON a.specialrequest = request_ipd.ipdapplicationformid
            LEFT JOIN billingpayment bp ON a.paymentstatus = bp.billingpaymentid
            LEFT JOIN billingpayment pending ON a.pendingamount = pending.billingpaymentid
            WHERE a.isdeleted = false

            UNION ALL

            /* Finalized Direct IPD Admissions do not create an Arrival row. */
            SELECT
                ipd.tenantid,
                t.businessname AS _tenantname,
                NULL::uuid AS arrivalid,
                ipd.ipdapplicationformid AS ipdnumber,
                ipd.bookingreferencenumber::varchar AS ipdnumber_master,
                ipd.patientname AS patient,
                (pp.firstname || ' ' || pp.lastname || ' ' || pp.mobilenumber)::varchar AS patient_master,
                patient_room.roomnumber AS room,
                r.roomnumber::varchar AS room_master,
                ipd.ipdapplicationformid AS estimatedarrival,
                COALESCE(ipd.estimatedarrival, preferred.dateofarrival, patient_room.fromdate) AS effective_arrival,
                COALESCE(ipd.estimatedarrival, preferred.dateofarrival, patient_room.fromdate)::varchar AS estimatedarrival_master,
                ipd.ipdapplicationformid AS bookingstatus,
                ipd.bookingstatus::varchar AS bookingstatus_master,
                ipd.ipdapplicationformid AS travelarrangement,
                (ipd.travelarrangement || ' ' || ipd.typeoftravelrequired)::varchar AS travelarrangement_master,
                ipd.ipdapplicationformid AS pickupfrom,
                ipd.pickupfrom::varchar AS pickupfrom_master,
                ipd.ipdapplicationformid AS wheelchairassistance,
                ipd.wheelchairassistance::varchar AS wheelchairassistance_master,
                ipd.ipdapplicationformid AS requireddinner,
                ipd.requireddinner::varchar AS requireddinner_master,
                ipd.ipdapplicationformid AS specialrequest,
                ipd.specialrequest::varchar AS specialrequest_master,
                NULL::uuid AS paymentstatus,
                receivable_summary.paymentstatus::varchar AS paymentstatus_master,
                NULL::uuid AS pendingamount,
                receivable_summary.pendingamount::varchar AS pendingamount_master,
                ipd.createduser,
                ipd.createddate,
                ipd.modifieduser,
                ipd.modifieddate,
                true AS isdirectadmission
            FROM ipdapplicationform ipd
            LEFT JOIN tenant t ON ipd.tenantid = t.tenantid
            LEFT JOIN patientprofile pp ON ipd.patientname = pp.patientprofileid
            LEFT JOIN LATERAL
            (
                SELECT ir.roomnumber, ir.fromdate
                  FROM ipdapplicationform_room ir
                 WHERE ir.ipdapplicationformid = ipd.ipdapplicationformid
                   AND ir.isdeleted = false
                   AND lower(trim(COALESCE(ir.allottedto, ''))) = 'patient'
                 ORDER BY ir.record_order, ir.fromdate
                 LIMIT 1
            ) patient_room ON true
            LEFT JOIN room r ON patient_room.roomnumber = r.roomid
            LEFT JOIN LATERAL
            (
                SELECT pd.dateofarrival
                  FROM ipdapplicationform_preferreddatesofadmission pd
                 WHERE pd.ipdapplicationformid = ipd.ipdapplicationformid
                   AND pd.isdeleted = false
                 ORDER BY pd.record_order, pd.dateofarrival
                 LIMIT 1
            ) preferred ON true
            LEFT JOIN LATERAL
            (
                SELECT
                    COALESCE(SUM(GREATEST(COALESCE(rec.amount, 0) - COALESCE(rec.paidamount, 0), 0)), 0) AS pendingamount,
                    CASE
                        WHEN COUNT(*) = 0 THEN 'Pending'
                        WHEN SUM(GREATEST(COALESCE(rec.amount, 0) - COALESCE(rec.paidamount, 0), 0)) <= 0 THEN 'Paid'
                        WHEN SUM(COALESCE(rec.paidamount, 0)) > 0 THEN 'Partially Paid'
                        ELSE 'Pending'
                    END AS paymentstatus
                FROM receivable rec
                WHERE rec.ipdnumber = ipd.ipdapplicationformid
                  AND COALESCE(rec.isdeleted, false) = false
                  AND COALESCE(rec.remarks, '') NOT ILIKE '%inactive%'
            ) receivable_summary ON true
            WHERE ipd.isdeleted = false
              AND lower(trim(COALESCE(ipd.verifiedstatus, ''))) in('direct admission','approved')
              --AND lower(trim(COALESCE(ipd.bookingstatus, ''))) = 'admitted'
              AND NOT EXISTS
              (
                  SELECT 1
                    FROM arrival existing_arrival
                   WHERE existing_arrival.ipdnumber = ipd.ipdapplicationformid
                     AND existing_arrival.isdeleted = false
              )
        ),
        filtered_rows AS
        (
            SELECT *
              FROM arrival_rows ar
             WHERE (lvar_tenantid IS NULL OR COALESCE(ar.tenantid::varchar, '') = ANY(lvar_tenantid))
               AND (pvar_ipdnumber IS NULL OR pvar_ipdnumber = '0' OR length(pvar_ipdnumber) = 0 OR ar.ipdnumber::varchar = pvar_ipdnumber)
               AND (pvar_patient IS NULL OR pvar_patient = '0' OR length(pvar_patient) = 0 OR ar.patient::varchar = pvar_patient)
               AND (pvar_room IS NULL OR pvar_room = '0' OR length(pvar_room) = 0 OR ar.room::varchar = pvar_room)
               AND (pvar_estimatedarrival IS NULL OR pvar_estimatedarrival = '0' OR length(trim(pvar_estimatedarrival)) = 0
                    OR ar.effective_arrival::date = to_date(left(trim(pvar_estimatedarrival), 10), 'DD/MM/YYYY'))
               AND (pvar_searchterm IS NULL
                    OR COALESCE(ar._tenantname, '') ILIKE pvar_searchterm
                    OR COALESCE(ar.ipdnumber_master, '') ILIKE pvar_searchterm
                    OR COALESCE(ar.patient_master, '') ILIKE pvar_searchterm
                    OR COALESCE(ar.room_master, '') ILIKE pvar_searchterm
                    OR COALESCE(ar.estimatedarrival_master, '') ILIKE pvar_searchterm
                    OR COALESCE(ar.bookingstatus_master, '') ILIKE pvar_searchterm
                    OR COALESCE(ar.travelarrangement_master, '') ILIKE pvar_searchterm
                    OR COALESCE(ar.pickupfrom_master, '') ILIKE pvar_searchterm
                    OR COALESCE(ar.wheelchairassistance_master, '') ILIKE pvar_searchterm
                    OR COALESCE(ar.requireddinner_master, '') ILIKE pvar_searchterm
                    OR COALESCE(ar.specialrequest_master, '') ILIKE pvar_searchterm
                    OR COALESCE(ar.paymentstatus_master, '') ILIKE pvar_searchterm
                    OR COALESCE(ar.pendingamount_master, '') ILIKE pvar_searchterm)
        )
        SELECT json_build_object(
            'count', (SELECT count(*) FROM filtered_rows),
            'summary', (
                SELECT json_build_object(
                    'totalArrivals', count(*),
                    'arrivalConfirmed', count(*) FILTER (
                        WHERE lower(trim(COALESCE(bookingstatus_master, ''))) = 'arrival confirmed'
                    ),
                    'assistanceRequired', count(*) FILTER (
                        WHERE lower(trim(COALESCE(wheelchairassistance_master, ''))) IN ('yes', 'true', 'required')
                    )
                )
                FROM filtered_rows
            ),
            'detail',
            (
                SELECT json_agg(row_to_json(d))
                  FROM
                  (
                      SELECT *
                        FROM filtered_rows
                       ORDER BY
                         CASE WHEN local_sortorder = 'asc' THEN
                           CASE local_sortcolumn
                             WHEN 'ipdnumber' THEN ipdnumber_master
                             WHEN 'patient' THEN patient_master
                             WHEN 'room' THEN room_master
                             WHEN 'estimatedarrival' THEN effective_arrival::text
                             WHEN 'bookingstatus' THEN bookingstatus_master
                             WHEN 'travelarrangement' THEN travelarrangement_master
                             WHEN 'pickupfrom' THEN pickupfrom_master
                             WHEN 'wheelchairassistance' THEN wheelchairassistance_master
                             WHEN 'requireddinner' THEN requireddinner_master
                             WHEN 'specialrequest' THEN specialrequest_master
                             WHEN 'paymentstatus' THEN paymentstatus_master
                             WHEN 'pendingamount' THEN pendingamount_master
                           END
                         END ASC,
                         CASE WHEN local_sortorder = 'desc' THEN
                           CASE local_sortcolumn
                             WHEN 'ipdnumber' THEN ipdnumber_master
                             WHEN 'patient' THEN patient_master
                             WHEN 'room' THEN room_master
                             WHEN 'estimatedarrival' THEN effective_arrival::text
                             WHEN 'bookingstatus' THEN bookingstatus_master
                             WHEN 'travelarrangement' THEN travelarrangement_master
                             WHEN 'pickupfrom' THEN pickupfrom_master
                             WHEN 'wheelchairassistance' THEN wheelchairassistance_master
                             WHEN 'requireddinner' THEN requireddinner_master
                             WHEN 'specialrequest' THEN specialrequest_master
                             WHEN 'paymentstatus' THEN paymentstatus_master
                             WHEN 'pendingamount' THEN pendingamount_master
                           END
                         END DESC,
                         effective_arrival ASC NULLS LAST,
                         createddate DESC
                       LIMIT pvar_pagesize
                      OFFSET pvar_pagenumber * pvar_pagesize
                  ) d
            )
        )
    );
END
$BODY$
LANGUAGE plpgsql;
