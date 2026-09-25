-- FUNCTION: public."UpdateRoomReceivableAmountForTransfer"(uuid, uuid, date, date, numeric, boolean, uuid, character varying)

-- DROP FUNCTION IF EXISTS public."UpdateRoomReceivableAmountForTransfer"(uuid, uuid, date, date, numeric, boolean, uuid, character varying);

CREATE OR REPLACE FUNCTION public."UpdateRoomReceivableAmountForTransfer"(
    pvar_ipdapplicationformid uuid,
    pvar_roomid uuid,
    pvar_fromdate date,
    pvar_todate date,
    pvar_newcostperday numeric,
    pvar_isattendant boolean,
    pvar_modifiedby uuid,
    pvar_remarks character varying,
    OUT "returnMessage" character varying
)
RETURNS character varying
LANGUAGE 'plpgsql'
COST 100
VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    v_old_allot_remark character varying;
    v_old_transfer_remark character varying;
    v_combined_transfer_remark character varying := 'TransferRoom:Patient, Attendant';
    v_effective_remarks character varying;
BEGIN
    /*
        Basic validations
    */
    IF pvar_ipdapplicationformid IS NULL THEN
        "returnMessage" := 'Invalid IPDApplicationFormid';
        RETURN;
    END IF;

    IF pvar_roomid IS NULL THEN
        "returnMessage" := 'Invalid room';
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

    IF pvar_newcostperday IS NULL THEN
        "returnMessage" := 'Invalid room cost';
        RETURN;
    END IF;

    IF pvar_modifiedby IS NULL THEN
        "returnMessage" := 'Invalid modified user';
        RETURN;
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM ipdapplicationform i
        WHERE i.ipdapplicationformid = pvar_ipdapplicationformid
    ) THEN
        "returnMessage" := 'IPD not found';
        RETURN;
    END IF;

    /*
        Remark markers used to identify old room receivable rows.
    */
    v_old_allot_remark :=
        CASE
            WHEN COALESCE(pvar_isattendant, false) = true
                THEN 'AllotRoom:Attendant'
            ELSE 'AllotRoom:Patient'
        END;

    v_old_transfer_remark :=
        CASE
            WHEN COALESCE(pvar_isattendant, false) = true
                THEN 'TransferRoom:Attendant'
            ELSE 'TransferRoom:Patient'
        END;

    v_effective_remarks := NULLIF(TRIM(COALESCE(pvar_remarks, '')), '');

    IF v_effective_remarks IS NULL THEN
        v_effective_remarks := v_old_transfer_remark;
    END IF;

    /*
        Update existing rows, even if they were wrongly soft-deleted earlier.
        Keep paidamount and billingpaymentid untouched.
    */
    UPDATE receivable
    SET
        isdeleted = false,
        room = pvar_roomid,

        costperday =
            CASE
                WHEN COALESCE(pvar_isattendant, false) = false
                    THEN pvar_newcostperday
                ELSE costperday
            END,

        attendantcostperday =
            CASE
                WHEN COALESCE(pvar_isattendant, false) = true
                    THEN pvar_newcostperday
                ELSE attendantcostperday
            END,

        amount = pvar_newcostperday,

        paymentstatus =
            CASE
                WHEN COALESCE(paidamount, 0) >= pvar_newcostperday THEN 'Paid'
                WHEN COALESCE(paidamount, 0) > 0 THEN 'Partially Paid'
                ELSE 'Pending'
            END,

        remarks = v_effective_remarks,
        modifieduser = pvar_modifiedby,
        modifieddate = NOW()
    WHERE ipdnumber = pvar_ipdapplicationformid
      AND receivabledate::date BETWEEN pvar_fromdate AND pvar_todate
      AND LOWER(COALESCE(receivablefor, '')) = 'room'
      AND (
            LOWER(COALESCE(remarks, '')) = LOWER(v_old_allot_remark)
         OR LOWER(COALESCE(remarks, '')) = LOWER(v_old_transfer_remark)
         OR LOWER(COALESCE(remarks, '')) = LOWER(v_combined_transfer_remark)
         OR LOWER(COALESCE(remarks, '')) = LOWER(v_effective_remarks)
      );

    /*
        Insert missing dates only.
        Example: if 06,07,08 existed but 09,10 did not,
        this inserts 09 and 10.
    */
    WITH day_series AS (
        SELECT gs::date AS receivable_date
        FROM generate_series(
            pvar_fromdate,
            pvar_todate,
            interval '1 day'
        ) gs
    ),
    base AS (
        SELECT
            i.tenantid,
            i.patientname,
            i.ipdapplicationformid
        FROM ipdapplicationform i
        WHERE i.ipdapplicationformid = pvar_ipdapplicationformid
    ),
    max_seq AS (
        SELECT
            COALESCE(
                MAX(
                    CASE
                        WHEN RIGHT(r.receivableno, 5) ~ '^[0-9]+$'
                            THEN RIGHT(r.receivableno, 5)::integer
                        ELSE 0
                    END
                ),
                0
            ) AS seq
        FROM receivable r
        WHERE LEFT(r.receivableno, 8) = to_char(NOW(), 'YYYYMMDD')
          AND r.receivableno NOT LIKE '%/%'
    ),
    missing_days AS (
        SELECT
            ds.receivable_date,
            row_number() OVER (ORDER BY ds.receivable_date) AS rn
        FROM day_series ds
        WHERE NOT EXISTS (
            SELECT 1
            FROM receivable r
            WHERE r.ipdnumber = pvar_ipdapplicationformid
              AND r.receivabledate::date = ds.receivable_date
              AND LOWER(COALESCE(r.receivablefor, '')) = 'room'
              AND COALESCE(r.isdeleted, false) = false
              AND (
                    LOWER(COALESCE(r.remarks, '')) = LOWER(v_effective_remarks)
                 OR LOWER(COALESCE(r.remarks, '')) = LOWER(v_old_allot_remark)
                 OR LOWER(COALESCE(r.remarks, '')) = LOWER(v_old_transfer_remark)
                 OR LOWER(COALESCE(r.remarks, '')) = LOWER(v_combined_transfer_remark)
              )
        )
    )
    INSERT INTO receivable
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
            to_char((ms.seq + md.rn)::integer, 'fm00000'),
        md.receivable_date,
        b.patientname,
        b.ipdapplicationformid,
        'Room',
        pvar_roomid,

        CASE
            WHEN COALESCE(pvar_isattendant, false) = false
                THEN pvar_newcostperday
            ELSE NULL
        END,

        CASE
            WHEN COALESCE(pvar_isattendant, false) = true
                THEN pvar_newcostperday
            ELSE NULL
        END,

        pvar_newcostperday,
        0,
        'Pending',
        v_effective_remarks,
        pvar_modifiedby,
        NOW(),
        false
    FROM missing_days md
    CROSS JOIN base b
    CROSS JOIN max_seq ms;

    "returnMessage" := '201.1';
    RETURN;

EXCEPTION
    WHEN OTHERS THEN
        "returnMessage" := SQLERRM;
        RETURN;
END;
$BODY$;

ALTER FUNCTION public."UpdateRoomReceivableAmountForTransfer"(uuid, uuid, date, date, numeric, boolean, uuid, character varying)
    OWNER TO md_nalamvazha;