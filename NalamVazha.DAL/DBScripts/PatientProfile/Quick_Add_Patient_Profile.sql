
CREATE OR REPLACE FUNCTION public."Quick_Add_Patient_Profile"(
	pvar_patientprofileid uuid,
	pvar_tenantid uuid,
	pvar_firstname character varying,
	pvar_lastname character varying,
	pvar_gender character varying,
	pvar_dateofbirth date,
	pvar_age integer,
	pvar_nationality character varying,
	pvar_emailaddress character varying,
	pvar_mobilenumber character varying,
	pvar_whatsappnumber character varying,
	pvar_photo character varying,
	pvar_paddressline1 character varying,
	pvar_paddressline2 character varying,
	pvar_pzip integer,
	pvar_ptown character varying,
	pvar_pcityordistrict character varying,
	pvar_ppstatename character varying,
	pvar_sameaspermanentaddress boolean,
	pvar_caddressline1 character varying,
	pvar_caddressline2 character varying,
	pvar_czip integer,
	pvar_ctown character varying,
	pvar_ccityordistrict character varying,
	pvar_cstatename character varying,
	pvar_idprooftype character varying,
	pvar_idproofnumber character varying,
	pvar_uploadidproof character varying,
	pvar_userpassword character varying,
	pvar_passwordkey character varying,
	pvar_countryoforigin uuid,
	pvar_bloodgroup character varying,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    lvar_curday_registrationid VARCHAR(10);
    lvar_val_registrationid INT;
    lvar_registrationid VARCHAR(50);
BEGIN

   IF pvar_patientprofileid IS NULL THEN
        pvar_patientprofileid := gen_random_uuid();
    END IF;

   lvar_registrationid := generate_formatted_numbers(
        pvar_tenantid,
        'TEMP-YYMMDD-9999',
        'patientprofile',
        'registrationid'
    );

    pvar_returnmessage := '';

	--tenant specific email check
IF EXISTS (
    SELECT 1 FROM PatientProfile
    WHERE LOWER(emailaddress) = LOWER(pvar_emailaddress)
      AND tenantid = pvar_tenantid
      AND COALESCE(isdeleted, false) = false
) THEN
    pvar_returnMessage := 'Email address already exists.';
    RETURN;
END IF;

-- Tenant-specific user email check
IF EXISTS (
    SELECT 1 FROM users
    WHERE LOWER(BTRIM(emailid)) = LOWER(BTRIM(pvar_emailaddress))
      AND tenantid = pvar_tenantid
      AND COALESCE(isdeleted, false) = false
      AND NULLIF(BTRIM(pvar_emailaddress), '') IS NOT NULL
) THEN
    pvar_returnMessage := 'A user account with this email already exists.';
    RETURN;
END IF;
IF EXISTS (
    SELECT 1 FROM PatientProfile
    WHERE LOWER(BTRIM(idproofnumber)) = LOWER(BTRIM(pvar_idproofnumber))
      AND tenantid = pvar_tenantid
      AND COALESCE(isdeleted, false) = false
      AND NULLIF(BTRIM(pvar_idproofnumber), '') IS NOT NULL
) THEN
    pvar_returnMessage := 'ID Proof Number Already Exists.';
    RETURN;
END IF;
    /* ── Insert into PatientProfile ── */
    INSERT INTO PatientProfile (
        PatientProfileid,
        tenantid,
        registrationid,
        firstname,
        lastname,
        gender,
        dateofbirth,
        age,
        nationality,
        countryoforigin,
        emailaddress,
        mobilenumber,
        whatsappnumber,
        photo,
        paddressline1,
        paddressline2,
        pzip,
        ptown,
        pcityordistrict,
        ppstatename,
        sameaspermanentaddress,
        caddressline1,
        caddressline2,
        czip,
        ctown,
        ccityordistrict,
        cstatename,
        idprooftype,
        idproofnumber,
        uploadidproof,
        bloodgroup,
        createduser
    ) VALUES (
        pvar_patientprofileid,
        pvar_tenantid,
        lvar_registrationid,
        pvar_firstname,
        pvar_lastname,
        pvar_gender,
        pvar_dateofbirth,
        pvar_age,
        pvar_nationality,
        pvar_countryoforigin,
        pvar_emailaddress,
        pvar_mobilenumber,
        pvar_whatsappnumber,
        pvar_photo,
        pvar_paddressline1,
        pvar_paddressline2,
        pvar_pzip,
        pvar_ptown,
        pvar_pcityordistrict,
        pvar_ppstatename,
        pvar_sameaspermanentaddress,
        pvar_caddressline1,
        pvar_caddressline2,
        pvar_czip,
        pvar_ctown,
        pvar_ccityordistrict,
        pvar_cstatename,
        pvar_idprooftype,
        pvar_idproofnumber,
        pvar_uploadidproof,
        pvar_bloodgroup,
        pvar_patientprofileid
    );

    /* ── Insert into users (so MailSender SP can find the record) ── */
    INSERT INTO users (
        usersid,
        tenantid,
        firstname,
        lastname,
        profilepicture,
        username,
        userpassword,
        passwordkey,
        emailid,
        mobilenumber,
        userrole,
        createduser
    ) VALUES (
        pvar_patientprofileid,
        pvar_tenantid,
        pvar_firstname,
        pvar_lastname,
        pvar_photo,
        pvar_emailaddress,          -- username = email
        pvar_userpassword,          -- encrypted password
        pvar_passwordkey,
        pvar_emailaddress,          -- emailid = email
        pvar_mobilenumber,
        'Health Seeker',
        pvar_patientprofileid
    );

    pvar_returnMessage := '201.1';

EXCEPTION WHEN OTHERS THEN
    pvar_returnMessage := 'Quick_Add_Patient_Profile failed: ' || SQLERRM;
END;
$BODY$;
