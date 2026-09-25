
			  CREATE OR REPLACE FUNCTION  "checkUserKey_sp_users"
			  (
				  pvar_username Varchar(128),
				  pvar_tenantid uuid
			  )
              RETURNS TABLE(
                username Varchar(128)
                ,userpassword Varchar(128)
                ,passwordkey Varchar(128)
                )
                AS $BODY$
			   BEGIN 
                    RETURN QUERY
					SELECT 
					users.username,
					users.userpassword,
					users.passwordkey
					FROM users
					WHERE 
                    (UPPER(BTRIM(users.username)) = UPPER(BTRIM(pvar_username))
					OR (
						LENGTH(regexp_replace(COALESCE(pvar_username, ''), '[^0-9]', '', 'g')) >= 10
						AND (pvar_tenantid IS NULL OR users.tenantid = pvar_tenantid)
						AND RIGHT(regexp_replace(COALESCE(users.mobilenumber, ''), '[^0-9]', '', 'g'), 10)
							= RIGHT(regexp_replace(pvar_username, '[^0-9]', '', 'g'), 10)
					)
					OR UPPER(BTRIM(users.emailid)) = UPPER(BTRIM(pvar_username))
					OR (
						users.userrole = 'Health Seeker'
						AND users.tenantid = pvar_tenantid
						AND EXISTS (
							SELECT 1
							FROM IPDApplicationForm ipd
							WHERE ipd.patientname = users.usersid
							  AND ipd.tenantid = pvar_tenantid
							  AND ipd.isdeleted = false
							  AND UPPER(BTRIM(ipd.bookingreferencenumber)) = UPPER(BTRIM(pvar_username))
						)
					)
					OR (
						users.userrole = 'Health Seeker'
						AND users.tenantid = pvar_tenantid
						AND EXISTS (
							SELECT 1
							FROM PatientProfile patient
							WHERE patient.PatientProfileid = users.usersid
							  AND patient.tenantid = pvar_tenantid
							  AND patient.isdeleted = false
							  AND UPPER(BTRIM(patient.registrationid)) = UPPER(BTRIM(pvar_username))
						)
					)
					)
					 AND users.isdeleted=false
					ORDER BY
						CASE WHEN users.tenantid = pvar_tenantid THEN 0 ELSE 1 END;
			  END
              $BODY$
              LANGUAGE plpgsql;
CREATE OR REPLACE FUNCTION "checkoauthauthentication_sp_users"(
pvar_userdata json)
    RETURNS TABLE(firstname character varying, lastname character varying, profilepicture character varying, username character varying, userpassword character varying, emailid character varying, mobilenumber character varying, userrole character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, tenantid uuid,tenantname character varying, natureofbusiness character varying,viewertenantids character varying, usersid uuid) 


AS $BODY$

BEGIN  

IF (pvar_userdata->>'source' = 'AzureADJwtBearer' OR pvar_userdata->>'source' = 'GoogleOpenIdConnect' OR pvar_userdata->>'source' = 'oidc') THEN
 
	IF NOT EXISTS(SELECT 1 FROM users WHERE UPPER(users.username) = UPPER(pvar_userdata->>'first_name') and users.emailid = pvar_userdata->>'email'
	AND users.isdeleted = false
	)
		THEN -- Insert new user if not found
		INSERT INTO users(firstname, lastname, profilepicture, username, userpassword, emailid, mobilenumber, userrole, passwordkey, createduser, usersid, tenantid) 
		VALUES (
                pvar_userdata->>'first_name', '', NULL, pvar_userdata->>'first_name', 
                'xxxxx', pvar_userdata->>'email', 'xxxxxxxxxx', pvar_userdata->>'role', 
                NULL, NULL, gen_random_uuid(), NULL
            );
	
	END IF;
	-- Return the user details after insertion or if the user already exists
	RETURN QUERY
		SELECT 
		users.firstname,
		users.lastname,
		users.profilepicture,
		users.username,
		users.userpassword,
		users.emailid,
		users.mobilenumber,
		users.userrole,
		users.createduser,
		users.createddate,
		users.modifieduser,
		users.modifieddate,
		COALESCE(users.tenantid, '00000000-0000-0000-0000-000000000000') AS tenantid,
coalesce((SELECT tenant.businessname from tenant where tenant.tenantid=users.tenantid),'') as tenantname,
coalesce((SELECT tenant.natureofbusiness from tenant where tenant.tenantid=users.tenantid),'') as natureofbusiness,
coalesce(users.viewertenantids,'') as viewertenantids,
		users.usersid
		FROM users
		WHERE UPPER(users.username) = UPPER(pvar_userdata->>'first_name')
		and users.emailid = pvar_userdata->>'email'
		and users.userrole = pvar_userdata->>'role'
		AND users.isdeleted = false;
	
END IF;

END

$BODY$
LANGUAGE plpgsql;


