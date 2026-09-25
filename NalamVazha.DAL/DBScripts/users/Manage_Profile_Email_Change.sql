CREATE OR REPLACE FUNCTION "Manage_Profile_Email_Change"
(
    pvar_profiletype varchar,
    pvar_profileid uuid,
    pvar_tenantid uuid,
    pvar_newemail varchar,
    pvar_modifieduser uuid,
    pvar_apply boolean,
    OUT pvar_returnmessage varchar
)
RETURNS varchar
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_profiletype varchar := upper(trim(COALESCE(pvar_profiletype, '')));
    lvar_newemail varchar := lower(trim(COALESCE(pvar_newemail, '')));
    lvar_current_email varchar;
    lvar_linked_userid uuid;
BEGIN
    pvar_returnmessage := '';

    IF lvar_profiletype NOT IN ('PEOPLE', 'PATIENTPROFILE') THEN
        pvar_returnmessage := 'Invalid profile type.';
    ELSIF lvar_newemail = '' THEN
        pvar_returnmessage := 'Email address is required.';
    ELSIF lvar_profiletype = 'PEOPLE'
          AND NOT "Check_Authorization"(pvar_modifieduser, 'People', 'edit') THEN
        pvar_returnmessage := '401.1';
    ELSIF lvar_profiletype = 'PATIENTPROFILE'
          AND NOT "Check_Authorization"(pvar_modifieduser, 'PatientProfile', 'edit') THEN
        pvar_returnmessage := '401.1';
    END IF;

    IF pvar_returnmessage = '' AND lvar_profiletype = 'PEOPLE' THEN
        SELECT emailid INTO lvar_current_email
        FROM People
        WHERE Peopleid = pvar_profileid
          AND tenantid = pvar_tenantid
          AND COALESCE(isdeleted, false) = false;

        SELECT usersid INTO lvar_linked_userid
        FROM users
        WHERE usersid = pvar_profileid
          AND tenantid = pvar_tenantid
          AND COALESCE(isdeleted, false) = false;
    ELSIF pvar_returnmessage = '' THEN
        SELECT emailaddress INTO lvar_current_email
        FROM PatientProfile
        WHERE PatientProfileid = pvar_profileid
          AND tenantid = pvar_tenantid
          AND COALESCE(isdeleted, false) = false;

        SELECT usersid INTO lvar_linked_userid
        FROM users
        WHERE tenantid = pvar_tenantid
          AND userrole = 'Health Seeker'
          AND COALESCE(isdeleted, false) = false
          AND (usersid = pvar_profileid
               OR upper(emailid) = upper(lvar_current_email)
               OR upper(username) = upper(lvar_current_email))
        ORDER BY CASE WHEN usersid = pvar_profileid THEN 0 ELSE 1 END
        LIMIT 1;
    END IF;

    IF pvar_returnmessage = '' AND lvar_current_email IS NULL THEN
        pvar_returnmessage := 'Profile not found. Email was not changed.';
    ELSIF pvar_returnmessage = '' AND lvar_linked_userid IS NULL THEN
        pvar_returnmessage := 'Linked user account not found. Email was not changed.';
    ELSIF pvar_returnmessage = '' AND EXISTS (
        SELECT 1 FROM users
        WHERE usersid <> lvar_linked_userid
          AND COALESCE(isdeleted, false) = false
          AND (upper(username) = upper(lvar_newemail)
               OR upper(emailid) = upper(lvar_newemail))
    ) THEN
        pvar_returnmessage := 'Email ID Already Exists.';
    END IF;

    IF pvar_returnmessage = '' AND pvar_apply THEN
        INSERT INTO history
        VALUES(lvar_profiletype, NOW(),
            CASE WHEN lvar_profiletype = 'PEOPLE'
                THEN (SELECT query_to_xml('SELECT * FROM People WHERE Peopleid=''' || pvar_profileid || '''', true, false, ''))
                ELSE (SELECT query_to_xml('SELECT * FROM PatientProfile WHERE PatientProfileid=''' || pvar_profileid || '''', true, false, ''))
            END);

        INSERT INTO history
        VALUES('users', NOW(),
            (SELECT query_to_xml('SELECT * FROM users WHERE usersid=''' || lvar_linked_userid || '''', true, false, '')));

        IF lvar_profiletype = 'PEOPLE' THEN
            UPDATE People SET emailid = lvar_newemail,
                modifieduser = pvar_modifieduser, modifieddate = NOW()
            WHERE Peopleid = pvar_profileid;
        ELSE
            UPDATE PatientProfile SET emailaddress = lvar_newemail,
                modifieduser = pvar_modifieduser, modifieddate = NOW()
            WHERE PatientProfileid = pvar_profileid;
        END IF;

        UPDATE users SET username = lvar_newemail, emailid = lvar_newemail,
            modifieduser = pvar_modifieduser, modifieddate = NOW()
        WHERE usersid = lvar_linked_userid;
    END IF;

    IF pvar_returnmessage = '' THEN
        pvar_returnmessage := '201.1';
    END IF;
EXCEPTION
    WHEN unique_violation THEN
        pvar_returnmessage := 'Email ID Already Exists.';
    WHEN OTHERS THEN
        pvar_returnmessage := SQLERRM;
END;
$BODY$;
