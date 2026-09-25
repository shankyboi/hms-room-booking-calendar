CREATE OR REPLACE FUNCTION "Appoinment_List"
(
    pvar_tenantid varchar,
    pvar_patient varchar(1024),
    pvar_origin varchar(1024),
    pvar_bookingreferencenumber varchar(1024),
    pvar_doctor varchar(1024),
    pvar_appointmentdate varchar(1024),
    pvar_task varchar(1024),
    pvar_status varchar(1024),
    pvar_pagesize integer,
    pvar_pagenumber integer,
    pvar_searchterm varchar,
    pvar_sort_fields json
)
RETURNS json AS $BODY$
DECLARE
    local_sortcolumn text;
    local_sortorder text;
    lvar_tenantid varchar[];
    lstr_usersid varchar;
BEGIN
    SELECT split_part(pvar_tenantid, '|', 1), split_part(pvar_tenantid, '|', 2)
      INTO lstr_usersid, pvar_tenantid;

    IF pvar_tenantid IS NULL OR pvar_tenantid = ''
       OR pvar_tenantid = '00000000-0000-0000-0000-000000000000' THEN
        SELECT string_to_array(viewertenantids, ',')
          INTO lvar_tenantid
          FROM users
         WHERE usersid::varchar = lstr_usersid;

        IF lvar_tenantid IS NULL THEN
            SELECT array_agg(tenantid) INTO lvar_tenantid FROM tenant;
        END IF;
    ELSE
        lvar_tenantid := ARRAY[pvar_tenantid];
    END IF;

    lvar_tenantid := lvar_tenantid
        || ARRAY[''::varchar, '00000000-0000-0000-0000-000000000000'::varchar];

    pvar_searchterm := CASE
        WHEN length(COALESCE(pvar_searchterm, '')) > 0 THEN '%' || pvar_searchterm || '%'
        ELSE NULL
    END;

    SELECT col, dir
      INTO local_sortcolumn, local_sortorder
      FROM json_to_recordset(pvar_sort_fields) AS x(col text, dir text)
     LIMIT 1;

    RETURN (
        WITH appointment_rows AS (
            SELECT ca.tenantid,
                   t.businessname AS _tenantname,
                   ca.clinicalappointmentid AS appoinmentid,
                   ca.clinicalappointmentid AS patient,
                   NULLIF(btrim(concat_ws(' ', NULLIF(pp.firstname, ''), NULLIF(pp.lastname, ''), NULLIF(pp.mobilenumber, ''))), '')::varchar AS patient_master,
                   ca.clinicalappointmentid AS origin,
                   ca.origin::varchar AS origin_master,
                   ca.clinicalappointmentid AS bookingreferencenumber,
                   COALESCE(
                       CASE
                           WHEN lower(trim(COALESCE(ca.origin, ''))) = 'ipd' THEN ipd.bookingreferencenumber
                           WHEN lower(trim(COALESCE(ca.origin, ''))) = 'opd' THEN opd.bookingreferencenumber
                       END,
                       ipd.bookingreferencenumber,
                       opd.bookingreferencenumber,
                       ca.bookingid
                   )::varchar AS bookingreferencenumber_master,
                   ca.clinicalappointmentid AS doctor,
                   NULLIF(btrim(concat_ws(' ', NULLIF(pr.firstname, ''), NULLIF(pr.lastname, ''))), '')::varchar AS doctor_master,
                   ca.clinicalappointmentid AS appointmentdate,
                   to_char(ca.appointmentdate, 'DD/MM/YYYY')::varchar AS appointmentdate_master,
                   ca.clinicalappointmentid AS task,
                   ca.tasktype::varchar AS task_master,
                   ca.clinicalappointmentid AS duration,
                   concat_ws(' - ', ca.durationfrom::varchar, ca.durationto::varchar)::varchar AS duration_master,
                   ca.clinicalappointmentid AS status,
                   ca.status::varchar AS status_master,
                   ca.appointmentdate AS effective_appointmentdate,
                   ca.patient AS patientid,
                   COALESCE(ca.actualpractitioner, ca.practitioner) AS practitionerid,
                   ca.createduser, ca.createddate, ca.modifieduser, ca.modifieddate
              FROM clinicalappointment ca
              LEFT JOIN tenant t ON ca.tenantid = t.tenantid
              LEFT JOIN patientprofile pp ON ca.patient = pp.patientprofileid
              LEFT JOIN people pr ON COALESCE(ca.actualpractitioner, ca.practitioner) = pr.peopleid
              LEFT JOIN ipdapplicationform ipd
                ON lower(trim(COALESCE(ca.origin, ''))) = 'ipd'
               AND ca.tenantid = ipd.tenantid
               AND (ca.bookingid = ipd.ipdapplicationformid::varchar
                    OR ca.bookingid = ipd.bookingreferencenumber)
               AND COALESCE(ipd.isdeleted, false) = false
              LEFT JOIN opdform opd
                ON lower(trim(COALESCE(ca.origin, ''))) = 'opd'
               AND ca.tenantid = opd.tenantid
               AND (ca.bookingid = opd.opdformid::varchar
                    OR ca.bookingid = opd.bookingreferencenumber)
               AND COALESCE(opd.isdeleted, false) = false
             WHERE COALESCE(ca.isdeleted, false) = false
        ), filtered AS (
            SELECT *
              FROM appointment_rows x
             WHERE (lvar_tenantid IS NULL OR COALESCE(x.tenantid::varchar, '') = ANY(lvar_tenantid))
               AND (COALESCE(pvar_patient, '') IN ('', '0')
                    OR x.patient::varchar = pvar_patient OR x.patientid::varchar = pvar_patient)
               AND (COALESCE(pvar_origin, '') IN ('', '0')
                    OR x.origin::varchar = pvar_origin OR lower(x.origin_master) = lower(pvar_origin))
               AND (COALESCE(pvar_bookingreferencenumber, '') IN ('', '0')
                    OR x.bookingreferencenumber::varchar = pvar_bookingreferencenumber
                    OR x.bookingreferencenumber_master = pvar_bookingreferencenumber)
               AND (COALESCE(pvar_doctor, '') IN ('', '0')
                    OR x.doctor::varchar = pvar_doctor OR x.practitionerid::varchar = pvar_doctor)
               AND (COALESCE(trim(pvar_appointmentdate), '') IN ('', '0')
                    OR x.effective_appointmentdate::date = to_date(left(trim(pvar_appointmentdate), 10), 'DD/MM/YYYY'))
               AND (COALESCE(pvar_task, '') IN ('', '0')
                    OR x.task::varchar = pvar_task OR lower(x.task_master) = lower(pvar_task))
               AND (COALESCE(pvar_status, '') IN ('', '0')
                    OR x.status::varchar = pvar_status OR lower(x.status_master) = lower(pvar_status))
               AND (pvar_searchterm IS NULL
                    OR COALESCE(x._tenantname, '') ILIKE pvar_searchterm
                    OR COALESCE(x.patient_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.origin_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.bookingreferencenumber_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.doctor_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.appointmentdate_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.task_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.duration_master, '') ILIKE pvar_searchterm
                    OR COALESCE(x.status_master, '') ILIKE pvar_searchterm)
        )
        SELECT json_build_object(
            'count', (SELECT count(*) FROM filtered),
            'summary', (
                SELECT json_build_object(
                    'totalAppointments', count(*),
                    'doctorCount', count(DISTINCT practitionerid),
                    'scheduled', count(*) FILTER (
                        WHERE lower(trim(COALESCE(status_master, ''))) = 'scheduled'
                    ),
                    'completed', count(*) FILTER (
                        WHERE lower(trim(COALESCE(status_master, ''))) = 'completed'
                    ),
                    'cancelled', count(*) FILTER (
                        WHERE lower(trim(COALESCE(status_master, ''))) IN ('cancelled', 'canceled')
                    ),
                    'doctors', COALESCE((
                        SELECT json_agg(
                            json_build_object(
                                'doctorId', doctor_summary.practitionerid,
                                'doctorName', doctor_summary.doctorname,
                                'total', doctor_summary.totalcount,
                                'scheduled', doctor_summary.scheduledcount,
                                'completed', doctor_summary.completedcount,
                                'cancelled', doctor_summary.cancelledcount
                            )
                            ORDER BY doctor_summary.doctorname
                        )
                        FROM (
                            SELECT practitionerid,
                                   COALESCE(NULLIF(trim(doctor_master), ''), 'Unassigned') AS doctorname,
                                   count(*) AS totalcount,
                                   count(*) FILTER (
                                       WHERE lower(trim(COALESCE(status_master, ''))) = 'scheduled'
                                   ) AS scheduledcount,
                                   count(*) FILTER (
                                       WHERE lower(trim(COALESCE(status_master, ''))) = 'completed'
                                   ) AS completedcount,
                                   count(*) FILTER (
                                       WHERE lower(trim(COALESCE(status_master, ''))) IN ('cancelled', 'canceled')
                                   ) AS cancelledcount
                              FROM filtered
                             GROUP BY practitionerid,
                                      COALESCE(NULLIF(trim(doctor_master), ''), 'Unassigned')
                        ) doctor_summary
                    ), '[]'::json)
                )
                FROM filtered
            ),
            'detail', (
                SELECT json_agg(row_to_json(q))
                  FROM (
                    SELECT *
                      FROM filtered
                     ORDER BY
                        CASE WHEN local_sortorder = 'asc' THEN
                            CASE local_sortcolumn
                                WHEN 'patient' THEN patient_master
                                WHEN 'origin' THEN origin_master
                                WHEN 'bookingreferencenumber' THEN bookingreferencenumber_master
                                WHEN 'doctor' THEN doctor_master
                                WHEN 'appointmentdate' THEN effective_appointmentdate::text
                                WHEN 'task' THEN task_master
                                WHEN 'duration' THEN duration_master
                                WHEN 'status' THEN status_master
                            END
                        END ASC,
                        CASE WHEN local_sortorder = 'desc' THEN
                            CASE local_sortcolumn
                                WHEN 'patient' THEN patient_master
                                WHEN 'origin' THEN origin_master
                                WHEN 'bookingreferencenumber' THEN bookingreferencenumber_master
                                WHEN 'doctor' THEN doctor_master
                                WHEN 'appointmentdate' THEN effective_appointmentdate::text
                                WHEN 'task' THEN task_master
                                WHEN 'duration' THEN duration_master
                                WHEN 'status' THEN status_master
                            END
                        END DESC,
                        effective_appointmentdate ASC,
                        createddate DESC
                     LIMIT pvar_pagesize
                    OFFSET pvar_pagenumber * pvar_pagesize
                  ) q
            )
        )
    );
END
$BODY$ LANGUAGE plpgsql;
