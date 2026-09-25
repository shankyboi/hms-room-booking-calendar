  CREATE OR REPLACE FUNCTION  "Confirm_Arrival"
			  (
				  pvar_IPDApplicationFormid uuid
,pvar_tenantid uuid
,
pvar_bookingreferencenumber Varchar(256)
,
pvar_verifiedstatus  Varchar(1024)
,
pvar_estimatedarrival Timestamp(3)
,
pvar_travelarrangement  Varchar(1024)
,
pvar_typeoftravelrequired  Varchar(1024)
,
pvar_pickupfrom Varchar(128)
,
pvar_requiredparkingspace  Varchar(1024)
,
pvar_wheelchairassistance  Varchar(1024)
,
pvar_requireddinner  Varchar(1024)
,
pvar_specialrequest Varchar(256)

				  ,pvar_modifieduser  uuid

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000)
              AS $BODY$
              DECLARE lv_viewactionroles Varchar(128);DECLARE lvar_fromdate date;
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/14/2026 05:16:22*/
			  IF "Check_Authorization"(pvar_modifieduser, 'IPDApplicationForm', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from IPDApplicationForm where upper(IPDApplicationForm.bookingreferencenumber) = upper(pvar_bookingreferencenumber) and IPDApplicationForm.tenantid=pvar_tenantid  and IPDApplicationForm.IPDApplicationFormid <> pvar_IPDApplicationFormid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Booking Reference Number Already Exists.';

																END IF;

               IF(pvar_requireddinner is not null AND pvar_requireddinner!='0' AND LENGTH(pvar_requireddinner)>0)
                                                            THEN
                                                                 if(CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_requireddinner, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc
                                                                from lookups  where fieldname='requireddinner'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_requireddinner, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'requireddinner value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_requiredparkingspace is not null AND pvar_requiredparkingspace!='0' AND LENGTH(pvar_requiredparkingspace)>0)
                                                            THEN
                                                                 if(CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_requiredparkingspace, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc
                                                                from lookups  where fieldname='requiredparkingspace'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_requiredparkingspace, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'requiredparkingspace value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_travelarrangement is not null AND pvar_travelarrangement!='0' AND LENGTH(pvar_travelarrangement)>0)
                                                            THEN
                                                                 if(CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_travelarrangement, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc
                                                                from lookups  where fieldname='travelarrangement'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_travelarrangement, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'travelarrangement value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_typeoftravelrequired is not null AND pvar_typeoftravelrequired!='0' AND LENGTH(pvar_typeoftravelrequired)>0)
                                                            THEN
                                                                 if(CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_typeoftravelrequired, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc
                                                                from lookups  where fieldname='typeoftravelrequired'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_typeoftravelrequired, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'typeoftravelrequired value is invalid';


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
IF(pvar_wheelchairassistance is not null AND pvar_wheelchairassistance!='0' AND LENGTH(pvar_wheelchairassistance)>0)
                                                            THEN
                                                                 if(CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_wheelchairassistance, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc
                                                                from lookups  where fieldname='wheelchairassistance'
                                                                and entityname='IPDApplicationForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_wheelchairassistance, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'wheelchairassistance value is invalid';


                                                                END IF;
                                                            END IF;

-- Determine the reference date to validate the estimated arrival against.
--
-- A single IPD booking can have MULTIPLE RoomAllocation rows: one for the patient,
-- one (or more) for an attendant, extra rows for split-stay date segments, and
-- legacy/test data that uses other "bookedfor" tags (e.g. "Other"). The previous
-- version of this check used "SELECT fromdate ... LIMIT 1" with no ORDER BY and no
-- filter on who the room was booked for, so it could non-deterministically grab an
-- attendant's or unrelated occupant's row instead of the patient's own room -- e.g.
-- it once compared against an attendant's 30-Jul start date when the patient's own
-- room actually started 15-Jul, incorrectly rejecting a perfectly valid arrival date.
--
-- Fix: prefer the earliest start date among rows explicitly booked for the patient.
-- Rows with no bookedfor value (older data predating that column) are treated as the
-- patient's own room, since that was the only kind of row that existed back then.
SELECT MIN(fromdate::date)
INTO lvar_fromdate
FROM RoomAllocation
WHERE ipdno = pvar_IPDApplicationFormid
  AND lower(COALESCE(bookedfor, 'patient')) = 'patient';

-- Fallback for bookings where no row is tagged "Patient" at all (e.g. legacy/test
-- data booked entirely under "Attendant"/"Other" tags) -- use the earliest date
-- across every room tied to this booking rather than skipping the check entirely.
IF lvar_fromdate IS NULL THEN
    SELECT MIN(fromdate::date)
    INTO lvar_fromdate
    FROM RoomAllocation
    WHERE ipdno = pvar_IPDApplicationFormid;
END IF;

-- Validate
IF lvar_fromdate IS NOT NULL THEN
    IF pvar_estimatedarrival::date < (lvar_fromdate - 1)
       OR pvar_estimatedarrival::date > (lvar_fromdate + 1)
    THEN
        pvar_returnMessage := 'Estimated arrival must be within ±1 day of the allotted room''s start date (' || to_char(lvar_fromdate, 'DD-Mon-YYYY') || ')';

        RETURN;
    END IF;
END IF;

			  IF(pvar_returnMessage='')
			  THEN
                -- Allow Confirm_Arrival only when bookingstatus = 'Provisional Confirmed' (payment received)
               /* IF NOT EXISTS (
                SELECT 1
                FROM IPDApplicationForm
                WHERE IPDApplicationFormid = pvar_IPDApplicationFormid
                  AND lower(coalesce(bookingstatus,'')) IN ('confirmed', 'provisional confirmed')
            ) THEN
                pvar_returnMessage := 'Cannot confirm arrival: Booking must be in Provisional Confirmed status (payment required first)';
                RETURN;
            END IF;*/
                    INSERT INTO history
VALUES('IPDApplicationForm', NOW(),
(SELECT query_to_xml('SELECT * FROM IPDApplicationForm WHERE IPDApplicationForm.IPDApplicationFormid= '''||pvar_IPDApplicationFormid||'''', true, false, '')));

                    -- Do not change verifiedstatus; bookingstatus will be set to Approved by application layer
                    UPDATE IPDApplicationForm SET
                    bookingreferencenumber=pvar_bookingreferencenumber
,estimatedarrival=pvar_estimatedarrival
,travelarrangement=pvar_travelarrangement
,typeoftravelrequired=pvar_typeoftravelrequired
,pickupfrom=pvar_pickupfrom
,requiredparkingspace=pvar_requiredparkingspace
,wheelchairassistance=pvar_wheelchairassistance
,requireddinner=pvar_requireddinner
,specialrequest=pvar_specialrequest
,bookingstatus='Arrival Confirmed'


                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE IPDApplicationFormid=pvar_IPDApplicationFormid;








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
																,'Confirm_Arrival'
																,'Authorization Failed Confirm_Arrival'
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
						,'Confirm_Arrival'
						,'update failed'
						);
                        pvar_returnMessage := 'Confirm_Arrival - update failed';*/

			  END
              $BODY$
              LANGUAGE plpgsql;
