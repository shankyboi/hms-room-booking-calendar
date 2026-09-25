DROP FUNCTION IF EXISTS public."Get_Latest_Assessment_For_Prefill"(uuid, uuid, uuid);

CREATE OR REPLACE FUNCTION public."Get_Latest_Assessment_For_Prefill"
(
    pvar_patientname uuid,
    pvar_taskname varchar,
    pvar_opdform uuid,
    pvar_ipdform uuid
)
RETURNS TABLE
(
    assessmentid uuid,
    questionnairetemplate uuid,
    assessmentnotes varchar,
    taskname varchar,
    sourceassessmentdate timestamp
)
LANGUAGE sql
STABLE
AS $function$
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
    SELECT
        assessment.assessmentid,
        assessment.questionnairetemplate,
        assessment.assessmentnotes,
        COALESCE(
            NULLIF(BTRIM(assessment.taskname), ''),
            previous_opd_task.taskname,
            source_template.taskname,
            context.taskname
        )::varchar AS taskname,
        assessment.assessmentdate AS sourceassessmentdate
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
    ORDER BY
             /* In OPD mode, "latest" means the latest previous OPD encounter,
                not whichever assessment happened to be edited most recently. */
             CASE WHEN pvar_opdform IS NOT NULL THEN previous_opd.createddate END DESC NULLS LAST,
             CASE WHEN pvar_ipdform IS NOT NULL THEN previous_ipd.createddate END DESC NULLS LAST,
             COALESCE(
                 assessment.modifieddate,
                 assessment.createddate,
                 assessment.assessmentdate
             ) DESC,
             assessment.assessmentid DESC
    LIMIT 1;
$function$;
