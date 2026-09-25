DROP FUNCTION IF EXISTS public."Get_Latest_Previous_Assessment_PatientAnswers"(uuid, uuid, uuid, uuid);

CREATE OR REPLACE FUNCTION public."Get_Latest_Previous_Assessment_PatientAnswers"(
    pvar_patientname uuid,
    pvar_questionnairetemplate uuid,
    pvar_taskname varchar,
    pvar_opdform uuid,
    pvar_ipdform uuid
)
RETURNS SETOF public.assessment_patientanswers
LANGUAGE sql
STABLE
AS $BODY$
    WITH resolved_context AS (
        SELECT COALESCE(
            NULLIF(BTRIM(pvar_taskname), ''),
            (
                SELECT task.taskname
                FROM public.opdform current_opd
                INNER JOIN public.task task
                    ON task.taskid = current_opd.task
                   AND COALESCE(task.isdeleted, false) = false
                WHERE current_opd.opdformid = pvar_opdform
                  AND COALESCE(current_opd.isdeleted, false) = false
                LIMIT 1
            )
        ) AS taskname
    )
    SELECT patient_answers.*
    FROM public.assessment_patientanswers patient_answers
    WHERE patient_answers.assessmentid = (
        SELECT assessment.assessmentid
        FROM public.assessment assessment
        LEFT JOIN public.opdform previous_opd
          ON previous_opd.opdformid = assessment.opdform
         AND COALESCE(previous_opd.isdeleted, false) = false
        LEFT JOIN public.task previous_opd_task
          ON previous_opd_task.taskid = previous_opd.task
         AND COALESCE(previous_opd_task.isdeleted, false) = false
        LEFT JOIN public.ipdapplicationform previous_ipd
          ON previous_ipd.ipdapplicationformid = assessment.ipdform
         AND COALESCE(previous_ipd.isdeleted, false) = false
        LEFT JOIN public.assessmenttemplate source_template
          ON source_template.assessmenttemplateid = assessment.questionnairetemplate
         AND COALESCE(source_template.isdeleted, false) = false
        CROSS JOIN resolved_context context
        WHERE assessment.patientname = pvar_patientname
          AND assessment.questionnairetemplate = pvar_questionnairetemplate
          AND COALESCE(assessment.isdeleted, false) = false
          AND NULLIF(REGEXP_REPLACE(LOWER(COALESCE(context.taskname, '')), '[^a-z0-9]', '', 'g'), '') IS NOT NULL
          AND REGEXP_REPLACE(LOWER(COALESCE(
                  NULLIF(BTRIM(assessment.taskname), ''),
                  previous_opd_task.taskname,
                  source_template.taskname,
                  '')), '[^a-z0-9]', '', 'g') =
              REGEXP_REPLACE(LOWER(COALESCE(context.taskname, '')), '[^a-z0-9]', '', 'g')
          AND (pvar_opdform IS NULL OR assessment.opdform IS NOT NULL)
          AND (pvar_ipdform IS NULL OR assessment.ipdform IS NOT NULL)
          AND (pvar_opdform IS NULL OR assessment.opdform IS DISTINCT FROM pvar_opdform)
          AND (pvar_ipdform IS NULL OR assessment.ipdform IS DISTINCT FROM pvar_ipdform)
          AND EXISTS (
              SELECT 1
              FROM public.assessment_patientanswers saved_answers
              WHERE saved_answers.assessmentid = assessment.assessmentid
          )
        ORDER BY
                 /* Prefer answers from the latest previous OPD encounter. */
                 CASE WHEN pvar_opdform IS NOT NULL THEN previous_opd.createddate END DESC NULLS LAST,
                 CASE WHEN pvar_ipdform IS NOT NULL THEN previous_ipd.createddate END DESC NULLS LAST,
                 COALESCE(
                     assessment.modifieddate,
                     assessment.createddate,
                     assessment.assessmentdate
                 ) DESC,
                 assessment.assessmentid DESC
        LIMIT 1
    )
    ORDER BY patient_answers.record_order;
$BODY$;
