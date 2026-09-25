
DROP FUNCTION IF EXISTS public.get_assessment_suggested_templates(uuid, character varying);
DROP FUNCTION IF EXISTS public.get_assessment_suggested_templates(uuid, character varying, character varying);

CREATE OR REPLACE FUNCTION public.get_assessment_suggested_templates(
	p_ipdapplicationform_id uuid,
	p_user_role character varying,
	p_taskname character varying DEFAULT NULL::character varying)
    RETURNS TABLE(assessmenttemplateid character varying, templatename character varying, ispreselected boolean, matchscore integer, isfallback boolean) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    l_patientprofileid     uuid;
    l_gender               varchar;
    l_dateofbirth          date;
    l_age                  int;
    l_agegroup             varchar;
    l_maritalstatus        varchar;
    l_relationshipstatus   varchar;
    l_medicalconditions    uuid[];
    l_taskname             varchar;
    l_tenantid             uuid;
    l_preselected          uuid;
    l_template_count       int;
    l_has_default          boolean;
BEGIN
    /* ---------- R1: Patient details ---------- */
    SELECT ipd.patientname, ipd.tenantid
      INTO l_patientprofileid, l_tenantid
      FROM IPDApplicationForm ipd
     WHERE ipd.IPDApplicationFormid = p_ipdapplicationform_id;

    IF l_patientprofileid IS NULL THEN
        RETURN;
    END IF;

    SELECT pp.gender, pp.dateofbirth, pp.maritalstatus
      INTO l_gender, l_dateofbirth, l_maritalstatus
      FROM PatientProfile pp
     WHERE pp.PatientProfileid = l_patientprofileid;

    l_age := EXTRACT(YEAR FROM AGE(CURRENT_DATE, COALESCE(l_dateofbirth, CURRENT_DATE)))::int;

    l_agegroup := CASE
        WHEN l_age <= 1   THEN 'Infancy'
        WHEN l_age <= 5   THEN 'Early Childhood'
        WHEN l_age <= 12  THEN 'Childhood'
        WHEN l_age <= 19  THEN 'Adolescence'
        WHEN l_age <= 35  THEN 'Young Adulthood'
        WHEN l_age <= 55  THEN 'Middle Adulthood'
        ELSE                   'Late Adulthood'
    END;

    /* ---------- R2: Marital + medical conditions ---------- */
    l_relationshipstatus := COALESCE(l_maritalstatus, '');

    SELECT COALESCE(ARRAY_AGG(mi.medicalconditionname) FILTER (WHERE mi.medicalconditionname IS NOT NULL), ARRAY[]::uuid[])
      INTO l_medicalconditions
      FROM IPDApplicationForm_medicalinfo mi
     WHERE mi.IPDApplicationFormid = p_ipdapplicationform_id;

    /* ---------- R4: Request/role -> taskname ---------- */
    l_taskname := NULLIF(TRIM(COALESCE(p_taskname, '')), '');

    IF l_taskname IS NULL THEN
        l_taskname := CASE
            WHEN lower(trim(COALESCE(p_user_role, ''))) IN
                 ('health seeker', 'frontdesk admin', 'front desk admin', 'doctor', 'intern doctor')
                THEN 'IP Screening'
            ELSE NULL
        END;
    END IF;

    IF l_taskname IS NULL THEN
        RETURN;
    END IF;

    /* ---------- R3 + R5: Score & rank ---------- */
    CREATE TEMP TABLE tmp_matches ON COMMIT DROP AS
    SELECT
        t.AssessmentTemplateid,
        t.templatename,
        COALESCE(t.isdefaulttemplate, false) AS is_default,
        COALESCE(t.createddate, TIMESTAMP '1900-01-01') AS createddate,
        COALESCE(SUM(
            (CASE WHEN ta.gender             IS NULL OR ta.gender = '' OR ta.gender = 'All' OR ta.gender = l_gender              THEN 1 ELSE 0 END) +
            (CASE WHEN ta.relationshipstatus IS NULL OR ta.relationshipstatus = '' OR ta.relationshipstatus = l_relationshipstatus THEN 1 ELSE 0 END) +
            (CASE WHEN ta.agegroup           IS NULL OR ta.agegroup = '' OR ta.agegroup = l_agegroup                              THEN 1 ELSE 0 END) +
            (CASE WHEN ta.medicalcondition   IS NULL OR ta.medicalcondition = ANY(l_medicalconditions)                            THEN 1 ELSE 0 END)
        ), 0)::int AS score,
        COALESCE(BOOL_OR(
            (ta.gender             IS NULL OR ta.gender = '' OR ta.gender = 'All' OR ta.gender = l_gender)              AND
            (ta.relationshipstatus IS NULL OR ta.relationshipstatus = '' OR ta.relationshipstatus = l_relationshipstatus) AND
            (ta.agegroup           IS NULL OR ta.agegroup = '' OR ta.agegroup = l_agegroup)                              AND
            (ta.medicalcondition   IS NULL OR ta.medicalcondition = ANY(l_medicalconditions))
        ), false) AS fully_matches
      FROM AssessmentTemplate t
      LEFT JOIN AssessmentTemplate_templateapplicability ta
        ON ta.AssessmentTemplateid = t.AssessmentTemplateid
     WHERE t.isdeleted = false
       AND t.templatename IS NOT NULL
       AND (l_tenantid IS NULL OR t.tenantid = l_tenantid OR t.tenantid IS NULL)
       -- Task names are configured manually and may contain spaces, for example
       -- "Discharge Checklist" instead of "Dischargechecklist".
       AND REGEXP_REPLACE(LOWER(COALESCE(t.taskname, '')), '[^a-z0-9]+', '', 'g')
           = REGEXP_REPLACE(LOWER(l_taskname), '[^a-z0-9]+', '', 'g')
     GROUP BY t.AssessmentTemplateid, t.templatename, t.isdefaulttemplate, t.createddate;

    SELECT COUNT(*), BOOL_OR(is_default)
      INTO l_template_count, l_has_default
      FROM tmp_matches;

    IF l_template_count > 0 THEN
        SELECT m.AssessmentTemplateid INTO l_preselected
          FROM tmp_matches m
         ORDER BY
            CASE WHEN m.is_default THEN 0 ELSE 1 END,
            CASE WHEN m.fully_matches THEN 0 ELSE 1 END,
            m.score DESC,
            m.createddate DESC,
            m.templatename ASC
         LIMIT 1;

        RETURN QUERY
        SELECT m.AssessmentTemplateid::varchar,
               m.templatename::varchar,
               CASE
                   WHEN l_template_count = 1 THEN true
                   WHEN l_has_default THEN m.is_default
                   ELSE (m.AssessmentTemplateid = l_preselected)
               END AS ispreselected,
               m.score,
               false AS isfallback
          FROM tmp_matches m
         WHERE (l_has_default AND m.is_default)
            OR (NOT l_has_default)
         ORDER BY
            CASE WHEN m.is_default THEN 0 ELSE 1 END,
            CASE WHEN m.fully_matches THEN 0 ELSE 1 END,
            (m.AssessmentTemplateid = l_preselected) DESC,
            m.score DESC,
            m.createddate DESC,
            m.templatename ASC;
    END IF;
END;
$BODY$;
