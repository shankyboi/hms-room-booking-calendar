DROP TRIGGER IF EXISTS trg_sync_opd_status_after_assessment_save ON Assessment;
DROP FUNCTION IF EXISTS public."Sync_OPD_Status_After_Assessment_Save"();

CREATE OR REPLACE FUNCTION public."Get_OPD_Assigned_Doctor"(
    pvar_opdformid uuid)
RETURNS uuid
LANGUAGE sql
STABLE
AS $BODY$
    SELECT COALESCE((
        SELECT ca.practitioner
        FROM ClinicalAppointment ca
        WHERE ca.bookingid = o.opdformid::varchar
          AND COALESCE(ca.isdeleted, false) = false
          AND LOWER(COALESCE(ca.status, '')) NOT IN
              ('cancelled', 'canceled', 'rescheduled', 'resheduled')
        ORDER BY ca.appointmentdate DESC, ca.createddate DESC
        LIMIT 1
    ), o.preferreddoctor)
    FROM OPDForm o
    WHERE o.opdformid = pvar_opdformid
      AND COALESCE(o.isdeleted, false) = false;
$BODY$;

CREATE OR REPLACE FUNCTION public."Get_OPD_Appointment_Mapped_Intern"(
    pvar_opdformid uuid)
RETURNS uuid
LANGUAGE sql
STABLE
AS $BODY$
    SELECT dim.interndoctor
    FROM OPDForm o
    LEFT JOIN LATERAL (
        SELECT c.appointmentdate
        FROM ClinicalAppointment c
        WHERE c.bookingid = o.opdformid::varchar
          AND COALESCE(c.isdeleted, false) = false
          AND LOWER(COALESCE(c.status, '')) NOT IN ('cancelled', 'canceled')
        ORDER BY c.appointmentdate DESC, c.createddate DESC
        LIMIT 1
    ) ca ON true
    INNER JOIN LATERAL (
        SELECT mapping.interndoctor
        FROM DoctorInternMap mapping
        WHERE mapping.seniordoctor = "Get_OPD_Assigned_Doctor"(o.opdformid)
          AND COALESCE(mapping.isdeleted, false) = false
          AND mapping.createddate::date <= COALESCE(ca.appointmentdate::date, CURRENT_DATE)
        ORDER BY mapping.createddate DESC
        LIMIT 1
    ) dim ON true
    WHERE o.opdformid = pvar_opdformid
      AND COALESCE(o.isdeleted, false) = false;
$BODY$;

CREATE OR REPLACE FUNCTION public."Can_Review_OPD_Assessment"(
    pvar_opdformid uuid,
    pvar_userid uuid)
RETURNS boolean
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_peopleid uuid;
BEGIN
    SELECT * INTO lvar_peopleid
    FROM "Get_PeopleId_By_UserId"(pvar_userid::varchar)
    LIMIT 1;

    RETURN EXISTS (
        SELECT 1 FROM OPDForm o
        WHERE o.opdformid = pvar_opdformid
          AND COALESCE(o.isdeleted, false) = false
          AND (
              (o.verifiedstatus = 'Assessment Intern Review Pending' AND (
                   ("Get_OPD_Assigned_Doctor"(o.opdformid) IN (lvar_peopleid, pvar_userid)
                    OR o.preferreddoctor IN (lvar_peopleid, pvar_userid))
                   OR EXISTS (
                       SELECT 1
                       FROM DoctorInternMap dim
                       WHERE dim.seniordoctor = "Get_OPD_Assigned_Doctor"(o.opdformid)
                         AND dim.interndoctor IN (lvar_peopleid, pvar_userid)
                         AND COALESCE(dim.isdeleted, false) = false
                         AND EXISTS (
                             SELECT 1
                             FROM (
                                 SELECT ca.appointmentdate, ca.durationfrom, ca.status
                                 FROM ClinicalAppointment ca
                                 WHERE ca.bookingid = o.opdformid::varchar
                                   AND COALESCE(ca.isdeleted, false) = false
                                   AND LOWER(COALESCE(ca.status, '')) NOT IN
                                       ('cancelled', 'canceled', 'rescheduled', 'resheduled')
                                 ORDER BY ca.appointmentdate DESC, ca.createddate DESC
                                 LIMIT 1
                             ) consultation
                             WHERE (
                                 consultation.appointmentdate
                                 + COALESCE(NULLIF(BTRIM(consultation.durationfrom), '')::time, time '00:00')
                             ) > LOCALTIMESTAMP
                               AND LOWER(COALESCE(consultation.status, '')) <> 'completed'
                               AND dim.createddate::date <= consultation.appointmentdate::date
                         )
                   )
               ) AND (
                   /* Apply the online payment gate equally to the allotted
                      doctor and the mapped intern. A zero/negative net
                      receivable is payment-complete via the shared helper. */
                   LOWER(COALESCE(o.appointmentmode, '')) NOT LIKE '%online%'
                   OR "OPD_Has_Successful_Payment"(o.opdformid)
               ))
              OR
              (o.verifiedstatus = 'Assessment Doctor Review Pending'
               AND ("Get_OPD_Assigned_Doctor"(o.opdformid) IN (lvar_peopleid, pvar_userid)
                    OR o.preferreddoctor IN (lvar_peopleid, pvar_userid))
               AND (
                   SELECT ca.appointmentdate::date
                   FROM ClinicalAppointment ca
                   WHERE ca.bookingid = o.opdformid::varchar
                     AND COALESCE(ca.isdeleted, false) = false
                     AND LOWER(COALESCE(ca.status, '')) NOT IN
                         ('cancelled', 'canceled', 'rescheduled', 'resheduled')
                   ORDER BY ca.appointmentdate DESC, ca.createddate DESC
                   LIMIT 1
               ) = CURRENT_DATE
              AND (
                  LOWER(COALESCE(o.appointmentmode, '')) NOT LIKE '%online%'
                  OR "OPD_Has_Successful_Payment"(o.opdformid)
              )
          )
    ));
