CREATE OR REPLACE FUNCTION public.create_attendant_booking_deposit_receivable(
    p_ipdformid uuid,
    p_roomid    uuid,
    p_tenantid  uuid,
    p_createdby uuid)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    v_bookingdeposit numeric(18,2) := 0;
    v_patientid      uuid;
    v_day_prefix     varchar(8);
    v_max_seq        integer := 0;

    c_remark_allot      CONSTANT varchar(256) := 'AllotRoom:AttendantBookingDeposit';
    c_remark_provisional CONSTANT varchar(256) := 'Auto:ProvisionalPending:AttendantRoomBookingDeposit';
BEGIN
    -- 1. Read attendantbookingdeposit from the allotted attendant room
    SELECT COALESCE(r.attendantbookingdeposit, 0)
    INTO v_bookingdeposit
    FROM Room r
    WHERE r.Roomid = p_roomid
    LIMIT 1;

    IF COALESCE(v_bookingdeposit, 0) <= 0 THEN
        RETURN 0;
    END IF;

    -- 2. Resolve patient UUID from IPDApplicationForm
    SELECT i.patientname
    INTO v_patientid
    FROM IPDApplicationForm i
    WHERE i.IPDApplicationFormid = p_ipdformid
      AND COALESCE(i.isdeleted, false) = false
    LIMIT 1;

    -- 3. Idempotency: soft-delete existing attendant booking deposit rows
    UPDATE Receivable
    SET isdeleted    = true,
        modifieduser = p_createdby,
        modifieddate = NOW()
    WHERE ipdnumber     = p_ipdformid
      AND receivablefor = 'Room'
      AND COALESCE(remarks, '') IN (c_remark_allot, c_remark_provisional)
      AND COALESCE(isdeleted, false) = false;

    -- 4. Generate receivable number
    SELECT to_char(NOW(), 'YYYYMMDD') INTO v_day_prefix;
    SELECT COALESCE(MAX(RIGHT(receivableno, 5))::integer, 0)
    INTO v_max_seq
    FROM Receivable
    WHERE LEFT(receivableno, 8) = v_day_prefix
      AND receivableno NOT LIKE '%/%';

    v_max_seq := v_max_seq + 1;

    -- 5. Insert the correct attendant booking deposit receivable
    INSERT INTO Receivable (
        receivableid, tenantid, receivableno, receivabledate,
        patientname, ipdnumber, receivablefor, room,
        amount, remarks, createduser, createddate,
        paidamount, paymentstatus, isdeleted
    )
    VALUES (
        gen_random_uuid(), p_tenantid,
        v_day_prefix || '-' || to_char(v_max_seq, 'fm00000'),
        CURRENT_DATE,
        v_patientid, p_ipdformid, 'IPD Booking Deposit - Attendant', p_roomid,
        v_bookingdeposit, c_remark_allot, p_createdby, NOW(),
        0, 'Unpaid', false
    );

    RETURN 1;
END;
$BODY$;

ALTER FUNCTION public.create_attendant_booking_deposit_receivable(uuid, uuid, uuid, uuid)
    OWNER TO md_nalamvazha;
