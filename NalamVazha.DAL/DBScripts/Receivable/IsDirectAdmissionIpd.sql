CREATE OR REPLACE FUNCTION "IsDirectAdmissionIpd"
(
	pvar_ipdapplicationformid uuid
)
RETURNS boolean
AS $BODY$
BEGIN
RETURN EXISTS (
	SELECT 1
	FROM IPDApplicationForm
	WHERE IPDApplicationForm.IPDApplicationFormid = pvar_ipdapplicationformid
	AND COALESCE(IPDApplicationForm.isdeleted, false) = false
	AND (
		lower(trim(COALESCE(IPDApplicationForm.bookingstatus, ''))) = 'direct admission'
		OR lower(trim(COALESCE(IPDApplicationForm.verifiedstatus, ''))) = 'direct admission'
	)
);
END
$BODY$
LANGUAGE plpgsql;
