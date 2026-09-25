CREATE OR REPLACE FUNCTION public."Get_Assessment_History"
(
    pvar_assessmentid uuid DEFAULT NULL,
    pvar_ipdform uuid DEFAULT NULL,
    pvar_opdform uuid DEFAULT NULL
)
RETURNS TABLE
(
    assessmenthistoryid uuid,
    assessmentid uuid,
    versionnumber integer,
    actiontype varchar,
    workflowstage varchar,
    assessmentdate timestamp,
    assessmentnotes text,
    eligibleforfinaladmission varchar,
    taskname varchar,
    actionby uuid,
    actionbyrole varchar,
    actiondate timestamp,
    reviewnotes text,
    assessment_snapshot jsonb,
    previoushistoryid uuid,
    actionbyname text
)
LANGUAGE sql
STABLE
AS $function$
    SELECT
        h.assessmenthistoryid,
        h.assessmentid,
        h.versionnumber,
        h.actiontype,
        h.workflowstage,
        h.assessmentdate,
        h.assessmentnotes,
        h.eligibleforfinaladmission,
        h.taskname,
        h.actionby,
        h.actionbyrole,
        h.actiondate,
        h.reviewnotes,
        h.assessment_snapshot,
        h.previoushistoryid,
        TRIM(CONCAT(COALESCE(u.firstname, ''), ' ', COALESCE(u.lastname, '')))
    FROM "AssessmentHistory" h
    LEFT JOIN users u
        ON u.usersid = h.actionby
    WHERE (pvar_assessmentid IS NULL OR h.assessmentid = pvar_assessmentid)
      AND (pvar_ipdform IS NULL OR h.ipdform = pvar_ipdform)
      AND (pvar_opdform IS NULL OR h.opdform = pvar_opdform)
    ORDER BY h.actiondate DESC, h.versionnumber DESC;
$function$;
