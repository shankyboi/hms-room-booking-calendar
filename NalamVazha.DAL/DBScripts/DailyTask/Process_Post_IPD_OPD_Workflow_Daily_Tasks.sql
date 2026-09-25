CREATE TABLE IF NOT EXISTS IPDPostDischargeCompletion
(
    IPDApplicationFormid uuid PRIMARY KEY REFERENCES IPDApplicationForm(IPDApplicationFormid),
    tenantid uuid NULL,
    feedbacksubmitted boolean NOT NULL DEFAULT false,
    feedbacksubmitteddate timestamp(3) NULL,
    dischargechecklistcompleted boolean NOT NULL DEFAULT false,
    dischargechecklistcompleteddate timestamp(3) NULL,
    modifieduser uuid NULL,
    modifieddate timestamp(3) NOT NULL DEFAULT NOW()
);

CREATE OR REPLACE FUNCTION "Set_IPD_Post_Discharge_Completion"
(
    pvar_ipdapplicationformid uuid,
    pvar_feedbacksubmitted boolean DEFAULT NULL,
    pvar_dischargechecklistcompleted boolean DEFAULT NULL,
    pvar_modifieduser uuid DEFAULT NULL
)
RETURNS void
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_tenantid uuid;
BEGIN
    SELECT tenantid INTO lvar_tenantid
      FROM IPDApplicationForm WHERE IPDApplicationFormid = pvar_ipdapplicationformid;
    IF NOT FOUND THEN RAISE EXCEPTION 'IPD application % was not found', pvar_ipdapplicationformid; END IF;

    INSERT INTO IPDPostDischargeCompletion
        (IPDApplicationFormid, tenantid, feedbacksubmitted, feedbacksubmitteddate,
         dischargechecklistcompleted, dischargechecklistcompleteddate, modifieduser, modifieddate)
    VALUES
        (pvar_ipdapplicationformid, lvar_tenantid, COALESCE(pvar_feedbacksubmitted, false),
         CASE WHEN pvar_feedbacksubmitted = true THEN NOW() END,
         COALESCE(pvar_dischargechecklistcompleted, false),
         CASE WHEN pvar_dischargechecklistcompleted = true THEN NOW() END,
         pvar_modifieduser, NOW())
    ON CONFLICT (IPDApplicationFormid) DO UPDATE SET
        feedbacksubmitted = COALESCE(pvar_feedbacksubmitted, IPDPostDischargeCompletion.feedbacksubmitted),
        feedbacksubmitteddate = CASE WHEN pvar_feedbacksubmitted = true THEN NOW()
                                     ELSE IPDPostDischargeCompletion.feedbacksubmitteddate END,
        dischargechecklistcompleted = COALESCE(pvar_dischargechecklistcompleted,
                                               IPDPostDischargeCompletion.dischargechecklistcompleted),
        dischargechecklistcompleteddate = CASE WHEN pvar_dischargechecklistcompleted = true THEN NOW()
                                                ELSE IPDPostDischargeCompletion.dischargechecklistcompleteddate END,
        modifieduser = COALESCE(pvar_modifieduser, IPDPostDischargeCompletion.modifieduser),
        modifieddate = NOW();
END
$BODY$;

CREATE OR REPLACE FUNCTION "Process_Post_IPD_OPD_Workflow_Daily_Tasks"
(
    pvar_business_date date,
    pvar_template_names jsonb,
    pvar_system_user_id uuid DEFAULT NULL
)
RETURNS TABLE(records_checked integer, tasks_created integer, tasks_skipped integer, tasks_closed integer, errors integer)
LANGUAGE plpgsql
AS $BODY$
DECLARE
    candidate record;
    template_record record;
    current_task_number integer;
    task_number_prefix varchar(8);
    closed_count integer;
