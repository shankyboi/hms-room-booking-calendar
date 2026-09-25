
CREATE OR REPLACE FUNCTION public.get_opd_assessmenttemplates(
	pvar_opdformid uuid,
	pvar_taskname character varying)
    RETURNS TABLE("AssessmentTemplateid" uuid, templatename character varying, ispreselected boolean) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    WITH opd_context AS (
        SELECT o.tenantid, o.task, t.taskname AS opd_taskname
        FROM OPDForm o
        LEFT JOIN Task t
               ON t.Taskid = o.task
              AND COALESCE(t.isdeleted, false) = false
        WHERE o.OPDFormid = pvar_opdformid
          AND COALESCE(o.isdeleted, false) = false
        LIMIT 1
    ),
    requested_task AS (
        SELECT NULLIF(TRIM(COALESCE(NULLIF(TRIM(pvar_taskname), ''), (SELECT oc.opd_taskname FROM opd_context oc LIMIT 1), '')), '') AS taskname
      ),
    task_templates AS (
        SELECT
            at.AssessmentTemplateid,
            at.templatename,
            COALESCE(at.isdefaulttemplate, false) AS is_default,
            COALESCE(at.createddate, TIMESTAMP '1900-01-01') AS createddate
        FROM AssessmentTemplate at
        JOIN opd_context oc ON true
        CROSS JOIN requested_task rt
        WHERE COALESCE(at.isdeleted, false) = false
          AND at.templatename IS NOT NULL
          AND rt.taskname IS NOT NULL
          AND LOWER(TRIM(COALESCE(at.taskname, ''))) = LOWER(TRIM(rt.taskname))
          AND at.tenantid IS NOT DISTINCT FROM oc.tenantid
    ),
    template_stats AS (
        SELECT COUNT(*) AS template_count, BOOL_OR(is_default) AS has_default
        FROM task_templates
    ),
    selected_templates AS (
        SELECT
            tt.AssessmentTemplateid,
            tt.templatename,
            tt.is_default,
            tt.createddate,
            ts.template_count
        FROM task_templates tt
        CROSS JOIN template_stats ts
        -- Return every template configured for the OPD task. The default flag only
        -- controls the initial selection; it must not remove alternative templates.
        WHERE ts.template_count > 0
    ),
    fallback_template AS (
        SELECT
            at.AssessmentTemplateid,
            at.templatename,
            COALESCE(at.isdefaulttemplate, false) AS is_default,
            COALESCE(at.createddate, TIMESTAMP '1900-01-01') AS createddate
        FROM AssessmentTemplate at
        JOIN opd_context oc ON true
        WHERE COALESCE(at.isdeleted, false) = false
          AND at.templatename IS NOT NULL
          AND (
              LOWER(TRIM(COALESCE(at.taskname, ''))) = 'default template'
              OR LOWER(TRIM(at.templatename)) = 'default template'
          )
          AND at.tenantid IS NOT DISTINCT FROM oc.tenantid
        ORDER BY
            CASE WHEN LOWER(TRIM(COALESCE(at.taskname, ''))) = 'default template' THEN 0 ELSE 1 END,
            CASE WHEN COALESCE(at.isdefaulttemplate, false) THEN 0 ELSE 1 END,
            at.createddate DESC NULLS LAST,
            at.templatename
        LIMIT 1
    ),
    final_templates AS (
        SELECT
            st.AssessmentTemplateid,
            st.templatename,
            CASE
                WHEN st.template_count = 1 THEN true
                WHEN st.is_default THEN true
                ELSE false
            END AS ispreselected,
            CASE WHEN st.is_default THEN 0 ELSE 1 END AS default_order,
            st.createddate
        FROM selected_templates st
        WHERE EXISTS (SELECT 1 FROM selected_templates)

        UNION ALL

        SELECT
    ft.AssessmentTemplateid,
    ft.templatename,
    true AS ispreselected,
    CASE WHEN ft.is_default THEN 0 ELSE 1 END AS default_order,
    ft.createddate
        FROM fallback_template ft
        WHERE NOT EXISTS (SELECT 1 FROM selected_templates)
    )
    SELECT ft.AssessmentTemplateid, ft.templatename, ft.ispreselected
    FROM final_templates ft
    ORDER BY ft.default_order, ft.createddate DESC, ft.templatename;
END
$BODY$;
