
			  CREATE OR REPLACE FUNCTION  "Update_People"
			  (
				  pvar_Peopleid uuid
,pvar_tenantid uuid
,
pvar_practitionerid Varchar(256)
,
pvar_firstname Varchar(128)
,
pvar_lastname Varchar(128)
,
pvar_workprofile  uuid
,
pvar_competencylevel  uuid
,
pvar_designation  uuid
,
pvar_contactnumber Varchar(10)
,
pvar_whatsappnumber Varchar(10)
,
pvar_emailid Varchar(128)
,
pvar_gender  Varchar(1024)
,
pvar_dob date
,
pvar_age int
,
pvar_employmentstatus  Varchar(1024)
,
pvar_joiningdate date
,
pvar_contractrenewaldate date
,
pvar_photo Varchar(256)
,
pvar_nationality  Varchar(1024)
,
pvar_specifycountry  uuid
,
pvar_idtype  Varchar(1024)
,
pvar_idnumber Varchar(128)
,
pvar_iddocument Varchar(256)
,
pvar_paddressline1 Varchar(256)
,
pvar_paddressline2 Varchar(256)
,
pvar_pzip int
,
pvar_ptown Varchar(128)
,
pvar_pcityordistrict Varchar(256)
,
pvar_pstatename Varchar(256)
,
pvar_sameaspermanentaddress Boolean
,
pvar_caddressline1 Varchar(128)
,
pvar_caddressline2 Varchar(128)
,
pvar_czip int
,
pvar_ctown Varchar(128)
,
pvar_ccityordistrict Varchar(128)
,
pvar_cstatename Varchar(128)
,
pvar_registrationnumber Varchar(128)
,
pvar_validtill date
,
pvar_licenceupload Varchar(256)
,
pvar_issuingauthority Varchar(128)
,
pvar_bio text
,pvar_screeningmeetinglink varchar
,pvar_emergencycontact json
,pvar_educationinfo json
,pvar_workexperience json
,pvar_preferredlanguageinfo json
,pvar_clinicaltaskinfo json
,pvar_status varchar(16)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              lvar_current_email Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 11:34:27*/
			  IF "Check_Authorization"(pvar_modifieduser, 'People', 'edit') THEN


			  pvar_returnMessage:='';

			  SELECT emailid INTO lvar_current_email
			  FROM People
			  WHERE Peopleid=pvar_Peopleid
			    AND tenantid=pvar_tenantid
			    AND COALESCE(isdeleted, false)=false;

			  IF NOT EXISTS (SELECT 1 FROM People WHERE Peopleid=pvar_Peopleid AND tenantid=pvar_tenantid AND COALESCE(isdeleted,false)=false) THEN
				pvar_returnMessage := 'Profile not found.';
			  ELSIF pvar_status<>'Draft' AND upper(trim(lvar_current_email)) <> upper(trim(pvar_emailid)) THEN
				pvar_returnMessage := 'Email change requires OTP verification.';
			  ELSIF pvar_status<>'Draft' AND NOT EXISTS (
				SELECT 1 FROM users
				WHERE usersid=pvar_Peopleid
				  AND tenantid=pvar_tenantid
				  AND COALESCE(isdeleted, false)=false
			  ) THEN
				pvar_returnMessage := 'Linked user account not found. Profile was not updated.';
			  END IF;

			  if EXISTS (SELECT * from People where upper(People.practitionerid) = upper(pvar_practitionerid) and People.tenantid=pvar_tenantid  and People.Peopleid <> pvar_Peopleid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Practitioner ID Already Exists.';

																END IF;
if EXISTS (SELECT * from People where upper(People.emailid) = upper(pvar_emailid) and People.tenantid=pvar_tenantid  and People.Peopleid <> pvar_Peopleid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Email ID Already Exists.';

																END IF;

IF pvar_returnMessage='' AND pvar_status<>'Draft' AND EXISTS (
	SELECT 1
	FROM users
	WHERE usersid=pvar_Peopleid
	  AND COALESCE(isdeleted, false)=false
) THEN
	IF EXISTS (
		SELECT 1
		FROM users
		WHERE UPPER(emailid)=UPPER(pvar_emailid)
		  AND usersid<>pvar_Peopleid
	) THEN
		pvar_returnMessage := 'Email ID Already Exists.';
	ELSIF EXISTS (
		SELECT 1
		FROM users
		WHERE UPPER(mobilenumber)=UPPER(pvar_contactnumber)
		  AND usersid<>pvar_Peopleid
	) THEN
		pvar_returnMessage := 'Mobile Number Already Exists.';
	ELSIF NOT EXISTS (
		SELECT 1
		FROM WorkProfile
		WHERE WorkProfileid=pvar_workprofile
		  AND COALESCE(isdeleted, false)=false
	) THEN
		pvar_returnMessage := 'Work Profile role not found.';
	END IF;
END IF;

               IF(pvar_employmentstatus is not null AND pvar_employmentstatus!='0' AND LENGTH(pvar_employmentstatus)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_employmentstatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='employmentstatus'
                                                                and entityname='People' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_employmentstatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'employmentstatus value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_gender is not null AND pvar_gender!='0' AND LENGTH(pvar_gender)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_gender, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='gender'
                                                                and entityname='People' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_gender, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'gender value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_idtype is not null AND pvar_idtype!='0' AND LENGTH(pvar_idtype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_idtype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='idtype'
                                                                and entityname='People' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_idtype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'idtype value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_nationality is not null AND pvar_nationality!='0' AND LENGTH(pvar_nationality)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_nationality, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='nationality'
                                                                and entityname='People' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_nationality, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'nationality value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('People', NOW(),
(SELECT query_to_xml('SELECT * FROM People WHERE People.Peopleid= '''||pvar_Peopleid||'''', true, false, '')));

                    
                    UPDATE People SET
                    firstname=pvar_firstname
,status=CASE WHEN pvar_status='Draft' THEN 'Draft' ELSE 'Active' END
,lastname=pvar_lastname
,workprofile=pvar_workprofile
,competencylevel=pvar_competencylevel
,designation=pvar_designation
,contactnumber=pvar_contactnumber
,whatsappnumber=pvar_whatsappnumber
,emailid=pvar_emailid
,gender=pvar_gender
,dob=pvar_dob
,age=pvar_age
,employmentstatus=pvar_employmentstatus
,joiningdate=pvar_joiningdate
,contractrenewaldate=pvar_contractrenewaldate
,photo=pvar_photo
,nationality=pvar_nationality
,specifycountry=pvar_specifycountry
,idtype=pvar_idtype
,idnumber=pvar_idnumber
,iddocument=pvar_iddocument
,paddressline1=pvar_paddressline1
,paddressline2=pvar_paddressline2
,pzip=pvar_pzip
,ptown=pvar_ptown
,pcityordistrict=pvar_pcityordistrict
,pstatename=pvar_pstatename
,sameaspermanentaddress=pvar_sameaspermanentaddress
,caddressline1=pvar_caddressline1
,caddressline2=pvar_caddressline2
,czip=pvar_czip
,ctown=pvar_ctown
,ccityordistrict=pvar_ccityordistrict
,cstatename=pvar_cstatename
,registrationnumber=pvar_registrationnumber
,validtill=pvar_validtill
,licenceupload=pvar_licenceupload
,issuingauthority=pvar_issuingauthority
,bio=pvar_bio
,screeningmeetinglink=pvar_screeningmeetinglink

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Peopleid=pvar_Peopleid;

                    

                    INSERT INTO history
VALUES('People_emergencycontact', NOW(),
(SELECT query_to_xml('SELECT * FROM People_emergencycontact WHERE People_emergencycontact.Peopleid= '''||pvar_Peopleid||'''', true, false, '')));

								DELETE FROM  People_emergencycontact WHERE Peopleid=pvar_Peopleid;
								
								
								INSERT INTO People_emergencycontact (
									Peopleid
									,People_emergencycontactid 
                                    ,record_order  
									,personname
,relationship
,phonenumber

									
									)
									SELECT 
									pvar_Peopleid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'personname' as personname
,j->>'relationship' as relationship
,j->>'phonenumber' as phonenumber

									
                                    FROM json_array_elements(pvar_emergencycontact) as j;
									
INSERT INTO history
VALUES('People_educationinfo', NOW(),
(SELECT query_to_xml('SELECT * FROM People_educationinfo WHERE People_educationinfo.Peopleid= '''||pvar_Peopleid||'''', true, false, '')));

								DELETE FROM  People_educationinfo WHERE Peopleid=pvar_Peopleid;
								
								
								INSERT INTO People_educationinfo (
									Peopleid
									,People_educationinfoid 
                                    ,record_order  
									,fieldofstudy
,degree
,educationinstitution
,certificationnumber
,yearofgraduation
,degreestatus

									
									)
									SELECT 
									pvar_Peopleid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'fieldofstudy' as fieldofstudy
,j->>'degree' as degree
,j->>'educationinstitution' as educationinstitution
,j->>'certificationnumber' as certificationnumber
,CAST(j->>'yearofgraduation' AS int) as yearofgraduation
,j->>'degreestatus' as degreestatus

									
                                    FROM json_array_elements(pvar_educationinfo) as j;
									
INSERT INTO history
VALUES('People_workexperience', NOW(),
(SELECT query_to_xml('SELECT * FROM People_workexperience WHERE People_workexperience.Peopleid= '''||pvar_Peopleid||'''', true, false, '')));

								DELETE FROM  People_workexperience WHERE Peopleid=pvar_Peopleid;
								
								
								INSERT INTO People_workexperience (
									Peopleid
									,People_workexperienceid 
                                    ,record_order  
									,designation
,institutionname
,fromdate
,todate

									
									)
									SELECT 
									pvar_Peopleid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'designation' as designation
,j->>'institutionname' as institutionname
,CAST(j->>'fromdate' AS date) as fromdate
,CAST(j->>'todate' AS date) as todate

									
                                    FROM json_array_elements(pvar_workexperience) as j;
									
INSERT INTO history
VALUES('People_preferredlanguageinfo', NOW(),
(SELECT query_to_xml('SELECT * FROM People_preferredlanguageinfo WHERE People_preferredlanguageinfo.Peopleid= '''||pvar_Peopleid||'''', true, false, '')));

								DELETE FROM  People_preferredlanguageinfo WHERE Peopleid=pvar_Peopleid;
								
								
								INSERT INTO People_preferredlanguageinfo (
									Peopleid
									,People_preferredlanguageinfoid 
                                    ,record_order  
									,languagesknown
,proficiency
,ability

									
									)
									SELECT 
									pvar_Peopleid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'languagesknown' as languagesknown
,j->>'proficiency' as proficiency
,j->>'ability' as ability

									
                                    FROM json_array_elements(pvar_preferredlanguageinfo) as j;
									
INSERT INTO history
VALUES('People_clinicaltaskinfo', NOW(),
(SELECT query_to_xml('SELECT * FROM People_clinicaltaskinfo WHERE People_clinicaltaskinfo.Peopleid= '''||pvar_Peopleid||'''', true, false, '')));

								DELETE FROM  People_clinicaltaskinfo WHERE Peopleid=pvar_Peopleid;
								
								
								INSERT INTO People_clinicaltaskinfo (
									Peopleid
									,People_clinicaltaskinfoid 
                                    ,record_order  
									,consultations
,workprofile
,tasktype
,taskname
,durationinminutes
,overbookingcount
,availableon
,workhourstarts
,workhourends
,priority
,feesamount

									
									)
									SELECT 
									pvar_Peopleid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,coalesce(j->>'consultations',gen_random_uuid()::varchar) as consultations
,CAST(j->>'workprofile' AS uuid) as workprofile
,CAST(j->>'tasktype' AS uuid) as tasktype
,CAST(j->>'taskname' AS  uuid) as taskname
,CAST(j->>'durationinminutes' AS int) as durationinminutes
,CAST(j->>'overbookingcount' AS int) as overbookingcount
,j->>'availableon' as availableon
,j->>'workhourstarts' as workhourstarts
,j->>'workhourends' as workhourends
,j->>'priority' as priority
,CAST(j->>'feesamount' AS decimal(18,2)) as feesamount

									
                                    FROM json_array_elements(pvar_clinicaltaskinfo) as j;
									
					INSERT INTO history
					SELECT 'users', NOW(),
						query_to_xml(
							'SELECT * FROM users WHERE usersid = ''' || pvar_Peopleid::text || '''',
							true,
							false,
							''
						)
					WHERE EXISTS (
						SELECT 1
						FROM users
						WHERE usersid=pvar_Peopleid
						  AND COALESCE(isdeleted, false)=false
					);

					UPDATE users
					SET firstname=pvar_firstname,
						lastname=pvar_lastname,
						profilepicture=pvar_photo,
						username=pvar_emailid,
						emailid=pvar_emailid,
						mobilenumber=pvar_contactnumber,
						userrole=WorkProfile.rolename,
						modifieduser=pvar_modifieduser,
						modifieddate=NOW()
					FROM WorkProfile
					WHERE users.usersid=pvar_Peopleid
					  AND COALESCE(users.isdeleted, false)=false
					  AND WorkProfile.WorkProfileid=pvar_workprofile
					  AND COALESCE(WorkProfile.isdeleted, false)=false;



					
							
					pvar_returnMessage :='201.1';
			
			  END IF;

			  
																ELSE
																

															
																INSERT INTO system_logging
																(
																Log_code
																,system_logging_guid
																,log_application
																,log_date
																,log_level
																,log_logger
																,log_message
																,log_user_name
																)
																VALUES
																('401.1'
																,gen_random_uuid()
																,'Store Proc Authorization Check'
																,NOW()
																,'Critical'
																,'Update_People'
																,'Authorization Failed Update_People'
																,pvar_modifieduser
																);
																pvar_returnMessage = '401.1';
																
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
						,'Update_People'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_People - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

