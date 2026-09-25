
CREATE OR REPLACE FUNCTION public."Update_IPD_Application_Form"(
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
	pvar_room json,
	pvar_bookingtype character varying,
	pvar_groupcode character varying,
	pvar_modifieduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);lvar_holiday_validation_msg Varchar(4000);
			  lvar_isdraft boolean := false;
              BEGIN
lvar_isdraft := lower(coalesce(pvar_bookingstatus, '')) = 'draft';

-- Keep incomplete draft fields nullable. Completed admissions continue to be
-- protected by the conditional table constraints and normal validation.
IF lvar_isdraft THEN
    pvar_generalcondition := NULLIF(BTRIM(pvar_generalcondition), '');
    pvar_consentform := NULLIF(
        pvar_consentform,
        '00000000-0000-0000-0000-000000000000'::uuid
    );
END IF;

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:01*/
			  IF "Check_Authorization"(pvar_modifieduser, 'IPDApplicationForm', 'edit') THEN

			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from IPDApplicationForm where upper(IPDApplicationForm.bookingreferencenumber) = upper(pvar_bookingreferencenumber) and IPDApplicationForm.tenantid=pvar_tenantid  and IPDApplicationForm.IPDApplicationFormid <> pvar_IPDApplicationFormid)
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
			  -- If Arrival = Departure holiday → single message; if different → show each with label
			  IF pvar_preferreddatesofadmission IS NOT NULL THEN
			      lvar_holiday_validation_msg := NULL;
			      SELECT STRING_AGG(
			          CASE
			              WHEN g.in_arrival AND g.in_departure THEN
			                  to_char(g.chk_date, 'DD/MM/YYYY')
			                      || ' is a Holiday (' || g.holidayname || '). '
			                      || g.taskname
			                      || CASE WHEN upper(g.task_isallowed) = 'NO'
			                              THEN ' is not allowed on this day.'
			                              ELSE ' has no available slots on this day (Count: 0).' END
			              WHEN g.in_arrival THEN
			                  to_char(g.chk_date, 'DD/MM/YYYY')
			                      || ' (Date of Arrival) is a Holiday (' || g.holidayname || '). '
			                      || g.taskname
			                      || CASE WHEN upper(g.task_isallowed) = 'NO'
			                              THEN ' is not allowed on this day.'
			                              ELSE ' has no available slots on this day (Count: 0).' END
			              ELSE
			                  to_char(g.chk_date, 'DD/MM/YYYY')
			                      || ' (Date of Departure) is a Holiday (' || g.holidayname || '). '
			                      || g.taskname
			                      || CASE WHEN upper(g.task_isallowed) = 'NO'
			                              THEN ' is not allowed on this day.'
			                              ELSE ' has no available slots on this day (Count: 0).' END
			          END,
			          ' | '
			      )
			      INTO lvar_holiday_validation_msg
			      FROM (
			          SELECT
			              c.chk_date, c.holidayname, c.taskname, c.task_isallowed,
			              BOOL_OR(c.in_arrival)    AS in_arrival,
			              BOOL_OR(c.in_departure)  AS in_departure
			          FROM (
			              -- Arrival dates that hit a blocked holiday task
			              SELECT
			                  CAST(j->>'dateofarrival' AS date)  AS chk_date,
			                  hc.holidayname, t.taskname,
			                  hcta.isallowed                     AS task_isallowed,
			                  TRUE  AS in_arrival,
			                  FALSE AS in_departure
			              FROM json_array_elements(pvar_preferreddatesofadmission) j
			              INNER JOIN HolidayCalendar hc
			                  ON hc.holidaydate = CAST(j->>'dateofarrival' AS date)
			                  AND hc.tenantid   = pvar_tenantid
			                  AND hc.isdeleted  = false
			              INNER JOIN HolidayCalendar_taskallowed hcta
			                  ON hcta.HolidayCalendarid = hc.HolidayCalendarid
			              INNER JOIN Task t ON t.Taskid = hcta.taskname
			              WHERE upper(hcta.isallowed) = 'NO' OR hcta.count = 0

			              UNION ALL

			              -- Departure dates that hit a blocked holiday task
			              SELECT
			                  CAST(j->>'dateofdeparture' AS date) AS chk_date,
			                  hc.holidayname, t.taskname,
			                  hcta.isallowed                      AS task_isallowed,
			                  FALSE AS in_arrival,
			                  TRUE  AS in_departure
			              FROM json_array_elements(pvar_preferreddatesofadmission) j
			              INNER JOIN HolidayCalendar hc
			                  ON hc.holidaydate = CAST(j->>'dateofdeparture' AS date)
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
 
			  IF(pvar_returnMessage='')
			  THEN
                IF EXISTS (
                SELECT 1
                FROM IPDApplicationForm
                WHERE IPDApplicationFormid = pvar_IPDApplicationFormid
                  AND lower(coalesce(verifiedstatus,'')) IN ('approved','rejected')
            ) THEN
                pvar_returnMessage := 'Cannot update: Already Approved/Rejected';
                RETURN;
            END IF;
                    INSERT INTO history
VALUES('IPDApplicationForm', NOW(),
(SELECT query_to_xml('SELECT * FROM IPDApplicationForm WHERE IPDApplicationForm.IPDApplicationFormid= '''||pvar_IPDApplicationFormid||'''', true, false, '')));

IF pvar_bookingstatus = 'Draft' THEN
    IF pvar_verifiedstatus != 'Direct Admission' THEN
        pvar_verifiedstatus := 'Draft';
    END IF;
ELSE
    -- Direct IPD is completed by the Front Desk and does not enter review.
    IF lower(trim(COALESCE(pvar_verifiedstatus, ''))) != 'direct admission' THEN
        pvar_verifiedstatus := 'Ready For Review';
    END IF;
END IF;
                    UPDATE IPDApplicationForm SET
                    bookingreferencenumber=pvar_bookingreferencenumber
,patientname=pvar_patientname
,firstname=pvar_firstname
,lastname=pvar_lastname
,gender=pvar_gender
,mobilenumber=pvar_mobilenumber
,whatsappnumber=pvar_whatsappnumber
,nationality=pvar_nationality
,countryoforigin=pvar_countryoforigin
,generalcondition=pvar_generalcondition
,bookingstatus=pvar_bookingstatus
,groupbooking=pvar_groupbooking
,areyouthegroupleader=pvar_areyouthegroupleader
,numberofmember=pvar_numberofmember
,groupleadersbookingreferencenumber=pvar_groupleadersbookingreferencenumber
,paddressline1=pvar_paddressline1
,paddressline2=pvar_paddressline2
,ppincode=pvar_ppincode
,ptown=pvar_ptown
,pcityordistrict=pvar_pcityordistrict
,pstatename=pvar_pstatename
,sameaspermanentaddress=pvar_sameaspermanentaddress
,caddressline1=pvar_caddressline1
,caddressline2=pvar_caddressline2
,cpincode=pvar_cpincode
,ctown=pvar_ctown
,ccityordistrict=pvar_ccityordistrict
,cstatename=pvar_cstatename
,flexiblewithdates=pvar_flexiblewithdates
,flexiblewithroomtype=pvar_flexiblewithroomtype
,joinwaitinglist=pvar_joinwaitinglist
,passportnumber=pvar_passportnumber
,passportissuingcountry=pvar_passportissuingcountry
,passportexpirydate=pvar_passportexpirydate
,uploadpassportcopy=pvar_uploadpassportcopy
,visatype=pvar_visatype
,visanumber=pvar_visanumber
,visaissuedcountry=pvar_visaissuedcountry
,visaissuedate=pvar_visaissuedate
,visaexpirydate=pvar_visaexpirydate
,uploadvisacopy=pvar_uploadvisacopy
,doyourequireahospitalprovidedattendant=pvar_doyourequireahospitalprovidedattendant
,preferredduration=pvar_preferredduration
,admissionreason=pvar_admissionreason
,consentform=pvar_consentform
,consentfile=pvar_consentfile
,agreefortermsandconditions=pvar_agreefortermsandconditions
,signature=pvar_signature
,verifiedstatus=pvar_verifiedstatus
 ,bookingtype = pvar_bookingtype
				 ,groupcode = pvar_groupcode
                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE IPDApplicationFormid=pvar_IPDApplicationFormid;

                    

                    
							-- Persist room selections supplied by Direct IPD Admission.
							-- Do not delete existing rooms when pvar_room is null because ordinary
							-- application edits do not submit the room collection.
							INSERT INTO IPDApplicationForm_room_history (
								IPDApplicationFormid, IPDApplicationForm_roomid, record_order,
								allottedto, roomnumber, fromdate, todate,
								action_date, action_by, action)
							SELECT r.IPDApplicationFormid, r.IPDApplicationForm_roomid, r.record_order,
								r.allottedto, r.roomnumber, r.fromdate, r.todate,
								NOW(), pvar_modifieduser, 'Updated'
							FROM json_array_elements(COALESCE(pvar_room, '[]'::json)) AS j
							JOIN IPDApplicationForm_room AS r
							  ON r.IPDApplicationForm_roomid = CAST(COALESCE(j->>'IPDApplicationForm_roomid', '00000000-0000-0000-0000-000000000000') AS uuid)
							 AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
							WHERE (r.allottedto, r.roomnumber, r.fromdate, r.todate) IS DISTINCT FROM
							      (j->>'allottedto', CAST(j->>'roomnumber' AS uuid),
							       CAST(j->>'fromdate' AS timestamp(3)), CAST(j->>'todate' AS timestamp(3)));

							UPDATE IPDApplicationForm_room AS r
							SET record_order = CAST(COALESCE(j->>'record_order', '0') AS int),
								allottedto = j->>'allottedto',
								roomnumber = CAST(j->>'roomnumber' AS uuid),
								fromdate = CAST(j->>'fromdate' AS timestamp(3)),
								todate = CAST(j->>'todate' AS timestamp(3)),
								action = 'updated', action_date = NOW(), action_by = pvar_modifieduser
							FROM json_array_elements(COALESCE(pvar_room, '[]'::json)) AS j
							WHERE r.IPDApplicationForm_roomid = CAST(COALESCE(j->>'IPDApplicationForm_roomid', '00000000-0000-0000-0000-000000000000') AS uuid)
							  AND r.IPDApplicationFormid = pvar_IPDApplicationFormid;

							INSERT INTO IPDApplicationForm_room (
								IPDApplicationFormid, IPDApplicationForm_roomid, record_order,
								allottedto, roomnumber, fromdate, todate,
								action_date, action_by, action)
							SELECT pvar_IPDApplicationFormid, gen_random_uuid(),
								CAST(COALESCE(j->>'record_order', '0') AS int),
								j->>'allottedto', CAST(j->>'roomnumber' AS uuid),
								CAST(j->>'fromdate' AS timestamp(3)), CAST(j->>'todate' AS timestamp(3)),
								NOW(), pvar_modifieduser, 'Added'
							FROM json_array_elements(COALESCE(pvar_room, '[]'::json)) AS j
							WHERE CAST(COALESCE(j->>'IPDApplicationForm_roomid', '00000000-0000-0000-0000-000000000000') AS uuid)
							      = '00000000-0000-0000-0000-000000000000'::uuid;

							-- Inserts deleted data into a history.
                            INSERT INTO IPDApplicationForm_preferreddatesofadmission_history
                            (
                            IPDApplicationFormid
                            ,IPDApplicationForm_preferreddatesofadmissionid 
                            ,record_order  
                            ,dateofarrival
,dateofdeparture
,daysofstay

                            ,action_date
                            ,action_by
                            ,action	
                            ) 
                            SELECT 
                            IPDApplicationFormid
                            ,IPDApplicationForm_preferreddatesofadmissionid 
                            ,record_order  
                            ,dateofarrival
,dateofdeparture
,daysofstay

                            ,NOW()
                            ,pvar_modifieduser
                            ,'Deleted'
                            FROM IPDApplicationForm_preferreddatesofadmission 
                            WHERE IPDApplicationForm_preferreddatesofadmissionid IN (
                            Select IPDApplicationForm_preferreddatesofadmissionid from IPDApplicationForm_preferreddatesofadmission
                            where IPDApplicationForm_preferreddatesofadmissionid not in (
                            SELECT 
                            CAST(coalesce(j->>'IPDApplicationForm_preferreddatesofadmissionid','00000000-0000-0000-0000-000000000000') as uuid) 
                            FROM json_array_elements(pvar_preferreddatesofadmission) as j
                            ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);

                                -- Mark IPDApplicationForm_preferreddatesofadmission data as deleted.
                                UPDATE IPDApplicationForm_preferreddatesofadmission 
                                set isdeleted=true
                                WHERE IPDApplicationForm_preferreddatesofadmissionid IN(
                                Select IPDApplicationForm_preferreddatesofadmissionid from IPDApplicationForm_preferreddatesofadmission
                                where IPDApplicationForm_preferreddatesofadmissionid not in (
                                SELECT 
                                CAST(coalesce(j->>'IPDApplicationForm_preferreddatesofadmissionid','00000000-0000-0000-0000-000000000000') as uuid)
                                FROM json_array_elements(pvar_preferreddatesofadmission) as j
                                ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);
	                                

                                -- Insert newly added items to IPDApplicationForm_preferreddatesofadmission
                                INSERT INTO IPDApplicationForm_preferreddatesofadmission(
                                IPDApplicationFormid
                                ,IPDApplicationForm_preferreddatesofadmissionid 
                                ,record_order  
                                ,dateofarrival
,dateofdeparture
,daysofstay

                                ,action_date
                                ,action_by
                                ,action	
                                ) 
                                SELECT 
                                pvar_IPDApplicationFormid
                                ,gen_random_uuid()
                                ,CAST(coalesce(j->>'record_order','0') as INT)
                                 ,CAST(j->>'dateofarrival' AS date) as dateofarrival
,CAST(j->>'dateofdeparture' AS date) as dateofdeparture
,CAST(j->>'daysofstay' AS int) as daysofstay

                                 ,NOW()
                                 ,pvar_modifieduser
                                ,'Added'
                                FROM json_array_elements(pvar_preferreddatesofadmission) as j
                                WHERE CAST(coalesce(j->>'IPDApplicationForm_preferreddatesofadmissionid','00000000-0000-0000-0000-000000000000') as uuid)='00000000-0000-0000-0000-000000000000';
                                        
                                -- INSERT UPDATED DATA INTO HISTORY
                                INSERT INTO IPDApplicationForm_preferreddatesofadmission_history(
                                IPDApplicationFormid
                                ,IPDApplicationForm_preferreddatesofadmissionid 
                                ,record_order  
                                ,dateofarrival
,dateofdeparture
,daysofstay

                                ,action_date
                                ,action_by
                                ,action	)
                                SELECT 
                                r.IPDApplicationFormid
                                ,r.IPDApplicationForm_preferreddatesofadmissionid
                                ,r.record_order 
                                ,r.dateofarrival
,r.dateofdeparture
,r.daysofstay

                                ,r.action_date
                                ,r.action_by
                                ,r.action
                                FROM json_array_elements(pvar_preferreddatesofadmission) as j
                                JOIN IPDApplicationForm_preferreddatesofadmission AS r 
                                ON r.IPDApplicationForm_preferreddatesofadmissionid = CAST(j->>'IPDApplicationForm_preferreddatesofadmissionid' AS uuid) 
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                WHERE 
                                r.IPDApplicationForm_preferreddatesofadmissionid = CAST(j->>'IPDApplicationForm_preferreddatesofadmissionid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.dateofarrival
,r.dateofdeparture
,r.daysofstay
) 
                                IS 
                                DISTINCT FROM (
                                 CAST(j->>'dateofarrival' AS date)
,CAST(j->>'dateofdeparture' AS date)
,CAST(j->>'daysofstay' AS int)

                                );

                                -- UPDATE DATA IN TRANSACTION
                                UPDATE IPDApplicationForm_preferreddatesofadmission AS r
                                SET 
                                record_order=CAST(coalesce(j->>'record_order','0') as INT)
                                ,dateofarrival=CAST(j->>'dateofarrival' AS date)
,dateofdeparture=CAST(j->>'dateofdeparture' AS date)
,daysofstay=CAST(j->>'daysofstay' AS int)

								
                                ,action='updated'
                                ,action_date=NOW()
                                ,action_by=pvar_modifieduser
                                FROM json_array_elements(pvar_preferreddatesofadmission) as j
                                WHERE 
                                r.IPDApplicationForm_preferreddatesofadmissionid = CAST(j->>'IPDApplicationForm_preferreddatesofadmissionid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.dateofarrival
,r.dateofdeparture
,r.daysofstay
) 
                                IS 
                                DISTINCT FROM (
                                CAST(j->>'dateofarrival' AS date)
,CAST(j->>'dateofdeparture' AS date)
,CAST(j->>'daysofstay' AS int)

                                );

							-- Inserts deleted data into a history.
                            INSERT INTO IPDApplicationForm_medicalinfo_history
                            (
                            IPDApplicationFormid
                            ,IPDApplicationForm_medicalinfoid 
                            ,record_order  
                            ,medicalconditionname
,duration
,unit
,severitylevel

                            ,action_date
                            ,action_by
                            ,action	
                            ) 
                            SELECT 
                            IPDApplicationFormid
                            ,IPDApplicationForm_medicalinfoid 
                            ,record_order  
                            ,medicalconditionname
,duration
,unit
,severitylevel

                            ,NOW()
                            ,pvar_modifieduser
                            ,'Deleted'
                            FROM IPDApplicationForm_medicalinfo 
                            WHERE IPDApplicationForm_medicalinfoid IN (
                            Select IPDApplicationForm_medicalinfoid from IPDApplicationForm_medicalinfo
                            where IPDApplicationForm_medicalinfoid not in (
                            SELECT 
                            CAST(coalesce(j->>'IPDApplicationForm_medicalinfoid','00000000-0000-0000-0000-000000000000') as uuid) 
                            FROM json_array_elements(pvar_medicalinfo) as j
                            ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);

                                -- Mark IPDApplicationForm_medicalinfo data as deleted.
                                UPDATE IPDApplicationForm_medicalinfo 
                                set isdeleted=true
                                WHERE IPDApplicationForm_medicalinfoid IN(
                                Select IPDApplicationForm_medicalinfoid from IPDApplicationForm_medicalinfo
                                where IPDApplicationForm_medicalinfoid not in (
                                SELECT 
                                CAST(coalesce(j->>'IPDApplicationForm_medicalinfoid','00000000-0000-0000-0000-000000000000') as uuid)
                                FROM json_array_elements(pvar_medicalinfo) as j
                                ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);
	                                

                                -- Insert newly added items to IPDApplicationForm_medicalinfo
                                INSERT INTO IPDApplicationForm_medicalinfo(
                                IPDApplicationFormid
                                ,IPDApplicationForm_medicalinfoid 
                                ,record_order  
                                ,medicalconditionname
,duration
,unit
,severitylevel

                                ,action_date
                                ,action_by
                                ,action	
                                ) 
                                SELECT 
                                pvar_IPDApplicationFormid
                                ,gen_random_uuid()
                                ,CAST(coalesce(j->>'record_order','0') as INT)
                                 ,CAST(j->>'medicalconditionname' AS  uuid) as medicalconditionname
,CAST(j->>'duration' AS decimal(18,2)) as duration
,j->>'unit' as unit
,j->>'severitylevel' as severitylevel

                                 ,NOW()
                                 ,pvar_modifieduser
                                ,'Added'
                                FROM json_array_elements(pvar_medicalinfo) as j
                                WHERE CAST(coalesce(j->>'IPDApplicationForm_medicalinfoid','00000000-0000-0000-0000-000000000000') as uuid)='00000000-0000-0000-0000-000000000000';
                                        
                                -- INSERT UPDATED DATA INTO HISTORY
                                INSERT INTO IPDApplicationForm_medicalinfo_history(
                                IPDApplicationFormid
                                ,IPDApplicationForm_medicalinfoid 
                                ,record_order  
                                ,medicalconditionname
,duration
,unit
,severitylevel

                                ,action_date
                                ,action_by
                                ,action	)
                                SELECT 
                                r.IPDApplicationFormid
                                ,r.IPDApplicationForm_medicalinfoid
                                ,r.record_order 
                                ,r.medicalconditionname
,r.duration
,r.unit
,r.severitylevel

                                ,r.action_date
                                ,r.action_by
                                ,r.action
                                FROM json_array_elements(pvar_medicalinfo) as j
                                JOIN IPDApplicationForm_medicalinfo AS r 
                                ON r.IPDApplicationForm_medicalinfoid = CAST(j->>'IPDApplicationForm_medicalinfoid' AS uuid) 
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                WHERE 
                                r.IPDApplicationForm_medicalinfoid = CAST(j->>'IPDApplicationForm_medicalinfoid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.medicalconditionname
,r.duration
,r.unit
,r.severitylevel
) 
                                IS 
                                DISTINCT FROM (
                                 CAST(j->>'medicalconditionname' AS  uuid)
,CAST(j->>'duration' AS decimal(18,2))
,j->>'unit'
,j->>'severitylevel'

                                );

                                -- UPDATE DATA IN TRANSACTION
                                UPDATE IPDApplicationForm_medicalinfo AS r
                                SET 
                                record_order=CAST(coalesce(j->>'record_order','0') as INT)
                                ,medicalconditionname=CAST(j->>'medicalconditionname' AS  uuid)
,duration=CAST(j->>'duration' AS decimal(18,2))
,unit=j->>'unit'
,severitylevel=j->>'severitylevel'

								
                                ,action='updated'
                                ,action_date=NOW()
                                ,action_by=pvar_modifieduser
                                FROM json_array_elements(pvar_medicalinfo) as j
                                WHERE 
                                r.IPDApplicationForm_medicalinfoid = CAST(j->>'IPDApplicationForm_medicalinfoid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.medicalconditionname
,r.duration
,r.unit
,r.severitylevel
) 
                                IS 
                                DISTINCT FROM (
                                CAST(j->>'medicalconditionname' AS  uuid)
,CAST(j->>'duration' AS decimal(18,2))
,j->>'unit'
,j->>'severitylevel'

                                );

							-- Inserts deleted data into a history.
                            INSERT INTO IPDApplicationForm_medicationinfo_history
                            (
                            IPDApplicationFormid
                            ,IPDApplicationForm_medicationinfoid 
                            ,record_order  
                            ,medicinename
,frequencyinaday
,medicationduration
,quantity

                            ,action_date
                            ,action_by
                            ,action	
                            ) 
                            SELECT 
                            IPDApplicationFormid
                            ,IPDApplicationForm_medicationinfoid 
                            ,record_order  
                            ,medicinename
,frequencyinaday
,medicationduration
,quantity

                            ,NOW()
                            ,pvar_modifieduser
                            ,'Deleted'
                            FROM IPDApplicationForm_medicationinfo 
                            WHERE IPDApplicationForm_medicationinfoid IN (
                            Select IPDApplicationForm_medicationinfoid from IPDApplicationForm_medicationinfo
                            where IPDApplicationForm_medicationinfoid not in (
                            SELECT 
                            CAST(coalesce(j->>'IPDApplicationForm_medicationinfoid','00000000-0000-0000-0000-000000000000') as uuid) 
                            FROM json_array_elements(pvar_medicationinfo) as j
                            ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);

                                -- Mark IPDApplicationForm_medicationinfo data as deleted.
                                UPDATE IPDApplicationForm_medicationinfo 
                                set isdeleted=true
                                WHERE IPDApplicationForm_medicationinfoid IN(
                                Select IPDApplicationForm_medicationinfoid from IPDApplicationForm_medicationinfo
                                where IPDApplicationForm_medicationinfoid not in (
                                SELECT 
                                CAST(coalesce(j->>'IPDApplicationForm_medicationinfoid','00000000-0000-0000-0000-000000000000') as uuid)
                                FROM json_array_elements(pvar_medicationinfo) as j
                                ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);
	                                

                                -- Insert newly added items to IPDApplicationForm_medicationinfo
                                INSERT INTO IPDApplicationForm_medicationinfo(
                                IPDApplicationFormid
                                ,IPDApplicationForm_medicationinfoid 
                                ,record_order  
                                ,medicinename
,frequencyinaday
,medicationduration
,quantity

                              ,action_date
                                ,action_by
                                ,action	
                                ) 
                                SELECT 
                                pvar_IPDApplicationFormid
                                ,gen_random_uuid()
                                ,CAST(coalesce(j->>'record_order','0') as INT)
                                 ,j->>'medicinename' as medicinename
,j->>'frequencyinaday' as frequencyinaday
,j->>'medicationduration' as medicationduration
,NULLIF(j->>'quantity','')::numeric(10,2) as quantity

                                 ,NOW()
                                 ,pvar_modifieduser
                                ,'Added'
                                FROM json_array_elements(pvar_medicationinfo) as j
                                WHERE CAST(coalesce(j->>'IPDApplicationForm_medicationinfoid','00000000-0000-0000-0000-000000000000') as uuid)='00000000-0000-0000-0000-000000000000';
                                        
                                -- INSERT UPDATED DATA INTO HISTORY
                                INSERT INTO IPDApplicationForm_medicationinfo_history(
                                IPDApplicationFormid
                                ,IPDApplicationForm_medicationinfoid 
                                ,record_order  
                                ,medicinename
,frequencyinaday
,medicationduration
,quantity

                                ,action_date
                                ,action_by
                                ,action	)
                                SELECT 
                                r.IPDApplicationFormid
                                ,r.IPDApplicationForm_medicationinfoid
                                ,r.record_order 
                                ,r.medicinename
,r.frequencyinaday
,r.medicationduration
,r.quantity

                                ,r.action_date
                                ,r.action_by
                                ,r.action
                                FROM json_array_elements(pvar_medicationinfo) as j
                                JOIN IPDApplicationForm_medicationinfo AS r 
                                ON r.IPDApplicationForm_medicationinfoid = CAST(j->>'IPDApplicationForm_medicationinfoid' AS uuid) 
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                WHERE 
                                r.IPDApplicationForm_medicationinfoid = CAST(j->>'IPDApplicationForm_medicationinfoid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.medicinename
,r.frequencyinaday
,r.medicationduration
,r.quantity
) 
                                IS 
                                DISTINCT FROM (
                                 j->>'medicinename'
,j->>'frequencyinaday'
,j->>'medicationduration'
,NULLIF(j->>'quantity','')::numeric(10,2)

                                );

                                -- UPDATE DATA IN TRANSACTION
                                UPDATE IPDApplicationForm_medicationinfo AS r
                                SET 
                                record_order=CAST(coalesce(j->>'record_order','0') as INT)
                                ,medicinename=j->>'medicinename'
,frequencyinaday=j->>'frequencyinaday'
,medicationduration=j->>'medicationduration'
,quantity=NULLIF(j->>'quantity','')::numeric(10,2)

								
                                ,action='updated'
                                ,action_date=NOW()
                                ,action_by=pvar_modifieduser
                                FROM json_array_elements(pvar_medicationinfo) as j
                                WHERE 
                                r.IPDApplicationForm_medicationinfoid = CAST(j->>'IPDApplicationForm_medicationinfoid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.medicinename
,r.frequencyinaday
,r.medicationduration
,r.quantity
) 
                                IS 
                                DISTINCT FROM (
                                j->>'medicinename'
,j->>'frequencyinaday'
,j->>'medicationduration'
,NULLIF(j->>'quantity','')::numeric(10,2)

                                );

							-- Inserts deleted data into a history.
                            INSERT INTO IPDApplicationForm_medicalrecords_history
                            (
                            IPDApplicationFormid
                            ,IPDApplicationForm_medicalrecordsid 
                            ,record_order  
                            ,medicalrecordname
,medicalrecordfile

                            ,action_date
                            ,action_by
                            ,action	
                            ) 
                            SELECT 
                            IPDApplicationFormid
                            ,IPDApplicationForm_medicalrecordsid 
                            ,record_order  
                            ,medicalrecordname
,medicalrecordfile

                            ,NOW()
                            ,pvar_modifieduser
                            ,'Deleted'
                            FROM IPDApplicationForm_medicalrecords 
                            WHERE IPDApplicationForm_medicalrecordsid IN (
                            Select IPDApplicationForm_medicalrecordsid from IPDApplicationForm_medicalrecords
                            where IPDApplicationForm_medicalrecordsid not in (
                            SELECT 
                            CAST(coalesce(j->>'IPDApplicationForm_medicalrecordsid','00000000-0000-0000-0000-000000000000') as uuid) 
                            FROM json_array_elements(pvar_medicalrecords) as j
                            ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);

                                -- Mark IPDApplicationForm_medicalrecords data as deleted.
                                UPDATE IPDApplicationForm_medicalrecords 
                                set isdeleted=true
                                WHERE IPDApplicationForm_medicalrecordsid IN(
                                Select IPDApplicationForm_medicalrecordsid from IPDApplicationForm_medicalrecords
                                where IPDApplicationForm_medicalrecordsid not in (
                                SELECT 
                                CAST(coalesce(j->>'IPDApplicationForm_medicalrecordsid','00000000-0000-0000-0000-000000000000') as uuid)
                                FROM json_array_elements(pvar_medicalrecords) as j
                                ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);
	                                

                                -- Insert newly added items to IPDApplicationForm_medicalrecords
                                INSERT INTO IPDApplicationForm_medicalrecords(
                                IPDApplicationFormid
                                ,IPDApplicationForm_medicalrecordsid 
                                ,record_order  
                                ,medicalrecordname
,medicalrecordfile

                                ,action_date
                                ,action_by
                                ,action	
                                ) 
                                SELECT 
                                pvar_IPDApplicationFormid
                                ,gen_random_uuid()
                                ,CAST(coalesce(j->>'record_order','0') as INT)
                                 ,j->>'medicalrecordname' as medicalrecordname
,j->>'medicalrecordfile' as medicalrecordfile

                                 ,NOW()
                                 ,pvar_modifieduser
                                ,'Added'
                                FROM json_array_elements(pvar_medicalrecords) as j
                                WHERE CAST(coalesce(j->>'IPDApplicationForm_medicalrecordsid','00000000-0000-0000-0000-000000000000') as uuid)='00000000-0000-0000-0000-000000000000';
                                        
                                -- INSERT UPDATED DATA INTO HISTORY
                                INSERT INTO IPDApplicationForm_medicalrecords_history(
                                IPDApplicationFormid
                                ,IPDApplicationForm_medicalrecordsid 
                                ,record_order  
                                ,medicalrecordname
,medicalrecordfile

                                ,action_date
                                ,action_by
                                ,action	)
                                SELECT 
                                r.IPDApplicationFormid
                                ,r.IPDApplicationForm_medicalrecordsid
                                ,r.record_order 
                                ,r.medicalrecordname
,r.medicalrecordfile

                                ,r.action_date
                                ,r.action_by
                                ,r.action
                                FROM json_array_elements(pvar_medicalrecords) as j
                                JOIN IPDApplicationForm_medicalrecords AS r 
                                ON r.IPDApplicationForm_medicalrecordsid = CAST(j->>'IPDApplicationForm_medicalrecordsid' AS uuid) 
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                WHERE 
                                r.IPDApplicationForm_medicalrecordsid = CAST(j->>'IPDApplicationForm_medicalrecordsid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.medicalrecordname
,r.medicalrecordfile
) 
                                IS 
                                DISTINCT FROM (
                                 j->>'medicalrecordname'
,j->>'medicalrecordfile'

                                );

                                -- UPDATE DATA IN TRANSACTION
                                UPDATE IPDApplicationForm_medicalrecords AS r
                                SET 
                                record_order=CAST(coalesce(j->>'record_order','0') as INT)
                                ,medicalrecordname=j->>'medicalrecordname'
,medicalrecordfile=j->>'medicalrecordfile'

								
                                ,action='updated'
                                ,action_date=NOW()
                                ,action_by=pvar_modifieduser
                                FROM json_array_elements(pvar_medicalrecords) as j
                                WHERE 
                                r.IPDApplicationForm_medicalrecordsid = CAST(j->>'IPDApplicationForm_medicalrecordsid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.medicalrecordname
,r.medicalrecordfile
) 
                                IS 
                                DISTINCT FROM (
                                j->>'medicalrecordname'
,j->>'medicalrecordfile'

                                );

							-- Inserts deleted data into a history.
                            INSERT INTO IPDApplicationForm_attendantinfo_history
                            (
                            IPDApplicationFormid
                            ,IPDApplicationForm_attendantinfoid 
                            ,record_order  
                            ,attendantname
,age
,gender
,phonenumber

                            ,action_date
                            ,action_by
                            ,action	
                            ) 
                            SELECT 
                            IPDApplicationFormid
                            ,IPDApplicationForm_attendantinfoid 
                            ,record_order  
                            ,attendantname
,age
,gender
,phonenumber

                            ,NOW()
                            ,pvar_modifieduser
                            ,'Deleted'
                            FROM IPDApplicationForm_attendantinfo 
                            WHERE IPDApplicationForm_attendantinfoid IN (
                            Select IPDApplicationForm_attendantinfoid from IPDApplicationForm_attendantinfo
                            where IPDApplicationForm_attendantinfoid not in (
                            SELECT 
                            CAST(coalesce(j->>'IPDApplicationForm_attendantinfoid','00000000-0000-0000-0000-000000000000') as uuid) 
                            FROM json_array_elements(pvar_attendantinfo) as j
                            ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);

                                -- Mark IPDApplicationForm_attendantinfo data as deleted.
                                UPDATE IPDApplicationForm_attendantinfo 
                                set isdeleted=true
                                WHERE IPDApplicationForm_attendantinfoid IN(
                                Select IPDApplicationForm_attendantinfoid from IPDApplicationForm_attendantinfo
                                where IPDApplicationForm_attendantinfoid not in (
                                SELECT 
                                CAST(coalesce(j->>'IPDApplicationForm_attendantinfoid','00000000-0000-0000-0000-000000000000') as uuid)
                                FROM json_array_elements(pvar_attendantinfo) as j
                                ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);
	                                

                                -- Insert newly added items to IPDApplicationForm_attendantinfo
                                INSERT INTO IPDApplicationForm_attendantinfo(
                                IPDApplicationFormid
                                ,IPDApplicationForm_attendantinfoid 
                                ,record_order  
                                ,attendantname
,age
,gender
,phonenumber

                                ,action_date
                                ,action_by
                                ,action	
                                ) 
                                SELECT 
                                pvar_IPDApplicationFormid
                                ,gen_random_uuid()
                                ,CAST(coalesce(j->>'record_order','0') as INT)
                                 ,j->>'attendantname' as attendantname
,CAST(j->>'age' AS int) as age
,j->>'gender' as gender
,j->>'phonenumber' as phonenumber

                                 ,NOW()
                                 ,pvar_modifieduser
                                ,'Added'
                                FROM json_array_elements(pvar_attendantinfo) as j
                                WHERE CAST(coalesce(j->>'IPDApplicationForm_attendantinfoid','00000000-0000-0000-0000-000000000000') as uuid)='00000000-0000-0000-0000-000000000000';
                                        
                                -- INSERT UPDATED DATA INTO HISTORY
                                INSERT INTO IPDApplicationForm_attendantinfo_history(
                                IPDApplicationFormid
                                ,IPDApplicationForm_attendantinfoid 
                                ,record_order  
                                ,attendantname
,age
,gender
,phonenumber

                                ,action_date
                                ,action_by
                                ,action	)
                                SELECT 
                                r.IPDApplicationFormid
                                ,r.IPDApplicationForm_attendantinfoid
                                ,r.record_order 
                                ,r.attendantname
,r.age
,r.gender
,r.phonenumber

                                ,r.action_date
                                ,r.action_by
                                ,r.action
                                FROM json_array_elements(pvar_attendantinfo) as j
                                JOIN IPDApplicationForm_attendantinfo AS r 
                                ON r.IPDApplicationForm_attendantinfoid = CAST(j->>'IPDApplicationForm_attendantinfoid' AS uuid) 
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                WHERE 
                                r.IPDApplicationForm_attendantinfoid = CAST(j->>'IPDApplicationForm_attendantinfoid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.attendantname
,r.age
,r.gender
,r.phonenumber
) 
                                IS 
                                DISTINCT FROM (
                                 j->>'attendantname'
,CAST(j->>'age' AS int)
,j->>'gender'
,j->>'phonenumber'

                                );

                                -- UPDATE DATA IN TRANSACTION
                                UPDATE IPDApplicationForm_attendantinfo AS r
                                SET 
                                record_order=CAST(coalesce(j->>'record_order','0') as INT)
                                ,attendantname=j->>'attendantname'
,age=CAST(j->>'age' AS int)
,gender=j->>'gender'
,phonenumber=j->>'phonenumber'

								
                                ,action='updated'
                                ,action_date=NOW()
                                ,action_by=pvar_modifieduser
                                FROM json_array_elements(pvar_attendantinfo) as j
                                WHERE 
                                r.IPDApplicationForm_attendantinfoid = CAST(j->>'IPDApplicationForm_attendantinfoid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.attendantname
,r.age
,r.gender
,r.phonenumber
) 
                                IS 
                                DISTINCT FROM (
                                j->>'attendantname'
,CAST(j->>'age' AS int)
,j->>'gender'
,j->>'phonenumber'

                                );

							-- Inserts deleted data into a history.
                            INSERT INTO IPDApplicationForm_roompreference_history
                            (
                            IPDApplicationFormid
                            ,IPDApplicationForm_roompreferenceid 
                            ,record_order  
                            ,roomtype

                            ,action_date
                            ,action_by
                            ,action	
                            ) 
                            SELECT 
                            IPDApplicationFormid
                            ,IPDApplicationForm_roompreferenceid 
                            ,record_order  
                            ,roomtype

                            ,NOW()
                            ,pvar_modifieduser
                            ,'Deleted'
                            FROM IPDApplicationForm_roompreference 
                            WHERE IPDApplicationForm_roompreferenceid IN (
                            Select IPDApplicationForm_roompreferenceid from IPDApplicationForm_roompreference
                            where IPDApplicationForm_roompreferenceid not in (
                            SELECT 
                            CAST(coalesce(j->>'IPDApplicationForm_roompreferenceid','00000000-0000-0000-0000-000000000000') as uuid) 
                            FROM json_array_elements(pvar_roompreference) as j
                            ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);

                                -- Mark IPDApplicationForm_roompreference data as deleted.
                                UPDATE IPDApplicationForm_roompreference 
                                set isdeleted=true
                                WHERE IPDApplicationForm_roompreferenceid IN(
                                Select IPDApplicationForm_roompreferenceid from IPDApplicationForm_roompreference
                                where IPDApplicationForm_roompreferenceid not in (
                                SELECT 
                                CAST(coalesce(j->>'IPDApplicationForm_roompreferenceid','00000000-0000-0000-0000-000000000000') as uuid)
                                FROM json_array_elements(pvar_roompreference) as j
                                ) AND IPDApplicationFormid=pvar_IPDApplicationFormid);
	                                

                                -- Insert newly added items to IPDApplicationForm_roompreference
                                INSERT INTO IPDApplicationForm_roompreference(
                                IPDApplicationFormid
                                ,IPDApplicationForm_roompreferenceid 
                                ,record_order  
                                ,roomtype

                                ,action_date
                                ,action_by
                                ,action	
                                ) 
                                SELECT 
                                pvar_IPDApplicationFormid
                                ,gen_random_uuid()
                                ,CAST(coalesce(j->>'record_order','0') as INT)
                                 ,CAST(j->>'roomtype' AS  uuid) as roomtype

                                 ,NOW()
                                 ,pvar_modifieduser
                                ,'Added'
                                FROM json_array_elements(pvar_roompreference) as j
                                WHERE CAST(coalesce(j->>'IPDApplicationForm_roompreferenceid','00000000-0000-0000-0000-000000000000') as uuid)='00000000-0000-0000-0000-000000000000';
                                        
                                -- INSERT UPDATED DATA INTO HISTORY
                                INSERT INTO IPDApplicationForm_roompreference_history(
                                IPDApplicationFormid
                                ,IPDApplicationForm_roompreferenceid 
                                ,record_order  
                                ,roomtype

                                ,action_date
                                ,action_by
                                ,action	)
                                SELECT 
                                r.IPDApplicationFormid
                                ,r.IPDApplicationForm_roompreferenceid
                                ,r.record_order 
                                ,r.roomtype

                                ,r.action_date
                                ,r.action_by
                                ,r.action
                                FROM json_array_elements(pvar_roompreference) as j
                                JOIN IPDApplicationForm_roompreference AS r 
                                ON r.IPDApplicationForm_roompreferenceid = CAST(j->>'IPDApplicationForm_roompreferenceid' AS uuid) 
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                WHERE 
                                r.IPDApplicationForm_roompreferenceid = CAST(j->>'IPDApplicationForm_roompreferenceid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.record_order, r.roomtype)
                                IS 
                                DISTINCT FROM (
                                 CAST(coalesce(j->>'record_order','0') as INT),
                                 CAST(j->>'roomtype' AS uuid)
                                );

                                -- UPDATE DATA IN TRANSACTION
                                UPDATE IPDApplicationForm_roompreference AS r
                                SET 
                                record_order=CAST(coalesce(j->>'record_order','0') as INT)
                                ,roomtype=CAST(j->>'roomtype' AS  uuid)

								
                                ,action='updated'
                                ,action_date=NOW()
                                ,action_by=pvar_modifieduser
                                FROM json_array_elements(pvar_roompreference) as j
                                WHERE 
                                r.IPDApplicationForm_roompreferenceid = CAST(j->>'IPDApplicationForm_roompreferenceid' AS uuid)
                                AND r.IPDApplicationFormid = pvar_IPDApplicationFormid
                                AND 
                                (r.record_order, r.roomtype)
                                IS 
                                DISTINCT FROM (
                                CAST(coalesce(j->>'record_order','0') as INT),
                                CAST(j->>'roomtype' AS uuid)
                                );

					
							
					IF pvar_attendantpreferreddates IS NOT NULL THEN
						UPDATE IPDApplicationForm_attendantpreferreddates p SET isdeleted=true, modifieduser=pvar_modifieduser, modifieddate=NOW()
						WHERE p.IPDApplicationFormid=pvar_IPDApplicationFormid AND NOT EXISTS (
							SELECT 1 FROM json_array_elements(pvar_attendantpreferreddates) j
							WHERE NULLIF(j->>'IPDApplicationForm_attendantpreferreddatesid','')::uuid=p.IPDApplicationForm_attendantpreferreddatesid);
						INSERT INTO IPDApplicationForm_attendantpreferreddates(IPDApplicationForm_attendantpreferreddatesid,IPDApplicationFormid,dateofarrivalatt,dateofdepartureatt,daysofstayatt,record_order,cma_client_row_id,createduser,createddate,isdeleted)
						SELECT COALESCE(NULLIF(NULLIF(j->>'IPDApplicationForm_attendantpreferreddatesid','')::uuid,'00000000-0000-0000-0000-000000000000'::uuid),gen_random_uuid()),pvar_IPDApplicationFormid,(j->>'dateofarrivalatt')::date,(j->>'dateofdepartureatt')::date,NULLIF(j->>'daysofstayatt','')::integer,(j->>'record_order')::integer,j->>'cma_client_row_id',pvar_modifieduser,NOW(),false
						FROM json_array_elements(pvar_attendantpreferreddates) j
						ON CONFLICT(IPDApplicationForm_attendantpreferreddatesid) DO UPDATE SET dateofarrivalatt=EXCLUDED.dateofarrivalatt,dateofdepartureatt=EXCLUDED.dateofdepartureatt,daysofstayatt=EXCLUDED.daysofstayatt,record_order=EXCLUDED.record_order,cma_client_row_id=EXCLUDED.cma_client_row_id,isdeleted=false,modifieduser=pvar_modifieduser,modifieddate=NOW();
					END IF;

					IF pvar_attendantroompreference IS NOT NULL THEN
						UPDATE IPDApplicationForm_attendantroompreference p SET isdeleted=true, modifieduser=pvar_modifieduser, modifieddate=NOW()
						WHERE p.IPDApplicationFormid=pvar_IPDApplicationFormid AND NOT EXISTS (
							SELECT 1 FROM json_array_elements(pvar_attendantroompreference) j
							WHERE NULLIF(j->>'IPDApplicationForm_attendantroompreferenceid','')::uuid=p.IPDApplicationForm_attendantroompreferenceid);
						INSERT INTO IPDApplicationForm_attendantroompreference(IPDApplicationForm_attendantroompreferenceid,IPDApplicationFormid,roomtypeatt,record_order,cma_client_row_id,createduser,createddate,isdeleted)
						SELECT COALESCE(NULLIF(NULLIF(j->>'IPDApplicationForm_attendantroompreferenceid','')::uuid,'00000000-0000-0000-0000-000000000000'::uuid),gen_random_uuid()),pvar_IPDApplicationFormid,NULLIF(j->>'roomtypeatt','')::uuid,(j->>'record_order')::integer,j->>'cma_client_row_id',pvar_modifieduser,NOW(),false
						FROM json_array_elements(pvar_attendantroompreference) j
						ON CONFLICT(IPDApplicationForm_attendantroompreferenceid) DO UPDATE SET roomtypeatt=EXCLUDED.roomtypeatt,record_order=EXCLUDED.record_order,cma_client_row_id=EXCLUDED.cma_client_row_id,isdeleted=false,modifieduser=pvar_modifieduser,modifieddate=NOW();
					END IF;

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
																,'Update_IPD_Application_Form'
																,'Authorization Failed Update_IPD_Application_Form'
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
						,'Update_IPD_Application_Form'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_IPD_Application_Form - update failed';*/
			  	
			  END
              
$BODY$;
