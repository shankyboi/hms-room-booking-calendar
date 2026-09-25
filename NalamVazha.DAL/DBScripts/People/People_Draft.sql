ALTER TABLE People ADD COLUMN IF NOT EXISTS status varchar(16) NOT NULL DEFAULT 'Active';
ALTER TABLE People ADD COLUMN IF NOT EXISTS draftdata jsonb NULL;

ALTER TABLE People ALTER COLUMN firstname DROP NOT NULL;
ALTER TABLE People ALTER COLUMN workprofile DROP NOT NULL;
ALTER TABLE People ALTER COLUMN competencylevel DROP NOT NULL;
ALTER TABLE People ALTER COLUMN designation DROP NOT NULL;
ALTER TABLE People ALTER COLUMN contactnumber DROP NOT NULL;
ALTER TABLE People ALTER COLUMN whatsappnumber DROP NOT NULL;
ALTER TABLE People ALTER COLUMN emailid DROP NOT NULL;
ALTER TABLE People ALTER COLUMN gender DROP NOT NULL;
ALTER TABLE People ALTER COLUMN dob DROP NOT NULL;
ALTER TABLE People ALTER COLUMN employmentstatus DROP NOT NULL;
ALTER TABLE People ALTER COLUMN idtype DROP NOT NULL;
ALTER TABLE People ALTER COLUMN idnumber DROP NOT NULL;
ALTER TABLE People ALTER COLUMN iddocument DROP NOT NULL;
ALTER TABLE People ALTER COLUMN paddressline1 DROP NOT NULL;
ALTER TABLE People ALTER COLUMN pzip DROP NOT NULL;
ALTER TABLE People ALTER COLUMN caddressline1 DROP NOT NULL;
ALTER TABLE People ALTER COLUMN czip DROP NOT NULL;

CREATE INDEX IF NOT EXISTS ix_people_status_tenantid ON People(status, tenantid);

CREATE OR REPLACE FUNCTION "Save_People_Draft"(
    pvar_peopleid uuid,
    pvar_tenantid uuid,
    pvar_profile jsonb,
    pvar_user uuid)
RETURNS uuid
LANGUAGE plpgsql
AS $BODY$
DECLARE
    saved_id uuid;
    saved_practitionerid varchar(256);
    practitioner_prefix varchar(4);
    practitioner_sequence integer;
BEGIN
    SELECT practitionerid
      INTO saved_practitionerid
      FROM People
     WHERE peopleid = pvar_peopleid;

    IF saved_practitionerid IS NULL
       OR BTRIM(saved_practitionerid) = ''
       OR saved_practitionerid = 'YYMM-999' THEN
        practitioner_prefix := to_char(NOW(), 'YYMM');
        -- Serialize number allocation so two simultaneous saves cannot receive the same ID.
        PERFORM pg_advisory_xact_lock(hashtext('People-practitionerid-' || practitioner_prefix));
        SELECT COALESCE(MAX(RIGHT(practitionerid, 3)::integer), 0) + 1
          INTO practitioner_sequence
          FROM People
         WHERE practitionerid ~ ('^' || practitioner_prefix || '-[0-9]{3}$');
        saved_practitionerid := practitioner_prefix || '-' || to_char(practitioner_sequence, 'FM000');
    END IF;

    pvar_profile := jsonb_set(
        pvar_profile,
        '{practitionerid}',
        to_jsonb(saved_practitionerid),
        true
    );

    INSERT INTO People
    (
        peopleid, tenantid, practitionerid, status, draftdata,
        createduser, createddate, modifieduser, modifieddate
    )
    VALUES
    (
        pvar_peopleid, pvar_tenantid, saved_practitionerid, 'Draft', pvar_profile,
        pvar_user, NOW(), pvar_user, NOW()
    )
    ON CONFLICT (peopleid) DO UPDATE SET
        tenantid = COALESCE(EXCLUDED.tenantid, People.tenantid),
        practitionerid = EXCLUDED.practitionerid,
        status = 'Draft',
        draftdata = EXCLUDED.draftdata,
        modifieduser = EXCLUDED.modifieduser,
        modifieddate = NOW()
    WHERE People.status = 'Draft'
    RETURNING peopleid INTO saved_id;

    RETURN saved_id;
END;
$BODY$;

CREATE OR REPLACE FUNCTION "Get_People_Draft"(pvar_peopleid uuid)
RETURNS text
LANGUAGE sql
AS $BODY$
    SELECT jsonb_set(
        draftdata,
        '{tenantid}',
        COALESCE(
            to_jsonb(NULLIF(draftdata->>'tenantid', '')::uuid),
            to_jsonb(tenantid),
            'null'::jsonb
        ),
        true
    )::text
    FROM People
    WHERE peopleid = pvar_peopleid
      AND status = 'Draft'
      AND isdeleted = false;
$BODY$;

CREATE OR REPLACE FUNCTION "Prepare_People_Draft_For_Activation"(pvar_peopleid uuid)
RETURNS boolean
LANGUAGE plpgsql
AS $BODY$
BEGIN
    DELETE FROM People
    WHERE peopleid = pvar_peopleid
      AND status = 'Draft';

    RETURN FOUND;
END;
$BODY$;
