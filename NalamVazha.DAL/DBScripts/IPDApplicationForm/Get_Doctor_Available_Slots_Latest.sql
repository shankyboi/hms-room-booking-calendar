CREATE OR REPLACE FUNCTION public."Get_Doctor_Available_Slots_Latest"(
	pvar_peopleid uuid,
	pvar_appointmentdate timestamp without time zone,
	pvar_tenantid uuid DEFAULT NULL::uuid,
	pvar_taskid uuid DEFAULT NULL::uuid,
	pvar_taskname character varying DEFAULT NULL::character varying)
    RETURNS TABLE(slotfrom character varying, slotto character varying, isbooked boolean)
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000
AS $BODY$
DECLARE
    lv_durationinminutes int;
    lv_dayofweek         varchar;
    lv_slotstart         interval;
    lv_slotend           interval;
    lv_workhour_start    time;
    lv_workhour_end      time;
    lv_start_interval    interval;
    lv_end_interval      interval;
    lv_extended_end      interval;
    lv_current_booking   int;
    lv_overbookingcount  int;
    lv_shiftid           uuid;
    lv_tp_shiftid        uuid;
    lv_tp_start          time;
    lv_tp_end            time;
BEGIN

    -- 1. Get shift timing from StaffAttendance
    SELECT
        sa.shiftstarttime::time,
        sa.shiftendtime::time,
        sa.shift
    INTO
        lv_workhour_start,
        lv_workhour_end,
        lv_shiftid
    FROM StaffAttendance sa
    WHERE sa.peoplename = pvar_peopleid
      AND sa.shiftdate = pvar_appointmentdate::date
      AND (pvar_tenantid IS NULL OR sa.tenantid = pvar_tenantid)
      AND COALESCE(sa.isdeleted, false) = false
      AND sa.shiftstarttime IS NOT NULL
      AND sa.shiftendtime IS NOT NULL
    ORDER BY sa.createddate DESC NULLS LAST
    LIMIT 1;

    -- 2. Fallback to ShiftPlanning only for future dates
    IF lv_workhour_start IS NULL OR lv_workhour_end IS NULL THEN

        IF pvar_appointmentdate::date = CURRENT_DATE THEN
            RETURN;
        END IF;

        SELECT
            s.shiftstarttime::time,
            s.shiftendtime::time,
            sp.shiftname
        INTO
            lv_workhour_start,
            lv_workhour_end,
            lv_shiftid
        FROM ShiftPlanning sp
        JOIN ShiftPlanning_people spp
             ON spp.ShiftPlanningid = sp.ShiftPlanningid
        JOIN Shift s
             ON s.Shiftid = sp.shiftname
        WHERE spp.personname = pvar_peopleid
          AND pvar_appointmentdate::date BETWEEN sp.validfrom AND sp.validto
          AND COALESCE(sp.isdeleted, false) = false
          AND (pvar_tenantid IS NULL OR sp.tenantid = pvar_tenantid)
        ORDER BY sp.createddate DESC
        LIMIT 1;
    END IF;

    -- 3. If no shift exists, return no slots
    IF lv_workhour_start IS NULL OR lv_workhour_end IS NULL THEN
        RETURN;
    END IF;

    lv_dayofweek := to_char(pvar_appointmentdate::date, 'FMDay');

    -- 4. Get task-specific time preference
    SELECT
        ptp.shiftname,
        tp.taskstarttime::time,
        tp.taskendtime::time
    INTO
        lv_tp_shiftid,
        lv_tp_start,
        lv_tp_end
    FROM PeopleTimePreference ptp
    JOIN PeopleTimePreference_timepreference tp
         ON tp.PeopleTimePreferenceid = ptp.PeopleTimePreferenceid
    JOIN Task t
         ON t.taskid = tp.taskname
    WHERE ptp.people = pvar_peopleid
      AND COALESCE(ptp.isdeleted, false) = false
      AND (pvar_tenantid IS NULL OR ptp.tenantid = pvar_tenantid)
      AND tp.availableon ILIKE ('%' || lv_dayofweek || '%')
      AND (
            (pvar_taskid IS NOT NULL AND tp.taskname = pvar_taskid)
         OR (
                pvar_taskid IS NULL
            AND COALESCE(TRIM(pvar_taskname), '') <> ''
            AND LOWER(TRIM(t.taskname)) = LOWER(TRIM(pvar_taskname))
         )
      )
    ORDER BY tp.record_order DESC
    LIMIT 1;

    -- Attendance/shift planning is the authoritative working window. A task
    -- preference can narrow it, but its absence must not suppress all slots.
    IF lv_tp_start IS NULL OR lv_tp_end IS NULL THEN
        lv_tp_start := lv_workhour_start;
        lv_tp_end   := lv_workhour_end;
    END IF;

    IF lv_tp_shiftid IS NOT NULL THEN
        lv_shiftid := lv_tp_shiftid;
    END IF;

    lv_start_interval := make_interval(secs => EXTRACT(EPOCH FROM lv_tp_start));
    lv_end_interval   := make_interval(secs => EXTRACT(EPOCH FROM lv_tp_end));

    -- Handle night shift crossing midnight
    IF lv_end_interval <= lv_start_interval THEN
        lv_end_interval := lv_end_interval + interval '24 hours';
    END IF;

    -- 5. Get selected task duration and overbooking count
    SELECT
        pct.durationinminutes,
        COALESCE(pct.overbookingcount, 0)
    INTO
        lv_durationinminutes,
        lv_overbookingcount
    FROM People_clinicaltaskinfo pct
    JOIN Task t
         ON pct.taskname = t.taskid
    WHERE pct.Peopleid = pvar_peopleid
      AND pct.availableon ILIKE ('%' || lv_dayofweek || '%')
      AND (pvar_tenantid IS NULL OR t.tenantid = pvar_tenantid)
      AND (
            (pvar_taskid IS NOT NULL AND pct.taskname = pvar_taskid)
         OR (
                pvar_taskid IS NULL
            AND COALESCE(TRIM(pvar_taskname), '') <> ''
            AND LOWER(TRIM(t.taskname)) = LOWER(TRIM(pvar_taskname))
         )
      )
    ORDER BY pct.record_order DESC
    LIMIT 1;

    IF lv_durationinminutes IS NULL OR lv_durationinminutes = 0 THEN
        RETURN;
    END IF;

    -- 6. Extend end time by overbooking count
    lv_extended_end := lv_end_interval
        + make_interval(mins => lv_overbookingcount * lv_durationinminutes);

    lv_slotstart := lv_start_interval;

    -- 7. Generate slots
    WHILE lv_slotstart < lv_extended_end LOOP

        lv_slotend := lv_slotstart + make_interval(mins => lv_durationinminutes);

        EXIT WHEN lv_slotend > lv_extended_end;

        -- A slot is no longer available once its start time has been reached.
        IF pvar_appointmentdate::date = (NOW() AT TIME ZONE 'Asia/Kolkata')::date THEN
            IF lv_slotstart <= make_interval(secs => EXTRACT(
                EPOCH FROM (NOW() AT TIME ZONE 'Asia/Kolkata')::time
            )) THEN
                lv_slotstart := lv_slotend;
                CONTINUE;
            END IF;
        END IF;

        -- Skip break time overlap
        IF lv_shiftid IS NOT NULL THEN
            IF EXISTS (
                SELECT 1
                FROM Shift_breakdurationdetails sbd
                WHERE sbd.Shiftid = lv_shiftid
                  AND make_interval(secs => EXTRACT(EPOCH FROM sbd.starttime::time)) < lv_slotend
                  AND make_interval(secs => EXTRACT(EPOCH FROM sbd.endtime::time)) > lv_slotstart
            ) THEN
                lv_slotstart := lv_slotend;
                CONTINUE;
            END IF;
        END IF;

        -- Check existing appointment overlap
        SELECT COUNT(*)
        INTO lv_current_booking
        FROM ClinicalAppointment ca
        WHERE ca.practitioner = pvar_peopleid
          AND ca.appointmentdate = pvar_appointmentdate::date
          AND (pvar_tenantid IS NULL OR ca.tenantid = pvar_tenantid)
          AND COALESCE(ca.isdeleted, false) = false
          AND UPPER(TRIM(ca.status)) IN (
                'CONFIRMED',
                'IN PROGRESS',
                'IN-PROGRESS',
                'SCHEDULED',
                'REQUESTED',
                'COMPLETED'
          )
          AND (ca.appointmentdate + ca.durationfrom::time) < (pvar_appointmentdate::date + lv_slotend)
          AND (ca.appointmentdate + ca.durationto::time) > (pvar_appointmentdate::date + lv_slotstart);

        RETURN QUERY
        SELECT
            to_char(pvar_appointmentdate::date + lv_slotstart, 'HH24:MI')::varchar,
            to_char(pvar_appointmentdate::date + lv_slotend, 'HH24:MI')::varchar,
            (lv_current_booking > 0);

        lv_slotstart := lv_slotend;

    END LOOP;
END;
$BODY$;
