CREATE OR REPLACE FUNCTION public."Add_OPD_Manual_Receivable"(
    pvar_opdformid uuid,
    pvar_tenantid uuid,
    pvar_patientname uuid,
    pvar_receivablefor varchar,
    pvar_amount numeric,
    pvar_remarks varchar,
    pvar_createduser uuid)
RETURNS varchar
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_status varchar;
    lvar_receivableno varchar;
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM users u
        WHERE u.usersid = pvar_createduser
          AND LOWER(TRIM(COALESCE(u.userrole, ''))) = 'frontdesk admin'
          AND COALESCE(u.isdeleted, false) = false) THEN
        RETURN 'Only Front Desk can add an OPD receivable.';
    END IF;

    IF COALESCE(pvar_amount, 0) <= 0 THEN
        RETURN 'Receivable amount must be greater than zero.';
    END IF;

    PERFORM pg_advisory_xact_lock(hashtext('opd-receivable:' || pvar_opdformid::text));

    SELECT verifiedstatus
    INTO lvar_status
    FROM OPDForm
    WHERE opdformid = pvar_opdformid
      AND COALESCE(isdeleted, false) = false
    FOR UPDATE;

    IF NOT FOUND THEN
        RETURN 'OPD form was not found.';
    END IF;

    IF LOWER(TRIM(COALESCE(lvar_status, ''))) NOT IN (
        'approved',
        'opd approved',
        'assessment pending',
        'assessment in progress',
        'assessment intern review pending',
        'assessment doctor review pending',
        'opd completed') THEN
        RETURN 'A receivable cannot be added at the current OPD stage.';
    END IF;

    IF EXISTS (
        SELECT 1
        FROM Receivable r
        WHERE r.opdnumber = pvar_opdformid
          AND LOWER(TRIM(COALESCE(r.receivablefor, ''))) =
              LOWER(TRIM(COALESCE(pvar_receivablefor, '')))
          AND COALESCE(r.isdeleted, false) = false
          AND LOWER(TRIM(COALESCE(r.paymentstatus, ''))) NOT IN
              ('cancelled', 'canceled', 'refunded')) THEN
        RETURN 'An active receivable already exists for this OPD and receivable type.';
    END IF;

    SELECT to_char(CURRENT_DATE, 'YYYYMMDD') || '-' ||
           to_char(COALESCE(MAX(
               CASE WHEN RIGHT(r.receivableno, 5) ~ '^[0-9]+$'
                    THEN RIGHT(r.receivableno, 5)::integer ELSE 0 END), 0) + 1, 'fm00000')
    INTO lvar_receivableno
    FROM Receivable r
    WHERE LEFT(COALESCE(r.receivableno, ''), 8) = to_char(CURRENT_DATE, 'YYYYMMDD')
      AND r.receivableno NOT LIKE '%/%';

    INSERT INTO Receivable(
        receivableid, tenantid, receivableno, receivabledate,
        patientname, opdnumber, receivablefor, amount, paidamount,
        paymentstatus, remarks, createduser, createddate, isdeleted)
    VALUES (
        gen_random_uuid(), pvar_tenantid, lvar_receivableno, CURRENT_DATE,
        pvar_patientname, pvar_opdformid, pvar_receivablefor, pvar_amount, 0,
        'Pending', pvar_remarks, pvar_createduser, NOW(), false);

    RETURN '201.1';
END;
$BODY$;
