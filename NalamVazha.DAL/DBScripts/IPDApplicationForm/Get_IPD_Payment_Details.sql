-- FUNCTION: public.Get_IPD_Payment_Details(text)

DROP FUNCTION IF EXISTS public."Get_IPD_Payment_Details"(text);

CREATE OR REPLACE FUNCTION public."Get_IPD_Payment_Details"(
    pvar_ipdapplicationformid text)
    RETURNS TABLE(ipdapplicationformid uuid, bookingreferencenumber character varying, firstname character varying, lastname character varying, mobilenumber character varying, packagename uuid, packagename_master character varying, packagebookingdeposit numeric, packagebookingadvance numeric, packagecost numeric, advanceamount numeric, totalamount numeric, bookingstatus character varying, tenantid uuid, patientname uuid, daysofstay integer, accommodationtype character varying, attendantcount integer, attendant_room_cost numeric, patientvisitid uuid, costperday numeric, patient_booking_deposit numeric, attendant_booking_deposit numeric, bookingdeposit_refundable boolean, bookingdeposit_refundtype character varying, bookingdeposit_refundpercentage numeric, blocked_room_details_json jsonb, tenantname character varying, organizationlogo character varying, addressline1 character varying, estimatedarrival character varying, groupcode character varying)
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1
AS $BODY$

