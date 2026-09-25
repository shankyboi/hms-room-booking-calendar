-- FUNCTION: public.create_provisional_booking_deposit_receivables(uuid, uuid, uuid)
-- Booking deposits are read directly from the allotted Room record:
--   patient  row → Room.bookingdeposit
--   attendant row → Room.attendantbookingdeposit
-- This ensures room-level overrides are respected instead of RoomType defaults.

CREATE OR REPLACE FUNCTION public.create_provisional_booking_deposit_receivables(
    p_ipdformid uuid,
    p_tenantid  uuid,
    p_createdby uuid)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    v_patientid                    UUID;
    v_packagename                  UUID;
    v_bookingstatus                VARCHAR(1080);
    v_isbookingdepositmandatory    BOOLEAN := false;
    v_patient_booking_deposit      NUMERIC(18,2) := 0;
    v_attendant_booking_deposit    NUMERIC(18,2) := 0;
    v_package_booking_deposit      NUMERIC(18,2) := 0;
    v_patient_roomid               UUID;
    v_attendant_roomid             UUID;
    v_day_prefix                   VARCHAR(8);
    v_max_seq                      INTEGER := 0;
    v_rows                         INTEGER := 0;

    c_remark_patient   CONSTANT VARCHAR(256) := 'Auto:ProvisionalPending:PatientRoomBookingDeposit';
    c_remark_package   CONSTANT VARCHAR(256) := 'Auto:ProvisionalPending:PackageBookingDeposit';
    c_remark_attendant CONSTANT VARCHAR(256) := 'Auto:ProvisionalPending:AttendantRoomBookingDeposit';
