
CREATE OR REPLACE FUNCTION "GetDuration_ByPractitioner"
(
    pvar_practitioner   Varchar(50),
    pvar_tenantid       Varchar
)
RETURNS TABLE(
    tasktype_text       Varchar,
    durationinminutes   int
)
AS $BODY$
DECLARE
    lvar_tenantid   varchar[];
    lstr_usersid    varchar;
BEGIN
    /*
      Joins People_clinicaltaskinfo → Task (tenant-matched) to resolve:
          ClinicalAppointment.tasktype (text)
        = Task.taskname (text)
        → Task.Taskid = People_clinicaltaskinfo.taskname (UUID)
        → durationinminutes

      Called before bulk-reschedule to get the target doctor's slot durations
      keyed by the tasktype text stored in ClinicalAppointment.
    */

    SELECT  SPLIT_PART(pvar_tenantid, '|', 1),
            SPLIT_PART(pvar_tenantid, '|', 2)
    INTO    lstr_usersid, pvar_tenantid;

    if (pvar_tenantid is null or pvar_tenantid = '' or pvar_tenantid = '00000000-0000-0000-0000-000000000000')
    then
        SELECT STRING_TO_ARRAY(viewertenantids, ',')
        INTO   lvar_tenantid
        FROM   users
        WHERE  users.usersid::varchar = lstr_usersid;

        if (lvar_tenantid is NULL)
        then
            SELECT array_agg(tenant.tenantid) INTO lvar_tenantid FROM tenant;
        end if;
    else
        lvar_tenantid := ARRAY[pvar_tenantid];
    end if;

    lvar_tenantid := lvar_tenantid
                  || ARRAY[''::character varying]
                  || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];

    RETURN QUERY
    SELECT
        CAST(t.taskname AS Varchar)         AS tasktype_text,
        pct.durationinminutes               AS durationinminutes
    FROM  People_clinicaltaskinfo pct
    INNER JOIN Task t
           ON  pct.taskname = t.Taskid
          AND  COALESCE(CAST(t.tenantid AS varchar), '') = ANY(lvar_tenantid)
          AND  t.isdeleted = false
    WHERE CAST(pct.Peopleid AS varchar) = pvar_practitioner
      AND pct.durationinminutes IS NOT NULL
      AND pct.durationinminutes > 0
    ORDER BY pct.record_order ASC NULLS LAST;

END
$BODY$
LANGUAGE plpgsql;
