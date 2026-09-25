-- FUNCTION: public.Extend_Stay(uuid, uuid, character varying, uuid, timestamp without time zone, timestamp without time zone, uuid)

-- DROP FUNCTION IF EXISTS public."Extend_Stay"(uuid, uuid, character varying, uuid, timestamp without time zone, timestamp without time zone, uuid);

CREATE OR REPLACE FUNCTION public."Extend_Stay"(
	pvar_ipdapplicationformid uuid,
	pvar_tenantid uuid,
	pvar_allottedto character varying,
	pvar_roomnumber uuid,
	pvar_extensionfromdate timestamp without time zone,
	pvar_newtodate timestamp without time zone,
	pvar_modifieduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    v_current_roomid uuid;
    v_current_roomnumber uuid;
    v_current_fromdate timestamp(3);
    v_current_todate timestamp(3);
    v_extension_from timestamp(3);
    v_next_order int;
    v_updated_preferred_dates int;

    v_roles text[];
    v_role text;
    v_has_patient boolean := false;
    v_has_attendant boolean := false;
    v_normalized_allottedto character varying;
    v_exact_current_found boolean := false;
    v_check_roomid uuid;
BEGIN
    IF NOT "Check_Authorization"(pvar_modifieduser, 'IPDApplicationForm', 'edit') THEN
        INSERT INTO system_logging
        (
            Log_code,
            system_logging_guid,
            log_application,
            log_date,
            log_level,
            log_logger,
            log_message,
            log_user_name
        )
        VALUES
        (
            '401.1',
            gen_random_uuid(),
            'Store Proc Authorization Check',
            NOW(),
            'Critical',
            'Extend_Stay',
            'Authorization Failed Extend_Stay',
            pvar_modifieduser
        );

        pvar_returnMessage := '401.1';
        RETURN;
    END IF;

    pvar_returnMessage := '';

    IF pvar_IPDApplicationFormid IS NULL THEN
        pvar_returnMessage := 'IPD Application Form is required';
        RETURN;
    END IF;

    IF pvar_allottedto IS NULL OR TRIM(pvar_allottedto) = '' THEN
        pvar_returnMessage := 'Extension type is required';
        RETURN;
    END IF;

    IF pvar_roomnumber IS NULL THEN
        pvar_returnMessage := 'Extension room is required';
        RETURN;
    END IF;

    IF pvar_newtodate IS NULL THEN
        pvar_returnMessage := 'New checkout date is required';
        RETURN;
    END IF;

    /*
        Normalize selected roles.

        Input can be:
        Patient
        Attendant
        Patient,Attendant
        Patient, Attendant
        Attendant,Patient

        Stored output will be:
        Patient
        Attendant
        Patient, Attendant
    */
    SELECT EXISTS
    (
        SELECT 1
        FROM unnest(string_to_array(COALESCE(pvar_allottedto, ''), ',')) AS x
        WHERE LOWER(TRIM(x)) = 'patient'
    )
    INTO v_has_patient;

    SELECT EXISTS
    (
        SELECT 1
        FROM unnest(string_to_array(COALESCE(pvar_allottedto, ''), ',')) AS x
        WHERE LOWER(TRIM(x)) = 'attendant'
    )
    INTO v_has_attendant;

    IF v_has_patient AND v_has_attendant THEN
        v_normalized_allottedto := 'Patient, Attendant';
        v_roles := ARRAY['Patient', 'Attendant'];
    ELSIF v_has_patient THEN
        v_normalized_allottedto := 'Patient';
        v_roles := ARRAY['Patient'];
    ELSIF v_has_attendant THEN
        v_normalized_allottedto := 'Attendant';
        v_roles := ARRAY['Attendant'];
    ELSE
        pvar_returnMessage := 'Invalid extension type';
        RETURN;
    END IF;

    /*
        Validate allocation exists for every selected role.
        Supports existing allottedto:
        Patient
        Attendant
        Patient,Attendant
        Patient, Attendant
    */
    FOREACH v_role IN ARRAY v_roles
    LOOP
        v_check_roomid := NULL;

        SELECT r.IPDApplicationForm_roomid
        INTO v_check_roomid
        FROM IPDApplicationForm_room r
        WHERE r.IPDApplicationFormid = pvar_IPDApplicationFormid
          AND COALESCE(r.isdeleted, false) = false
          AND EXISTS
          (
              SELECT 1
              FROM unnest(string_to_array(COALESCE(r.allottedto, ''), ',')) AS saved_role
              WHERE LOWER(TRIM(saved_role)) = LOWER(TRIM(v_role))
          )
        ORDER BY r.todate DESC, r.fromdate DESC, r.record_order DESC
        LIMIT 1;

        IF v_check_roomid IS NULL THEN
            pvar_returnMessage := 'Active room allocation not found for ' || TRIM(v_role);
            RETURN;
        END IF;
    END LOOP;

    /*
        First try exact role-set match.
        Example:
        selected = Patient, Attendant
        saved    = Patient, Attendant
    */
    SELECT r.IPDApplicationForm_roomid,
           r.roomnumber,
           r.fromdate,
           r.todate
    INTO v_current_roomid,
         v_current_roomnumber,
         v_current_fromdate,
         v_current_todate
    FROM IPDApplicationForm_room r
    WHERE r.IPDApplicationFormid = pvar_IPDApplicationFormid
      AND COALESCE(r.isdeleted, false) = false

      -- Every selected role must exist in saved allottedto
      AND NOT EXISTS
      (
          SELECT 1
          FROM unnest(v_roles) AS input_role
          WHERE NOT EXISTS
          (
              SELECT 1
              FROM unnest(string_to_array(COALESCE(r.allottedto, ''), ',')) AS saved_role
              WHERE LOWER(TRIM(saved_role)) = LOWER(TRIM(input_role))
          )
      )

      -- Every saved role must exist in selected roles
      AND NOT EXISTS
      (
          SELECT 1
          FROM unnest(string_to_array(COALESCE(r.allottedto, ''), ',')) AS saved_role
          WHERE TRIM(saved_role) <> ''
            AND NOT EXISTS
            (
                SELECT 1
                FROM unnest(v_roles) AS input_role
                WHERE LOWER(TRIM(input_role)) = LOWER(TRIM(saved_role))
            )
      )
    ORDER BY r.todate DESC, r.fromdate DESC, r.record_order DESC
    LIMIT 1;

    IF v_current_roomid IS NOT NULL THEN
        v_exact_current_found := true;
    END IF;

    /*
        If exact match is not found, find latest allocation
        that overlaps with selected role.
    */
    IF v_current_roomid IS NULL THEN
        SELECT r.IPDApplicationForm_roomid,
               r.roomnumber,
               r.fromdate,
               r.todate
        INTO v_current_roomid,
             v_current_roomnumber,
             v_current_fromdate,
             v_current_todate
        FROM IPDApplicationForm_room r
        WHERE r.IPDApplicationFormid = pvar_IPDApplicationFormid
          AND COALESCE(r.isdeleted, false) = false
          AND EXISTS
          (
              SELECT 1
              FROM unnest(v_roles) AS input_role
              INNER JOIN unnest(string_to_array(COALESCE(r.allottedto, ''), ',')) AS saved_role
                  ON LOWER(TRIM(input_role)) = LOWER(TRIM(saved_role))
          )
        ORDER BY r.todate DESC, r.fromdate DESC, r.record_order DESC
        LIMIT 1;

        v_exact_current_found := false;
    END IF;

    IF v_current_roomid IS NULL THEN
        pvar_returnMessage := 'Active room allocation not found for selected extension type';
        RETURN;
    END IF;

    /*
        DATE OVERLAP FIX:
        Current room already includes v_current_todate.
        Extension should start from next day.
    */
    v_extension_from := v_current_todate + INTERVAL '1 day';

    /*
        If UI sends extension from date, it must match calculated next date.
        We will still use calculated value to avoid overlap.
    */
    IF pvar_extensionfromdate IS NOT NULL
       AND pvar_extensionfromdate::date <> v_extension_from::date THEN
        v_extension_from := v_current_todate + INTERVAL '1 day';
    END IF;

    /*
        Allow one-day extension:
        extension from = 25/06/2026
        new to date    = 25/06/2026
    */
    IF pvar_newtodate < v_extension_from THEN
        pvar_returnMessage := 'New checkout date should be on or after extension start date';
        RETURN;
    END IF;

    INSERT INTO history
    VALUES
    (
        'IPDApplicationForm',
        NOW(),
        (
            SELECT query_to_xml(
                'SELECT * FROM IPDApplicationForm WHERE IPDApplicationForm.IPDApplicationFormid= ''' || pvar_IPDApplicationFormid || '''',
                true,
                false,
                ''
            )
        )
    );

    /*
        Capture history for latest room allocation of every selected role.
        DISTINCT avoids duplicate history when one row is Patient, Attendant.
    */
    WITH selected_roles AS
    (
        SELECT unnest(v_roles) AS role_name
    ),
    latest_room_ids AS
    (
        SELECT DISTINCT ON (sr.role_name)
            sr.role_name,
            r.IPDApplicationForm_roomid
        FROM selected_roles sr
        JOIN IPDApplicationForm_room r
          ON r.IPDApplicationFormid = pvar_IPDApplicationFormid
         AND COALESCE(r.isdeleted, false) = false
         AND EXISTS
         (
             SELECT 1
             FROM unnest(string_to_array(COALESCE(r.allottedto, ''), ',')) AS saved_role
             WHERE LOWER(TRIM(saved_role)) = LOWER(TRIM(sr.role_name))
         )
        ORDER BY sr.role_name, r.todate DESC, r.fromdate DESC, r.record_order DESC
    )
    INSERT INTO IPDApplicationForm_room_history
    (
        IPDApplicationFormid,
        IPDApplicationForm_roomid,
        record_order,
        allottedto,
        roomnumber,
        fromdate,
        todate,
        action_date,
        action_by,
        action
    )
    SELECT DISTINCT
        r.IPDApplicationFormid,
        r.IPDApplicationForm_roomid,
        r.record_order,
        r.allottedto,
        r.roomnumber,
        r.fromdate,
        r.todate,
        NOW(),
        pvar_modifieduser,
        'Extend Stay'
    FROM IPDApplicationForm_room r
    INNER JOIN latest_room_ids lri
        ON r.IPDApplicationForm_roomid = lri.IPDApplicationForm_roomid;

    /*
        Update vs Insert:
        Same room + exact same role-set => update current row.
        Otherwise insert one new extension row from v_extension_from.
    */
    IF v_exact_current_found = true AND v_current_roomnumber = pvar_roomnumber THEN
        UPDATE IPDApplicationForm_room
        SET todate = pvar_newtodate,
            allottedto = v_normalized_allottedto,
            action = 'Extend Stay',
            action_date = NOW(),
            action_by = pvar_modifieduser
        WHERE IPDApplicationForm_roomid = v_current_roomid;
    ELSE
        SELECT COALESCE(MAX(record_order), 0) + 1
        INTO v_next_order
        FROM IPDApplicationForm_room
        WHERE IPDApplicationFormid = pvar_IPDApplicationFormid;

        INSERT INTO IPDApplicationForm_room
        (
            IPDApplicationFormid,
            IPDApplicationForm_roomid,
            record_order,
            allottedto,
            roomnumber,
            fromdate,
            todate,
            action_date,
            action_by,
            action,
            isdeleted
        )
        VALUES
        (
            pvar_IPDApplicationFormid,
            gen_random_uuid(),
            v_next_order,
            v_normalized_allottedto,
            pvar_roomnumber,
            v_extension_from,
            pvar_newtodate,
            NOW(),
            pvar_modifieduser,
            'Extend Stay',
            false
        );
    END IF;

    WITH allotted_preferred_date AS
    (
        SELECT IPDApplicationForm_preferreddatesofadmissionid
        FROM IPDApplicationForm_preferreddatesofadmission
        WHERE IPDApplicationFormid = pvar_IPDApplicationFormid
          AND COALESCE(isdeleted, false) = false
          AND dateofarrival <= v_current_fromdate::date
          AND dateofdeparture >= v_current_todate::date
        ORDER BY record_order NULLS LAST, dateofarrival, dateofdeparture
        LIMIT 1
    )
    UPDATE IPDApplicationForm_preferreddatesofadmission pd
    SET dateofdeparture = pvar_newtodate::date,
        daysofstay = GREATEST((pvar_newtodate::date - pd.dateofarrival) + 1, 1),
        action_date = NOW(),
        action_by = pvar_modifieduser,
        action = 'Extend Stay'
    FROM allotted_preferred_date apd
    WHERE pd.IPDApplicationForm_preferreddatesofadmissionid = apd.IPDApplicationForm_preferreddatesofadmissionid;

    GET DIAGNOSTICS v_updated_preferred_dates = ROW_COUNT;

    IF v_updated_preferred_dates <= 0 THEN
        pvar_returnMessage := 'Preferred admission date was not found for this IPD application.';
        RETURN;
    END IF;

    UPDATE IPDApplicationForm
    SET modifieduser = pvar_modifieduser,
        modifieddate = NOW()
    WHERE IPDApplicationFormid = pvar_IPDApplicationFormid;

    pvar_returnMessage := '201.1';
END;
$BODY$;

ALTER FUNCTION public."Extend_Stay"(uuid, uuid, character varying, uuid, timestamp without time zone, timestamp without time zone, uuid)
    OWNER TO md_nalamvazha;

GRANT EXECUTE ON FUNCTION public."Extend_Stay"(uuid, uuid, character varying, uuid, timestamp without time zone, timestamp without time zone, uuid) TO PUBLIC;

GRANT EXECUTE ON FUNCTION public."Extend_Stay"(uuid, uuid, character varying, uuid, timestamp without time zone, timestamp without time zone, uuid) TO md_nalamvazha;

