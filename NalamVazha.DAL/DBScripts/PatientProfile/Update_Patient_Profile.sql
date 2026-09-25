
			  CREATE OR REPLACE FUNCTION  "Update_Patient_Profile"
			  (
				  pvar_PatientProfileid uuid
,pvar_tenantid uuid
,
pvar_registrationid Varchar(256)
,
pvar_firstname Varchar(128)
,
pvar_lastname Varchar(128)
,
pvar_gender  Varchar(1024)
,
pvar_dateofbirth date
,
pvar_age int
,
pvar_nationality  Varchar(1024)
,
pvar_countryoforigin  uuid
,
pvar_emailaddress Varchar(128)
,
pvar_mobilenumber Varchar(10)
,
pvar_whatsappnumber Varchar(10)
,
pvar_photo Varchar(256)
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
pvar_ppstatename Varchar(256)
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
pvar_idprooftype  Varchar(1024)
,
pvar_idproofnumber Varchar(128)
,
pvar_uploadidproof Varchar(256)
,
pvar_languagesknown Varchar(256)
,
pvar_languagespreferrable Varchar(256)
,
pvar_otherlanguages Varchar(256)
,
pvar_maritalstatus  Varchar(1024)
,
pvar_education  Varchar(1024)
,
pvar_occupation  uuid
,
pvar_meditationpractice  Varchar(1024)
,
pvar_typeofpractice Varchar(128)
,
pvar_creativeactivities Varchar(256)
,
pvar_othercreativeactivities Varchar(128)
,
pvar_insurancetype  Varchar(1024)
,
pvar_insurancecompany Varchar(128)
,
pvar_policynumber Varchar(128)
,
pvar_policyclaimlimit decimal(18,2)
,
pvar_policyexpirydate date
,
pvar_referralsource  uuid
,
pvar_referraltype  Varchar(1024)
,
pvar_referrername Varchar(128)
,
pvar_referrerphonenumber Varchar(10)
,
pvar_magazinename Varchar(128)
,
pvar_socialmediaplatform  Varchar(1024)
,
pvar_otherreferral Varchar(128)
,pvar_emergencycontactinfo json

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              lvar_current_email Varchar(128);
              lvar_healthseeker_userid uuid;
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:57*/
			  IF "Check_Authorization"(pvar_modifieduser, 'PatientProfile', 'edit') THEN


			  pvar_returnMessage:='';

			  SELECT emailaddress INTO lvar_current_email
			  FROM PatientProfile
			  WHERE PatientProfileid=pvar_PatientProfileid
			    AND tenantid=pvar_tenantid
			    AND COALESCE(isdeleted, false)=false;

			  SELECT usersid INTO lvar_healthseeker_userid
			  FROM users
			  WHERE userrole='Health Seeker'
			    AND COALESCE(isdeleted, false)=false
			    AND tenantid=pvar_tenantid
			    AND (usersid=pvar_PatientProfileid
			         OR upper(emailid)=upper(lvar_current_email)
			         OR upper(username)=upper(lvar_current_email))
			  ORDER BY CASE WHEN usersid=pvar_PatientProfileid THEN 0 ELSE 1 END
			  LIMIT 1;

			  IF lvar_current_email IS NULL THEN
				pvar_returnMessage := 'Profile not found.';
			  ELSIF upper(trim(lvar_current_email)) <> upper(trim(pvar_emailaddress)) THEN
				pvar_returnMessage := 'Email change requires OTP verification.';
			  ELSIF lvar_healthseeker_userid IS NULL THEN
				pvar_returnMessage := 'Linked user account not found. Profile was not updated.';
			  END IF;

			  if EXISTS (SELECT * from PatientProfile where upper(PatientProfile.registrationid) = upper(pvar_registrationid) and PatientProfile.tenantid=pvar_tenantid  and PatientProfile.PatientProfileid <> pvar_PatientProfileid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Registration ID Already Exists.';

																END IF;
if EXISTS (SELECT * from PatientProfile where upper(PatientProfile.idproofnumber) = upper(pvar_idproofnumber) and PatientProfile.tenantid=pvar_tenantid  and PatientProfile.PatientProfileid <> pvar_PatientProfileid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'ID Proof Number Already Exists.';

																END IF;

               IF(pvar_creativeactivities is not null AND pvar_creativeactivities!='0' AND LENGTH(pvar_creativeactivities)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_creativeactivities, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='creativeactivities'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_creativeactivities, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'creativeactivities value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_education is not null AND pvar_education!='0' AND LENGTH(pvar_education)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_education, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='education'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_education, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'education value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_gender is not null AND pvar_gender!='0' AND LENGTH(pvar_gender)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_gender, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='gender'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_gender, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'gender value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_idprooftype is not null AND pvar_idprooftype!='0' AND LENGTH(pvar_idprooftype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_idprooftype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='idprooftype'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_idprooftype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'idprooftype value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_insurancetype is not null AND pvar_insurancetype!='0' AND LENGTH(pvar_insurancetype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_insurancetype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='insurancetype'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_insurancetype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'insurancetype value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_languagesknown is not null AND pvar_languagesknown!='0' AND LENGTH(pvar_languagesknown)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_languagesknown, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='languagesknown'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_languagesknown, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'languagesknown value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_languagespreferrable is not null AND pvar_languagespreferrable!='0' AND LENGTH(pvar_languagespreferrable)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_languagespreferrable, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='languagespreferrable'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_languagespreferrable, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'languagespreferrable value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_maritalstatus is not null AND pvar_maritalstatus!='0' AND LENGTH(pvar_maritalstatus)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_maritalstatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='maritalstatus'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_maritalstatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'maritalstatus value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_meditationpractice is not null AND pvar_meditationpractice!='0' AND LENGTH(pvar_meditationpractice)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_meditationpractice, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='meditationpractice'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_meditationpractice, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'meditationpractice value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_nationality is not null AND pvar_nationality!='0' AND LENGTH(pvar_nationality)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_nationality, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='nationality'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_nationality, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'nationality value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_referraltype is not null AND pvar_referraltype!='0' AND LENGTH(pvar_referraltype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_referraltype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='referraltype'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_referraltype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'referraltype value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_socialmediaplatform is not null AND pvar_socialmediaplatform!='0' AND LENGTH(pvar_socialmediaplatform)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_socialmediaplatform, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='socialmediaplatform'
                                                                and entityname='PatientProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_socialmediaplatform, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'socialmediaplatform value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN

                    INSERT INTO history
VALUES('PatientProfile', NOW(),
(SELECT query_to_xml('SELECT * FROM PatientProfile WHERE PatientProfile.PatientProfileid= '''||pvar_PatientProfileid||'''', true, false, '')));

                    
                    UPDATE PatientProfile SET
                    registrationid=pvar_registrationid
,firstname=pvar_firstname
,lastname=pvar_lastname
,gender=pvar_gender
,dateofbirth=pvar_dateofbirth
,age=pvar_age
,nationality=pvar_nationality
,countryoforigin=pvar_countryoforigin
,emailaddress=pvar_emailaddress
,mobilenumber=pvar_mobilenumber
,whatsappnumber=pvar_whatsappnumber
,photo=pvar_photo
,paddressline1=pvar_paddressline1
,paddressline2=pvar_paddressline2
,pzip=pvar_pzip
,ptown=pvar_ptown
,pcityordistrict=pvar_pcityordistrict
,ppstatename=pvar_ppstatename
,sameaspermanentaddress=pvar_sameaspermanentaddress
,caddressline1=pvar_caddressline1
,caddressline2=pvar_caddressline2
,czip=pvar_czip
,ctown=pvar_ctown
,ccityordistrict=pvar_ccityordistrict
,cstatename=pvar_cstatename
,idprooftype=pvar_idprooftype
,idproofnumber=pvar_idproofnumber
,uploadidproof=pvar_uploadidproof
,languagesknown=coalesce(pvar_languagesknown,'')
,languagespreferrable=coalesce(pvar_languagespreferrable,'')
,otherlanguages=pvar_otherlanguages
,maritalstatus=pvar_maritalstatus
,education=pvar_education
,occupation=pvar_occupation
,meditationpractice=pvar_meditationpractice
,typeofpractice=pvar_typeofpractice
,creativeactivities=coalesce(pvar_creativeactivities,'')
,othercreativeactivities=pvar_othercreativeactivities
,insurancetype=pvar_insurancetype
,insurancecompany=pvar_insurancecompany
,policynumber=pvar_policynumber
,policyclaimlimit=pvar_policyclaimlimit
,policyexpirydate=pvar_policyexpirydate
,referralsource=pvar_referralsource
,referraltype=pvar_referraltype
,referrername=pvar_referrername
,referrerphonenumber=pvar_referrerphonenumber
,magazinename=pvar_magazinename
,socialmediaplatform=pvar_socialmediaplatform
,otherreferral=pvar_otherreferral

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE PatientProfileid=pvar_PatientProfileid;

                    INSERT INTO history
                        VALUES('users', NOW(),
                        (SELECT query_to_xml('SELECT * FROM users WHERE users.usersid= '''||lvar_healthseeker_userid||'''', true, false, '')));

                        UPDATE users SET
                        firstname=pvar_firstname
                        ,lastname=pvar_lastname
                        ,profilepicture=pvar_photo
                        ,username=pvar_emailaddress
                        ,emailid=pvar_emailaddress
                        ,mobilenumber=pvar_mobilenumber
                        ,modifieduser=pvar_modifieduser
                        ,modifieddate=NOW()
                        WHERE usersid=lvar_healthseeker_userid;

                    

                    INSERT INTO history
VALUES('PatientProfile_emergencycontactinfo', NOW(),
(SELECT query_to_xml('SELECT * FROM PatientProfile_emergencycontactinfo WHERE PatientProfile_emergencycontactinfo.PatientProfileid= '''||pvar_PatientProfileid||'''', true, false, '')));

								DELETE FROM  PatientProfile_emergencycontactinfo WHERE PatientProfileid=pvar_PatientProfileid;
								
								
								INSERT INTO PatientProfile_emergencycontactinfo (
									PatientProfileid
									,PatientProfile_emergencycontactinfoid 
                                    ,record_order  
									,personname
,relationship
,phonenumber

									
									)
									SELECT 
									pvar_PatientProfileid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'personname' as personname
,j->>'relationship' as relationship
,j->>'phonenumber' as phonenumber

									
                                    FROM json_array_elements(pvar_emergencycontactinfo) as j;
									



					
							
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
																,'Update_Patient_Profile'
																,'Authorization Failed Update_Patient_Profile'
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
						,'Update_Patient_Profile'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Patient_Profile - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

