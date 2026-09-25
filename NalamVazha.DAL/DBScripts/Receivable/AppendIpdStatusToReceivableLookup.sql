CREATE OR REPLACE FUNCTION "AppendIpdStatusToReceivableLookup"
(
	pvar_ipdapplicationformid uuid
)
RETURNS TABLE(bookingstatus Varchar, verifiedstatus Varchar)
AS $BODY$
BEGIN
RETURN QUERY
SELECT IPDApplicationForm.bookingstatus, IPDApplicationForm.verifiedstatus
FROM IPDApplicationForm
WHERE IPDApplicationForm.IPDApplicationFormid = pvar_ipdapplicationformid
AND COALESCE(IPDApplicationForm.isdeleted, false) = false;
END
$BODY$
LANGUAGE plpgsql;
