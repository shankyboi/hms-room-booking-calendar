CREATE TABLE IF NOT EXISTS booking_emergency_contact
(
    booking_emergency_contact_id uuid PRIMARY KEY,
    entity_name varchar(32) NOT NULL,
    entity_id uuid NOT NULL,
    record_order integer NOT NULL DEFAULT 0,
    person_name varchar(128) NOT NULL,
    relationship varchar(128) NOT NULL,
    phone_number varchar(32) NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_booking_emergency_contact_entity
    ON booking_emergency_contact(entity_name, entity_id, record_order);

CREATE OR REPLACE FUNCTION "Save_Booking_Emergency_Contacts"
(
    pvar_entity_name varchar,
    pvar_entity_id uuid,
    pvar_contacts_json text
)
RETURNS void
LANGUAGE plpgsql
AS $BODY$
BEGIN
    DELETE FROM booking_emergency_contact
    WHERE entity_name = pvar_entity_name
      AND entity_id = pvar_entity_id;

    INSERT INTO booking_emergency_contact
    (
        booking_emergency_contact_id,
        entity_name,
        entity_id,
        record_order,
        person_name,
        relationship,
        phone_number
    )
    SELECT
        gen_random_uuid(),
        pvar_entity_name,
        pvar_entity_id,
        contact.ordinality - 1,
        trim(contact.value->>'personname'),
        trim(contact.value->>'relationship'),
        trim(contact.value->>'phonenumber')
    FROM jsonb_array_elements(COALESCE(NULLIF(pvar_contacts_json, ''), '[]')::jsonb)
         WITH ORDINALITY AS contact(value, ordinality)
    WHERE NULLIF(trim(contact.value->>'personname'), '') IS NOT NULL
      AND NULLIF(trim(contact.value->>'relationship'), '') IS NOT NULL
      AND NULLIF(trim(contact.value->>'phonenumber'), '') IS NOT NULL;
END;
$BODY$;

-- Save the OPD-specific contact snapshot and also make newly entered contacts
-- available from the patient's profile. Existing profile contacts are kept;
-- matching is performed on the normalized phone number to prevent duplicates.
CREATE OR REPLACE FUNCTION "Save_OPD_Emergency_Contacts"
(
    pvar_opdformid uuid,
    pvar_patientprofileid uuid,
    pvar_contacts_json text,
    pvar_modifieduser uuid
)
RETURNS integer
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_inserted_count integer := 0;
BEGIN
    PERFORM "Save_Booking_Emergency_Contacts"
    (
        'OPDForm',
        pvar_opdformid,
        pvar_contacts_json
    );

    WITH incoming AS
    (
        SELECT
            contact.ordinality,
            BTRIM(contact.value->>'personname') AS personname,
            BTRIM(contact.value->>'relationship') AS relationship,
            BTRIM(contact.value->>'phonenumber') AS phonenumber,
            regexp_replace(contact.value->>'phonenumber', '[^0-9]', '', 'g') AS normalized_phone
        FROM jsonb_array_elements(
            COALESCE(NULLIF(pvar_contacts_json, ''), '[]')::jsonb
        ) WITH ORDINALITY AS contact(value, ordinality)
        WHERE NULLIF(BTRIM(contact.value->>'personname'), '') IS NOT NULL
          AND NULLIF(BTRIM(contact.value->>'relationship'), '') IS NOT NULL
          AND NULLIF(BTRIM(contact.value->>'phonenumber'), '') IS NOT NULL
    ),
    unique_incoming AS
    (
        SELECT DISTINCT ON (normalized_phone)
            ordinality,
            personname,
            relationship,
            phonenumber,
            normalized_phone
        FROM incoming
        WHERE normalized_phone <> ''
        ORDER BY normalized_phone, ordinality
    ),
    profile_order AS
    (
        SELECT COALESCE(MAX(record_order), -1) AS last_order
        FROM PatientProfile_emergencycontactinfo
        WHERE PatientProfileid = pvar_patientprofileid
    ),
    inserted AS
    (
        INSERT INTO PatientProfile_emergencycontactinfo
        (
            PatientProfile_emergencycontactinfoid,
            PatientProfileid,
            record_order,
            personname,
            relationship,
            phonenumber
        )
        SELECT
            gen_random_uuid(),
            pvar_patientprofileid,
            profile_order.last_order + CAST(ROW_NUMBER() OVER (ORDER BY contact.ordinality) AS integer),
            contact.personname,
            contact.relationship,
            contact.phonenumber
        FROM unique_incoming contact
        CROSS JOIN profile_order
        WHERE EXISTS
        (
            SELECT 1
            FROM PatientProfile profile
            WHERE profile.PatientProfileid = pvar_patientprofileid
              AND COALESCE(profile.isdeleted, false) = false
        )
          AND NOT EXISTS
        (
            SELECT 1
            FROM PatientProfile_emergencycontactinfo existing_contact
            WHERE existing_contact.PatientProfileid = pvar_patientprofileid
              AND regexp_replace(existing_contact.phonenumber, '[^0-9]', '', 'g') = contact.normalized_phone
        )
        RETURNING 1
    )
    SELECT COUNT(*) INTO lvar_inserted_count FROM inserted;

    IF lvar_inserted_count > 0 THEN
        UPDATE PatientProfile
        SET modifieduser = pvar_modifieduser,
            modifieddate = NOW()
        WHERE PatientProfileid = pvar_patientprofileid;
    END IF;

    RETURN lvar_inserted_count;
END;
$BODY$;

CREATE OR REPLACE FUNCTION "Get_Booking_Emergency_Contacts"
(
    pvar_entity_name varchar,
    pvar_entity_id uuid
)
RETURNS TABLE
(
    personname varchar,
    relationship varchar,
    phonenumber varchar
)
LANGUAGE sql
STABLE
AS $BODY$
    SELECT
        contact.person_name AS personname,
        contact.relationship,
        contact.phone_number AS phonenumber
    FROM booking_emergency_contact contact
    WHERE contact.entity_name = pvar_entity_name
      AND contact.entity_id = pvar_entity_id
    ORDER BY contact.record_order;
$BODY$;
