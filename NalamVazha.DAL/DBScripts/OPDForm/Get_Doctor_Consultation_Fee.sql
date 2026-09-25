
CREATE OR REPLACE FUNCTION "Get_Doctor_Consultation_Fee"
(
    pvar_peopleid uuid,
    pvar_tasktype varchar DEFAULT NULL
)
RETURNS TABLE(
    tasktype         varchar,
    taskname         varchar,
    durationinminutes int,
    feesamount       decimal
)
AS $BODY$
BEGIN
    RETURN QUERY
    SELECT
        COALESCE(tt.tasktypename, '')::varchar  AS tasktype,
        COALESCE(t.taskname,      '')::varchar  AS taskname,
        pct.durationinminutes,
        COALESCE(pct.feesamount, 0)             AS feesamount
    FROM   People_clinicaltaskinfo pct
    LEFT JOIN TaskType tt ON tt.TaskTypeid = pct.tasktype
    LEFT JOIN Task      t  ON t.Taskid     = pct.taskname
    WHERE  pct.Peopleid = pvar_peopleid
      AND t.taskname IN ('OP New', 'OP Follow up')
      AND  (pvar_tasktype IS NULL OR pvar_tasktype = '' OR tt.tasktypename ILIKE pvar_tasktype)
    ORDER BY pct.record_order DESC;
END;
$BODY$
LANGUAGE plpgsql;