END;
$BODY$;

CREATE OR REPLACE FUNCTION public."Can_Save_OPD_Assessment"(
    pvar_opdformid uuid,
    pvar_userid uuid)
RETURNS boolean
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_status varchar;
BEGIN
    SELECT verifiedstatus INTO lvar_status
    FROM OPDForm
    WHERE opdformid = pvar_opdformid
      AND COALESCE(isdeleted, false) = false;

    IF NOT FOUND THEN
        RETURN false;
    END IF;

    RETURN lvar_status NOT IN ('Assessment Intern Review Pending', 'Assessment Doctor Review Pending')
        OR "Can_Review_OPD_Assessment"(pvar_opdformid, pvar_userid);
END;
$BODY$;

CREATE OR REPLACE FUNCTION public."Advance_OPD_Assessment_Status"(
    pvar_opdformid uuid,
    pvar_userid uuid)
RETURNS varchar
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_status varchar;
    lvar_assigneddoctor uuid;
    lvar_peopleid uuid;
    lvar_is_online boolean := false;
    lvar_nextstatus varchar := 'Assessment Intern Review Pending';
BEGIN
    SELECT o.verifiedstatus,
           "Get_OPD_Assigned_Doctor"(o.opdformid),
           LOWER(COALESCE(o.appointmentmode, '')) LIKE '%online%'
      INTO lvar_status, lvar_assigneddoctor, lvar_is_online
    FROM OPDForm o
    WHERE o.opdformid = pvar_opdformid
      AND COALESCE(o.isdeleted, false) = false
    FOR UPDATE;

    IF NOT FOUND THEN
        RETURN 'OPD form was not found.';
    END IF;
    IF lvar_assigneddoctor IS NULL THEN
        RETURN 'An allotted doctor is required before assessment review.';
    END IF;

    -- Online OPD assessments are not released to the doctors until payment
    -- succeeds. The payment trigger below releases an already-saved assessment;
    -- this check also handles the inverse order where payment happens first.
    IF lvar_status NOT IN ('Assessment Form - In Draft', 'Assessment Intern Review Pending', 'Assessment Doctor Review Pending', 'OPD Completed')
       AND lvar_is_online
       AND NOT "OPD_Has_Successful_Payment"(pvar_opdformid) THEN
        RETURN '201.1';
    END IF;

    IF lvar_status = 'OPD Completed' THEN
        RETURN '201.1';
    END IF;
       SELECT * INTO lvar_peopleid
    FROM "Get_PeopleId_By_UserId"(pvar_userid::varchar)
    LIMIT 1;
    IF lvar_status = 'Assessment Intern Review Pending' THEN
        IF NOT "Can_Review_OPD_Assessment"(pvar_opdformid, pvar_userid) THEN
            RETURN 'Only the allotted doctor or an active mapped intern can review this assessment.';
        END IF;
        lvar_nextstatus := CASE
            WHEN lvar_assigneddoctor IN (lvar_peopleid, pvar_userid) THEN 'OPD Completed'
            ELSE 'Assessment Doctor Review Pending'
        END;
    ELSIF lvar_status = 'Assessment Doctor Review Pending' THEN
        IF NOT "Can_Review_OPD_Assessment"(pvar_opdformid, pvar_userid) THEN
            RETURN 'Only the allotted senior doctor can review this assessment.';
        END IF;
        lvar_nextstatus := 'OPD Completed';
    END IF;

    UPDATE OPDForm
    SET verifiedstatus = lvar_nextstatus,
        verifiedby = pvar_userid,
        verifieddate = NOW(),
        modifieduser = pvar_userid,
        modifieddate = NOW()
    WHERE opdformid = pvar_opdformid;

    INSERT INTO reviewlogsOPDForm(opdformid, verifiedstatus, reviewcomments, createduser)
    VALUES (pvar_opdformid, lvar_nextstatus, 'Assessment workflow update', pvar_userid);

    RETURN '201.1';
