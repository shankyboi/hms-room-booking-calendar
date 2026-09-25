CREATE OR REPLACE FUNCTION "CheckEmailExists_People"(
	pvar_emailid Varchar(128),
	pvar_tenantid Varchar(50)
)
RETURNS BOOLEAN
AS $BODY$
BEGIN
	IF EXISTS (
		SELECT 1 FROM People
		WHERE upper(People.emailid::varchar) = upper(pvar_emailid)
		AND People.tenantid = pvar_tenantid::uuid
	) THEN
		RETURN TRUE;
	END IF;

	IF EXISTS (
		SELECT 1 FROM users
		WHERE upper(users.emailid::varchar) = upper(pvar_emailid)
		AND users.tenantid = pvar_tenantid::uuid
		AND COALESCE(users.isdeleted, false) = false
	) THEN
		RETURN TRUE;
	END IF;

	RETURN FALSE;
END
$BODY$
LANGUAGE plpgsql;
