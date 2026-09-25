CREATE OR REPLACE FUNCTION "Cancel_IPD_Appointments_By_Booking"
(
	pvar_ipdapplicationformid varchar,
	pvar_modifieduser uuid,
	OUT pvar_returnMessage varchar(4000)
)
RETURNS varchar(4000)
AS $BODY$
BEGIN
	
INSERT INTO history
VALUES('ClinicalAppointment', NOW(),
(SELECT query_to_xml('SELECT * FROM ClinicalAppointment WHERE ClinicalAppointment.bookingid= '''||pvar_ipdapplicationformid||'''', true, false, '')));

	UPDATE ClinicalAppointment
	SET status = 'Cancelled',
		modifieduser = pvar_modifieduser,
		modifieddate = NOW()
	WHERE COALESCE(isdeleted, false) = false
	  AND bookingid = pvar_ipdapplicationformid
	  AND COALESCE(status, '') <> 'Cancelled';

	pvar_returnMessage := '201.1';

EXCEPTION WHEN OTHERS THEN
	pvar_returnMessage := SQLERRM;
END;
$BODY$
LANGUAGE plpgsql;