END;
$BODY$;

CREATE OR REPLACE FUNCTION public."Save_OPD_Assessment_Draft_Status"(
    pvar_opdformid uuid,
    pvar_userid uuid)
RETURNS varchar
LANGUAGE plpgsql
AS $BODY$
BEGIN
    UPDATE OPDForm
    SET verifiedstatus = 'Assessment Form - In Draft',
        verifiedby = pvar_userid,
        verifieddate = NOW(),
        modifieduser = pvar_userid,
        modifieddate = NOW()
    WHERE opdformid = pvar_opdformid
      AND COALESCE(isdeleted, false) = false
      AND COALESCE(verifiedstatus, '') NOT IN
          ('Assessment Intern Review Pending', 'Assessment Doctor Review Pending', 'OPD Completed');

    IF NOT FOUND THEN
        RETURN 'The OPD assessment draft status could not be updated.';
    END IF;

    INSERT INTO reviewlogsOPDForm(opdformid, verifiedstatus, reviewcomments, createduser)
    VALUES (pvar_opdformid, 'Assessment Form - In Draft',
            'Assessment saved as draft', pvar_userid);

    RETURN '201.1';
END;
$BODY$;

CREATE OR REPLACE FUNCTION public."Release_OPD_Assessment_After_Payment"(
    pvar_opdformid uuid,
    pvar_userid uuid)
RETURNS boolean
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_released boolean := false;
BEGIN
    IF pvar_opdformid IS NULL
       OR NOT "OPD_Has_Successful_Payment"(pvar_opdformid) THEN
        RETURN false;
    END IF;

    UPDATE OPDForm o
    SET verifiedstatus = 'Assessment Intern Review Pending',
        verifiedby = COALESCE(pvar_userid, o.verifiedby),
        verifieddate = NOW(),
        modifieduser = COALESCE(pvar_userid, o.modifieduser),
        modifieddate = NOW()
    WHERE o.opdformid = pvar_opdformid
      AND COALESCE(o.isdeleted, false) = false
      AND LOWER(COALESCE(o.verifiedstatus, '')) IN
          ('approved', 'opd approved', 'assessment pending', 'assessment in progress')
      AND LOWER(COALESCE(o.appointmentmode, '')) LIKE '%online%'
      AND EXISTS (
          SELECT 1
          FROM Assessment a
          WHERE a.opdform = o.opdformid
            AND COALESCE(a.isdeleted, false) = false);

    lvar_released := FOUND;
    IF lvar_released AND pvar_userid IS NOT NULL THEN
        INSERT INTO reviewlogsOPDForm(opdformid, verifiedstatus, reviewcomments, createduser)
        VALUES (pvar_opdformid, 'Assessment Intern Review Pending',
                'Online OPD payment completed; assessment released for review', pvar_userid);
    END IF;

    RETURN lvar_released;
END;
$BODY$;

CREATE OR REPLACE FUNCTION public."Sync_OPD_Assessment_After_Receivable_Change"()
RETURNS trigger
LANGUAGE plpgsql
AS $BODY$
BEGIN
    /* Add Receivable can fully waive an Online OPD without creating a
       BillingPayment row. Re-evaluate the shared payment-complete predicate
       whenever an OPD receivable changes so a saved assessment is released. */
    IF NEW.opdnumber IS NOT NULL THEN
        PERFORM "Release_OPD_Assessment_After_Payment"(
            NEW.opdnumber,
            COALESCE(NEW.modifieduser, NEW.createduser));
    END IF;
    RETURN NEW;
