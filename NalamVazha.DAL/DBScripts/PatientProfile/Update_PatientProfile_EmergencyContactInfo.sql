CREATE OR REPLACE FUNCTION "Update_PatientProfile_EmergencyContactInfo"
(
    pvar_patientprofileid uuid,
    pvar_emergencycontactinfo json,
    pvar_modifieduser uuid
)
RETURNS varchar
LANGUAGE plpgsql
AS $BODY$
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM PatientProfile
        WHERE PatientProfileid = pvar_patientprofileid
          AND COALESCE(isdeleted, false) = false
    ) THEN
        RETURN 'Patient profile was not found.';
    END IF;

    INSERT INTO history
    VALUES
    (
        'PatientProfile_emergencycontactinfo',
        NOW(),
        (SELECT query_to_xml(
            'SELECT * FROM PatientProfile_emergencycontactinfo WHERE PatientProfileid = ''' ||
            pvar_patientprofileid || '''', true, false, ''))
    );

    DELETE FROM PatientProfile_emergencycontactinfo
    WHERE PatientProfileid = pvar_patientprofileid;

    INSERT INTO PatientProfile_emergencycontactinfo
    (
        PatientProfileid,
        PatientProfile_emergencycontactinfoid,
        record_order,
        personname,
        relationship,
        phonenumber
    )
    SELECT
        pvar_patientprofileid,
        gen_random_uuid(),
        CAST(COALESCE(j->>'record_order', '0') AS integer),
        j->>'personname',
        j->>'relationship',
        j->>'phonenumber'
    FROM json_array_elements(pvar_emergencycontactinfo) AS j;

    UPDATE PatientProfile
    SET modifieduser = pvar_modifieduser,
        modifieddate = NOW()
    WHERE PatientProfileid = pvar_patientprofileid;

    RETURN '201.1';
END;
$BODY$;
