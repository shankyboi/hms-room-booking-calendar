-- FUNCTION: public.create_room_receivables(uuid, uuid, date, date, numeric, boolean, uuid, uuid, character varying)

-- DROP FUNCTION IF EXISTS public.create_room_receivables(uuid, uuid, date, date, numeric, boolean, uuid, uuid, character varying);

CREATE OR REPLACE FUNCTION public.create_room_receivables(
	p_ipdformid uuid,
	p_roomid uuid,
	p_fromdate date,
	p_todate date,
	p_costperday numeric,
	p_isattendant boolean,
	p_tenantid uuid,
	p_createdby uuid,
	p_remarks_override character varying DEFAULT NULL::character varying)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    v_patientid      UUID;
    v_effective_tenantid UUID := p_tenantid;
    v_tenant_shortcode VARCHAR(10);
    v_number_prefix  VARCHAR(32);
    v_max_seq        INTEGER;
    v_rows           INTEGER;
    v_remarks        VARCHAR(256);
    v_receivablefor  VARCHAR(1024) := 'Room';
BEGIN
    -- Guard: nothing to do if range is invalid
    IF p_fromdate > p_todate THEN
        RETURN 0;
    END IF;

    -- Resolve remarks string
    v_remarks := CASE
        WHEN p_remarks_override IS NOT NULL AND LENGTH(TRIM(p_remarks_override)) > 0
            THEN TRIM(p_remarks_override)
        WHEN p_isattendant THEN 'AllotRoom:Attendant'
        ELSE 'AllotRoom:Patient'
    END;

    -- Look up the patient UUID and tenant from the IPD form
    SELECT patientname, COALESCE(v_effective_tenantid, tenantid)
    INTO v_patientid, v_effective_tenantid
    FROM   IPDApplicationForm
    WHERE  IPDApplicationFormid = p_ipdformid;

    IF v_patientid IS NULL THEN
        RAISE EXCEPTION 'PatientProfile not found for IPDApplicationForm %', p_ipdformid;
    END IF;

    SELECT NULLIF(TRIM(shortcode), '')
    INTO v_tenant_shortcode
    FROM tenant
    WHERE tenantid = v_effective_tenantid
      AND COALESCE(isdeleted, false) = false;

    -- Idempotent: remove any previous rows for this room + IPD form + same remarks tag
   DELETE FROM Receivable
WHERE ipdnumber = p_ipdformid
  AND room = p_roomid
  AND remarks = v_remarks
  AND receivabledate BETWEEN p_fromdate AND p_todate
  AND COALESCE(isdeleted, false) = false; 

    -- Build tenant monthly receivableno prefix (TenantShortCode-YYYY-MM)
    v_number_prefix := COALESCE(v_tenant_shortcode, '') || '-' || to_char(NOW(), 'YYYY-MM');

    -- Get the current max sequence counter for this tenant/month
    SELECT COALESCE(MAX(
        CASE
            WHEN RIGHT(receivableno, 3) ~ '^[0-9]+$' THEN RIGHT(receivableno, 3)::INTEGER
            ELSE 0
        END
    ), 0)
    INTO   v_max_seq
    FROM   Receivable
    WHERE  receivableno LIKE v_number_prefix || '-%'
      AND  tenantid = v_effective_tenantid
      AND  COALESCE(isdeleted, false) = false;

    -- Bulk-insert one row per day in [p_fromdate, p_todate)
    INSERT INTO Receivable (
        Receivableid,
        tenantid,
        receivableno,
        receivabledate,
        patientname,
        ipdnumber,
        receivablefor,
        room,
        amount,
        remarks,
        createduser,
        isdeleted
    )
    SELECT
        gen_random_uuid(),
        v_effective_tenantid,
        v_number_prefix || '-' || to_char(v_max_seq + ROW_NUMBER() OVER (ORDER BY d), 'fm000'),
        d::DATE,
        v_patientid,
        p_ipdformid,
        v_receivablefor,
        p_roomid,
        p_costperday,
        v_remarks,
        p_createdby,
        false
		 FROM generate_series(p_fromdate, p_todate, INTERVAL '1 day') AS d;

    --FROM generate_series(p_fromdate, p_todate - INTERVAL '1 day', INTERVAL '1 day') AS d;

    GET DIAGNOSTICS v_rows = ROW_COUNT;
        -- Also refresh provisional booking-deposit receivables for this IPD.
    -- This safely no-ops unless bookingstatus = 'Provisional Pending'.
    PERFORM create_provisional_booking_deposit_receivables(
        p_ipdformid,
        p_tenantid,
        p_createdby
    );

    RETURN v_rows;
END;
$BODY$;

GRANT EXECUTE ON FUNCTION public.create_room_receivables(uuid, uuid, date, date, numeric, boolean, uuid, uuid, character varying) TO PUBLIC;