BEGIN
    IF pvar_business_date IS NULL OR pvar_template_names IS NULL THEN
        RAISE EXCEPTION 'Business date and task-template mapping are required';
    END IF;

    records_checked := 0; tasks_created := 0; tasks_skipped := 0; tasks_closed := 0; errors := 0;
    task_number_prefix := to_char(pvar_business_date, 'YYYY') || '-' || to_char(pvar_business_date, 'DDD');
    PERFORM pg_advisory_xact_lock(hashtext('Process_IPD_Workflow_Daily_Tasks'));

    CREATE TEMP TABLE post_workflow_pending
    (
        scenario_key text NOT NULL,
        source_type text NOT NULL,
        source_id uuid NOT NULL,
        tenant_id uuid,
        patient_id uuid,
        created_user uuid NOT NULL,
        event_date date NOT NULL,
        template_name text NOT NULL,
        PRIMARY KEY (scenario_key, source_type, source_id)
    ) ON COMMIT DROP;

    SELECT (SELECT COUNT(*) FROM IPDApplicationForm WHERE COALESCE(isdeleted, false) = false)
         + (SELECT COUNT(*) FROM OPDForm WHERE COALESCE(isdeleted, false) = false)
      INTO records_checked;

    -- 1/2. Discharged patient feedback and checklist remain incomplete.
    INSERT INTO post_workflow_pending
    SELECT 'feedback_followup', 'IPD', ipd.IPDApplicationFormid, ipd.tenantid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           COALESCE(ipd.bookingstatusdate, ipd.modifieddate, ipd.createddate)::date,
           pvar_template_names->>'feedback_followup'
      FROM IPDApplicationForm ipd
      LEFT JOIN IPDPostDischargeCompletion pc ON pc.IPDApplicationFormid = ipd.IPDApplicationFormid
     WHERE lower(btrim(COALESCE(ipd.bookingstatus, ''))) = 'discharged'
       AND COALESCE(ipd.isdeleted, false) = false AND COALESCE(pc.feedbacksubmitted, false) = false;

    INSERT INTO post_workflow_pending
    SELECT 'discharge_checklist', 'IPD', ipd.IPDApplicationFormid, ipd.tenantid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           COALESCE(ipd.bookingstatusdate, ipd.modifieddate, ipd.createddate)::date,
           pvar_template_names->>'discharge_checklist'
      FROM IPDApplicationForm ipd
      LEFT JOIN IPDPostDischargeCompletion pc ON pc.IPDApplicationFormid = ipd.IPDApplicationFormid
     WHERE lower(btrim(COALESCE(ipd.bookingstatus, ''))) = 'discharged'
       AND COALESCE(ipd.isdeleted, false) = false AND COALESCE(pc.dischargechecklistcompleted, false) = false;

    -- 3. Cancelled IPD has a refundable payment but no refund marker.
    INSERT INTO post_workflow_pending
    SELECT 'ipd_cancellation_refund', 'IPD', ipd.IPDApplicationFormid, ipd.tenantid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           COALESCE(ipd.bookingstatusdate, ipd.modifieddate, ipd.createddate)::date,
           pvar_template_names->>'ipd_cancellation_refund'
      FROM IPDApplicationForm ipd
     WHERE lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN ('cancelled', 'cancelled by patient')
       AND COALESCE(ipd.isdeleted, false) = false
       AND EXISTS (SELECT 1 FROM BillingPayment bp WHERE bp.ipdnumber = ipd.IPDApplicationFormid
                    AND COALESCE(bp.amount, 0) > 0 AND COALESCE(bp.isdeleted, false) = false)
       AND NOT "IPD_Has_Cancellation_Or_Refund"(ipd.IPDApplicationFormid);

    -- 4. Cancelled OPD appointment has a refundable payment but no initiated refund.
    INSERT INTO post_workflow_pending
    SELECT 'opd_cancellation_refund', 'OPD', opd.OPDFormid, opd.tenantid, opd.patientname,
           COALESCE(pvar_system_user_id, opd.createduser), MIN(COALESCE(ca.modifieddate, ca.createddate))::date,
           pvar_template_names->>'opd_cancellation_refund'
      FROM OPDForm opd
      JOIN ClinicalAppointment ca ON ca.bookingid = opd.OPDFormid::varchar
     WHERE lower(btrim(COALESCE(ca.status, ''))) IN ('cancelled', 'canceled')
       AND COALESCE(opd.isdeleted, false) = false AND COALESCE(ca.isdeleted, false) = false
       AND EXISTS (SELECT 1 FROM BillingPayment bp WHERE bp.opdnumber = opd.OPDFormid
                    AND COALESCE(bp.amount, 0) > 0 AND COALESCE(bp.isdeleted, false) = false)
       AND NOT "OPD_Has_Refund_Initiated"(opd.OPDFormid)
     GROUP BY opd.OPDFormid, opd.tenantid, opd.patientname, opd.createduser;

    -- OPD assessments awaiting either intern or senior-doctor review must be
    -- visible in the OPD Daily Task category until the review is completed.
    INSERT INTO post_workflow_pending
    SELECT 'opd_assessment_review_pending', 'OPD', opd.OPDFormid, opd.tenantid, opd.patientname,
           COALESCE(pvar_system_user_id, opd.createduser),
           COALESCE(opd.verifieddate, opd.modifieddate, opd.createddate)::date,
           pvar_template_names->>'opd_assessment_review_pending'
      FROM OPDForm opd
     WHERE lower(btrim(COALESCE(opd.verifiedstatus, ''))) IN
           ('assessment intern review pending', 'assessment doctor review pending')
       AND COALESCE(opd.isdeleted, false) = false;

    -- 5/7. A cancelled/discharged IPD still has active room occupancy.
    INSERT INTO post_workflow_pending
    SELECT 'cancelled_room_release', 'IPD', ipd.IPDApplicationFormid, ipd.tenantid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           COALESCE(ipd.bookingstatusdate, ipd.modifieddate, ipd.createddate)::date,
           pvar_template_names->>'cancelled_room_release'
      FROM IPDApplicationForm ipd
     WHERE lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN ('cancelled', 'cancelled by patient')
       AND COALESCE(ipd.isdeleted, false) = false
       AND EXISTS (SELECT 1 FROM RoomOccupancyStatus ros WHERE ros.ipdno = ipd.IPDApplicationFormid
                    AND lower(btrim(COALESCE(ros.status, ''))) IN ('booked', 'blocked', 'occupied')
                    AND COALESCE(ros.isdeleted, false) = false);

    INSERT INTO post_workflow_pending
    SELECT 'discharged_room_release', 'IPD', ipd.IPDApplicationFormid, ipd.tenantid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           COALESCE(ipd.bookingstatusdate, ipd.modifieddate, ipd.createddate)::date,
           pvar_template_names->>'discharged_room_release'
      FROM IPDApplicationForm ipd
     WHERE lower(btrim(COALESCE(ipd.bookingstatus, ''))) = 'discharged'
       AND COALESCE(ipd.isdeleted, false) = false
       AND EXISTS (SELECT 1 FROM RoomOccupancyStatus ros WHERE ros.ipdno = ipd.IPDApplicationFormid
                    AND lower(btrim(COALESCE(ros.status, ''))) IN ('booked', 'blocked', 'occupied')
                    AND COALESCE(ros.isdeleted, false) = false);

    -- Screening/consultation doctor marked the patient ineligible, but the room is still active.
    INSERT INTO post_workflow_pending
    SELECT 'screening_ineligible_room_release', 'IPD', ipd.IPDApplicationFormid, ipd.tenantid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           MAX(COALESCE(a.modifieddate, a.createddate))::date,
           pvar_template_names->>'screening_ineligible_room_release'
      FROM IPDApplicationForm ipd
      JOIN LATERAL
           (SELECT a1.* FROM Assessment a1
             WHERE a1.ipdform = ipd.IPDApplicationFormid AND COALESCE(a1.isdeleted, false) = false
             ORDER BY COALESCE(a1.modifieddate, a1.createddate) DESC NULLS LAST LIMIT 1) a ON true
     WHERE lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN
           ('screening scheduled', 'rejected')
       AND lower(btrim(COALESCE(to_jsonb(a)->>'eligibleforfinaladmission', ''))) IN
           ('screeningrejected', 'ineligible', 'not eligible', 'no')
       AND COALESCE(ipd.isdeleted, false) = false
       AND EXISTS (SELECT 1 FROM RoomOccupancyStatus ros WHERE ros.ipdno = ipd.IPDApplicationFormid
                    AND lower(btrim(COALESCE(ros.status, ''))) IN ('booked', 'blocked', 'occupied')
                    AND COALESCE(ros.isdeleted, false) = false)
     GROUP BY ipd.IPDApplicationFormid, ipd.tenantid, ipd.patientname, ipd.createduser;

    INSERT INTO post_workflow_pending
    SELECT 'consultation_ineligible_room_release', 'IPD', ipd.IPDApplicationFormid, ipd.tenantid, ipd.patientname,
           COALESCE(pvar_system_user_id, ipd.createduser),
           MAX(COALESCE(a.modifieddate, a.createddate))::date,
           pvar_template_names->>'consultation_ineligible_room_release'
      FROM IPDApplicationForm ipd
      JOIN LATERAL
           (SELECT a1.* FROM Assessment a1
             WHERE a1.ipdform = ipd.IPDApplicationFormid AND COALESCE(a1.isdeleted, false) = false
             ORDER BY COALESCE(a1.modifieddate, a1.createddate) DESC NULLS LAST LIMIT 1) a ON true
     WHERE lower(btrim(COALESCE(ipd.bookingstatus, ''))) IN
           ('consultation scheduled', 'rejected')
       AND lower(btrim(COALESCE(to_jsonb(a)->>'eligibleforfinaladmission', ''))) IN
           ('ineligible', 'not eligible', 'no', 'rejected')
       AND COALESCE(ipd.isdeleted, false) = false
       AND EXISTS (SELECT 1 FROM RoomOccupancyStatus ros WHERE ros.ipdno = ipd.IPDApplicationFormid
                    AND lower(btrim(COALESCE(ros.status, ''))) IN ('booked', 'blocked', 'occupied')
                    AND COALESCE(ros.isdeleted, false) = false)
     GROUP BY ipd.IPDApplicationFormid, ipd.tenantid, ipd.patientname, ipd.createduser;

    -- Complete tasks whose pending condition no longer exists.
    UPDATE DailyTask dt SET status = 'Completed', modifieduser = COALESCE(pvar_system_user_id, dt.createduser),
           modifieddate = clock_timestamp()
      FROM TaskTemplate tt
     WHERE dt.taskname = tt.TaskTemplateid AND COALESCE(dt.isdeleted, false) = false
       AND lower(btrim(COALESCE(dt.status, ''))) NOT IN ('completed', 'closed', 'cancelled', 'canceled')
       AND lower(btrim(tt.taskname)) IN
           (SELECT DISTINCT lower(btrim(value)) FROM jsonb_each_text(pvar_template_names)
             WHERE key IN ('feedback_followup','discharge_checklist','ipd_cancellation_refund',
                           'opd_cancellation_refund','cancelled_room_release','discharged_room_release',
                           'screening_ineligible_room_release','consultation_ineligible_room_release'))
       AND NOT EXISTS
           (SELECT 1 FROM post_workflow_pending p
             WHERE lower(btrim(p.template_name)) = lower(btrim(tt.taskname))
               AND p.tenant_id IS NOT DISTINCT FROM dt.tenantid
               AND ((p.source_type = 'IPD' AND dt.ipdreferencenumber = p.source_id)
                 OR (p.source_type = 'OPD' AND dt.opdreferencenumber = p.source_id)));
    GET DIAGNOSTICS closed_count = ROW_COUNT;
    tasks_closed := closed_count;

    -- Complete only OPD review tasks here. The same template name can also be
    -- used by IPD, so source/category scoping prevents cross-workflow closure.
    UPDATE DailyTask dt
       SET status = 'Completed',
           modifieduser = COALESCE(pvar_system_user_id, dt.createduser),
           modifieddate = clock_timestamp()
      FROM TaskTemplate tt, TypeofTask tot
     WHERE dt.taskname = tt.TaskTemplateid
       AND dt.tasktype = tot.TypeofTaskid
       AND lower(btrim(tt.taskname)) = lower(btrim(pvar_template_names->>'opd_assessment_review_pending'))
       AND lower(btrim(tot.tasktype)) = 'opd'
       AND dt.opdreferencenumber IS NOT NULL
       AND COALESCE(dt.isdeleted, false) = false
       AND lower(btrim(COALESCE(dt.status, ''))) NOT IN ('completed','closed','cancelled','canceled')
       AND NOT EXISTS
           (SELECT 1 FROM post_workflow_pending p
             WHERE p.scenario_key = 'opd_assessment_review_pending'
               AND p.source_id = dt.opdreferencenumber
               AND p.tenant_id IS NOT DISTINCT FROM dt.tenantid);
    GET DIAGNOSTICS closed_count = ROW_COUNT;
    tasks_closed := tasks_closed + closed_count;

    SELECT COALESCE(MAX(CASE WHEN RIGHT(taskno, 5) ~ '^[0-9]{5}$' THEN RIGHT(taskno, 5)::integer END), 0)
      INTO current_task_number FROM DailyTask WHERE taskno LIKE task_number_prefix || '-_____';

    FOR candidate IN
        SELECT * FROM post_workflow_pending
         WHERE scenario_key = 'opd_assessment_review_pending'
            OR event_date + 1 <= pvar_business_date
         ORDER BY scenario_key, source_id
    LOOP
        IF NULLIF(btrim(candidate.template_name), '') IS NULL THEN errors := errors + 1; CONTINUE; END IF;
        SELECT tt.TaskTemplateid, tt.tasktype INTO template_record FROM TaskTemplate tt
         WHERE lower(btrim(tt.taskname)) = lower(btrim(candidate.template_name))
           AND tt.tasktype IS NOT NULL AND COALESCE(tt.isdeleted, false) = false
         ORDER BY tt.createddate DESC NULLS LAST LIMIT 1;
        IF NOT FOUND THEN errors := errors + 1; RAISE WARNING 'Task template "%" not found', candidate.template_name; CONTINUE; END IF;

        -- The shared Assessment Follow-UP template may be configured under
        -- IPD Application. OPD review tasks must always appear under OPD.
        IF candidate.scenario_key = 'opd_assessment_review_pending' THEN
            SELECT tot.TypeofTaskid INTO template_record.tasktype
              FROM TypeofTask tot
             WHERE lower(btrim(tot.tasktype)) = 'opd'
               AND COALESCE(tot.isdeleted, false) = false
               AND (tot.tenantid IS NOT DISTINCT FROM candidate.tenant_id
                    OR tot.tenantid IS NULL)
             ORDER BY CASE WHEN tot.tenantid IS NOT DISTINCT FROM candidate.tenant_id THEN 0 ELSE 1 END
             LIMIT 1;
            IF template_record.tasktype IS NULL THEN
                errors := errors + 1;
                RAISE WARNING 'OPD Daily Task category was not found for tenant %', candidate.tenant_id;
                CONTINUE;
            END IF;
        END IF;

        IF EXISTS (SELECT 1 FROM DailyTask dt WHERE dt.tenantid IS NOT DISTINCT FROM candidate.tenant_id
                    AND ((candidate.source_type = 'IPD' AND dt.ipdreferencenumber = candidate.source_id)
                      OR (candidate.source_type = 'OPD' AND dt.opdreferencenumber = candidate.source_id))
                    AND dt.taskname = template_record.TaskTemplateid AND dt.tasktype = template_record.tasktype
                    AND COALESCE(dt.isdeleted, false) = false
                    AND lower(btrim(COALESCE(dt.status, ''))) NOT IN ('completed','closed','cancelled','canceled'))
        THEN tasks_skipped := tasks_skipped + 1; CONTINUE; END IF;

        current_task_number := current_task_number + 1;
        INSERT INTO DailyTask
            (DailyTaskid, tenantid, taskno, dateandtime, tasktype, taskname, patientname,
             ipdreferencenumber, opdreferencenumber, status, createduser, createddate, isdeleted)
        VALUES
            (gen_random_uuid(), candidate.tenant_id,
             task_number_prefix || '-' || to_char(current_task_number, 'FM00000'), pvar_business_date::timestamp,
             template_record.tasktype, template_record.TaskTemplateid, candidate.patient_id,
             CASE WHEN candidate.source_type = 'IPD' THEN candidate.source_id END,
             CASE WHEN candidate.source_type = 'OPD' THEN candidate.source_id END,
             'Pending', candidate.created_user, clock_timestamp(), false);
        tasks_created := tasks_created + 1;
    END LOOP;
    RETURN NEXT;
END
$BODY$;
