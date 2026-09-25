
CREATE OR REPLACE FUNCTION public."Add_IPD_Application_Form"(
	pvar_ipdapplicationformid uuid,
	pvar_tenantid uuid,
	pvar_bookingreferencenumber character varying,
	pvar_patientname uuid,
	pvar_firstname character varying,
	pvar_lastname character varying,
	pvar_gender character varying,
	pvar_mobilenumber character varying,
	pvar_whatsappnumber character varying,
	pvar_nationality character varying,
	pvar_countryoforigin uuid,
	pvar_generalcondition character varying,
	pvar_bookingstatus character varying,
	pvar_groupbooking character varying,
	pvar_areyouthegroupleader character varying,
	pvar_numberofmember integer,
	pvar_groupleadersbookingreferencenumber character varying,
	pvar_paddressline1 character varying,
	pvar_paddressline2 character varying,
	pvar_ppincode integer,
	pvar_ptown character varying,
	pvar_pcityordistrict character varying,
	pvar_pstatename character varying,
	pvar_sameaspermanentaddress boolean,
	pvar_caddressline1 character varying,
	pvar_caddressline2 character varying,
	pvar_cpincode integer,
	pvar_ctown character varying,
	pvar_ccityordistrict character varying,
	pvar_cstatename character varying,
	pvar_flexiblewithdates character varying,
	pvar_flexiblewithroomtype character varying,
	pvar_joinwaitinglist character varying,
	pvar_passportnumber character varying,
	pvar_passportissuingcountry uuid,
	pvar_passportexpirydate date,
	pvar_uploadpassportcopy character varying,
	pvar_visatype character varying,
	pvar_visanumber character varying,
	pvar_visaissuedcountry uuid,
	pvar_visaissuedate date,
	pvar_visaexpirydate date,
	pvar_uploadvisacopy character varying,
	pvar_doyourequireahospitalprovidedattendant character varying,
	pvar_preferredduration character varying,
	pvar_admissionreason text,
	pvar_consentform uuid,
	pvar_consentfile character varying,
	pvar_agreefortermsandconditions boolean,
	pvar_signature text,
	pvar_verifiedstatus character varying,
	pvar_preferreddatesofadmission json,
	pvar_medicalinfo json,
	pvar_medicationinfo json,
	pvar_medicalrecords json,
	pvar_attendantinfo json,
	pvar_roompreference json,
	pvar_attendantpreferreddates json,
	pvar_attendantroompreference json,
	pvar_bookingtype character varying,
	pvar_groupcode character varying,
	pvar_createduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
              DECLARE lv_viewactionroles Varchar(128);
			  lvar_isdraft boolean := false;
			  lvar_curday_bookingreferencenumber Varchar(10);lvar_val_bookingreferencenumber int;
			  lvar_phase Varchar(128);lvar_holiday_validation_msg Varchar(4000);
              DECLARE lvar_registrationid varchar(50);
  
			DECLARE v_tenant_shortcode VARCHAR(10);
			 DECLARE existingrecordcount INT;
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:01*/
		
lvar_isdraft := lower(coalesce(pvar_bookingstatus, '')) = 'draft';

-- Drafts may be saved before the medical-condition and consent steps are
-- completed.  The application model sends an empty UUID for an unselected
-- consent form, so persist that value as NULL for drafts only.
IF lvar_isdraft THEN
    pvar_generalcondition := NULLIF(BTRIM(pvar_generalcondition), '');
    pvar_consentform := NULLIF(
        pvar_consentform,
        '00000000-0000-0000-0000-000000000000'::uuid
    );
END IF;
			  
                                                                                    if pvar_IPDApplicationFormid is null then
                                                                                    pvar_IPDApplicationFormid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
/*select cast(to_char(NOW(),'yyyy') as Varchar(4)) || RIGHT('00' ||cast(to_char(NOW(),'MM') as Varchar(2)),2) 
                                        || RIGHT('00' ||cast(to_char(NOW(),'dd') as Varchar(2)),2) INTO lvar_curday_bookingreferencenumber;
                                        select COALESCE(max(RIGHT(IPDApplicationForm.bookingreferencenumber,3)),'0') INTO lvar_val_bookingreferencenumber from
                                        IPDApplicationForm where substring(IPDApplicationForm.bookingreferencenumber,1,8) = lvar_curday_bookingreferencenumber and (IPDApplicationForm.bookingreferencenumber) NOT LIKE '%/%';
                                        lvar_val_bookingreferencenumber:=lvar_val_bookingreferencenumber + 1;*/
                                        /*pvar_bookingreferencenumber:= (lvar_curday_bookingreferencenumber||'-'|| cast(to_char(lvar_val_bookingreferencenumber,'fm000') as Varchar(3)));*/
SELECT shortcode INTO v_tenant_shortcode
FROM tenant
WHERE tenantid = pvar_tenantid
  AND isdeleted = false;

SELECT registrationid INTO lvar_registrationid
FROM patientprofile 
WHERE patientprofileid = pvar_patientname;

SELECT COUNT(*) INTO existingrecordcount 
FROM ipdapplicationform 
WHERE patientname = pvar_patientname;--  AND isdeleted = false;

 IF existingrecordcount  =0 THEN
 
existingrecordcount=1;

ELSE
    existingrecordcount := existingrecordcount + 1;
END IF;

pvar_bookingreferencenumber := 
   -- v_tenant_shortcode || '-' || 
    lvar_registrationid || '/I-' || 
    LPAD(existingrecordcount::TEXT, 4, '0');
			  IF "Check_Authorization"(pvar_createduser, 'IPDApplicationForm', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from IPDApplicationForm where upper(IPDApplicationForm.bookingreferencenumber::varchar) = upper(pvar_bookingreferencenumber::varchar) and IPDApplicationForm.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Booking Reference Number Already Exists.';

																END IF;

              IF(pvar_areyouthegroupleader is not null AND pvar_areyouthegroupleader!='0' AND LENGTH(pvar_areyouthegroupleader)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_areyouthegroupleader, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='areyouthegroupleader'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_areyouthegroupleader, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'areyouthegroupleader value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_bookingstatus is not null AND pvar_bookingstatus!='0' AND LENGTH(pvar_bookingstatus)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_bookingstatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='bookingstatus'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_bookingstatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'bookingstatus value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_doyourequireahospitalprovidedattendant is not null AND pvar_doyourequireahospitalprovidedattendant!='0' AND LENGTH(pvar_doyourequireahospitalprovidedattendant)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_doyourequireahospitalprovidedattendant, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='doyourequireahospitalprovidedattendant'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_doyourequireahospitalprovidedattendant, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'doyourequireahospitalprovidedattendant value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_flexiblewithdates is not null AND pvar_flexiblewithdates!='0' AND LENGTH(pvar_flexiblewithdates)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_flexiblewithdates, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='flexiblewithdates'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_flexiblewithdates, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'flexiblewithdates value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_flexiblewithroomtype is not null AND pvar_flexiblewithroomtype!='0' AND LENGTH(pvar_flexiblewithroomtype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_flexiblewithroomtype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='flexiblewithroomtype'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_flexiblewithroomtype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'flexiblewithroomtype value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_gender is not null AND pvar_gender!='0' AND LENGTH(pvar_gender)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_gender, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='gender'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_gender, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'gender value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_generalcondition is not null AND pvar_generalcondition!='0' AND LENGTH(pvar_generalcondition)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_generalcondition, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='generalcondition'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_generalcondition, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'generalcondition value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_groupbooking is not null AND pvar_groupbooking!='0' AND LENGTH(pvar_groupbooking)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_groupbooking, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='groupbooking'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_groupbooking, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'groupbooking value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_joinwaitinglist is not null AND pvar_joinwaitinglist!='0' AND LENGTH(pvar_joinwaitinglist)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_joinwaitinglist, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='joinwaitinglist'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_joinwaitinglist, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'joinwaitinglist value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_nationality is not null AND pvar_nationality!='0' AND LENGTH(pvar_nationality)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_nationality, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='nationality'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_nationality, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'nationality value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_preferredduration is not null AND pvar_preferredduration!='0' AND LENGTH(pvar_preferredduration)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_preferredduration, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='preferredduration'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_preferredduration, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'preferredduration value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_verifiedstatus is not null AND pvar_verifiedstatus!='0' AND LENGTH(pvar_verifiedstatus)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_verifiedstatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='verifiedstatus'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_verifiedstatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'verifiedstatus value is invalid';

                                                                END IF;
                                                            END IF;
IF(pvar_visatype is not null AND pvar_visatype!='0' AND LENGTH(pvar_visatype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_visatype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='visatype'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_visatype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'visatype value is invalid';

                                                                END IF;
                                                            END IF;
                                                            -- Holiday Calendar validation for IPD Preferred Dates of Admission
IF pvar_preferreddatesofadmission IS NOT NULL THEN
    lvar_holiday_validation_msg := NULL;
    SELECT STRING_AGG(
        to_char(g.chk_date, 'DD/MM/YYYY')
            || ' (Date of Arrival) is a Holiday (' || g.holidayname || '). '
            || g.taskname
            || CASE WHEN upper(g.task_isallowed) = 'NO'
                    THEN ' is not allowed on this day.'
                    ELSE ' has no available slots on this day (Count: 0).' END,
        ' | '
    )
    INTO lvar_holiday_validation_msg
    FROM (
        SELECT
            c.chk_date, c.holidayname, c.taskname, c.task_isallowed
        FROM (
            -- Arrival dates that hit a blocked holiday task
            SELECT
                CAST(j->>'dateofarrival' AS date)  AS chk_date,
                hc.holidayname, t.taskname,
                hcta.isallowed                     AS task_isallowed
            FROM json_array_elements(pvar_preferreddatesofadmission) j
            INNER JOIN HolidayCalendar hc
                ON hc.holidaydate = CAST(j->>'dateofarrival' AS date)
                AND hc.tenantid   = pvar_tenantid
                AND hc.isdeleted  = false
            INNER JOIN HolidayCalendar_taskallowed hcta
                ON hcta.HolidayCalendarid = hc.HolidayCalendarid
            INNER JOIN Task t ON t.Taskid = hcta.taskname
            WHERE upper(hcta.isallowed) = 'NO' OR hcta.count = 0
        ) c
        GROUP BY c.chk_date, c.holidayname, c.taskname, c.task_isallowed
    ) g;

    IF lvar_holiday_validation_msg IS NOT NULL THEN
        pvar_returnMessage := pvar_returnMessage || lvar_holiday_validation_msg;
    END IF;
END IF;
  
			  if(pvar_returnMessage='')
			  THEN

			IF pvar_bookingstatus = 'Draft' THEN
    IF pvar_verifiedstatus != 'Direct Admission' THEN
        pvar_verifiedstatus := 'Draft';
    END IF;
END IF;
if(pvar_verifiedstatus !='Direct Admission' or pvar_verifiedstatus='' or pvar_verifiedstatus is null)
			THEN
			   pvar_verifiedstatus:='Ready For Frontdesk Review';
			   END IF;

              lvar_phase := CASE
                  WHEN lower(trim(COALESCE(pvar_bookingstatus, ''))) IN (
                      'pending',
                      'draft',
                      'provisional booking',
                      'provisional confirmed',
                      'waitlisted',
                      'rejected',
                      'rework requested',
                      'doctor review requested',
                      'ipd approved by doctor',
                      'request for doctor review',
                      'ready for doctor review',
                      'ready for frontdesk review',
                      'cancellation requested',
                      'cancelled by patient',
                      'cancelled'
                  ) THEN 'Intake'
                  WHEN lower(trim(COALESCE(pvar_bookingstatus, ''))) IN (
                      'assessment form - in draft',
                      'assessment form - review pending',
                      'assessment form reviewed',
                      'screening scheduled',
                      'admission approved',
                      'approved for admission',
                      'screening completed by the patient - in draft',
                      'screening completed by the patient',
                      'screening completed by the patient - intern doctor review pending',
                      'intern doctor reviewed'
                  ) THEN 'Assessment'
                  WHEN lower(trim(COALESCE(pvar_bookingstatus, ''))) IN (
                      'arrival confirmed',
                      'confirmed for arrival',
                      'patient arrived',
                      'consultation scheduled',
                      'admission confirmed',
                      'admitted'
                  ) THEN 'Admission'
                  WHEN lower(trim(COALESCE(pvar_bookingstatus, ''))) IN (
                      'discharged',
                      'extended stay'
                  ) THEN 'Stay'
                  ELSE 'Intake'
              END;

			  INSERT INTO IPDApplicationForm(
				 bookingreferencenumber
,patientname
,firstname
,lastname
,gender
,mobilenumber
,whatsappnumber
,nationality
,countryoforigin
,generalcondition
,bookingstatus
,phase
,bookingstatusdate
,groupbooking
,areyouthegroupleader
,numberofmember
,groupleadersbookingreferencenumber
,paddressline1
,paddressline2
,ppincode
,ptown
,pcityordistrict
,pstatename
,sameaspermanentaddress
,caddressline1
,caddressline2
,cpincode
,ctown
,ccityordistrict
,cstatename
,flexiblewithdates
,flexiblewithroomtype
,joinwaitinglist
,passportnumber
,passportissuingcountry
,passportexpirydate
,uploadpassportcopy
,visatype
,visanumber
,visaissuedcountry
,visaissuedate
,visaexpirydate
,uploadvisacopy
,doyourequireahospitalprovidedattendant
,preferredduration
,admissionreason
,consentform
,consentfile
,agreefortermsandconditions
,signature
,verifiedstatus

				 ,createduser
				 ,IPDApplicationFormid
				 ,tenantid
				 ,bookingtype
				 ,groupcode
                
			  )
			  VALUES (
 				 pvar_bookingreferencenumber
,pvar_patientname
,pvar_firstname
,pvar_lastname
,pvar_gender
,pvar_mobilenumber
,pvar_whatsappnumber
,pvar_nationality
,pvar_countryoforigin
,pvar_generalcondition
,pvar_bookingstatus
,lvar_phase
,NOW()
,pvar_groupbooking
,pvar_areyouthegroupleader
,pvar_numberofmember
,pvar_groupleadersbookingreferencenumber
,pvar_paddressline1
,pvar_paddressline2
,pvar_ppincode
,pvar_ptown
,pvar_pcityordistrict
,pvar_pstatename
,pvar_sameaspermanentaddress
,pvar_caddressline1
,pvar_caddressline2
,pvar_cpincode
,pvar_ctown
,pvar_ccityordistrict
,pvar_cstatename
,pvar_flexiblewithdates
,pvar_flexiblewithroomtype
,pvar_joinwaitinglist
,pvar_passportnumber
,pvar_passportissuingcountry
,pvar_passportexpirydate
,pvar_uploadpassportcopy
,pvar_visatype
,pvar_visanumber
,pvar_visaissuedcountry
,pvar_visaissuedate
,pvar_visaexpirydate
,pvar_uploadvisacopy
,pvar_doyourequireahospitalprovidedattendant
,pvar_preferredduration
,pvar_admissionreason
,pvar_consentform
,pvar_consentfile
,pvar_agreefortermsandconditions
,pvar_signature
,pvar_verifiedstatus

				 ,pvar_createduser
				 ,pvar_IPDApplicationFormid
				 ,pvar_tenantid
				 ,pvar_bookingtype
				 ,pvar_groupcode
                   
			  );

              INSERT INTO reviewlogsIPDApplicationForm
              (
                  ipdapplicationformid,
                  verifiedstatus,
                  reviewcomments,
                  bookingstatus,
                  phase,
                  previousbookingstatus,
                  previousphase,
                  logtype,
                  createduser
              )
              VALUES
              (
                  pvar_IPDApplicationFormid,
                  pvar_bookingstatus,
                  'Initial booking status',
                  pvar_bookingstatus,
                  lvar_phase,
                  NULL,
                  NULL,
                  'Booking Status',
                  pvar_createduser
              );
			   
               

			  

			  
								
								
								INSERT INTO IPDApplicationForm_preferreddatesofadmission (
									IPDApplicationFormid
									,IPDApplicationForm_preferreddatesofadmissionid 
                                    ,record_order  
                                    ,action_date,action_by,action
    
									,dateofarrival
,dateofdeparture
,daysofstay

									
                                
									)
									SELECT 
									pvar_IPDApplicationFormid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
                                    ,NOW(),pvar_createduser,'Added' 
									,CAST(j->>'dateofarrival' AS date) as dateofarrival
,CAST(j->>'dateofdeparture' AS date) as dateofdeparture
,CAST(j->>'daysofstay' AS int) as daysofstay

									
                                    FROM json_array_elements(pvar_preferreddatesofadmission) as j;

								INSERT INTO IPDApplicationForm_attendantpreferreddates (
									IPDApplicationFormid, IPDApplicationForm_attendantpreferreddatesid, record_order,
									createddate, createduser, isdeleted, dateofarrivalatt, dateofdepartureatt, daysofstayatt, cma_client_row_id)
								SELECT pvar_IPDApplicationFormid, gen_random_uuid(), CAST(coalesce(j->>'record_order','0') AS integer),
									NOW(), pvar_createduser, false, CAST(j->>'dateofarrivalatt' AS date), CAST(j->>'dateofdepartureatt' AS date),
									NULLIF(j->>'daysofstayatt','')::integer, j->>'cma_client_row_id'
								FROM json_array_elements(pvar_attendantpreferreddates) j;
									

								
								
								INSERT INTO IPDApplicationForm_medicalinfo (
									IPDApplicationFormid
									,IPDApplicationForm_medicalinfoid 
                                    ,record_order  
                                    ,action_date,action_by,action
    
									,medicalconditionname
,duration
,unit
,severitylevel

									
                                
									)
									SELECT 
									pvar_IPDApplicationFormid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
                                    ,NOW(),pvar_createduser,'Added' 
									,CAST(j->>'medicalconditionname' AS  uuid) as medicalconditionname
,CAST(j->>'duration' AS decimal(18,2)) as duration
,j->>'unit' as unit
,j->>'severitylevel' as severitylevel

									
                                    FROM json_array_elements(pvar_medicalinfo) as j;
									

								
								
								INSERT INTO IPDApplicationForm_medicationinfo (
									IPDApplicationFormid
									,IPDApplicationForm_medicationinfoid 
                                    ,record_order  
                                    ,action_date,action_by,action
    
									,medicinename
,frequencyinaday
,medicationduration
,quantity

									
                                
									)
									SELECT 
									pvar_IPDApplicationFormid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
                                    ,NOW(),pvar_createduser,'Added' 
									,j->>'medicinename' as medicinename
,j->>'frequencyinaday' as frequencyinaday
,j->>'medicationduration' as medicationduration
,NULLIF(j->>'quantity','')::numeric(10,2) as quantity

									
                                    FROM json_array_elements(pvar_medicationinfo) as j;
									

								
								
								INSERT INTO IPDApplicationForm_medicalrecords (
									IPDApplicationFormid
									,IPDApplicationForm_medicalrecordsid 
                                    ,record_order  
                                    ,action_date,action_by,action
    
									,medicalrecordname
,medicalrecordfile

									
                                
									)
									SELECT 
									pvar_IPDApplicationFormid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
                                    ,NOW(),pvar_createduser,'Added' 
									,j->>'medicalrecordname' as medicalrecordname
,j->>'medicalrecordfile' as medicalrecordfile

									
                                    FROM json_array_elements(pvar_medicalrecords) as j;
									

								
								
								INSERT INTO IPDApplicationForm_attendantinfo (
									IPDApplicationFormid
									,IPDApplicationForm_attendantinfoid 
                                    ,record_order  
                                    ,action_date,action_by,action
    
									,attendantname
,age
,gender
,phonenumber

									
                                
									)
									SELECT 
									pvar_IPDApplicationFormid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
                                    ,NOW(),pvar_createduser,'Added' 
									,j->>'attendantname' as attendantname
,CAST(j->>'age' AS int) as age
,j->>'gender' as gender
,j->>'phonenumber' as phonenumber

									
                                    FROM json_array_elements(pvar_attendantinfo) as j;
									

								
								
								INSERT INTO IPDApplicationForm_roompreference (
									IPDApplicationFormid
									,IPDApplicationForm_roompreferenceid 
                                    ,record_order  
                                    ,action_date,action_by,action
    
									,roomtype

									
                                
									)
									SELECT 
									pvar_IPDApplicationFormid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
                                    ,NOW(),pvar_createduser,'Added' 
									,CAST(j->>'roomtype' AS  uuid) as roomtype

									
                                    FROM json_array_elements(pvar_roompreference) as j;

								INSERT INTO IPDApplicationForm_attendantroompreference (
									IPDApplicationFormid, IPDApplicationForm_attendantroompreferenceid, record_order,
									createddate, createduser, isdeleted, roomtypeatt, cma_client_row_id)
								SELECT pvar_IPDApplicationFormid, gen_random_uuid(), CAST(coalesce(j->>'record_order','0') AS integer),
									NOW(), pvar_createduser, false, NULLIF(j->>'roomtypeatt','')::uuid, j->>'cma_client_row_id'
								FROM json_array_elements(pvar_attendantroompreference) j;
									

					 
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
																,'Add_IPD_Application_Form'
																,'Authorization Failed Add_IPD_Application_Form'
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
						,'Add_IPD_Application_Form'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_IPD_Application_Form - Insert failed';*/
			  	
			  END

$BODY$;
