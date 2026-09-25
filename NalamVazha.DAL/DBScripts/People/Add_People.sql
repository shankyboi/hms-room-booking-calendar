
			  CREATE OR REPLACE FUNCTION  "Add_People"
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

				  ,pvar_createduser  uuid 
				  ,pvar_userpassword varchar
				  ,pvar_passwordkey varchar
				  ,pvar_usersid uuid

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);lvar_curday_practitionerid Varchar(10);lvar_val_practitionerid int;
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 11:34:27*/
		

			  
                                                                                    if pvar_Peopleid is null then
                                                                                    pvar_Peopleid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
IF pvar_practitionerid IS NULL OR BTRIM(pvar_practitionerid) = '' OR pvar_practitionerid = 'YYMM-999' THEN
                                        select cast(to_char(NOW(),'yy') as Varchar(2)) || RIGHT('00' ||cast(to_char(NOW(),'MM') as Varchar(2)),2)
                                          INTO lvar_curday_practitionerid;
                                        PERFORM pg_advisory_xact_lock(hashtext('People-practitionerid-' || lvar_curday_practitionerid));
                                        select COALESCE(max(RIGHT(People.practitionerid,3)),'0') INTO lvar_val_practitionerid from
                                        People where substring(People.practitionerid,1,4) = lvar_curday_practitionerid and (People.practitionerid) NOT LIKE '%/%';
                                        lvar_val_practitionerid:=lvar_val_practitionerid + 1;
                                        pvar_practitionerid:= (lvar_curday_practitionerid||'-'|| cast(to_char(lvar_val_practitionerid,'fm000') as Varchar(3)));
END IF;

			  IF "Check_Authorization"(pvar_createduser, 'People', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from People where upper(People.practitionerid::varchar) = upper(pvar_practitionerid::varchar) and People.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Practitioner ID Already Exists.';

																END IF;
IF EXISTS (SELECT * from People where upper(People.emailid::varchar) = upper(pvar_emailid::varchar) and People.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Email ID Already Exists.';

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
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO People(
				 practitionerid
,firstname
,lastname
,workprofile
,competencylevel
,designation
,contactnumber
,whatsappnumber
,emailid
,gender
,dob
,age
,employmentstatus
,joiningdate
,contractrenewaldate
,photo
,nationality
,specifycountry
,idtype
,idnumber
,iddocument
,paddressline1
,paddressline2
,pzip
,ptown
,pcityordistrict
,pstatename
,sameaspermanentaddress
,caddressline1
,caddressline2
,czip
,ctown
,ccityordistrict
,cstatename
,registrationnumber
,validtill
,licenceupload
,issuingauthority
,bio
,screeningmeetinglink

				 ,createduser
				 ,Peopleid
				 ,tenantid
				 ,status
                
			  )
			  VALUES (
 				 pvar_practitionerid
,pvar_firstname
,pvar_lastname
,pvar_workprofile
,pvar_competencylevel
,pvar_designation
,pvar_contactnumber
,pvar_whatsappnumber
,pvar_emailid
,pvar_gender
,pvar_dob
,pvar_age
,pvar_employmentstatus
,pvar_joiningdate
,pvar_contractrenewaldate
,pvar_photo
,pvar_nationality
,pvar_specifycountry
,pvar_idtype
,pvar_idnumber
,pvar_iddocument
,pvar_paddressline1
,pvar_paddressline2
,pvar_pzip
,pvar_ptown
,pvar_pcityordistrict
,pvar_pstatename
,pvar_sameaspermanentaddress
,pvar_caddressline1
,pvar_caddressline2
,pvar_czip
,pvar_ctown
,pvar_ccityordistrict
,pvar_cstatename
,pvar_registrationnumber
,pvar_validtill
,pvar_licenceupload
,pvar_issuingauthority
,pvar_bio
,pvar_screeningmeetinglink

				 ,pvar_createduser
				 ,pvar_Peopleid
				 ,pvar_tenantid
				 ,CASE WHEN pvar_status='Draft' THEN 'Draft' ELSE 'Active' END
                   
			  );
			   
               

			  


			  
								
								
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
									
					INSERT INTO users
					(
						usersid, tenantid, firstname, lastname, profilepicture,
						username, userpassword, passwordkey, emailid, mobilenumber,
						userrole, createduser, createddate, isdeleted
					)
					SELECT
						pvar_usersid, pvar_tenantid, pvar_firstname, pvar_lastname, pvar_photo,
						pvar_emailid, pvar_userpassword, pvar_passwordkey, pvar_emailid, pvar_contactnumber,
						WorkProfile.rolename, pvar_createduser, NOW(), false
					FROM WorkProfile
					WHERE WorkProfile.WorkProfileid = pvar_workprofile
					  AND COALESCE(WorkProfile.isdeleted, false) = false;


					 
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
																,'Add_People'
																,'Authorization Failed Add_People'
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
						,'Add_People'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_People - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

