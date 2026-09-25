
CREATE OR REPLACE FUNCTION "Insert_Room_Occupancy_For_Booking"
(
    pvar_roomoccupancystatusid uuid,
    pvar_tenantid              uuid,
    pvar_roomallocationno      varchar(256),
    pvar_patientname           uuid,
    pvar_ipdno                 uuid,
    pvar_block                 uuid,
    pvar_building              uuid,
    pvar_floor                 uuid,
    pvar_room                  uuid,
    pvar_bookeddate            date,
    pvar_status                varchar(1024),
    pvar_bookedfor             varchar(256),
    pvar_createduser           uuid,
    OUT pvar_returnMessage varchar(4000)
)
RETURNS varchar(4000)
AS $BODY$
BEGIN
    INSERT INTO roomoccupancystatus
    (
        roomoccupancystatusid,
        tenantid,
        roomallocationno,
        patientvisit,
        patientname,
        ipdno,
        block,
        building,
        floor,
        room,
        bookeddate,
        status,
        bookedfor,
        createduser,
        createddate
    )
    VALUES
    (
        COALESCE(pvar_roomoccupancystatusid, gen_random_uuid()),
        pvar_tenantid,
        pvar_roomallocationno,
        NULL,
        pvar_patientname,
        pvar_ipdno,
        pvar_block,
        pvar_building,
        pvar_floor,
        pvar_room,
        pvar_bookeddate,
        pvar_status,
        pvar_bookedfor,
        pvar_createduser,
        NOW()
    );

    pvar_returnMessage := '201.1';
EXCEPTION WHEN OTHERS THEN
    pvar_returnMessage := SQLERRM;
END
$BODY$
LANGUAGE plpgsql;
