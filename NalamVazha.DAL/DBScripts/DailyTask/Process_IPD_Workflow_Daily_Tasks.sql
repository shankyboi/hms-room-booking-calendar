CREATE OR REPLACE FUNCTION "Process_IPD_Workflow_Daily_Tasks"
(
    pvar_business_date date,
    pvar_template_names jsonb,
    pvar_system_user_id uuid DEFAULT NULL
)
RETURNS TABLE
(
    records_checked integer,
    tasks_created integer,
    tasks_skipped integer,
    tasks_closed integer,
    errors integer
)
LANGUAGE plpgsql
AS $BODY$
DECLARE
    candidate record;
    template_record record;
    mapping_record record;
    rollover_record record;
    appointment_record record;
    closed_in_scenario integer;
    current_task_number integer;
    task_number_prefix varchar(8);
BEGIN
    IF pvar_business_date IS NULL OR pvar_template_names IS NULL THEN
        RAISE EXCEPTION 'Business date and task-template mapping are required';
    END IF;

    records_checked := 0;
    tasks_created := 0;
    tasks_skipped := 0;
    tasks_closed := 0;
    errors := 0;
    task_number_prefix := to_char(pvar_business_date, 'YYYY') || '-' || to_char(pvar_business_date, 'DDD');

    PERFORM pg_advisory_xact_lock(hashtext('Process_IPD_Workflow_Daily_Tasks'));

    CREATE TEMP TABLE workflow_pending
    (
        scenario_key text NOT NULL,
        tenant_id uuid,
        source_id uuid NOT NULL,
        patient_id uuid,
        created_user uuid NOT NULL,
        event_date date NOT NULL,
        template_name text NOT NULL,
        daily_reminder boolean NOT NULL DEFAULT false,
        PRIMARY KEY (scenario_key, source_id)
    ) ON COMMIT DROP;

    CREATE TEMP TABLE workflow_due (LIKE workflow_pending INCLUDING ALL) ON COMMIT DROP;

    SELECT COUNT(*)::integer INTO records_checked
      FROM IPDApplicationForm ipd
     WHERE COALESCE(ipd.isdeleted, false) = false;

    -- Original rule: an application created yesterday is still unreviewed.
    INSERT INTO workflow_pending
    SELECT 'new_ipd_arrived', ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser), ipd.createddate::date,
           pvar_template_names->>'new_ipd_arrived', false
      FROM IPDApplicationForm ipd
     WHERE COALESCE(ipd.isdeleted, false) = false
       AND (ipd.verifiedstatus IS NULL OR btrim(ipd.verifiedstatus) = ''
            OR lower(btrim(ipd.verifiedstatus)) IN ('not reviewed', 'pending'))
       AND ipd.createddate::date = pvar_business_date - 1;

    -- 1. Doctor review follow-up after room-allotment review request.
    INSERT INTO workflow_pending
    SELECT 'doctor_review_followup', ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           COALESCE(ipd.bookingstatusdate, ipd.modifieddate, ipd.createddate)::date,
           pvar_template_names->>'doctor_review_followup', false
      FROM IPDApplicationForm ipd
     WHERE COALESCE(ipd.isdeleted, false) = false
       AND lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN
           ('doctor review', 'doctor review requested', 'request for doctor review', 'ready for doctor review')
       AND lower(btrim(COALESCE(ipd.verifiedstatus, 'pending'))) NOT IN ('approved', 'rejected');

    -- 2. Waiting-list follow-up while no room has been allocated.
    INSERT INTO workflow_pending
    SELECT 'waiting_list_followup', ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser), ipd.createddate::date,
           pvar_template_names->>'waiting_list_followup', false
      FROM IPDApplicationForm ipd
     WHERE COALESCE(ipd.isdeleted, false) = false
       AND lower(btrim(COALESCE(ipd.joinwaitinglist, ''))) IN ('true', 'yes', '1', 'enabled')
       AND lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN ('waitlisted', 'waiting list')
       AND NOT EXISTS
           (SELECT 1 FROM RoomAllocation ra
             WHERE ra.ipdno = ipd.IPDApplicationFormid AND COALESCE(ra.isdeleted, false) = false);

    -- 3. Positive booking deposit remains unpaid for at least one day.
    INSERT INTO workflow_pending
    SELECT 'booking_deposit_pending', ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           COALESCE(MIN(r.createddate)::date, ipd.createddate::date),
           pvar_template_names->>'booking_deposit_pending', false
      FROM IPDApplicationForm ipd
      JOIN Receivable r ON r.ipdnumber = ipd.IPDApplicationFormid
                       AND lower(btrim(COALESCE(r.receivablefor, ''))) LIKE 'ipd booking deposit%'
                       AND r.amount > 0 AND COALESCE(r.isdeleted, false) = false
     WHERE COALESCE(ipd.isdeleted, false) = false
       AND NOT EXISTS
           (SELECT 1 FROM BillingPayment bp
             WHERE bp.ipdnumber = ipd.IPDApplicationFormid
               AND lower(btrim(COALESCE(bp.receivablefor, ''))) LIKE 'ipd booking deposit%'
               AND lower(btrim(COALESCE(bp.paymentstatus, ''))) IN ('success', 'paid', 'completed')
               AND COALESCE(bp.isdeleted, false) = false)
     GROUP BY ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname, ipd.createduser, ipd.createddate;

    -- 4. Provisional confirmation has no assessment.
    INSERT INTO workflow_pending
    SELECT 'assessment_not_added', ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           COALESCE(ipd.bookingstatusdate, ipd.modifieddate, ipd.createddate)::date,
           pvar_template_names->>'assessment_not_added', false
      FROM IPDApplicationForm ipd
     WHERE COALESCE(ipd.isdeleted, false) = false
       AND lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN
           ('provisional booking', 'provisional confirmed')
       AND NOT EXISTS
           (SELECT 1 FROM Assessment a
             WHERE a.ipdform = ipd.IPDApplicationFormid AND COALESCE(a.isdeleted, false) = false);

    -- 5. Assessment remains in draft.
    INSERT INTO workflow_pending
    SELECT 'assessment_draft', ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser), MIN(a.createddate)::date,
           pvar_template_names->>'assessment_draft', false
      FROM IPDApplicationForm ipd
      JOIN Assessment a ON a.ipdform = ipd.IPDApplicationFormid AND COALESCE(a.isdeleted, false) = false
     WHERE COALESCE(ipd.isdeleted, false) = false
       AND lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN ('assessment form - in draft', 'draft')
     GROUP BY ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname, ipd.createduser;

    -- 6. Submitted assessment still awaits doctor review.
    INSERT INTO workflow_pending
    SELECT 'assessment_review_pending', ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           COALESCE(ipd.bookingstatusdate, MAX(a.modifieddate), MAX(a.createddate))::date,
           pvar_template_names->>'assessment_review_pending', false
      FROM IPDApplicationForm ipd
      JOIN Assessment a ON a.ipdform = ipd.IPDApplicationFormid AND COALESCE(a.isdeleted, false) = false
     WHERE COALESCE(ipd.isdeleted, false) = false
       AND lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN
           ('assessment form - review pending', 'screening completed by the patient - intern doctor review pending')
     GROUP BY ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname, ipd.createduser, ipd.bookingstatusdate;

    -- 7. Reviewed assessment has no allotted doctor.
    INSERT INTO workflow_pending
    SELECT 'doctor_not_allotted', ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           COALESCE(ipd.bookingstatusdate, ipd.modifieddate, ipd.createddate)::date,
           pvar_template_names->>'doctor_not_allotted', false
      FROM IPDApplicationForm ipd
     WHERE COALESCE(ipd.isdeleted, false) = false
       AND lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN
           ('assessment form reviewed', 'approved by doctor', 'ipd approved by doctor')
       AND NOT EXISTS
           (SELECT 1 FROM Assessment a
             WHERE a.ipdform = ipd.IPDApplicationFormid AND a.doctorname IS NOT NULL
               AND COALESCE(a.isdeleted, false) = false)
       AND NOT EXISTS
           (SELECT 1 FROM PatientVisit pv
             WHERE pv.ipdnumber = ipd.IPDApplicationFormid AND pv.consultingdoctor IS NOT NULL
               AND COALESCE(pv.isdeleted, false) = false);

    -- 8/9. Arrival confirmation and its per-day reminder share the same pending rule.
    INSERT INTO workflow_pending
    SELECT 'arrival_confirmation_pending', ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser), MIN(ra.fromdate),
           pvar_template_names->>'arrival_confirmation_pending', false
      FROM IPDApplicationForm ipd
      JOIN RoomAllocation ra ON ra.ipdno = ipd.IPDApplicationFormid AND COALESCE(ra.isdeleted, false) = false
     WHERE COALESCE(ipd.isdeleted, false) = false
       AND lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN ('admission approved', 'approved for admission')
     GROUP BY ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname, ipd.createduser;

    INSERT INTO workflow_pending
    SELECT 'arrival_confirmation_reminder', tenant_id, source_id, patient_id, created_user,
           event_date, pvar_template_names->>'arrival_confirmation_reminder', true
      FROM workflow_pending WHERE scenario_key = 'arrival_confirmation_pending';

    -- 10. Arrival confirmed, but consultation doctor is not allotted.
    INSERT INTO workflow_pending
    SELECT 'consultation_doctor_pending', ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           COALESCE(ipd.bookingstatusdate, ipd.estimatedarrival, ipd.modifieddate, ipd.createddate)::date,
           pvar_template_names->>'consultation_doctor_pending', false
      FROM IPDApplicationForm ipd
     WHERE COALESCE(ipd.isdeleted, false) = false
       AND lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN
           ('arrival confirmed', 'confirmed for arrival', 'patient arrived')
       AND NOT EXISTS
           (SELECT 1 FROM PatientVisit pv
             WHERE pv.ipdnumber = ipd.IPDApplicationFormid AND pv.consultingdoctor IS NOT NULL
               AND COALESCE(pv.isdeleted, false) = false);

    -- 11. Consultation is scheduled/allotted but has not been approved.
    INSERT INTO workflow_pending
    SELECT 'consultation_approval_pending', ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser), MIN(pv.createddate)::date,
           pvar_template_names->>'consultation_approval_pending', false
      FROM IPDApplicationForm ipd
      JOIN PatientVisit pv ON pv.ipdnumber = ipd.IPDApplicationFormid
                          AND pv.consultingdoctor IS NOT NULL AND COALESCE(pv.isdeleted, false) = false
     WHERE COALESCE(ipd.isdeleted, false) = false
       AND lower(btrim(COALESCE(ipd.bookingstatus, ''))) = 'consultation scheduled'
     GROUP BY ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname, ipd.createduser;

    -- 12. Consultation approved/admission confirmed, but patient is not admitted.
    INSERT INTO workflow_pending
    SELECT 'patient_admission_pending', ipd.tenantid, ipd.IPDApplicationFormid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           COALESCE(ipd.bookingstatusdate, ipd.modifieddate, ipd.createddate)::date,
           pvar_template_names->>'patient_admission_pending', false
      FROM IPDApplicationForm ipd
     WHERE COALESCE(ipd.isdeleted, false) = false
       AND lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN
           ('consultation approved', 'admission confirmed', 'eligible for admission');

    -- Close active tasks only after their scenario's pending condition is no longer true.
    FOR mapping_record IN SELECT key AS scenario_key, value AS template_name FROM jsonb_each_text(pvar_template_names)
    LOOP
        UPDATE DailyTask dt
           SET status = 'Completed', modifieduser = COALESCE(pvar_system_user_id, dt.createduser), modifieddate = clock_timestamp()
          FROM TaskTemplate tt
         WHERE dt.taskname = tt.TaskTemplateid
           AND lower(btrim(tt.taskname)) = lower(btrim(mapping_record.template_name))
           AND COALESCE(dt.isdeleted, false) = false
           AND lower(btrim(COALESCE(dt.status, ''))) NOT IN ('completed', 'closed', 'cancelled', 'canceled')
           AND NOT EXISTS
               (SELECT 1
                  FROM workflow_pending wp
                 WHERE lower(btrim(wp.template_name)) = lower(btrim(mapping_record.template_name))
                   AND wp.source_id = dt.ipdreferencenumber
                   AND wp.tenant_id IS NOT DISTINCT FROM dt.tenantid);
        GET DIAGNOSTICS closed_in_scenario = ROW_COUNT;
        tasks_closed := tasks_closed + closed_in_scenario;
    END LOOP;

    -- Most scenarios become due after one full day. Waiting-list follows up immediately;
    -- arrival tasks are restricted to the three-day room-arrival window.
    INSERT INTO workflow_due
    SELECT * FROM workflow_pending wp
     WHERE (wp.scenario_key = 'waiting_list_followup')
        OR (wp.scenario_key IN ('arrival_confirmation_pending', 'arrival_confirmation_reminder')
            AND pvar_business_date BETWEEN wp.event_date - 1 AND wp.event_date + 1)
        OR (wp.scenario_key NOT IN ('waiting_list_followup', 'arrival_confirmation_pending', 'arrival_confirmation_reminder')
            AND wp.event_date + 1 <= pvar_business_date);

    SELECT COALESCE(MAX(CASE WHEN RIGHT(dt.taskno, 5) ~ '^[0-9]{5}$'
                             THEN RIGHT(dt.taskno, 5)::integer END), 0)
      INTO current_task_number
      FROM DailyTask dt
     WHERE dt.taskno LIKE task_number_prefix || '-_____';

    /* Carry the latest still-active instance of every logical task into the
       business date.  Selecting only the latest instance prevents yesterday's
       complete history from being copied again and again. */
    FOR rollover_record IN
        SELECT DISTINCT ON
        (
            dt.tenantid, dt.tasktype, dt.taskname, dt.patientname,
            dt.ipdreferencenumber, dt.opdreferencenumber,
            COALESCE(dt.activityname, '')
        )
            dt.*,
            COALESCE(NULLIF(btrim(dt.priority), ''), NULLIF(btrim(tt.priority), ''), 'Low') AS effective_priority
        FROM DailyTask dt
        LEFT JOIN TaskTemplate tt ON tt.TaskTemplateid=dt.taskname
        WHERE COALESCE(dt.isdeleted, false)=false
          AND COALESCE(dt.dateandtime, dt.createddate)::date < pvar_business_date
          AND lower(btrim(COALESCE(dt.status, 'pending'))) NOT IN
              ('completed', 'closed', 'cancelled', 'canceled')
        ORDER BY
            dt.tenantid, dt.tasktype, dt.taskname, dt.patientname,
            dt.ipdreferencenumber, dt.opdreferencenumber,
            COALESCE(dt.activityname, ''),
            COALESCE(dt.dateandtime, dt.createddate) DESC NULLS LAST
    LOOP
        IF EXISTS
        (
            SELECT 1
            FROM DailyTask today
            WHERE COALESCE(today.dateandtime, today.createddate)::date=pvar_business_date
              AND today.tenantid IS NOT DISTINCT FROM rollover_record.tenantid
              AND today.tasktype IS NOT DISTINCT FROM rollover_record.tasktype
              AND today.taskname IS NOT DISTINCT FROM rollover_record.taskname
              AND today.patientname IS NOT DISTINCT FROM rollover_record.patientname
              AND today.ipdreferencenumber IS NOT DISTINCT FROM rollover_record.ipdreferencenumber
              AND today.opdreferencenumber IS NOT DISTINCT FROM rollover_record.opdreferencenumber
              AND COALESCE(today.activityname, '')=COALESCE(rollover_record.activityname, '')
              AND COALESCE(today.isdeleted, false)=false
        ) THEN
            /* A previous run may already have created today's row using the
               template priority. Do not merely skip it: apply the rollover
               escalation, while never downgrading a priority already raised
               manually or by another run. */
            UPDATE DailyTask today
               SET priority = CASE lower(btrim(rollover_record.effective_priority))
                    WHEN 'low' THEN 'Medium'
                    WHEN 'medium' THEN 'High'
                    WHEN 'normal' THEN 'High'
                    WHEN 'high' THEN 'Critical'
                    WHEN 'urgent' THEN 'Critical'
                    WHEN 'critical' THEN 'Critical'
                    ELSE 'Medium' END,
                   modifieduser = COALESCE(pvar_system_user_id, today.modifieduser, today.createduser),
                   modifieddate = clock_timestamp()
             WHERE COALESCE(today.dateandtime, today.createddate)::date=pvar_business_date
               AND today.tenantid IS NOT DISTINCT FROM rollover_record.tenantid
               AND today.tasktype IS NOT DISTINCT FROM rollover_record.tasktype
               AND today.taskname IS NOT DISTINCT FROM rollover_record.taskname
               AND today.patientname IS NOT DISTINCT FROM rollover_record.patientname
               AND today.ipdreferencenumber IS NOT DISTINCT FROM rollover_record.ipdreferencenumber
               AND today.opdreferencenumber IS NOT DISTINCT FROM rollover_record.opdreferencenumber
               AND COALESCE(today.activityname, '')=COALESCE(rollover_record.activityname, '')
               AND COALESCE(today.isdeleted, false)=false
               AND CASE lower(btrim(COALESCE(today.priority, '')))
                       WHEN 'critical' THEN 4 WHEN 'urgent' THEN 4
                       WHEN 'high' THEN 3 WHEN 'medium' THEN 2
                       WHEN 'normal' THEN 2 WHEN 'low' THEN 1 ELSE 0 END
                   < CASE lower(btrim(rollover_record.effective_priority))
                       WHEN 'critical' THEN 4 WHEN 'urgent' THEN 4
                       WHEN 'high' THEN 4 WHEN 'medium' THEN 3
                       WHEN 'normal' THEN 3 WHEN 'low' THEN 2 ELSE 2 END;
            tasks_skipped := tasks_skipped + 1;
            CONTINUE;
        END IF;

        current_task_number := current_task_number + 1;
        INSERT INTO DailyTask
        (
            DailyTaskid, tenantid, taskno, dateandtime, tasktype, taskname,
            patientname, patientcategory, ipdreferencenumber, opdreferencenumber,
            status, amount, activityname, description, priority, assignedto,
            createduser, createddate, isdeleted
        )
        VALUES
        (
            gen_random_uuid(), rollover_record.tenantid,
            task_number_prefix || '-' || to_char(current_task_number, 'FM00000'),
            pvar_business_date::timestamp + COALESCE(rollover_record.dateandtime::time, time '00:00'),
            rollover_record.tasktype, rollover_record.taskname,
            rollover_record.patientname, rollover_record.patientcategory,
            rollover_record.ipdreferencenumber, rollover_record.opdreferencenumber,
            'Pending', rollover_record.amount, rollover_record.activityname,
            rollover_record.description,
            CASE lower(btrim(rollover_record.effective_priority))
                WHEN 'low' THEN 'Medium'
                WHEN 'medium' THEN 'High'
                WHEN 'normal' THEN 'High'
                WHEN 'high' THEN 'Critical'
                WHEN 'urgent' THEN 'Critical'
                WHEN 'critical' THEN 'Critical'
                ELSE 'Medium'
            END,
            /* assignedto references users. createduser in legacy/generated
               tasks may contain a People/source identifier, so leave the
               assignment empty when no valid user was assigned previously. */
            rollover_record.assignedto,
            COALESCE(pvar_system_user_id, rollover_record.createduser),
            clock_timestamp(), false
        );
        tasks_created := tasks_created + 1;
    END LOOP;

    /* Populate the Appointment category from actual booked appointments.
       Available/blocked slot rows are not patient tasks. */
    FOR appointment_record IN
        SELECT
            ca.*,
            CASE WHEN lower(btrim(COALESCE(ca.origin, '')))='ipd'
                 THEN pvar_template_names->>'arrival_confirmation_pending'
                 ELSE pvar_template_names->>'consultation_approval_pending'
            END AS template_name,
            CASE WHEN lower(btrim(COALESCE(ca.status, ''))) IN ('cancelled','canceled','completed','done')
                 THEN 'Completed' ELSE 'Pending' END AS daily_status
        FROM ClinicalAppointment ca
        WHERE ca.appointmentdate=pvar_business_date
          AND COALESCE(ca.isdeleted, false)=false
          AND lower(btrim(COALESCE(ca.status, ''))) NOT IN ('available','blocked')
    LOOP
        IF EXISTS (SELECT 1 FROM DailyTask dt
                    WHERE dt.sourceappointmentid=appointment_record.ClinicalAppointmentid
                      AND COALESCE(dt.isdeleted, false)=false) THEN
            tasks_skipped := tasks_skipped + 1;
            CONTINUE;
        END IF;

        SELECT tt.TaskTemplateid, tt.tasktype, tt.priority
          INTO template_record
          FROM TaskTemplate tt
         WHERE lower(btrim(tt.taskname))=lower(btrim(appointment_record.template_name))
           AND COALESCE(tt.isdeleted, false)=false
         ORDER BY tt.createddate DESC NULLS LAST LIMIT 1;

        IF NOT FOUND THEN
            errors := errors + 1;
            RAISE WARNING 'Appointment task template "%" not found', appointment_record.template_name;
            CONTINUE;
        END IF;

        current_task_number := current_task_number + 1;
        INSERT INTO DailyTask
        (
            DailyTaskid, tenantid, taskno, dateandtime, tasktype, taskname,
            patientname, ipdreferencenumber, opdreferencenumber, status,
            priority, assignedto, sourceappointmentid, createduser, createddate, isdeleted
        )
        VALUES
        (
            gen_random_uuid(), appointment_record.tenantid,
            task_number_prefix || '-' || to_char(current_task_number, 'FM00000'),
            pvar_business_date::timestamp + CASE
                WHEN btrim(COALESCE(appointment_record.durationfrom, '')) ~* '^([0-9]{1,2}):[0-9]{2}\s*(AM|PM)$'
                    THEN to_timestamp(btrim(appointment_record.durationfrom), 'HH12:MI AM')::time
                WHEN btrim(COALESCE(appointment_record.durationfrom, '')) ~ '^([01]?[0-9]|2[0-3]):[0-9]{2}$'
                    THEN btrim(appointment_record.durationfrom)::time
                ELSE time '00:00' END,
            template_record.tasktype, template_record.TaskTemplateid,
            appointment_record.patient,
            CASE WHEN lower(btrim(COALESCE(appointment_record.origin, '')))='ipd'
                      AND appointment_record.bookingid ~* '^[0-9a-f-]{36}$'
                 THEN appointment_record.bookingid::uuid END,
            CASE WHEN lower(btrim(COALESCE(appointment_record.origin, '')))='opd'
                      AND appointment_record.bookingid ~* '^[0-9a-f-]{36}$'
                 THEN appointment_record.bookingid::uuid END,
            appointment_record.daily_status,
            COALESCE(NULLIF(btrim(template_record.priority), ''), 'Medium'),
            COALESCE(pvar_system_user_id, appointment_record.createduser),
            appointment_record.ClinicalAppointmentid,
            COALESCE(pvar_system_user_id, appointment_record.createduser), clock_timestamp(), false
        );
        tasks_created := tasks_created + 1;
    END LOOP;

    FOR candidate IN SELECT * FROM workflow_due ORDER BY scenario_key, source_id
    LOOP
        IF NULLIF(btrim(candidate.template_name), '') IS NULL THEN
            errors := errors + 1;
            RAISE WARNING 'No task-template name configured for scenario %', candidate.scenario_key;
            CONTINUE;
        END IF;

        SELECT tt.TaskTemplateid, tt.tasktype
          INTO template_record
          FROM TaskTemplate tt
         WHERE lower(btrim(tt.taskname)) = lower(btrim(candidate.template_name))
           AND tt.tasktype IS NOT NULL AND COALESCE(tt.isdeleted, false) = false
         ORDER BY tt.createddate DESC NULLS LAST LIMIT 1;

        IF NOT FOUND THEN
            errors := errors + 1;
            RAISE WARNING 'Shared task template "%" not found', candidate.template_name;
            CONTINUE;
        END IF;

        IF EXISTS
        (
            SELECT 1 FROM DailyTask dt
             WHERE dt.tenantid IS NOT DISTINCT FROM candidate.tenant_id
               AND dt.ipdreferencenumber = candidate.source_id
               AND dt.taskname = template_record.TaskTemplateid
               AND dt.tasktype = template_record.tasktype
               AND COALESCE(dt.isdeleted, false) = false
               AND lower(btrim(COALESCE(dt.status, ''))) NOT IN ('completed', 'closed', 'cancelled', 'canceled')
               AND (NOT candidate.daily_reminder OR dt.dateandtime::date = pvar_business_date)
        ) THEN
            tasks_skipped := tasks_skipped + 1;
            CONTINUE;
        END IF;

        current_task_number := current_task_number + 1;
        INSERT INTO DailyTask
        (
            DailyTaskid, tenantid, taskno, dateandtime, tasktype, taskname,
            patientname, ipdreferencenumber, status, createduser, createddate, isdeleted
        )
        VALUES
        (
            gen_random_uuid(), candidate.tenant_id,
            task_number_prefix || '-' || to_char(current_task_number, 'FM00000'),
            pvar_business_date::timestamp, template_record.tasktype, template_record.TaskTemplateid,
            candidate.patient_id, candidate.source_id, 'Pending', candidate.created_user,
            clock_timestamp(), false
        );
        tasks_created := tasks_created + 1;
    END LOOP;

    RETURN NEXT;
END
$BODY$;
