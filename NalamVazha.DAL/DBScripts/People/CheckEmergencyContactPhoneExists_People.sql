-- Is this emergency-contact phone number already recorded against a DIFFERENT person?
-- pvar_peopleid is the person being added/edited; pass '' when adding, so every existing
-- row counts. Called from PeopleDAL.CheckEmergencyContactPhoneExists.
CREATE OR REPLACE FUNCTION "CheckEmergencyContactPhoneExists_People"(
	pvar_phonenumber Varchar(20),
	pvar_peopleid Varchar(50)
)
RETURNS BOOLEAN
AS $BODY$
BEGIN
	RETURN EXISTS (
		SELECT 1
		FROM People_emergencycontact
		WHERE People_emergencycontact.phonenumber = pvar_phonenumber
		AND (COALESCE(pvar_peopleid, '') = ''
		     OR People_emergencycontact.Peopleid::varchar <> pvar_peopleid)
	);
END
$BODY$
LANGUAGE plpgsql;
