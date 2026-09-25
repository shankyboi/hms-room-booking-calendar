
CREATE OR REPLACE FUNCTION "Get_Doctor_Available_Slots"
(
    pvar_peopleid     uuid,
    pvar_appointmentdate date
)
RETURNS TABLE(slotfrom varchar, slotto varchar, isbooked boolean)
AS $BODY$
DECLARE
    lv_workhourstarts    varchar;
    lv_workhourends      varchar;
    lv_durationinminutes int;
    lv_dayofweek         varchar;
    lv_slotstart         time;
    lv_slotend           time;
    lv_workhour_start    time;
    lv_workhour_end      time;
	lv_is_present boolean;
	
BEGIN
    --Check for Staff Attendance(doctor present or absent)
	SELECT EXISTS (
    	SELECT 1
    	FROM StaffAttendance sa
    	WHERE sa.peoplename = pvar_peopleid AND sa.shiftdate = pvar_appointmentdate AND sa.shiftstarttime IS NOT NULL AND sa.shiftendtime IS NOT NULL
	) INTO lv_is_present;

	IF NOT lv_is_present THEN
    	RETURN;
	END IF;

	--check for schedule
	lv_dayofweek := to_char(pvar_appointmentdate, 'FMDay');

    SELECT 
        pct.workhourstarts, 
        pct.workhourends, 
        pct.durationinminutes
    INTO   
        lv_workhourstarts, 
        lv_workhourends, 
        lv_durationinminutes
    FROM People_clinicaltaskinfo pct
    WHERE pct.Peopleid = pvar_peopleid
      AND pct.availableon ILIKE ('%' || lv_dayofweek || '%')
      AND pct.taskname IN ('OP New', 'OP Follow up')
    ORDER BY pct.record_order DESC
    LIMIT 1;

    IF lv_workhourstarts IS NULL 
       OR lv_durationinminutes IS NULL 
       OR lv_durationinminutes = 0 THEN
        RETURN;
    END IF;

	--generate slots + booking check

    lv_workhour_start := lv_workhourstarts::time;
    lv_workhour_end   := lv_workhourends::time;
    lv_slotstart      := lv_workhour_start;

	  WHILE lv_slotstart < lv_workhour_end LOOP

        lv_slotend := lv_slotstart + (lv_durationinminutes || ' minutes')::interval;

        EXIT WHEN lv_slotend > lv_workhour_end;

        RETURN QUERY
        SELECT
            to_char(lv_slotstart, 'HH24:MI'),
            to_char(lv_slotend,   'HH24:MI'),
            EXISTS (
                SELECT 1
                FROM ClinicalAppointment ca
                WHERE ca.practitioner    = pvar_peopleid
                  AND ca.appointmentdate = pvar_appointmentdate
                  AND ca.durationfrom    = to_char(lv_slotstart, 'HH24:MI')
                  AND COALESCE(ca.isdeleted, false) = false
                  AND ca.status NOT IN ('Cancelled')
            );

        lv_slotstart := lv_slotend;

    END LOOP;
END;
$BODY$
LANGUAGE plpgsql;
