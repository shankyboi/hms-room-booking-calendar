ALTER TABLE DailyTask ADD COLUMN IF NOT EXISTS closedby uuid REFERENCES users(usersid);
ALTER TABLE DailyTask ADD COLUMN IF NOT EXISTS closeddate Timestamp(3);

CREATE OR REPLACE FUNCTION "Close_Daily_Task"
(
    pvar_dailytaskid uuid,
    pvar_tenantid uuid,
    pvar_closedby uuid,
    pvar_comments varchar DEFAULT NULL,
    OUT pvar_returnmessage varchar(4000)
)
RETURNS varchar(4000)
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_tasktype uuid;
    lvar_status varchar;
BEGIN
    IF NULLIF(btrim(pvar_comments), '') IS NULL THEN
        pvar_returnmessage := 'Comments are required.';
        RETURN;
    END IF;

    IF NOT "Check_Authorization"(pvar_closedby, 'DailyTask', 'edit') THEN
        pvar_returnmessage := '401.1';
        RETURN;
    END IF;

    SELECT tasktype, lower(btrim(COALESCE(status, ''))) INTO lvar_tasktype, lvar_status
      FROM DailyTask
     WHERE DailyTaskid = pvar_dailytaskid
       AND tenantid IS NOT DISTINCT FROM pvar_tenantid
       AND COALESCE(isdeleted, false) = false
     FOR UPDATE;

    IF NOT FOUND THEN
        pvar_returnmessage := '404.1';
        RETURN;
    END IF;

    IF lvar_status NOT IN ('pending', 'in progress', 'waiting', 'not completed', 'overdue', 'completed') THEN
        pvar_returnmessage := '409.1';
        RETURN;
    END IF;

    UPDATE DailyTask
       SET status = 'Closed', closeddate = clock_timestamp(), closedby = pvar_closedby,
           modifieddate = clock_timestamp(), modifieduser = pvar_closedby
     WHERE DailyTaskid = pvar_dailytaskid;

    INSERT INTO TaskActionLog
       (TaskActionLogid, tenantid, taskname, tasktype, actiondate, actionby,
        summary, description, comments, createduser, createddate, isdeleted)
    VALUES
       (gen_random_uuid(), pvar_tenantid, pvar_dailytaskid, lvar_tasktype, clock_timestamp(), pvar_closedby,
        'Task Closed', COALESCE(NULLIF(btrim(pvar_comments), ''), 'Task Closed'), pvar_comments,
        pvar_closedby, clock_timestamp(), false);

    pvar_returnmessage := '201.1';
EXCEPTION WHEN OTHERS THEN
    pvar_returnmessage := SQLERRM;
END
$BODY$;
