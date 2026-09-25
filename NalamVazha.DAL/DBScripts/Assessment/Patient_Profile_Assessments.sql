
CREATE OR REPLACE FUNCTION public."Patient_Profile_Assessments"
(
    pvar_tenantid character varying DEFAULT NULL::character varying,
    pvar_patientname character varying DEFAULT NULL::character varying
)
RETURNS TABLE
(
    assessmentid uuid,
    patientname uuid,
    patientvisit uuid,
    ipdform uuid,
    opdform uuid,
    assessmentdate timestamp,
    createddate timestamp,
    ipdform_master Varchar,
    opdform_master Varchar,
    patientvisit_master Varchar,
    assessmenttype Varchar,
    assessmentref Varchar
)
LANGUAGE 'plpgsql'
COST 100
VOLATILE PARALLEL UNSAFE
ROWS 1000
AS $BODY$
DECLARE
    lvar_tenantid varchar[];
    lstr_usersid varchar;
BEGIN
    SELECT SPLIT_PART(pvar_tenantid, '|', 1), SPLIT_PART(pvar_tenantid, '|', 2)
    INTO lstr_usersid, pvar_tenantid;

    IF pvar_tenantid IS NULL OR pvar_tenantid = '' OR pvar_tenantid = '00000000-0000-0000-0000-000000000000' THEN
        SELECT STRING_TO_ARRAY(viewertenantids, ',') INTO lvar_tenantid
        FROM users
        WHERE users.usersid::varchar = lstr_usersid;

        IF lvar_tenantid IS NULL THEN
            SELECT array_agg(tenant.tenantid::varchar) INTO lvar_tenantid FROM tenant;
        END IF;
    ELSE
        lvar_tenantid = ARRAY[pvar_tenantid];
    END IF;

    lvar_tenantid := lvar_tenantid || ARRAY[''::character varying] || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];

    RETURN QUERY
    SELECT
        a.assessmentid,
        COALESCE(a.patientname, ipd.patientname, opd.patientname, pv.patientname) AS patientname,
        a.patientvisit,
        a.ipdform,
        a.opdform,
        a.assessmentdate,
        a.createddate::timestamp,
        CAST(ipd.bookingreferencenumber AS varchar) AS ipdform_master,
        CAST(opd.bookingreferencenumber AS varchar) AS opdform_master,
        CAST(pv.visitnumber AS varchar) AS patientvisit_master,
        CAST(
            CASE
                WHEN a.opdform IS NOT NULL THEN 'OPD'
                WHEN a.ipdform IS NOT NULL THEN 'IPD'
                WHEN a.patientvisit IS NOT NULL THEN COALESCE(pv.visittype, 'Visit')
                ELSE 'Assessment'
            END AS varchar
        ) AS assessmenttype,
        CAST(COALESCE(opd.bookingreferencenumber, ipd.bookingreferencenumber, pv.visitnumber, '') AS varchar) AS assessmentref
    FROM Assessment a
    LEFT JOIN IPDApplicationForm ipd ON a.ipdform = ipd.IPDApplicationFormid AND COALESCE(ipd.isdeleted, false) = false
    LEFT JOIN OPDForm opd ON a.opdform = opd.OPDFormid AND COALESCE(opd.isdeleted, false) = false
    LEFT JOIN PatientVisit pv ON a.patientvisit = pv.PatientVisitid AND COALESCE(pv.isdeleted, false) = false
    WHERE COALESCE(a.isdeleted, false) = false
        AND COALESCE(CAST(COALESCE(a.tenantid, ipd.tenantid, opd.tenantid, pv.tenantid) AS varchar), '') = Any(lvar_tenantid)
        AND (
            pvar_patientname IS NULL
            OR pvar_patientname = ''
            OR pvar_patientname = '0'
            OR CAST(a.patientname AS varchar) = pvar_patientname
            OR CAST(ipd.patientname AS varchar) = pvar_patientname
            OR CAST(opd.patientname AS varchar) = pvar_patientname
            OR CAST(pv.patientname AS varchar) = pvar_patientname
        )
    ORDER BY COALESCE(a.assessmentdate, a.createddate::timestamp) DESC;
END
$BODY$;
