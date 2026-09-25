
CREATE OR REPLACE FUNCTION "Update_Room_Allocation_ToDate"
(
    pvar_ipdno        uuid,
    pvar_room         uuid,
    pvar_bookedfor    varchar(256),
    pvar_todate       date,
    pvar_modifieduser uuid,
    OUT pvar_roomallocationid  uuid,
    OUT pvar_returnMessage     varchar(4000)
)
RETURNS record
AS $BODY$
BEGIN
    UPDATE roomallocation
    SET    todate       = pvar_todate,
           modifieduser = pvar_modifieduser,
           modifieddate = NOW()
    WHERE  ipdno              = pvar_ipdno
      AND  room               = pvar_room
      AND  lower(bookedfor)   = lower(pvar_bookedfor)
      AND  isdeleted          = false
    RETURNING roomallocationid INTO pvar_roomallocationid;

    pvar_returnMessage := '201.1';
EXCEPTION WHEN OTHERS THEN
    pvar_returnMessage := SQLERRM;
END
$BODY$
LANGUAGE plpgsql;
