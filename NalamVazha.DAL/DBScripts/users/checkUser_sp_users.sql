
				CREATE OR REPLACE FUNCTION  "checkUser_sp_users"
				(
                    pvar_username Varchar(256),
                    pvar_userpassword Varchar(256),
                    pvar_source Varchar(256),
                    pvar_devicename Varchar(256),
                    pvar_deviceid Varchar(256),
                    pvar_notificationid Varchar(256),
                    pvar_mobilenumber Varchar(256),
                    pvar_tenantid uuid
				)
                RETURNS TABLE(
                firstname Varchar
,lastname Varchar
,profilepicture Varchar
,username Varchar
,userpassword Varchar
,emailid Varchar
,mobilenumber Varchar
,userrole Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,tenantid uuid
                ,usersid uuid
                ,tenantname Varchar(150)
                ,natureofbusiness character varying
                ,viewertenantids character varying
                
				
				
                )
				AS $BODY$
				DECLARE v_usersid uuid;
                BEGIN  

                IF(pvar_source='Internal') THEN
		            RETURN QUERY
					SELECT 
					users.firstname
,users.lastname
,users.profilepicture
,users.username
,users.userpassword
,users.emailid
,users.mobilenumber
,users.userrole

					,users.createduser,users.createddate,users.modifieduser,users.modifieddate
					,coalesce(users.tenantid,'00000000-0000-0000-0000-000000000000') as tenantid
                    ,users.usersid
					,coalesce((SELECT tenant.businessname from tenant where tenant.tenantid=users.tenantid  AND tenant.isdeleted=false),'') as tenantname
                     ,coalesce((SELECT tenant.natureofbusiness from tenant where tenant.tenantid=users.tenantid),'') as natureofbusiness
					,coalesce(users.viewertenantids,'') as viewertenantids
                    
					
					
					FROM users
					WHERE 
                    (upper(BTRIM(users.username)) = upper(BTRIM(pvar_username))
		            OR (
                        LENGTH(regexp_replace(COALESCE(pvar_username, ''), '[^0-9]', '', 'g')) >= 10
                        AND RIGHT(regexp_replace(COALESCE(users.mobilenumber, ''), '[^0-9]', '', 'g'), 10)
                            = RIGHT(regexp_replace(pvar_username, '[^0-9]', '', 'g'), 10)
                    )
                    OR upper(BTRIM(users.emailid)) = upper(BTRIM(pvar_username))
                    OR (
                        users.userrole = 'Health Seeker'
                        AND users.tenantid = pvar_tenantid
                        AND EXISTS (
                            SELECT 1
                            FROM IPDApplicationForm ipd
                            WHERE ipd.patientname = users.usersid
                              AND ipd.tenantid = pvar_tenantid
                              AND ipd.isdeleted = false
                              AND upper(BTRIM(ipd.bookingreferencenumber)) = upper(BTRIM(pvar_username))
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
                              AND upper(BTRIM(patient.registrationid)) = upper(BTRIM(pvar_username))
                        )
                    ))
                    AND users.userpassword=pvar_userpassword
					 AND users.isdeleted=false
                    ORDER BY
                        CASE WHEN users.tenantid = pvar_tenantid THEN 0 ELSE 1 END
                    LIMIT 1;
                ELSE IF(pvar_source='allowbyid') THEN
                     RETURN QUERY
					SELECT 
					users.firstname
,users.lastname
,users.profilepicture
,users.username
,users.userpassword
,users.emailid
,users.mobilenumber
,users.userrole

					,users.createduser,users.createddate,users.modifieduser,users.modifieddate
					,coalesce(users.tenantid,'00000000-0000-0000-0000-000000000000') as tenantid
                    ,users.usersid
					,coalesce((SELECT tenant.businessname from tenant where tenant.tenantid=users.tenantid  AND tenant.isdeleted=false),'') as tenantname
 ,coalesce((SELECT tenant.natureofbusiness from tenant where tenant.tenantid=users.tenantid),'') as natureofbusiness
 ,coalesce(users.viewertenantids,'') as viewertenantids
                    
					
					
					FROM users
					WHERE  CAST(users.usersid AS Varchar)=pvar_username 
                      AND users.isdeleted=false;
                ELSE

                               
                         SELECT users.usersid 
                                        INTO v_usersid 
                                        FROM   users
                                        WHERE  (upper(users.username)= upper(pvar_username)
                                        OR     users.mobilenumber=pvar_username
                                        OR     users.emailid=pvar_username) LIMIT 1; 

                        IF(pvar_mobilenumber IS NOT NULL) THEN
                                SELECT 
					        users.usersid INTO v_usersid 
					        FROM users
					        WHERE 
                            (upper(users.username)= upper(pvar_username)
		                    OR  users.mobilenumber=pvar_username
                            OR  users.mobilenumber=pvar_mobilenumber
                            OR users.emailid=pvar_username)
                           -- AND users.userpassword=pvar_userpassword
					         AND users.isdeleted=false;
                        END IF;
             

                            IF NOT EXISTS
                                (SELECT *
                                FROM   users_alloweddevices
                                WHERE  deviceid=pvar_deviceid OR notificationid=pvar_notificationid) THEN
 

                                        INSERT INTO users_alloweddevices 
                                        (
                                        users_alloweddevicesid
                                        ,usersid
                                        ,devicename
                                        ,deviceid
                                        ,notificationid
                                        )
                                        VALUES
                                        (
                                        gen_random_uuid(),
                                        v_usersid,
                                        pvar_devicename,
                                        pvar_deviceid,
                                        pvar_notificationid
                                        );
                        
                          END IF;
                        
                                RETURN QUERY 	
                                    SELECT 
					                users.firstname
,users.lastname
,users.profilepicture
,users.username
,users.userpassword
,users.emailid
,users.mobilenumber
,users.userrole

					                ,users.createduser,users.createddate,users.modifieduser,users.modifieddate
					                ,coalesce(users.tenantid,'00000000-0000-0000-0000-000000000000') as tenantid
                                    ,users.usersid
					                ,coalesce((SELECT tenant.businessname from tenant where tenant.tenantid=users.tenantid  AND tenant.isdeleted=false),'') as tenantname
 ,coalesce((SELECT tenant.natureofbusiness from tenant where tenant.tenantid=users.tenantid),'') as natureofbusiness
 ,coalesce(users.viewertenantids,'') as viewertenantids
                                    
									
									
					                FROM users
					                 WHERE  users.usersid=v_usersid 
                                 AND users.isdeleted=false;

                        END IF;

                END IF;
                 




					
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


