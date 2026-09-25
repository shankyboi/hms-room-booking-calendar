-- FUNCTION: public."SyncRoomReceivablesAfterTransferNoDelete"(uuid, date, date, uuid, numeric, character varying, uuid)

-- DROP FUNCTION IF EXISTS public."SyncRoomReceivablesAfterTransferNoDelete"(uuid, date, date, uuid, numeric, character varying, uuid);

CREATE OR REPLACE FUNCTION public."SyncRoomReceivablesAfterTransferNoDelete"(
    pvar_ipdapplicationformid uuid,
    pvar_fromdate date,
    pvar_todate date,
    pvar_newroomid uuid,
    pvar_newcostperday numeric,
    pvar_transferrole character varying,
    pvar_modifieduser uuid,
    OUT "returnMessage" character varying
)
RETURNS character varying
LANGUAGE 'plpgsql'
COST 100
VOLATILE PARALLEL UNSAFE
AS $BODY$
BEGIN

    IF pvar_ipdapplicationformid IS NULL THEN
        "returnMessage" := 'Invalid IPDApplicationFormid';
        RETURN;
    END IF;

    IF pvar_fromdate IS NULL OR pvar_todate IS NULL THEN
        "returnMessage" := 'Invalid date range';
        RETURN;
    END IF;

    IF pvar_fromdate > pvar_todate THEN
        "returnMessage" := 'From date cannot be greater than To date';
        RETURN;
    END IF;

    IF pvar_newroomid IS NULL THEN
        "returnMessage" := 'Invalid room';
        RETURN;
    END IF;

    IF pvar_newcostperday IS NULL THEN
        "returnMessage" := 'Invalid room cost';
        RETURN;
    END IF;

    IF pvar_modifieduser IS NULL THEN
        "returnMessage" := 'Invalid modified user';
        RETURN;
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM public.ipdapplicationform i
        WHERE i.ipdapplicationformid = pvar_ipdapplicationformid
          AND COALESCE(i.isdeleted, false) = false
    ) THEN
        "returnMessage" := 'IPD not found';
        RETURN;
    END IF;

    /*
        Avoid temp table conflict when same DB session is reused by connection pooling.
    */
    DROP TABLE IF EXISTS pg_temp.tmp_transfer_room_bill_lines;

    /*
        STEP 1:
        Build final room bill lines from ipdapplicationform_room.

        Important:
        - Do not use receivable.remarks for logic.
        - Do not alter receivable table in this step.
        - ipdapplicationform_room is source of truth after Transfer_Room SP.
    */
    CREATE TEMP TABLE tmp_transfer_room_bill_lines
    ON COMMIT DROP
    AS
    WITH active_rooms AS
    (
        SELECT
            r.ipdapplicationformid,
            r.roomnumber,
            r.allottedto,

            GREATEST(r.fromdate::date, pvar_fromdate) AS effective_fromdate,
            LEAST(r.todate::date, pvar_todate) AS effective_todate,

            CASE
                WHEN r.roomnumber = pvar_newroomid
                 AND LOWER(COALESCE(pvar_transferrole, '')) LIKE '%patient%'
                    THEN pvar_newcostperday
                ELSE COALESCE(rm.costperday, 0)
            END AS patient_cost,

            CASE
                WHEN r.roomnumber = pvar_newroomid
                 AND LOWER(COALESCE(pvar_transferrole, '')) LIKE '%attendant%'
                 AND LOWER(COALESCE(pvar_transferrole, '')) NOT LIKE '%patient%'
                    THEN pvar_newcostperday
                ELSE COALESCE(rm.attendantcostperday, 0)
            END AS attendant_cost

        FROM public.ipdapplicationform_room r

        LEFT JOIN public.room rm
            ON rm.roomid = r.roomnumber

        WHERE r.ipdapplicationformid = pvar_ipdapplicationformid
          AND COALESCE(r.isdeleted, false) = false
          AND r.fromdate::date <= pvar_todate
          AND r.todate::date >= pvar_fromdate
    ),
    expanded_days AS
    (
        SELECT
            ar.ipdapplicationformid,
            ar.roomnumber,
            ar.allottedto,
            gs::date AS receivabledate,
            ar.patient_cost,
            ar.attendant_cost
        FROM active_rooms ar
        CROSS JOIN LATERAL generate_series(
            ar.effective_fromdate,
            ar.effective_todate,
            interval '1 day'
        ) gs
    )
    SELECT
        ipdapplicationformid,
        roomnumber,
        receivabledate,

        CASE
            WHEN LOWER(COALESCE(allottedto, '')) LIKE '%patient%'
             AND LOWER(COALESCE(allottedto, '')) LIKE '%attendant%'
                THEN 'Patient, Attendant'

            WHEN LOWER(COALESCE(allottedto, '')) LIKE '%attendant%'
                THEN 'Attendant'

            ELSE 'Patient'
        END AS billing_role,

        CASE
            WHEN LOWER(COALESCE(allottedto, '')) LIKE '%patient%'
             AND LOWER(COALESCE(allottedto, '')) LIKE '%attendant%'
                THEN patient_cost

            WHEN LOWER(COALESCE(allottedto, '')) LIKE '%attendant%'
                THEN attendant_cost

            ELSE patient_cost
        END AS bill_amount

    FROM expanded_days;

    /*
        STEP 2:
        Old room receivable is no longer active.
        Do not delete.
        Do not make paidamount = 0.
        Keep paidamount because money may already be adjusted there.
    */
    UPDATE public.receivable r
    SET
        amount = 0,
        costperday = NULL,
        attendantcostperday = NULL,

        paymentstatus =
            CASE
                WHEN COALESCE(r.paidamount, 0) > 0
                    THEN 'Credit / Reallocation Pending'
                ELSE 'Waived'
            END,

        remarks = 'Room transfer adjustment - inactive room/date',
        modifieduser = pvar_modifieduser,
        modifieddate = NOW()

    WHERE r.ipdnumber = pvar_ipdapplicationformid
      AND r.receivabledate::date BETWEEN pvar_fromdate AND pvar_todate
      AND LOWER(COALESCE(r.receivablefor, '')) = 'room'
      AND COALESCE(r.isdeleted, false) = false
      AND NOT EXISTS
      (
          SELECT 1
          FROM tmp_transfer_room_bill_lines a
          WHERE a.receivabledate = r.receivabledate::date
            AND a.roomnumber = r.room
      );

    /*
        STEP 3:
        Update existing active room/date receivables.

        Match only:
        - IPD
        - Room
        - Receivable Date
        - receivablefor = Room

        No remarks matching.
    */
    WITH active_ranked AS
    (
        SELECT
            a.*,
            ROW_NUMBER() OVER
            (
                PARTITION BY a.roomnumber, a.receivabledate
                ORDER BY
                    CASE
                        WHEN a.billing_role = 'Patient, Attendant' THEN 1
                        WHEN a.billing_role = 'Patient' THEN 2
                        WHEN a.billing_role = 'Attendant' THEN 3
                        ELSE 9
                    END
            ) AS active_rn
        FROM tmp_transfer_room_bill_lines a
    ),
    existing_ranked AS
    (
        SELECT
            r.receivableid,
            r.room,
            r.receivabledate::date AS receivabledate,
            ROW_NUMBER() OVER
            (
                PARTITION BY r.room, r.receivabledate::date
                ORDER BY
                    CASE
                        WHEN COALESCE(r.costperday, 0) > 0
                         AND COALESCE(r.attendantcostperday, 0) > 0 THEN 1

                        WHEN COALESCE(r.costperday, 0) > 0 THEN 2

                        WHEN COALESCE(r.attendantcostperday, 0) > 0 THEN 3

                        ELSE 9
                    END,
                    r.createddate,
                    r.receivableid
            ) AS existing_rn
        FROM public.receivable r
        WHERE r.ipdnumber = pvar_ipdapplicationformid
          AND r.receivabledate::date BETWEEN pvar_fromdate AND pvar_todate
          AND LOWER(COALESCE(r.receivablefor, '')) = 'room'
          AND COALESCE(r.isdeleted, false) = false
          AND EXISTS
          (
              SELECT 1
              FROM tmp_transfer_room_bill_lines a
              WHERE a.receivabledate = r.receivabledate::date
                AND a.roomnumber = r.room
          )
    ),
    matched AS
    (
        SELECT
            e.receivableid,
            a.billing_role,
            a.bill_amount
        FROM active_ranked a
        INNER JOIN existing_ranked e
            ON e.room = a.roomnumber
           AND e.receivabledate = a.receivabledate
           AND e.existing_rn = a.active_rn
    )
    UPDATE public.receivable r
    SET
        costperday =
            CASE
                WHEN m.billing_role IN ('Patient', 'Patient, Attendant')
                    THEN m.bill_amount
                ELSE NULL
            END,

        attendantcostperday =
            CASE
                WHEN m.billing_role = 'Attendant'
                    THEN m.bill_amount
                ELSE NULL
            END,

        amount = m.bill_amount,

        paymentstatus =
            CASE
                WHEN COALESCE(m.bill_amount, 0) <= 0 THEN 'Waived'
                WHEN COALESCE(r.paidamount, 0) >= m.bill_amount THEN 'Paid'
                WHEN COALESCE(r.paidamount, 0) > 0 THEN 'Partially Paid'
                ELSE 'Pending'
            END,

        remarks =
            CASE
                WHEN m.billing_role = 'Patient, Attendant'
                    THEN 'Room transfer adjustment - Patient + Attendant'
                WHEN m.billing_role = 'Attendant'
                    THEN 'Room transfer adjustment - Attendant'
                ELSE 'Room transfer adjustment - Patient'
            END,

        modifieduser = pvar_modifieduser,
        modifieddate = NOW()

    FROM matched m
    WHERE r.receivableid = m.receivableid;

    /*
        STEP 4:
        Waive extra duplicate room/date rows.
        Do not delete.
        Do not reset paidamount.
    */
    WITH active_count AS
    (
        SELECT
            roomnumber,
            receivabledate,
            COUNT(*) AS active_count
        FROM tmp_transfer_room_bill_lines
        GROUP BY roomnumber, receivabledate
    ),
    existing_ranked AS
    (
        SELECT
            r.receivableid,
            r.room,
            r.receivabledate::date AS receivabledate,
            ROW_NUMBER() OVER
            (
                PARTITION BY r.room, r.receivabledate::date
                ORDER BY
                    CASE
                        WHEN COALESCE(r.costperday, 0) > 0
                         AND COALESCE(r.attendantcostperday, 0) > 0 THEN 1

                        WHEN COALESCE(r.costperday, 0) > 0 THEN 2

                        WHEN COALESCE(r.attendantcostperday, 0) > 0 THEN 3

                        ELSE 9
                    END,
                    r.createddate,
                    r.receivableid
            ) AS existing_rn
        FROM public.receivable r
        WHERE r.ipdnumber = pvar_ipdapplicationformid
          AND r.receivabledate::date BETWEEN pvar_fromdate AND pvar_todate
          AND LOWER(COALESCE(r.receivablefor, '')) = 'room'
          AND COALESCE(r.isdeleted, false) = false
          AND EXISTS
          (
              SELECT 1
              FROM tmp_transfer_room_bill_lines a
              WHERE a.receivabledate = r.receivabledate::date
                AND a.roomnumber = r.room
          )
    )
    UPDATE public.receivable r
    SET
        amount = 0,
        costperday = NULL,
        attendantcostperday = NULL,

        paymentstatus =
            CASE
                WHEN COALESCE(r.paidamount, 0) > 0
                    THEN 'Credit / Reallocation Pending'
                ELSE 'Waived'
            END,

        remarks = 'Room transfer adjustment - duplicate inactive line',
        modifieduser = pvar_modifieduser,
        modifieddate = NOW()

    FROM existing_ranked e
    INNER JOIN active_count a
        ON a.roomnumber = e.room
       AND a.receivabledate = e.receivabledate

    WHERE r.receivableid = e.receivableid
      AND e.existing_rn > a.active_count;

    /*
        STEP 5:
        Insert missing receivable rows for new active room/date lines.

        Example:
        Patient + Attendant old room
        Attendant transferred to new room

        New attendant room/date receivables may not exist.
        So insert them.
    */
    WITH active_ranked AS
    (
        SELECT
            a.*,
            ROW_NUMBER() OVER
            (
                PARTITION BY a.roomnumber, a.receivabledate
                ORDER BY
                    CASE
                        WHEN a.billing_role = 'Patient, Attendant' THEN 1
                        WHEN a.billing_role = 'Patient' THEN 2
                        WHEN a.billing_role = 'Attendant' THEN 3
                        ELSE 9
                    END
            ) AS active_rn
        FROM tmp_transfer_room_bill_lines a
    ),
    existing_count AS
    (
        SELECT
            r.room,
            r.receivabledate::date AS receivabledate,
            COUNT(*) AS existing_count
        FROM public.receivable r
        WHERE r.ipdnumber = pvar_ipdapplicationformid
          AND r.receivabledate::date BETWEEN pvar_fromdate AND pvar_todate
          AND LOWER(COALESCE(r.receivablefor, '')) = 'room'
          AND COALESCE(r.isdeleted, false) = false
        GROUP BY r.room, r.receivabledate::date
    ),
    missing_lines AS
    (
        SELECT
            a.*,
            ROW_NUMBER() OVER
            (
                ORDER BY a.receivabledate, a.roomnumber, a.active_rn
            ) AS insert_rn
        FROM active_ranked a
        LEFT JOIN existing_count e
            ON e.room = a.roomnumber
           AND e.receivabledate = a.receivabledate
        WHERE a.active_rn > COALESCE(e.existing_count, 0)
    ),
    base AS
    (
        SELECT
            tenantid,
            patientname,
            ipdapplicationformid
        FROM public.ipdapplicationform
        WHERE ipdapplicationformid = pvar_ipdapplicationformid
    ),
    max_seq AS
    (
        SELECT
            COALESCE(
                MAX(
                    CASE
                        WHEN RIGHT(receivableno, 5) ~ '^[0-9]+$'
                            THEN RIGHT(receivableno, 5)::integer
                        ELSE 0
                    END
                ),
                0
            ) AS seq
        FROM public.receivable
        WHERE LEFT(receivableno, 8) = to_char(NOW(), 'YYYYMMDD')
          AND receivableno NOT LIKE '%/%'
    )
    INSERT INTO public.receivable
    (
        receivableid,
        tenantid,
        receivableno,
        receivabledate,
        patientname,
        ipdnumber,
        receivablefor,
        room,
        costperday,
        attendantcostperday,
        amount,
        paidamount,
        paymentstatus,
        remarks,
        createduser,
        createddate,
        isdeleted
    )
    SELECT
        gen_random_uuid(),
        b.tenantid,
        to_char(NOW(), 'YYYYMMDD') || '-' ||
            to_char((ms.seq + ml.insert_rn)::integer, 'fm00000'),

        ml.receivabledate,
        b.patientname,
        b.ipdapplicationformid,
        'Room',
        ml.roomnumber,

        CASE
            WHEN ml.billing_role IN ('Patient', 'Patient, Attendant')
                THEN ml.bill_amount
            ELSE NULL
        END,

        CASE
            WHEN ml.billing_role = 'Attendant'
                THEN ml.bill_amount
            ELSE NULL
        END,

        ml.bill_amount,
        0,

        CASE
            WHEN COALESCE(ml.bill_amount, 0) <= 0 THEN 'Waived'
            ELSE 'Pending'
        END,

        CASE
            WHEN ml.billing_role = 'Patient, Attendant'
                THEN 'Room transfer adjustment - Patient + Attendant'
            WHEN ml.billing_role = 'Attendant'
                THEN 'Room transfer adjustment - Attendant'
            ELSE 'Room transfer adjustment - Patient'
        END,

        pvar_modifieduser,
        NOW(),
        false

    FROM missing_lines ml
    CROSS JOIN base b
    CROSS JOIN max_seq ms;

    DROP TABLE IF EXISTS pg_temp.tmp_transfer_room_bill_lines;

    "returnMessage" := '201.1';
    RETURN;

EXCEPTION
    WHEN OTHERS THEN
        DROP TABLE IF EXISTS pg_temp.tmp_transfer_room_bill_lines;
        "returnMessage" := SQLERRM;
        RETURN;
END;
$BODY$;

ALTER FUNCTION public."SyncRoomReceivablesAfterTransferNoDelete"(uuid, date, date, uuid, numeric, character varying, uuid)
    OWNER TO md_nalamvazha;