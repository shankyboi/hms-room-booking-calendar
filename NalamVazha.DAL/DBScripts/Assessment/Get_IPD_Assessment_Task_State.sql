CREATE OR REPLACE FUNCTION public."Get_IPD_Assessment_Task_State"(
    pvar_ipdapplicationformid uuid)
RETURNS TABLE(assessmentid uuid, taskname varchar)
LANGUAGE sql
STABLE
AS $BODY$
    SELECT
        a.assessmentid,
        COALESCE(NULLIF(BTRIM(h.taskname), ''), a.taskname)::varchar AS taskname
      FROM assessment a
      JOIN LATERAL
           (
               SELECT
                   ah.eligibleforfinaladmission,
                   ah.taskname,
                   ah.actiondate,
                   ah.versionnumber
                 FROM "AssessmentHistory" ah
                WHERE ah.assessmentid = a.assessmentid
                ORDER BY ah.versionnumber DESC, ah.actiondate DESC
                LIMIT 1
           ) h ON true
     WHERE a.ipdform = pvar_ipdapplicationformid
       AND COALESCE(a.isdeleted, false) = false
       AND NULLIF(BTRIM(COALESCE(h.taskname, a.taskname)), '') IS NOT NULL
       AND LOWER(BTRIM(COALESCE(h.eligibleforfinaladmission, ''))) IN ('patient', 'approved')
     ORDER BY h.actiondate DESC, a.createddate DESC;
$BODY$;
