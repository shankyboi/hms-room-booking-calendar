
CREATE OR REPLACE FUNCTION "Release_Room_On_Cancellation"
(
    pvar_ipdno        uuid,
    pvar_modifieduser uuid,
    OUT pvar_returnMessage varchar(4000)
)
RETURNS varchar(4000)
AS $BODY$
BEGIN
    UPDATE roomoccupancystatus
    SET    isdeleted    = true,
           status       = 'Cancelled',
           modifieduser = pvar_modifieduser,
           modifieddate = NOW()
    WHERE  ipdno     = pvar_ipdno
      AND  isdeleted = false;

    UPDATE roomallocation
    SET    status       = 'Cancelled',
           modifieduser = pvar_modifieduser,
           modifieddate = NOW()
    WHERE  ipdno = pvar_ipdno
      AND  COALESCE(status, '') <> 'Cancelled';

    pvar_returnMessage := '201.1';
EXCEPTION WHEN OTHERS THEN
    pvar_returnMessage := SQLERRM;
END
$BODY$
LANGUAGE plpgsql;
