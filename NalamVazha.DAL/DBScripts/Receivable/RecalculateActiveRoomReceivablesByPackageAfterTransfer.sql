-- FUNCTION: public."RecalculateActiveRoomReceivablesByPackageAfterTransfer"(uuid, uuid)

-- DROP FUNCTION IF EXISTS public."RecalculateActiveRoomReceivablesByPackageAfterTransfer"(uuid, uuid);

CREATE OR REPLACE FUNCTION public."RecalculateActiveRoomReceivablesByPackageAfterTransfer"(
    pvar_ipdapplicationformid uuid,
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

    IF pvar_modifieduser IS NULL THEN
        "returnMessage" := 'Invalid modified user';
        RETURN;
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM public.ipdapplicationform ipd
        WHERE ipd.ipdapplicationformid = pvar_ipdapplicationformid
          AND COALESCE(ipd.isdeleted, false) = false
    ) THEN
        "returnMessage" := 'IPD not found';
        RETURN;
    END IF;

    WITH ipd_pkg AS
    (
        SELECT
            ipd.ipdapplicationformid,

            COALESCE(
                ipd.packagename,
                '00000000-0000-0000-0000-000000000000'
            )::uuid AS packageid,

            COALESCE(tp.noofdays, 0)::integer AS package_days

        FROM public.ipdapplicationform ipd

        LEFT JOIN public.treatmentpackage tp
            ON tp.treatmentpackageid = ipd.packagename
           AND COALESCE(tp.isdeleted, false) = false

        WHERE ipd.ipdapplicationformid = pvar_ipdapplicationformid
          AND COALESCE(ipd.isdeleted, false) = false
    ),
    active_room_receivables AS
    (
        SELECT
            r.receivableid,
            r.receivabledate::date AS receivabledate,
            r.room,
            COALESCE(r.remarks, '') AS old_remarks,
            COALESCE(r.paidamount, 0) AS paidamount,

            rm.roomtype,

            COALESCE(rm.costperday, 0) AS actual_patient_cost,
            COALESCE(rm.attendantcostperday, 0) AS actual_attendant_cost,

            p.packageid,
            p.package_days,

            CASE
                WHEN p.packageid <> '00000000-0000-0000-0000-000000000000'::uuid
                 AND p.package_days > 0
                    THEN true
                ELSE false
            END AS has_package,

            CASE
                WHEN prt.treatmentpackage_roomtypesid IS NOT NULL
                    THEN true
                ELSE false
            END AS roomtype_exists_in_package,

            LEAST(
                GREATEST(COALESCE(prt.percentagecovered, 0), 0),
                100
            ) AS package_discount_percent,

            CASE
                WHEN COALESCE(r.remarks, '') ILIKE '%Patient%'
                 AND COALESCE(r.remarks, '') ILIKE '%Attendant%'
                    THEN 'Patient, Attendant'

                WHEN COALESCE(r.remarks, '') ILIKE '%Attendant%'
                 AND COALESCE(r.remarks, '') NOT ILIKE '%Patient%'
                    THEN 'Attendant'

                ELSE 'Patient'
            END AS billing_role,

            CASE
                WHEN COALESCE(r.remarks, '') ILIKE '%Attendant%'
                 AND COALESCE(r.remarks, '') NOT ILIKE '%Patient%'
                    THEN true
                ELSE false
            END AS is_attendant_only,

            CASE
                WHEN COALESCE(r.remarks, '') ILIKE '%Attendant%'
                 AND COALESCE(r.remarks, '') NOT ILIKE '%Patient%'
                    THEN false
                ELSE true
            END AS is_patient_billing

        FROM public.receivable r

        INNER JOIN public.room rm
            ON rm.roomid = r.room
           AND COALESCE(rm.isdeleted, false) = false

        CROSS JOIN ipd_pkg p

        LEFT JOIN public.treatmentpackage_roomtypes prt
            ON prt.treatmentpackageid = p.packageid
           AND prt.roomtype = rm.roomtype

        WHERE r.ipdnumber = pvar_ipdapplicationformid
          AND LOWER(COALESCE(r.receivablefor, '')) = 'room'
          AND COALESCE(r.isdeleted, false) = false

          /*
              Important:
              Only transferred-out inactive rows should be excluded.
              Active old room days and active new room days are recalculated.
          */
          AND COALESCE(r.remarks, '') NOT ILIKE '%inactive%'
    ),
    numbered AS
    (
        SELECT
            a.*,

            /*
                Count active package-eligible patient room rows.

                Included:
                - old active room days
                - new extended / transferred room days

                Excluded:
                - inactive transferred-out room rows
                - attendant-only rows
                - roomtypes not in treatmentpackage_roomtypes
            */
            SUM(
                CASE
                    WHEN a.has_package = true
                     AND a.is_patient_billing = true
                     AND a.is_attendant_only = false
                     AND a.roomtype_exists_in_package = true
                        THEN 1
                    ELSE 0
                END
            ) OVER () AS package_room_day_count,

            /*
                Sequence only active package-eligible patient room rows.
                First package_days rows get discount.
                Extra rows get full room cost.
            */
            SUM(
                CASE
                    WHEN a.has_package = true
                     AND a.is_patient_billing = true
                     AND a.is_attendant_only = false
                     AND a.roomtype_exists_in_package = true
                        THEN 1
                    ELSE 0
                END
            ) OVER
            (
                ORDER BY a.receivabledate, a.receivableid
                ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
            ) AS package_room_day_seq

        FROM active_room_receivables a
    ),
    calculated AS
    (
        SELECT
            n.*,

            CASE
                WHEN n.has_package = true
                 AND n.is_patient_billing = true
                 AND n.is_attendant_only = false
                 AND n.roomtype_exists_in_package = true
                 AND n.package_days > 0
                 AND n.package_room_day_seq <= n.package_days
                    THEN true
                ELSE false
            END AS package_applied,

            CASE
                WHEN n.has_package = true
                 AND n.is_patient_billing = true
                 AND n.is_attendant_only = false
                 AND n.roomtype_exists_in_package = true
                 AND n.package_days > 0
                 AND n.package_room_day_seq <= n.package_days
                    THEN
                        CASE
                            /*
                                Example:
                                Package days = 10
                                Package percentage = 5
                                Active package room days = 7
                                Effective percentage = 5 * 7 / 10 = 3.5
                            */
                            WHEN n.package_room_day_count > 0
                             AND n.package_room_day_count < n.package_days
                                THEN ROUND(
                                    n.package_discount_percent
                                    * n.package_room_day_count
                                    / n.package_days,
                                    2
                                )
                            ELSE n.package_discount_percent
                        END
                ELSE 0
            END AS effective_discount_percent

        FROM numbered n
    ),
    priced AS
    (
        SELECT
            c.*,

            CASE
                WHEN c.is_attendant_only = true
                    THEN c.actual_attendant_cost

                WHEN c.package_applied = true
                    THEN ROUND(
                        c.actual_patient_cost
                        * (100 - c.effective_discount_percent)
                        / 100,
                        2
                    )

                ELSE c.actual_patient_cost
            END AS new_amount

        FROM calculated c
    )
    UPDATE public.receivable r
    SET
        amount = COALESCE(p.new_amount, 0),

        roomtype = p.roomtype,

        costperday =
            CASE
                WHEN p.billing_role IN ('Patient', 'Patient, Attendant')
                    THEN COALESCE(p.new_amount, 0)
                ELSE NULL
            END,

        attendantcostperday =
            CASE
                WHEN p.billing_role = 'Attendant'
                    THEN COALESCE(p.new_amount, 0)
                ELSE NULL
            END,

        paymentstatus =
            CASE
                WHEN COALESCE(p.new_amount, 0) <= 0
                    THEN 'Waived'

                WHEN COALESCE(r.paidamount, 0) > COALESCE(p.new_amount, 0)
                    THEN 'Credit / Reallocation Pending'

                WHEN COALESCE(r.paidamount, 0) = COALESCE(p.new_amount, 0)
                    THEN 'Paid'

                WHEN COALESCE(r.paidamount, 0) > 0
                    THEN 'Partially Paid'

                ELSE 'Pending'
            END,

        modifieduser = pvar_modifieduser,
        modifieddate = NOW()

    FROM priced p
    WHERE r.receivableid = p.receivableid;

    "returnMessage" := '201.1';
    RETURN;

EXCEPTION
    WHEN OTHERS THEN
        "returnMessage" := SQLERRM;
        RETURN;
END;
$BODY$;

ALTER FUNCTION public."RecalculateActiveRoomReceivablesByPackageAfterTransfer"(uuid, uuid)
    OWNER TO md_nalamvazha;