BEGIN
    RETURN QUERY
    WITH target_ipd AS (
        SELECT i.*
        FROM ipdapplicationform i
        WHERE i.ipdapplicationformid = pvar_ipdapplicationformid::uuid
          AND COALESCE(i.isdeleted, false) = false
    ),
    stay_details AS (
        SELECT COALESCE(pd.daysofstay, 0)::integer AS daysofstay
        FROM IPDApplicationForm_preferreddatesofadmission pd
        JOIN target_ipd i
            ON i.ipdapplicationformid = pd.ipdapplicationformid
        WHERE COALESCE(pd.isdeleted, false) = false
        ORDER BY pd.record_order
        LIMIT 1
    ),
    room_pref AS (
        SELECT
            r.roomtype AS roomtypeid,
            COALESCE(tpr.costperday, rt.costperday, 0) AS costperday,
            rt.deposittype,
            rt.bookingdeposit,
            rt.variableofbookingdays,
            rt.name AS roomname
        FROM target_ipd i
        JOIN roomoccupancystatus ros
            ON ros.ipdno = i.ipdapplicationformid
           AND COALESCE(ros.isdeleted, false) = false
        JOIN room r
            ON r.roomid = ros.room
        LEFT JOIN RoomType rt
            ON rt.roomtypeid = r.roomtype
        LEFT JOIN TreatmentPackage_roomtypes tpr
            ON tpr.roomtype = r.roomtype
           AND tpr.TreatmentPackageid = i.packagename
        ORDER BY ros.createddate DESC NULLS LAST, ros.roomoccupancystatusid DESC
        LIMIT 1
    ),
    attendant_room AS (
        SELECT
            COALESCE(tpr.costperday, rt.attendantcostperday, rt.costperday, 0) AS costperday,
            rt.deposittype,
            rt.bookingdeposit,
            rt.variableofbookingdays
        FROM target_ipd i
        JOIN IPDApplicationForm_room ir
            ON ir.ipdapplicationformid = i.ipdapplicationformid
           AND COALESCE(ir.isdeleted, false) = false
           AND LOWER(COALESCE(ir.allottedto, '')) = 'attendant'
        JOIN Room r
            ON r.roomid = ir.roomnumber
        LEFT JOIN RoomType rt
            ON rt.roomtypeid = r.roomtype
        LEFT JOIN TreatmentPackage_roomtypes tpr
            ON tpr.roomtype = r.roomtype
           AND tpr.TreatmentPackageid = i.packagename
        ORDER BY ir.record_order
        LIMIT 1
    ),
    payment_calc AS (
        SELECT
            CASE
                WHEN i.packagename IS NOT NULL
                 AND i.packagename <> '00000000-0000-0000-0000-000000000000'::uuid
                 AND COALESCE(tp.packagebookingadvance, 0) > 0
                    THEN COALESCE(tp.packagebookingadvance, 0)
                WHEN i.packagename IS NOT NULL
                 AND i.packagename <> '00000000-0000-0000-0000-000000000000'::uuid
                 AND COALESCE(tp.packagebookingdeposit, 0) > 0
                    THEN COALESCE(tp.packagebookingdeposit, 0)
                WHEN COALESCE(rp.deposittype, 'Fixed') = 'Fixed'
                    THEN COALESCE(rp.bookingdeposit, 0)
                WHEN rp.deposittype = 'Variable'
                    THEN COALESCE(rp.costperday, 0) * COALESCE(sd.daysofstay, 0) * COALESCE(rp.variableofbookingdays, 0) / 100
                ELSE 0
            END AS patient_booking_deposit,
            CASE
                WHEN COALESCE(ar.deposittype, 'Fixed') = 'Fixed'
                    THEN COALESCE(ar.bookingdeposit, 0)
                WHEN ar.deposittype = 'Variable'
                    THEN COALESCE(ar.costperday, 0) * COALESCE(sd.daysofstay, 0) * COALESCE(ar.variableofbookingdays, 0) / 100
                ELSE 0
            END AS attendant_booking_deposit
        FROM target_ipd i
        LEFT JOIN treatmentpackage tp
            ON tp.treatmentpackageid = i.packagename
        LEFT JOIN stay_details sd
            ON true
        LEFT JOIN room_pref rp
            ON true
        LEFT JOIN attendant_room ar
            ON true
    ),
    refund_policy AS (
        SELECT
            COALESCE(policy.refundtype, '')::varchar AS bookingdeposit_refundtype,
            COALESCE(policy.refundpercentage, 0) AS bookingdeposit_refundpercentage,
            (policy.refundtype IS NOT NULL) AS bookingdeposit_refundable
        FROM target_ipd i
        LEFT JOIN room_pref rp
            ON true
        LEFT JOIN LATERAL (
            SELECT refundtype, refundpercentage
            FROM (
                SELECT
                    tprf.refundtype::varchar AS refundtype,
                    tprf.refundpercentage,
                    tprf.record_order,
                    1 AS source_priority
                FROM TreatmentPackage_refundpolicy tprf
                WHERE i.packagename IS NOT NULL
                  AND i.packagename <> '00000000-0000-0000-0000-000000000000'::uuid
                  AND tprf.TreatmentPackageid = i.packagename
                  AND LOWER(REPLACE(COALESCE(tprf.refundtype, ''), ' ', '')) = 'bookingdeposit'

                UNION ALL

                SELECT
                    rtrf.refundtype::varchar AS refundtype,
                    rtrf.refundpercentage,
                    rtrf.record_order,
                    2 AS source_priority
                FROM RoomType_refundpolicy rtrf
                WHERE (i.packagename IS NULL OR i.packagename = '00000000-0000-0000-0000-000000000000'::uuid)
                  AND rtrf.RoomTypeid = rp.roomtypeid
                  AND LOWER(REPLACE(COALESCE(rtrf.refundtype, ''), ' ', '')) = 'bookingdeposit'
            ) candidate_policy
            ORDER BY source_priority, record_order DESC
            LIMIT 1
        ) policy
            ON true
    ),
    attendant_count AS (
        SELECT COUNT(*)::integer AS count
        FROM target_ipd i
        JOIN IPDApplicationForm_attendantinfo ait
            ON ait.ipdapplicationformid = i.ipdapplicationformid
           AND COALESCE(ait.isdeleted, false) = false
    ),
    patient_visit AS (
        SELECT pv.patientvisitid
        FROM target_ipd i
        JOIN patientvisit pv
            ON pv.ipdnumber = i.ipdapplicationformid
        ORDER BY pv.createddate DESC NULLS LAST
        LIMIT 1
    ),
    blocked_room_details AS (
        SELECT
            COALESCE(
                jsonb_agg(
                    jsonb_build_object(
                        'allottedto', irr.allottedto,
                        'roomnumber', COALESCE(rr.roomnumber, ''),
                        'fromdate', to_char(irr.fromdate::date, 'YYYY-MM-DD'),
                        'todate', to_char(irr.todate::date, 'YYYY-MM-DD'),
                        'roomid', irr.roomnumber,
                        'costperday', COALESCE(rr.costperday, 0),
                        'attendantcostperday', COALESCE(rr.attendantcostperday, 0),
                        'bookingdeposit', COALESCE(rr.bookingdeposit, 0),
                        'attendantbookingdeposit', COALESCE(rr.attendantbookingdeposit, 0)
                    )
                    ORDER BY irr.record_order
                ),
                '[]'::jsonb
            ) AS details_json
        FROM target_ipd i
        JOIN IPDApplicationForm_room irr
            ON irr.ipdapplicationformid = i.ipdapplicationformid
           AND COALESCE(irr.isdeleted, false) = false
        LEFT JOIN Room rr
            ON rr.roomid = irr.roomnumber
    )
    SELECT
        i.ipdapplicationformid,
        i.bookingreferencenumber,
        i.firstname,
        i.lastname,
        i.mobilenumber,
        i.packagename,
        tp.packagename::varchar,
        COALESCE(tp.packagebookingdeposit, 0),
        COALESCE(tp.packagebookingadvance, 0),
        COALESCE(tp.packagecost, 0),
        (COALESCE(pc.patient_booking_deposit, 0) + COALESCE(pc.attendant_booking_deposit, 0)) AS advanceamount,
        (
            CASE
                WHEN COALESCE(tp.packagecost, 0) > 0
                    THEN COALESCE(tp.packagecost, 0)
                WHEN i.packagename IS NOT NULL
                 AND i.packagename <> '00000000-0000-0000-0000-000000000000'::uuid
                 AND COALESCE(tp.packagebookingadvance, 0) > 0
                    THEN COALESCE(tp.packagebookingadvance, 0)
                WHEN i.packagename IS NOT NULL
                 AND i.packagename <> '00000000-0000-0000-0000-000000000000'::uuid
                 AND COALESCE(tp.packagebookingdeposit, 0) > 0
                    THEN COALESCE(tp.packagebookingdeposit, 0)
                ELSE COALESCE(rp.costperday, 0) * COALESCE(sd.daysofstay, 0)
            END
            +
            CASE
                WHEN ar.deposittype = 'Fixed'
                    THEN COALESCE(ar.bookingdeposit, 0)
                WHEN ar.deposittype = 'Variable'
                    THEN COALESCE(ar.costperday, 0) * COALESCE(sd.daysofstay, 0) * COALESCE(ar.variableofbookingdays, 0) / 100
                ELSE 0
            END
        ) AS totalamount,
        i.bookingstatus,
        i.tenantid,
        i.patientname,
        COALESCE(sd.daysofstay, 0),
        COALESCE(rp.roomname, ''),
        COALESCE(ac.count, 0),
        COALESCE(pc.attendant_booking_deposit, 0),
        COALESCE(pv.patientvisitid, '00000000-0000-0000-0000-000000000000'::uuid) AS patientvisitid,
        COALESCE(rp.costperday, 0) AS costperday,
        COALESCE(pc.patient_booking_deposit, 0) AS patient_booking_deposit,
        COALESCE(pc.attendant_booking_deposit, 0) AS attendant_booking_deposit,
        COALESCE(rpf.bookingdeposit_refundable, false) AS bookingdeposit_refundable,
        COALESCE(rpf.bookingdeposit_refundtype, '') AS bookingdeposit_refundtype,
        COALESCE(rpf.bookingdeposit_refundpercentage, 0) AS bookingdeposit_refundpercentage,
        COALESCE(brd.details_json, '[]'::jsonb) AS blocked_room_details_json,
        COALESCE(t.businessname, '')::varchar AS tenantname,
        COALESCE(t.organizationlogo, '')::varchar AS organizationlogo,
        COALESCE(t.addressline1, '')::varchar AS addressline1,
        CAST(COALESCE(to_char(i.estimatedarrival, 'DD/MM/YYYY HH24:MI'), '') AS character varying) AS estimatedarrival,
        i.groupcode::varchar
    FROM target_ipd i
    LEFT JOIN tenant t
        ON t.tenantid = i.tenantid
    LEFT JOIN treatmentpackage tp
        ON tp.treatmentpackageid = i.packagename
    LEFT JOIN stay_details sd
        ON true
    LEFT JOIN room_pref rp
        ON true
    LEFT JOIN attendant_room ar
        ON true
    LEFT JOIN payment_calc pc
        ON true
    LEFT JOIN refund_policy rpf
        ON true
    LEFT JOIN attendant_count ac
        ON true
    LEFT JOIN patient_visit pv
        ON true
    LEFT JOIN blocked_room_details brd
        ON true;
END;
$BODY$;
