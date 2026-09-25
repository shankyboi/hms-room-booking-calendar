
			  CREATE OR REPLACE FUNCTION  "Update_Profile"
			  (
				  pvar_usersid uuid
,pvar_tenantid uuid
,
pvar_firstname Varchar(50)
,
pvar_lastname Varchar(50)
,
pvar_profilepicture Varchar(1024)
,
pvar_username Varchar(150)
,
pvar_emailid Varchar(128)
,
pvar_mobilenumber Varchar(20)
,
pvar_userrole   Varchar(1024)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:21*/
			  


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from users where upper(users.username) = upper(pvar_username)  and users.usersid <> pvar_usersid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'UserName Already Exists.';

																END IF;
if EXISTS (SELECT * from users where upper(users.emailid) = upper(pvar_emailid)  and users.usersid <> pvar_usersid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Email ID Already Exists.';

																END IF;
if EXISTS (SELECT * from users where upper(users.mobilenumber) = upper(pvar_mobilenumber)  and users.usersid <> pvar_usersid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Mobile Number Already Exists.';

																END IF;

               IF(pvar_userrole is not null AND pvar_userrole!='0' AND LENGTH(pvar_userrole)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                    FROM regexp_split_to_table(pvar_userrole, ',') AS T1
                                                                        INNER JOIN (Select DISTINCT roles.rolename from roles) AS T2 on T1.T1 = T2.rolename) AS int) <> CAST((SELECT Count(T1.T1)
                                                                    FROM regexp_split_to_table(pvar_userrole, ',')  AS T1) AS int))
                                                                    THEN
                                                                         pvar_returnMessage := pvar_returnMessage || ' userrole value is invalid';


                                                                    END IF;
                                                                    END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('users', NOW(),
(SELECT query_to_xml('SELECT * FROM users WHERE users.usersid= '''||pvar_usersid||'''', true, false, '')));

                    
                    UPDATE users SET
                    firstname=pvar_firstname
,lastname=pvar_lastname
,profilepicture=pvar_profilepicture
,username=pvar_username
,emailid=pvar_emailid
,mobilenumber=pvar_mobilenumber
,userrole=pvar_userrole

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE usersid=pvar_usersid;

                    update users set viewertenantids=tenantid::varchar||','||(SELECT STRING_AGG(tenantid::varchar, ',') AS tenantids
FROM tenant
WHERE parentid =(select tenantid from users where usersid=pvar_usersid))
where usersid=pvar_usersid;

                    


					
							
					pvar_returnMessage :='201.1';
			
			  END IF;

			  

			  			 /* EXCEPTION WHEN OTHERS THEN
			 
						INSERT INTO system_logging
						(
						Log_code
						,system_logging_guid
						,log_application
						,log_date
						,log_level
						,log_logger
						,log_message
						)
						VALUES
						('16'
						,gen_random_uuid()
						,'Postgre Function Exception'
						,NOW()
						,'16'
						,'Update_Profile'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Profile - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

