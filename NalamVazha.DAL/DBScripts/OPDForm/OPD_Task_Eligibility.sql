DROP FUNCTION IF EXISTS public."Validate_OPD_Task_Eligibility"(uuid, uuid, timestamp without time zone);
DROP FUNCTION IF EXISTS public."Validate_OPD_Task_Eligibility"(uuid, uuid, timestamp without time zone, uuid);
DROP FUNCTION IF EXISTS public."Get_OPD_Task_Eligibility"(uuid, timestamp without time zone);
DROP FUNCTION IF EXISTS public."Get_OPD_Task_Eligibility"(uuid, timestamp without time zone, uuid);

CREATE OR REPLACE FUNCTION public."Get_OPD_Task_Eligibility"(
    pvar_patientname uuid,
    pvar_asof timestamp without time zone DEFAULT NOW(),
    pvar_excludeopdformid uuid DEFAULT NULL)
RETURNS varchar
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_hasopd boolean;
    lvar_hasipd boolean;
BEGIN
    SELECT EXISTS (
    SELECT 1
    FROM OPDForm opd
    WHERE opd.patientname = pvar_patientname
      AND COALESCE(opd.isdeleted, false) = false
      AND (pvar_excludeopdformid IS NULL OR opd.opdformid <> pvar_excludeopdformid)
      AND LOWER(BTRIM(COALESCE(opd.verifiedstatus, ''))) NOT LIKE '%cancel%'
      AND LOWER(BTRIM(COALESCE(opd.verifiedstatus, ''))) NOT LIKE '%reject%'
      AND opd.createddate <= pvar_asof
    ) INTO lvar_hasopd;

    SELECT EXISTS (
    SELECT 1
    FROM IPDApplicationForm ipd
    WHERE ipd.patientname = pvar_patientname
      AND COALESCE(ipd.isdeleted, false) = false
      AND LOWER(BTRIM(COALESCE(ipd.bookingstatus, ''))) NOT LIKE '%cancel%'
      AND LOWER(BTRIM(COALESCE(ipd.bookingstatus, ''))) NOT LIKE '%reject%'
      AND ipd.createddate <= pvar_asof
    ) INTO lvar_hasipd;

    IF NOT lvar_hasopd AND NOT lvar_hasipd THEN
        RETURN 'OP New';
    END IF;

    RETURN 'OP Follow-up';
END;
$BODY$;

CREATE OR REPLACE FUNCTION public."Validate_OPD_Task_Eligibility"(
    pvar_patientname uuid,
    pvar_task uuid,
    pvar_asof timestamp without time zone DEFAULT NOW(),
    pvar_excludeopdformid uuid DEFAULT NULL)
RETURNS varchar
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_expected varchar;
    lvar_taskname varchar;
    lvar_normalizedtaskname varchar;
BEGIN
    lvar_expected := "Get_OPD_Task_Eligibility"(
        pvar_patientname,
        pvar_asof,
        pvar_excludeopdformid);

    SELECT taskname INTO lvar_taskname
    FROM Task
    WHERE taskid = pvar_task
      AND COALESCE(isdeleted, false) = false;

    IF lvar_taskname IS NULL THEN
        RETURN 'Please choose a valid OPD consultation task.';
    END IF;

    lvar_normalizedtaskname := REGEXP_REPLACE(
        LOWER(lvar_taskname),
        '[^a-z0-9]',
        '',
        'g');

    IF lvar_expected = 'OP New'
       AND lvar_normalizedtaskname IN ('opfollowup', 'onlineopfollowup') THEN
        RETURN 'No previous OPD or IPD booking was found. Please choose OP New or Online OP New.';
    END IF;

    IF lvar_expected = 'OP Follow-up'
       AND lvar_normalizedtaskname IN ('opnew', 'onlineopnew') THEN
        RETURN 'The patient has a previous OPD or IPD booking. Please choose OP Follow-up or Online OP Follow-up.';
    END IF;

    IF lvar_normalizedtaskname NOT IN (
        'opnew',
        'opfollowup',
        'onlineopnew',
        'onlineopfollowup'
    ) THEN
        RETURN 'Please choose a valid OP New or OP Follow-up consultation task.';
    END IF;

    RETURN '201.1';
END;
$BODY$;
