
CREATE OR REPLACE FUNCTION public."Add_Patient_Profile"(
	pvar_patientprofileid uuid,
	pvar_tenantid uuid,
	pvar_registrationid character varying,
	pvar_firstname character varying,
	pvar_lastname character varying,
	pvar_gender character varying,
	pvar_dateofbirth date,
	pvar_age integer,
	pvar_bloodgroup character varying,
	pvar_nationality character varying,
	pvar_countryoforigin uuid,
	pvar_emailaddress character varying,
	pvar_mobilenumber character varying,
	pvar_whatsappnumber character varying,
	pvar_photo character varying,
	pvar_paddressline1 character varying,
	pvar_paddressline2 character varying,
	pvar_pzip integer,
	pvar_ptown character varying,
	pvar_pcityordistrict character varying,
	pvar_ppstatename character varying,
	pvar_sameaspermanentaddress boolean,
	pvar_caddressline1 character varying,
	pvar_caddressline2 character varying,
	pvar_czip integer,
	pvar_ctown character varying,
	pvar_ccityordistrict character varying,
	pvar_cstatename character varying,
	pvar_idprooftype character varying,
	pvar_idproofnumber character varying,
	pvar_uploadidproof character varying,
	pvar_languagesknown character varying,
	pvar_languagespreferrable character varying,
	pvar_otherlanguages character varying,
	pvar_maritalstatus character varying,
	pvar_education character varying,
	pvar_occupation uuid,
	pvar_meditationpractice character varying,
	pvar_typeofpractice character varying,
	pvar_creativeactivities character varying,
	pvar_othercreativeactivities character varying,
	pvar_insurancetype character varying,
	pvar_insurancecompany character varying,
	pvar_policynumber character varying,
	pvar_policyclaimlimit numeric,
	pvar_policyexpirydate date,
	pvar_referralsource uuid,
	pvar_referraltype character varying,
	pvar_referrername character varying,
	pvar_referrerphonenumber character varying,
	pvar_magazinename character varying,
	pvar_socialmediaplatform character varying,
	pvar_otherreferral character varying,
	pvar_emergencycontactinfo json,
	pvar_createduser uuid,
	pvar_userpassword character varying,
	pvar_passwordkey character varying,
	pvar_usersid uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);lvar_curday_registrationid Varchar(10);lvar_val_registrationid int;
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/09/2026 10:03:56*/
		

			  
                                                                                    if pvar_PatientProfileid is null then
                                                                                    pvar_PatientProfileid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
pvar_registrationid := generate_formatted_numbers(
    pvar_tenantid,
    'YYYY-MM-999',    
    'patientprofile',  
    'registrationid'
);
			  IF "Check_Authorization"(pvar_createduser, 'PatientProfile', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from PatientProfile where upper(PatientProfile.registrationid::varchar) = upper(pvar_registrationid::varchar) and PatientProfile.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Registration ID Already Exists.';

																END IF;
IF EXISTS (SELECT * from PatientProfile where upper(PatientProfile.idproofnumber::varchar) = upper(pvar_idproofnumber::varchar) and PatientProfile.tenantid=pvar_tenantid)
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
															IF EXISTS (
  SELECT 1
  FROM users
  WHERE upper(emailid::varchar) = upper(pvar_emailaddress::varchar)
    AND tenantid = pvar_tenantid
    AND COALESCE(isdeleted, false) = false
)
THEN
  pvar_returnMessage := pvar_returnMessage || 'Email ID Already Exists in Users.';
END IF;
																  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO PatientProfile(
				 registrationid
,firstname
,lastname
,gender
,dateofbirth
,age
,bloodgroup
,nationality
,countryoforigin
,emailaddress
,mobilenumber
,whatsappnumber
,photo
,paddressline1
,paddressline2
,pzip
,ptown
,pcityordistrict
,ppstatename
,sameaspermanentaddress
,caddressline1
,caddressline2
,czip
,ctown
,ccityordistrict
,cstatename
,idprooftype
,idproofnumber
,uploadidproof
,languagesknown
,languagespreferrable
,otherlanguages
,maritalstatus
,education
,occupation
,meditationpractice
,typeofpractice
,creativeactivities
,othercreativeactivities
,insurancetype
,insurancecompany
,policynumber
,policyclaimlimit
,policyexpirydate
,referralsource
,referraltype
,referrername
,referrerphonenumber
,magazinename
,socialmediaplatform
,otherreferral

				 ,createduser
				 ,PatientProfileid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_registrationid
,pvar_firstname
,pvar_lastname
,pvar_gender
,pvar_dateofbirth
,pvar_age
,pvar_bloodgroup
,pvar_nationality
,pvar_countryoforigin
,pvar_emailaddress
,pvar_mobilenumber
,pvar_whatsappnumber
,pvar_photo
,pvar_paddressline1
,pvar_paddressline2
,pvar_pzip
,pvar_ptown
,pvar_pcityordistrict
,pvar_ppstatename
,pvar_sameaspermanentaddress
,pvar_caddressline1
,pvar_caddressline2
,pvar_czip
,pvar_ctown
,pvar_ccityordistrict
,pvar_cstatename
,pvar_idprooftype
,pvar_idproofnumber
,pvar_uploadidproof
,coalesce(pvar_languagesknown,'')
,coalesce(pvar_languagespreferrable,'')
,pvar_otherlanguages
,pvar_maritalstatus
,pvar_education
,pvar_occupation
,pvar_meditationpractice
,pvar_typeofpractice
,coalesce(pvar_creativeactivities,'')
,pvar_othercreativeactivities
,pvar_insurancetype
,pvar_insurancecompany
,pvar_policynumber
,pvar_policyclaimlimit
,pvar_policyexpirydate
,pvar_referralsource
,pvar_referraltype
,pvar_referrername
,pvar_referrerphonenumber
,pvar_magazinename
,pvar_socialmediaplatform
,pvar_otherreferral

				 ,pvar_createduser
				 ,pvar_PatientProfileid
				 ,pvar_tenantid
                   
			  );
			   
               
INSERT INTO users (
     usersid
    ,tenantid
    ,firstname
    ,lastname
    ,profilepicture
    ,username
    ,userpassword
    ,passwordkey
    ,emailid
    ,mobilenumber
    ,userrole
    ,createduser
    ,createddate
    ,isdeleted
)
VALUES (
     pvar_usersid
    ,pvar_tenantid
    ,pvar_firstname
    ,pvar_lastname
    ,pvar_photo
    ,pvar_emailaddress       
    ,pvar_userpassword
    ,pvar_passwordkey
    ,pvar_emailaddress
    ,pvar_mobilenumber
    ,'Health Seeker'
    ,pvar_createduser
    ,NOW()
    ,false
);
			  

			  
								
								
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
																,'Add_Patient_Profile'
																,'Authorization Failed Add_Patient_Profile'
																,pvar_createduser
																);
																pvar_returnMessage := '401.1';
																
																END IF;
			  /*EXCEPTION WHEN OTHERS THEN
			 
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
						,'Store Proc Exception'
						,NOW()
						,'16'
						,'Add_Patient_Profile'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Patient_Profile - Insert failed';*/
			  	
			  END
              
$BODY$;