END;
$BODY$;

DROP TRIGGER IF EXISTS trg_sync_opd_assessment_after_receivable ON Receivable;
CREATE TRIGGER trg_sync_opd_assessment_after_receivable
AFTER INSERT OR UPDATE OF amount, paidamount, paymentstatus, isdeleted, opdnumber ON Receivable
FOR EACH ROW
EXECUTE FUNCTION public."Sync_OPD_Assessment_After_Receivable_Change"();

CREATE OR REPLACE FUNCTION public."Sync_OPD_Assessment_After_Payment"()
RETURNS trigger
LANGUAGE plpgsql
AS $BODY$
BEGIN
    IF NEW.opdnumber IS NOT NULL
       AND COALESCE(NEW.amount, 0) > 0
       AND LOWER(COALESCE(NEW.paymentstatus, '')) IN ('paid', 'success', 'completed')
       AND LOWER(COALESCE(NEW.receivablefor, '')) NOT LIKE '%refund%' THEN
        PERFORM "Release_OPD_Assessment_After_Payment"(
            NEW.opdnumber,
            COALESCE(NEW.createduser, NEW.collectedby));
    END IF;
    RETURN NEW;
END;
$BODY$;

DROP TRIGGER IF EXISTS trg_sync_opd_assessment_after_payment ON BillingPayment;
CREATE TRIGGER trg_sync_opd_assessment_after_payment
AFTER INSERT OR UPDATE OF paymentstatus, amount, opdnumber ON BillingPayment
FOR EACH ROW
EXECUTE FUNCTION public."Sync_OPD_Assessment_After_Payment"();

-- Normalize records completed under the former final-status label.
UPDATE OPDForm
SET verifiedstatus = 'OPD Completed'
WHERE verifiedstatus = 'Assessment Approved';

DROP FUNCTION IF EXISTS public."Get_OPD_Online_Doctor_Workflow_Context"(uuid);

CREATE OR REPLACE FUNCTION public."Get_OPD_Online_Doctor_Workflow_Context"(
    pvar_opdformid uuid)
RETURNS TABLE(
    opdformid uuid, tenantid uuid, bookingreferencenumber varchar,
    patientname varchar, patientemail varchar, appointmentmode varchar, taskname varchar, verifiedstatus varchar,
    appointmentdate date, durationfrom varchar, durationto varchar,
    seniordoctor uuid, seniordoctorname varchar, seniordoctoremail varchar, seniormeetinglink varchar,
    interndoctor uuid, interndoctorname varchar, interndoctoremail varchar, internmeetinglink varchar)
LANGUAGE plpgsql
AS $BODY$
BEGIN
    RETURN QUERY
    SELECT o.opdformid, o.tenantid, o.bookingreferencenumber,
           TRIM(CONCAT_WS(' ', pp.firstname, pp.lastname))::varchar,
           pp.emailaddress::varchar,
           o.appointmentmode, t.taskname, o.verifiedstatus,
           ca.appointmentdate::date, ca.durationfrom::varchar, ca.durationto::varchar,
           senior.peopleid,
           TRIM(CONCAT_WS(' ', senior.firstname, senior.lastname))::varchar,
           senior.emailid::varchar, senior.screeningmeetinglink::varchar,
           intern.peopleid,
           TRIM(CONCAT_WS(' ', intern.firstname, intern.lastname))::varchar,
           intern.emailid::varchar, intern.screeningmeetinglink::varchar
    FROM OPDForm o
    INNER JOIN PatientProfile pp ON pp.patientprofileid = o.patientname
    LEFT JOIN Task t ON t.taskid = o.task
    LEFT JOIN People senior ON senior.peopleid = "Get_OPD_Assigned_Doctor"(o.opdformid)
    LEFT JOIN LATERAL (
        SELECT c.appointmentdate, c.durationfrom, c.durationto
        FROM ClinicalAppointment c
        WHERE c.bookingid = o.opdformid::varchar
          AND COALESCE(c.isdeleted, false) = false
          AND LOWER(COALESCE(c.status, '')) NOT IN ('cancelled', 'canceled')
        ORDER BY c.appointmentdate DESC, c.createddate DESC
        LIMIT 1
    ) ca ON true
    LEFT JOIN People intern
      ON intern.peopleid = "Get_OPD_Appointment_Mapped_Intern"(o.opdformid)
    WHERE o.opdformid = pvar_opdformid
      AND COALESCE(o.isdeleted, false) = false;
END;
$BODY$;
