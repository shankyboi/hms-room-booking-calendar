
CREATE OR REPLACE FUNCTION "Update_Room_Occupancy_By_IPD"
(
    pvar_ipdno        uuid,
    pvar_newstatus    varchar(1024),
    pvar_modifieduser uuid,
    OUT pvar_returnMessage varchar(4000)
)
RETURNS varchar(4000)
AS $BODY$
BEGIN
    UPDATE "RoomOccupancyStatus"
    SET    status       = pvar_newstatus,
           modifieduser = pvar_modifieduser,
           modifieddate = NOW()
    WHERE  ipdno  = pvar_ipdno
      AND  status = 'Blocked';

    pvar_returnMessage := '201.1';
EXCEPTION WHEN OTHERS THEN
    pvar_returnMessage := SQLERRM;
END
$BODY$
LANGUAGE plpgsql;
