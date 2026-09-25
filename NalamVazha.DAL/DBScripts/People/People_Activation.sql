-- Used by PeopleDAL.Update_People when a Draft person is being activated, replacing the
-- INSERT that used to be written inline in the DAL. Runs inside the caller's transaction.
--
-- The other queries that used to be inline here now reuse existing SPs instead:
--   stored status + practitioner ID -> "getById_sp_People"
--   linked login credentials        -> "getById_sp_all_users"
--
-- This one has no existing equivalent: "Register_Profile" expects the caller to supply and
-- validate pvar_userrole and reports problems as a message, whereas activation derives the
-- role from the person's work profile and must fail the transaction outright. The body below
-- is the same insert "Add_People" performs when a person is created complete.
CREATE OR REPLACE FUNCTION "Create_People_Activation_User"(
	pvar_usersid uuid,
	pvar_tenantid uuid,
	pvar_firstname Varchar(50),
	pvar_lastname Varchar(50),
	pvar_photo Varchar(4000),
	pvar_email Varchar(128),
	pvar_password Varchar(128),
	pvar_passwordkey Varchar(150),
	pvar_mobile Varchar(20),
	pvar_workprofile uuid,
	pvar_modifieduser uuid
)
RETURNS INTEGER
AS $BODY$
DECLARE
	inserted_count INTEGER;
BEGIN
	INSERT INTO users
	(
		usersid, tenantid, firstname, lastname, profilepicture, username,
		userpassword, passwordkey, emailid, mobilenumber, userrole,
		createduser, createddate, isdeleted
	)
	SELECT pvar_usersid, pvar_tenantid, pvar_firstname, pvar_lastname, pvar_photo, pvar_email,
	       pvar_password, pvar_passwordkey, pvar_email, pvar_mobile, WorkProfile.rolename,
	       pvar_modifieduser, NOW(), false
	FROM WorkProfile
	WHERE WorkProfile.WorkProfileid = pvar_workprofile
	AND COALESCE(WorkProfile.isdeleted, false) = false;

	GET DIAGNOSTICS inserted_count = ROW_COUNT;
	RETURN inserted_count;
END
$BODY$
LANGUAGE plpgsql;
