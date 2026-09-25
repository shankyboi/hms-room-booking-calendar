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
    'OPDForm',
    src.attributetype,
    'medicationduration',
    src.fielddesc,
    src.createduser,
    src.createddate,
    src.modifieduser,
    src.modifieddate,
    src.isdeleted
FROM dblink(
    'host=98.70.42.225 port=5432 dbname=nalamvazha user=md_nalamvazha password=u&v8yIBojXuj',
    'SELECT
        attributetype,
        fielddesc,
        createduser,
        createddate,
        modifieduser,
        modifieddate,
        isdeleted
     FROM public.lookups
     WHERE entityname = ''IPDApplicationForm''
       AND fieldname = ''medicationduration'''
) AS src(
    attributetype VARCHAR(128),
    fielddesc VARCHAR(128),
    createduser UUID,
    createddate TIMESTAMP,
    modifieduser UUID,
    modifieddate TIMESTAMP,
    isdeleted BOOLEAN
)
WHERE NOT EXISTS (
    SELECT 1
    FROM lookups existing
    WHERE existing.entityname = 'OPDForm'
      AND existing.fieldname = 'medicationduration'
);
