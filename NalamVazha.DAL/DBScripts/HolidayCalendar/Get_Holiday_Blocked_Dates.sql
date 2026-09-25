CREATE OR REPLACE FUNCTION "Get_Holiday_Blocked_Dates"
(
    pvar_tenantid uuid,
    pvar_taskid uuid,
    pvar_taskname varchar,
    pvar_datefrom date,
    pvar_dateto date
)
RETURNS TABLE(
    holidaydate date,
    holidayname varchar,
    isallowed varchar,
    count bigint,
    bookedcount bigint,
    isblocked boolean
)
AS $BODY$
BEGIN
    RETURN QUERY
    WITH selected_task AS (
        SELECT
            t.taskid AS taskuuid,
            t.taskname AS taskname_text,
            tt.tasktypename
        FROM Task t
        LEFT JOIN TaskType tt ON tt.tasktypeid = t.tasktype
        WHERE COALESCE(t.isdeleted, false) = false
          AND (pvar_tenantid IS NULL OR t.tenantid = pvar_tenantid)
          AND (
                (pvar_taskid IS NOT NULL AND t.taskid = pvar_taskid)
             OR (pvar_taskid IS NULL AND COALESCE(pvar_taskname, '') <> '' AND (
                    LOWER(TRIM(t.taskname)) = LOWER(TRIM(pvar_taskname))
                 OR LOWER(TRIM(t.taskname)) = LOWER(TRIM('IP ' || pvar_taskname))
             ))
             OR (pvar_taskid IS NULL AND COALESCE(pvar_taskname, '') = '')
          )
        ORDER BY CASE WHEN pvar_taskid IS NOT NULL AND t.taskid = pvar_taskid THEN 0 ELSE 1 END
    ),
    holiday_rules AS (
        SELECT
            hc.holidaycalendarid,
            hc.holidaydate::date AS holidaydate,
            hc.holidayname,
            ta.isallowed,
            COALESCE(ta.count, 0) AS allowedcount,
            st.taskuuid,
            st.taskname_text,
            st.tasktypename
        FROM HolidayCalendar hc
        JOIN HolidayCalendar_taskallowed ta ON ta.holidaycalendarid = hc.holidaycalendarid
        JOIN selected_task st ON st.taskuuid = ta.taskname
        WHERE COALESCE(hc.isdeleted, false) = false
          AND (pvar_tenantid IS NULL OR hc.tenantid = pvar_tenantid)
          AND (pvar_datefrom IS NULL OR hc.holidaydate::date >= pvar_datefrom)
          AND (pvar_dateto IS NULL OR hc.holidaydate::date <= pvar_dateto)
    ),
    appointment_counts AS (
        SELECT
            hr.holidaycalendarid,
            hr.taskuuid,
            COUNT(ca.clinicalappointmentid) AS bookedcount
        FROM holiday_rules hr
        LEFT JOIN ClinicalAppointment ca
          ON COALESCE(ca.isdeleted, false) = false
         AND COALESCE(ca.status, '') NOT IN ('Cancelled', 'AdmittedCancelled')
         AND ca.appointmentdate::date = hr.holidaydate
         AND (
                LOWER(TRIM(COALESCE(ca.tasktype, ''))) = LOWER(TRIM(COALESCE(hr.taskname_text, '')))
             OR LOWER(TRIM(COALESCE(ca.tasktype, ''))) = LOWER(TRIM(COALESCE(hr.tasktypename, '')))
         )
        GROUP BY hr.holidaycalendarid, hr.taskuuid
    )
    SELECT
        hr.holidaydate,
        hr.holidayname,
        hr.isallowed,
        hr.allowedcount AS count,
        COALESCE(ac.bookedcount, 0) AS bookedcount,
        CASE
            WHEN LOWER(TRIM(COALESCE(hr.isallowed, ''))) IN ('no', 'false', '0', 'n') THEN true
            WHEN hr.allowedcount = 0 THEN true
            WHEN hr.allowedcount > 0 AND COALESCE(ac.bookedcount, 0) >= hr.allowedcount THEN true
            ELSE false
        END AS isblocked
    FROM holiday_rules hr
    LEFT JOIN appointment_counts ac ON ac.holidaycalendarid = hr.holidaycalendarid
                               AND ac.taskuuid = hr.taskuuid
    WHERE LOWER(TRIM(COALESCE(hr.isallowed, ''))) IN ('no', 'false', '0', 'n')
       OR hr.allowedcount = 0
       OR (hr.allowedcount > 0 AND COALESCE(ac.bookedcount, 0) >= hr.allowedcount)
    ORDER BY hr.holidaydate;
END;
$BODY$
LANGUAGE plpgsql;
