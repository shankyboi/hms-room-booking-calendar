
CREATE OR REPLACE FUNCTION public."Update_OPD_Form"(
	pvar_opdformid uuid,
	pvar_tenantid uuid,
	pvar_bookingreferencenumber character varying,
	pvar_patientname uuid,
	pvar_appointmentmode character varying,
	pvar_preferreddoctor uuid,
	pvar_verifiedstatus character varying,
	pvar_medicalinfo json,
	pvar_medicationinfo json,
	pvar_medicalrecords json,
	pvar_appointmentpreferences json,
	pvar_modifieduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:53*/
			  IF "Check_Authorization"(pvar_modifieduser, 'OPDForm', 'edit') THEN

			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from OPDForm where upper(OPDForm.bookingreferencenumber) = upper(pvar_bookingreferencenumber) and OPDForm.tenantid=pvar_tenantid  and OPDForm.OPDFormid <> pvar_OPDFormid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Booking Reference Number Already Exists.';

																END IF;

               IF(pvar_verifiedstatus is not null AND pvar_verifiedstatus!='0' AND LENGTH(pvar_verifiedstatus)>0)
                                                            THEN
                                                                 if(CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_verifiedstatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc
                                                                from lookups  where fieldname='verifiedstatus'
                                                                and entityname='OPDForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_verifiedstatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'verifiedstatus value is invalid';

                                                                END IF;
                                                            END IF;

               IF(pvar_appointmentmode is not null AND pvar_appointmentmode!='0' AND LENGTH(pvar_appointmentmode)>0)
                                                            THEN
                                                                 if(CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_appointmentmode, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc
                                                                from lookups  where fieldname='appointmentmode'
                                                                and entityname='OPDForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_appointmentmode, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'appointmentmode value is invalid';

                                                                END IF;
                                                            END IF;

			  IF(pvar_returnMessage='')
			  THEN
                IF EXISTS (
                SELECT 1
                FROM OPDForm
                WHERE OPDFormid = pvar_OPDFormid
                  AND lower(coalesce(verifiedstatus,'')) IN ('approved','rejected')
            ) THEN
                pvar_returnMessage := 'Cannot update: Already Approved/Rejected';
                RETURN;
            END IF;
                    INSERT INTO history
VALUES('OPDForm', NOW(),
(SELECT query_to_xml('SELECT * FROM OPDForm WHERE OPDForm.OPDFormid= '''||pvar_OPDFormid||'''', true, false, '')));

                    --pvar_verifiedstatus:='Ready For Review';
					pvar_verifiedstatus:='Ready For OPD Review';
                    UPDATE OPDForm SET
                    bookingreferencenumber=pvar_bookingreferencenumber
,patientname=pvar_patientname
,appointmentmode=pvar_appointmentmode
,preferreddoctor=pvar_preferreddoctor
,verifiedstatus=pvar_verifiedstatus

                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE OPDFormid=pvar_OPDFormid;

                    INSERT INTO history
VALUES('OPDForm_medicalinfo', NOW(),
(SELECT query_to_xml('SELECT * FROM OPDForm_medicalinfo WHERE OPDForm_medicalinfo.OPDFormid= '''||pvar_OPDFormid||'''', true, false, '')));

								DELETE FROM  OPDForm_medicalinfo WHERE OPDFormid=pvar_OPDFormid;

								INSERT INTO OPDForm_medicalinfo (
									OPDFormid
									,OPDForm_medicalinfoid
                                    ,record_order
									,medicalconditionname
,duration
,unit
,severitylevel

									)
									SELECT
									pvar_OPDFormid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'medicalconditionname' AS  uuid) as medicalconditionname
,CAST(j->>'duration' AS decimal(18,2)) as duration
,j->>'unit' as unit
,j->>'severitylevel' as severitylevel

                                    FROM json_array_elements(pvar_medicalinfo) as j
                                    WHERE NOT (
                                        COALESCE(TRIM(j->>'medicalconditionname'),'') = ''
                                        AND COALESCE(NULLIF(TRIM(j->>'duration'),''),'0') IN ('0','0.0','0.00')
                                        AND COALESCE(TRIM(j->>'unit'),'') = ''
                                        AND COALESCE(TRIM(j->>'severitylevel'),'') = ''
                                    );

INSERT INTO history
VALUES('OPDForm_medicationinfo', NOW(),
(SELECT query_to_xml('SELECT * FROM OPDForm_medicationinfo WHERE OPDForm_medicationinfo.OPDFormid= '''||pvar_OPDFormid||'''', true, false, '')));

								DELETE FROM  OPDForm_medicationinfo WHERE OPDFormid=pvar_OPDFormid;

								INSERT INTO OPDForm_medicationinfo (
									OPDFormid
									,OPDForm_medicationinfoid
                                    ,record_order
									,medicinename
,frequencyinaday
,medicationduration

									)
									SELECT
									pvar_OPDFormid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'medicinename' as medicinename
,j->>'frequencyinaday' as frequencyinaday
,j->>'medicationduration' as medicationduration

                                    FROM json_array_elements(pvar_medicationinfo) as j
                                    WHERE COALESCE(TRIM(j->>'medicinename'),'') <> ''
                                       OR COALESCE(TRIM(j->>'frequencyinaday'),'') <> ''
                                       OR COALESCE(TRIM(j->>'medicationduration'),'') <> '';

INSERT INTO history
VALUES('OPDForm_medicalrecords', NOW(),
(SELECT query_to_xml('SELECT * FROM OPDForm_medicalrecords WHERE OPDForm_medicalrecords.OPDFormid= '''||pvar_OPDFormid||'''', true, false, '')));

								DELETE FROM  OPDForm_medicalrecords WHERE OPDFormid=pvar_OPDFormid;

								INSERT INTO OPDForm_medicalrecords (
									OPDFormid
									,OPDForm_medicalrecordsid
                                    ,record_order
									,medicalrecordname
,medicalrecordfile

									)
									SELECT
									pvar_OPDFormid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'medicalrecordname' as medicalrecordname
,j->>'medicalrecordfile' as medicalrecordfile

                                    FROM json_array_elements(pvar_medicalrecords) as j
                                    WHERE COALESCE(TRIM(j->>'medicalrecordname'),'') <> ''
                                       OR COALESCE(TRIM(j->>'medicalrecordfile'),'') <> '';

INSERT INTO history
VALUES('OPDForm_appointmentpreferences', NOW(),
(SELECT query_to_xml('SELECT * FROM OPDForm_appointmentpreferences WHERE OPDForm_appointmentpreferences.OPDFormid= '''||pvar_OPDFormid||'''', true, false, '')));

								DELETE FROM  OPDForm_appointmentpreferences WHERE OPDFormid=pvar_OPDFormid;

								INSERT INTO OPDForm_appointmentpreferences (
									OPDFormid
									,OPDForm_appointmentpreferencesid
                                    ,record_order
									,preferreddate
,slotpreference

									)
									SELECT
									pvar_OPDFormid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(NULLIF(j->>'preferreddate','') AS date) as preferreddate
,j->>'slotpreference' as slotpreference

                                    FROM json_array_elements(pvar_appointmentpreferences) as j
                                    WHERE COALESCE(TRIM(j->>'preferreddate'),'') <> ''
                                       OR COALESCE(TRIM(j->>'slotpreference'),'') <> '';

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
																,'Update_OPD_Form'
																,'Authorization Failed Update_OPD_Form'
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
						,'Update_OPD_Form'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_OPD_Form - update failed';*/

			  END
              
$BODY$;
