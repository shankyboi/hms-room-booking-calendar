
CREATE OR REPLACE FUNCTION public."Add_OPD_Form"(
	pvar_opdformid uuid,
	pvar_tenantid uuid,
	pvar_bookingreferencenumber character varying,
	pvar_patientname uuid,
	pvar_appointmentmode character varying,
	pvar_preferreddoctor uuid,
	pvar_task uuid,
	pvar_verifiedstatus character varying,
	pvar_medicalinfo json,
	pvar_medicationinfo json,
	pvar_medicalrecords json,
	pvar_appointmentpreferences json,
	pvar_createduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
              DECLARE lv_viewactionroles Varchar(128);lvar_curday_bookingreferencenumber Varchar(10);lvar_val_bookingreferencenumber int;lvar_registrationid VARCHAR(50);v_tenant_shortcode VARCHAR(10);existingrecordcount INT;
              lvar_patientvisitid UUID;lvar_visitnumber_pv VARCHAR(20);lvar_curday_visitnumber_pv VARCHAR(10);lvar_val_visitnumber_pv INT;lvar_queueid UUID;lvar_preferreddate date;lvar_taskvalidation VARCHAR(4000);
			  BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:53*/

                                                                                    if pvar_OPDFormid is null then
                                                                                    pvar_OPDFormid:=gen_random_uuid();
                                                                                    end if;

/*select cast(to_char(NOW(),'yyyy') as Varchar(4)) || RIGHT('00' ||cast(to_char(NOW(),'MM') as Varchar(2)),2)
                                        || RIGHT('00' ||cast(to_char(NOW(),'dd') as Varchar(2)),2) INTO lvar_curday_bookingreferencenumber;
                                       select COALESCE(max(RIGHT(OPDForm.bookingreferencenumber,3)),'0') INTO lvar_val_bookingreferencenumber from
                                         OPDForm where substring(OPDForm.bookingreferencenumber,1,8) = lvar_curday_bookingreferencenumber and (OPDForm.bookingreferencenumber) NOT LIKE '%/%';
                                      lvar_val_bookingreferencenumber:=lvar_val_bookingreferencenumber + 1;*/
                               
SELECT shortcode INTO v_tenant_shortcode
FROM tenant
WHERE tenantid = pvar_tenantid
  AND isdeleted = false;

SELECT registrationid INTO lvar_registrationid
FROM patientprofile 
WHERE patientprofileid = pvar_patientname;

-- Serialize reference allocation per patient. COUNT + 1 can reuse an existing
-- suffix when records have gaps and can also collide during concurrent saves.
PERFORM pg_advisory_xact_lock(hashtextextended(pvar_patientname::text, 0));

SELECT COALESCE(MAX(
    CASE
        WHEN split_part(bookingreferencenumber, '/O-', 2) ~ '^[0-9]+$'
            THEN split_part(bookingreferencenumber, '/O-', 2)::integer
        ELSE 0
    END
), 0)
INTO existingrecordcount
FROM OPDForm
WHERE patientname = pvar_patientname;


existingrecordcount := existingrecordcount + 1;

pvar_bookingreferencenumber := 
   -- v_tenant_shortcode || '-' || 
    lvar_registrationid || '/O-' || 
    LPAD(existingrecordcount::TEXT, 4, '0');

			  IF "Check_Authorization"(pvar_createduser, 'OPDForm', 'create') THEN
			  pvar_returnMessage:='';
			  
			 	  lvar_taskvalidation := "Validate_OPD_Task_Eligibility"(
				pvar_patientname,
				pvar_task,
				NOW()::timestamp,
				pvar_OPDFormid);
				
			  IF lvar_taskvalidation <> '201.1' THEN
				pvar_returnMessage := lvar_taskvalidation;
			  END IF;
			  
			  IF EXISTS (SELECT * from OPDForm where upper(OPDForm.bookingreferencenumber::varchar) = upper(pvar_bookingreferencenumber::varchar) and OPDForm.tenantid=pvar_tenantid and OPDForm.isdeleted = false)
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

			  if(pvar_returnMessage='')
			  THEN

			  --pvar_verifiedstatus:='Ready For Review';

			  INSERT INTO OPDForm(
				 bookingreferencenumber
,patientname
,appointmentmode
,task
,preferreddoctor
,verifiedstatus

				 ,createduser
				 ,OPDFormid
				 ,tenantid

			  )
			  VALUES (
 				 pvar_bookingreferencenumber
,pvar_patientname
,pvar_appointmentmode
,pvar_task
,pvar_preferreddoctor
,pvar_verifiedstatus

				 ,pvar_createduser
				 ,pvar_OPDFormid
				 ,pvar_tenantid

			  );

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

				SELECT MIN(preferreddate)
				INTO lvar_preferreddate
				FROM OPDForm_appointmentpreferences
				WHERE OPDFormid = pvar_OPDFormid
				  AND preferreddate IS NOT NULL;

-- If status is Approved, create PatientVisit and PatientQueue
			  IF UPPER(pvar_verifiedstatus) IN ('APPROVED', 'OPD APPROVED') THEN
			      lvar_patientvisitid := gen_random_uuid();
			      SELECT cast(to_char(NOW(),'yyyy') as Varchar(4)) || RIGHT('00' ||cast(to_char(NOW(),'MM') as Varchar(2)),2)
			          || RIGHT('00' ||cast(to_char(NOW(),'dd') as Varchar(2)),2) INTO lvar_curday_visitnumber_pv;
			      SELECT COALESCE(max(RIGHT(pv.visitnumber,5)),'0') INTO lvar_val_visitnumber_pv
			      FROM PatientVisit pv
			      WHERE substring(pv.visitnumber,1,8) = lvar_curday_visitnumber_pv AND (pv.visitnumber) NOT LIKE '%/%';
			      lvar_val_visitnumber_pv := lvar_val_visitnumber_pv + 1;
			      lvar_visitnumber_pv := (lvar_curday_visitnumber_pv||'-'|| cast(to_char(lvar_val_visitnumber_pv,'fm00000') as Varchar(5)));
			      INSERT INTO PatientVisit(
			          PatientVisitid, tenantid, visitnumber, visitdatetime, patientname,
			          visittype, opdnumber, consultingdoctor, visitstatus, createduser
			      ) VALUES (
			          lvar_patientvisitid, pvar_tenantid, lvar_visitnumber_pv,
			          COALESCE(lvar_preferreddate, CURRENT_DATE), pvar_patientname,
			          'OPD', pvar_OPDFormid, pvar_preferreddoctor, 'Checked-In', pvar_createduser
			      );
			      SELECT Queueid INTO lvar_queueid
			      FROM Queue
			      WHERE task = pvar_task AND tenantid = pvar_tenantid AND isdeleted = false
			      LIMIT 1;
			      IF lvar_queueid IS NOT NULL THEN
			          INSERT INTO PatientQueue(
			              PatientQueueid, tenantid, patientvisit, visitdatetime, patientname,
			              queuename, queuestatus, createduser
			          ) VALUES (
			              gen_random_uuid(), pvar_tenantid, lvar_patientvisitid,
			              COALESCE(lvar_preferreddate, CURRENT_DATE), pvar_patientname,
			              lvar_queueid, 'Waiting', pvar_createduser
			          );
			      END IF;
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
																,'Add_OPD_Form'
																,'Authorization Failed Add_OPD_Form'
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
						,'Add_OPD_Form'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_OPD_Form - Insert failed';*/

			  END
              
$BODY$;

