INSERT INTO lookups (
    lookupid,
    entityname,
    attributetype,
    fieldname,
    fielddesc,
    createduser,
    createddate,
    modifieduser,
    modifieddate,
    isdeleted
)
SELECT
    gen_random_uuid(),
    src.entityname,
    src.attributetype,
    src.fieldname,
    src.fielddesc,
    NULL,
    NOW(),
    NULL,
    NOW(),
    false
FROM (
    VALUES
        ('OPDForm', 'single', 'appointmentmode', 'Online,Offline'),
        ('OPDForm', 'single', 'slotpreference', 'Any Slot,Early Morning,Closer to Lunch,Closer to Day')
) AS src(entityname, attributetype, fieldname, fielddesc)
WHERE NOT EXISTS (
    SELECT 1
    FROM lookups existing
    WHERE existing.entityname = src.entityname
      AND existing.fieldname = src.fieldname
);

UPDATE lookups
SET attributetype = 'single',
    fielddesc = 'Any Slot,Early Morning,Closer to Lunch,Closer to Day',
    modifieddate = NOW()
WHERE entityname = 'OPDForm'
  AND fieldname = 'slotpreference'
  AND isdeleted = false;
