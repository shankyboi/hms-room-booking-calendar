-- Import GroupBooking metadata from the reference NalamVazha database.
-- Requires the postgres dblink extension in the target database:
-- CREATE EXTENSION IF NOT EXISTS dblink;

INSERT INTO roleauthorization (
    roleauthorizationid,
    controllername,
    actionname,
    viewname,
    viewactionroles,
    actiondisplayname,
    actionmethodname
)
SELECT
    gen_random_uuid(),
    'GroupBooking',
    src.actionname,
    src.viewname,
    src.viewactionroles,
    REPLACE(src.actiondisplayname, 'Room Group', 'Group Booking'),
    REPLACE(src.actionmethodname, 'Room_Group', 'Group_Booking')
FROM dblink(
    'host=98.70.42.225 port=5432 dbname=dev_nalamvazha user=md_nalamvazha password=u&v8yIBojXuj',
    'SELECT
        actionname,
        viewname,
        viewactionroles,
        actiondisplayname,
        actionmethodname
     FROM public.roleauthorization
     WHERE controllername = ''RoomGroup'''
) AS src(
    actionname VARCHAR(128),
    viewname VARCHAR(128),
    viewactionroles VARCHAR(256),
    actiondisplayname VARCHAR(128),
    actionmethodname VARCHAR(128)
)
WHERE NOT EXISTS (
    SELECT 1
    FROM roleauthorization existing
    WHERE existing.controllername = 'GroupBooking'
      AND existing.actionname = src.actionname
      AND existing.viewname = src.viewname
);

UPDATE roleauthorization
SET actionmethodname = 'Detail_Group_Booking'
WHERE controllername = 'GroupBooking'
  AND viewname = 'detail';

INSERT INTO projectstructure (
    businessfunctionname,
    businessfunctionicon,
    projectelementname,
    elementtype,
    parentnode,
    subsystem,
    structureorder,
    assignedentityname,
    displayicon
)
SELECT
    src.businessfunctionname,
    src.businessfunctionicon,
    'Group Booking',
    src.elementtype,
    src.parentnode,
    src.subsystem,
    src.structureorder,
    'GroupBooking',
    src.displayicon
FROM dblink(
    'host=98.70.42.225 port=5432 dbname=dev_nalamvazha user=md_nalamvazha password=u&v8yIBojXuj',
    'SELECT
        businessfunctionname,
        businessfunctionicon,
        elementtype,
        parentnode,
        subsystem,
        structureorder,
        displayicon
     FROM public.projectstructure
     WHERE assignedentityname = ''RoomGroup'''
) AS src(
    businessfunctionname VARCHAR(128),
    businessfunctionicon VARCHAR(128),
    elementtype VARCHAR(128),
    parentnode VARCHAR(128),
    subsystem VARCHAR(128),
    structureorder INTEGER,
    displayicon VARCHAR(128)
)
WHERE NOT EXISTS (
    SELECT 1
    FROM projectstructure existing
    WHERE existing.assignedentityname = 'GroupBooking'
      AND existing.projectelementname = 'Group Booking'
      AND existing.elementtype = src.elementtype
      AND COALESCE(existing.parentnode, '') = COALESCE(src.parentnode, '')
);

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
    src.lookupid,
    src.entityname,
    src.attributetype,
    src.fieldname,
    src.fielddesc,
    src.createduser,
    src.createddate,
    src.modifieduser,
    src.modifieddate,
    src.isdeleted
FROM dblink(
    'host=98.70.42.225 port=5432 dbname=dev_nalamvazha user=md_nalamvazha password=u&v8yIBojXuj',
    'SELECT
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
     FROM public.lookups
     WHERE entityname = ''GroupBooking'''
) AS src(
    lookupid UUID,
    entityname VARCHAR(128),
    attributetype VARCHAR(128),
    fieldname VARCHAR(128),
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
    WHERE existing.lookupid = src.lookupid
);
