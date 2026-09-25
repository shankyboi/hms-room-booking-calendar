CREATE OR REPLACE FUNCTION "DesignationAlreadyExists"
(
	pvar_tenantid uuid,
	pvar_workprofile uuid,
	pvar_designation Varchar(128),
	pvar_Designationid uuid
)
RETURNS boolean
AS $BODY$
BEGIN
RETURN EXISTS (
	SELECT 1 FROM Designation
	WHERE (tenantid = pvar_tenantid OR (tenantid IS NULL AND pvar_tenantid IS NULL))
	AND workprofile = pvar_workprofile
	AND lower(trim(designation)) = lower(trim(pvar_designation))
	AND isdeleted = false
	AND (pvar_Designationid IS NULL OR Designationid <> pvar_Designationid)
);
END
$BODY$
LANGUAGE plpgsql;
