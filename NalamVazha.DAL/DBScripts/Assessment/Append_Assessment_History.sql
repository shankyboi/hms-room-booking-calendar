CREATE OR REPLACE FUNCTION public."Append_Assessment_History"
(
    pvar_assessmenthistoryid uuid,
    pvar_assessmentid uuid,
    pvar_tenantid uuid,
    pvar_patientname uuid,
    pvar_patientvisit uuid,
    pvar_ipdform uuid,
    pvar_opdform uuid,
    pvar_actiontype varchar,
    pvar_workflowstage varchar,
    pvar_assessmentdate timestamp,
    pvar_questionnairetemplate uuid,
    pvar_doctorname uuid,
    pvar_assessedby varchar,
    pvar_assessmentnotes text,
    pvar_eligibleforfinaladmission varchar,
    pvar_taskname varchar,
    pvar_assessment_snapshot jsonb,
    pvar_actionby uuid,
    pvar_actionbyrole varchar,
    pvar_reviewnotes text
)
RETURNS varchar
LANGUAGE plpgsql
AS $function$
DECLARE
    lvar_versionnumber integer;
    lvar_previoushistoryid uuid;
BEGIN
    PERFORM pg_advisory_xact_lock(
        hashtextextended(CAST(pvar_assessmentid AS text), 0)
    );

    SELECT
        COALESCE(MAX(versionnumber), 0) + 1,
        (
            ARRAY_AGG(assessmenthistoryid ORDER BY versionnumber DESC)
                FILTER (WHERE assessmenthistoryid IS NOT NULL)
        )[1]
    INTO lvar_versionnumber, lvar_previoushistoryid
    FROM "AssessmentHistory"
    WHERE assessmentid = pvar_assessmentid;

    INSERT INTO "AssessmentHistory"
    (
        assessmenthistoryid,
        assessmentid,
        tenantid,
        patientname,
        patientvisit,
        ipdform,
        opdform,
        versionnumber,
        actiontype,
        workflowstage,
        assessmentdate,
        questionnairetemplate,
        doctorname,
        assessedby,
        assessmentnotes,
        eligibleforfinaladmission,
        taskname,
        assessment_snapshot,
        actionby,
        actionbyrole,
        actiondate,
        reviewnotes,
        previoushistoryid
    )
    VALUES
    (
        pvar_assessmenthistoryid,
        pvar_assessmentid,
        pvar_tenantid,
        pvar_patientname,
        pvar_patientvisit,
        pvar_ipdform,
        pvar_opdform,
        lvar_versionnumber,
        pvar_actiontype,
        pvar_workflowstage,
        pvar_assessmentdate,
        pvar_questionnairetemplate,
        pvar_doctorname,
        pvar_assessedby,
        pvar_assessmentnotes,
        pvar_eligibleforfinaladmission,
        pvar_taskname,
        pvar_assessment_snapshot,
        pvar_actionby,
        pvar_actionbyrole,
        CURRENT_TIMESTAMP,
        pvar_reviewnotes,
        lvar_previoushistoryid
    );

    RETURN '201.1';
EXCEPTION
    WHEN unique_violation THEN
        RETURN 'Assessment history version already exists.';
    WHEN OTHERS THEN
        RAISE;
END;
$function$;
