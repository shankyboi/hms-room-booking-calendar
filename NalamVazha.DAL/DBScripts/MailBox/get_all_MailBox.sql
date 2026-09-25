CREATE OR REPLACE FUNCTION public."get_all_MailBox"(
    pvar_tenantid character varying DEFAULT NULL::character varying)
RETURNS TABLE(
    senderdisplayname character varying,
    senderemail character varying,
    password character varying,
    emailhostname character varying,
    portnumber integer,
    applicableservice character varying,
    emailfooter text,
    createduser uuid,
    createddate timestamp without time zone,
    modifieduser uuid,
    modifieddate timestamp without time zone,
    tenantid uuid,
    "MailBoxid" uuid)
LANGUAGE 'plpgsql'
COST 100
VOLATILE
PARALLEL UNSAFE
ROWS 1000
AS $BODY$
DECLARE
    lvar_tenantid varchar[];
    lstr_usersid varchar;
    lstr_input varchar;
    lstr_effective_tenantid varchar;
BEGIN
    lstr_input := NULLIF(TRIM(pvar_tenantid), '');

    IF lstr_input IS NOT NULL AND POSITION('|' IN lstr_input) > 0 THEN
        lstr_usersid := NULLIF(SPLIT_PART(lstr_input, '|', 1), '');
        lstr_effective_tenantid := NULLIF(SPLIT_PART(lstr_input, '|', 2), '');
    ELSE
        lstr_usersid := NULL;
        lstr_effective_tenantid := lstr_input;
    END IF;

    IF lstr_effective_tenantid IS NULL
       OR lstr_effective_tenantid = ''
       OR lstr_effective_tenantid = '00000000-0000-0000-0000-000000000000'
    THEN
        IF lstr_usersid IS NOT NULL THEN
            SELECT STRING_TO_ARRAY(viewertenantids, ',')
            INTO lvar_tenantid
            FROM users
            WHERE users.usersid::varchar = lstr_usersid;
        END IF;

        IF lvar_tenantid IS NULL THEN
            SELECT array_agg(tenant.tenantid::varchar)
            INTO lvar_tenantid
            FROM tenant;
        END IF;
    ELSE
        lvar_tenantid := ARRAY[lstr_effective_tenantid];
    END IF;

    lvar_tenantid := lvar_tenantid
        || ARRAY[''::character varying]
        || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];

    RETURN QUERY
    SELECT
        MailBox.senderdisplayname,
        MailBox.senderemail,
        MailBox.password,
        MailBox.emailhostname,
        MailBox.portnumber,
        MailBox.applicableservice,
        MailBox.emailfooter,
        MailBox.createduser,
        MailBox.createddate,
        MailBox.modifieduser,
        MailBox.modifieddate,
        MailBox.tenantid,
        MailBox.MailBoxid
    FROM MailBox
    LEFT OUTER JOIN tenant ON MailBox.tenantid = tenant.tenantid
    WHERE
        (lvar_tenantid IS NULL OR COALESCE(CAST(MailBox.tenantid AS varchar), '') = ANY(lvar_tenantid))
        AND MailBox.isdeleted = false
    ORDER BY MailBox.createddate DESC;
END
$BODY$;