BEGIN
    IF p_ipdformid IS NULL THEN
        RETURN 0;
    END IF;

    SELECT i.patientname, i.packagename, i.bookingstatus,
           COALESCE(i.isbookingdepositmandatory, false)
    INTO v_patientid, v_packagename, v_bookingstatus,
         v_isbookingdepositmandatory
    FROM IPDApplicationForm i
    WHERE i.IPDApplicationFormid = p_ipdformid
      AND COALESCE(i.isdeleted, false) = false;

    IF v_patientid IS NULL THEN
        RAISE EXCEPTION 'IPDApplicationForm not found: %', p_ipdformid;
    END IF;

    -- Remove any previously generated deposit rows when the booking no longer
    -- requires a deposit, then bypass the payment stage completely.
    IF NOT v_isbookingdepositmandatory THEN
        DELETE FROM Receivable
        WHERE ipdnumber = p_ipdformid
          AND COALESCE(isdeleted, false) = false
          AND remarks IN (c_remark_patient, c_remark_package, c_remark_attendant);
        RETURN 0;
    END IF;

    IF LOWER(COALESCE(v_bookingstatus, '')) NOT IN ('provisional booking', 'provisional confirmed') THEN
        RETURN 0;
    END IF;

    -- Resolve currently allotted patient/attendant room IDs
    SELECT r.roomnumber
    INTO v_patient_roomid
    FROM IPDApplicationForm_room r
    WHERE r.IPDApplicationFormid = p_ipdformid
      AND COALESCE(r.isdeleted, false) = false
      AND LOWER(COALESCE(r.allottedto, '')) = 'patient'
    ORDER BY r.record_order
    LIMIT 1;

    SELECT r.roomnumber
    INTO v_attendant_roomid
    FROM IPDApplicationForm_room r
    WHERE r.IPDApplicationFormid = p_ipdformid
      AND COALESCE(r.isdeleted, false) = false
      AND LOWER(COALESCE(r.allottedto, '')) = 'attendant'
    ORDER BY r.record_order
    LIMIT 1;

    -- Read booking deposits directly from the allotted Room records
    -- (room-level values override RoomType defaults)
    IF v_patient_roomid IS NOT NULL THEN
        SELECT COALESCE(rm.bookingdeposit, 0)
        INTO v_patient_booking_deposit
        FROM Room rm
        WHERE rm.Roomid = v_patient_roomid;
    END IF;

    IF v_attendant_roomid IS NOT NULL THEN
        SELECT COALESCE(rm.attendantbookingdeposit, 0)
        INTO v_attendant_booking_deposit
        FROM Room rm
        WHERE rm.Roomid = v_attendant_roomid;
    END IF;

    -- Package booking deposit comes from TreatmentPackage directly
    IF v_packagename IS NOT NULL AND v_packagename <> '00000000-0000-0000-0000-000000000000'::uuid THEN
        SELECT COALESCE(tp.packagebookingdeposit, 0)
        INTO v_package_booking_deposit
        FROM TreatmentPackage tp
        WHERE tp.treatmentpackageid = v_packagename;
    END IF;

    -- Idempotent clear: only our auto-generated provisional rows
    DELETE FROM Receivable
    WHERE ipdnumber = p_ipdformid
      AND COALESCE(isdeleted, false) = false
      AND remarks IN (c_remark_patient, c_remark_package, c_remark_attendant);

    SELECT to_char(NOW(), 'YYYYMMDD') INTO v_day_prefix;
    SELECT COALESCE(MAX(RIGHT(receivableno, 5))::INTEGER, 0)
    INTO v_max_seq
    FROM Receivable
    WHERE LEFT(receivableno, 8) = v_day_prefix
      AND receivableno NOT LIKE '%/%';

    -- Patient deposit: package booking deposit takes priority when a package is selected
    IF v_packagename IS NOT NULL AND v_packagename <> '00000000-0000-0000-0000-000000000000'::uuid THEN
        IF COALESCE(v_package_booking_deposit, 0) > 0 THEN
            v_max_seq := v_max_seq + 1;
            INSERT INTO Receivable (
                Receivableid, tenantid, receivableno, receivabledate, patientname, ipdnumber,
                receivablefor, amount, remarks, createduser, isdeleted
            )
            VALUES (
                gen_random_uuid(), p_tenantid,
                v_day_prefix || '-' || to_char(v_max_seq, 'fm00000'),
                CURRENT_DATE, v_patientid, p_ipdformid,
                'IPD Booking Deposit - Patient', v_package_booking_deposit, c_remark_package,
                p_createdby, false
            );
            v_rows := v_rows + 1;
        END IF;
    ELSE
        IF COALESCE(v_patient_booking_deposit, 0) > 0 THEN
            v_max_seq := v_max_seq + 1;
            INSERT INTO Receivable (
                Receivableid, tenantid, receivableno, receivabledate, patientname, ipdnumber,
                receivablefor, room, amount, remarks, createduser, isdeleted
            )
            VALUES (
                gen_random_uuid(), p_tenantid,
                v_day_prefix || '-' || to_char(v_max_seq, 'fm00000'),
                CURRENT_DATE, v_patientid, p_ipdformid,
                'IPD Booking Deposit - Patient', v_patient_roomid, v_patient_booking_deposit, c_remark_patient,
                p_createdby, false
            );
            v_rows := v_rows + 1;
        END IF;
    END IF;

    -- Attendant booking deposit: always from the allotted attendant room
    IF COALESCE(v_attendant_booking_deposit, 0) > 0 THEN
        v_max_seq := v_max_seq + 1;
        INSERT INTO Receivable (
            Receivableid, tenantid, receivableno, receivabledate, patientname, ipdnumber,
            receivablefor, room, amount, remarks, createduser, isdeleted
        )
        VALUES (
            gen_random_uuid(), p_tenantid,
            v_day_prefix || '-' || to_char(v_max_seq, 'fm00000'),
            CURRENT_DATE, v_patientid, p_ipdformid,
            'IPD Booking Deposit - Attendant', v_attendant_roomid, v_attendant_booking_deposit, c_remark_attendant,
            p_createdby, false
        );
        v_rows := v_rows + 1;
    END IF;

    RETURN v_rows;
END;
$BODY$;

ALTER FUNCTION public.create_provisional_booking_deposit_receivables(uuid, uuid, uuid)
    OWNER TO md_nalamvazha;
