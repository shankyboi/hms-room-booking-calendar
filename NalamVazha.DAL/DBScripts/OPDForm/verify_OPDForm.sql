
CREATE OR REPLACE FUNCTION public."verify_OPDForm"(
	pvar_opdformid character varying,
	pvar_verifiedby character varying,
	pvar_verifiedstatus character varying,
	pvar_reviewcomments character varying,
	pvar_task character varying,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

                DECLARE lvar_opdformid_array UUID[];
                lvar_opdformid UUID;
                lvar_invalid_status_count INTEGER;
                lvar_total_opdform_count INTEGER;
				lvar_taskvalidation VARCHAR(4000);
                /* PatientVisit / PatientQueue helpers */
                lvar_tenantid       UUID;
                lvar_patientname    UUID;
                lvar_preferreddoctor UUID;
                lvar_task_uuid      UUID;
                lvar_preferreddate  TIMESTAMP;
                lvar_patientvisitid UUID;
                lvar_visitnumber_pv VARCHAR(20);
                lvar_curday_visitnumber_pv VARCHAR(10);
                lvar_val_visitnumber_pv INT;
                lvar_queueid        UUID;
              BEGIN
	        /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 05:47:00*/

              lvar_opdformid_array := STRING_TO_ARRAY(pvar_opdformid, ',');

              lvar_total_opdform_count := array_length(lvar_opdformid_array, 1);
               -- Check for invalid status orders
			   IF upper(pvar_verifiedstatus) <> 'CANCELLED' THEN
               SELECT COUNT(*) INTO lvar_invalid_status_count
               FROM OPDForm
               WHERE opdformid = ANY(lvar_opdformid_array)
               AND verifiedstatus NOT IN ('Ready For Review', 'Ready For OPD Review');

               IF lvar_invalid_status_count > 0 THEN
        CASE
            WHEN lvar_total_opdform_count = 1 THEN
                pvar_returnMessage := 'The given record is already reviewed. Please try again.';
            WHEN lvar_invalid_status_count = lvar_total_opdform_count THEN
                pvar_returnMessage := 'All given records are already reviewed. Please try again.';
            ELSE
                pvar_returnMessage := 'Some of the given records are already reviewed. Please try again.';
        END CASE;
        RETURN;
		 END IF;
    END IF;

            FOREACH lvar_opdformid IN ARRAY lvar_opdformid_array
            LOOP
			
	  SELECT "Validate_OPD_Task_Eligibility"(
				  patientname,
				  COALESCE(NULLIF(BTRIM(pvar_task), '')::UUID, OPDForm.task),
				  NOW()::timestamp,
				  lvar_opdformid)
			  INTO lvar_taskvalidation FROM OPDForm WHERE opdformid=lvar_opdformid;
			  IF lvar_taskvalidation <> '201.1' THEN
				pvar_returnMessage := lvar_taskvalidation;
				RETURN;
			  END IF;
			  
			  UPDATE OPDForm
			  SET verifiedby=CAST(pvar_verifiedby AS UUID)
			  ,verifiedstatus=pvar_verifiedstatus
			  ,verifieddate=NOW()
			  ,reviewcomments=pvar_reviewcomments
			  ,task=CASE WHEN pvar_task IS NOT NULL AND pvar_task != '' THEN CAST(pvar_task AS UUID) ELSE task END
			  WHERE  opdformid=lvar_opdformid;

              INSERT INTO reviewlogsOPDForm(opdformid, verifiedstatus, reviewcomments, createduser)
              VALUES (lvar_opdformid, pvar_verifiedstatus, pvar_reviewcomments, CAST(pvar_verifiedby AS UUID));

              /* ── Auto-create PatientVisit + PatientQueue when Approved ── */
              IF upper(pvar_verifiedstatus) IN ('APPROVED', 'OPD APPROVED') THEN

                  /* Fetch OPD form details needed for PatientVisit / PatientQueue */
                  SELECT tenantid, patientname, preferreddoctor, task, preferreddate
                  INTO lvar_tenantid, lvar_patientname, lvar_preferreddoctor, lvar_task_uuid, lvar_preferreddate
                  FROM OPDForm
                  WHERE opdformid = lvar_opdformid;

                  /* Only insert if no PatientVisit already exists for this OPD form */
                  IF NOT EXISTS (
                      SELECT 1 FROM PatientVisit WHERE opdnumber = lvar_opdformid AND isdeleted = false
                  ) THEN

                      /* Generate visit number: YYYYMMDD-NNNNN */
                      lvar_patientvisitid := gen_random_uuid();
                      SELECT cast(to_char(NOW(),'yyyy') AS VARCHAR(4))
                             || RIGHT('00' || cast(to_char(NOW(),'MM') AS VARCHAR(2)), 2)
                             || RIGHT('00' || cast(to_char(NOW(),'dd') AS VARCHAR(2)), 2)
                      INTO lvar_curday_visitnumber_pv;

                      SELECT COALESCE(MAX(RIGHT(pv.visitnumber, 5)::INT), 0)
                      INTO lvar_val_visitnumber_pv
                      FROM PatientVisit pv
                      WHERE SUBSTRING(pv.visitnumber, 1, 8) = lvar_curday_visitnumber_pv
                        AND pv.visitnumber NOT LIKE '%/%';

                      lvar_val_visitnumber_pv := lvar_val_visitnumber_pv + 1;
                      lvar_visitnumber_pv := lvar_curday_visitnumber_pv || '-'
                                            || LPAD(lvar_val_visitnumber_pv::TEXT, 5, '0');

                      INSERT INTO PatientVisit(
                          PatientVisitid, tenantid, visitnumber, visitdatetime,
                          patientname, visittype, opdnumber, consultingdoctor,
                          visitstatus, createduser
                      ) VALUES (
                          lvar_patientvisitid, lvar_tenantid, lvar_visitnumber_pv,
                          COALESCE(lvar_preferreddate, NOW()),
                          lvar_patientname, 'OPD', lvar_opdformid, lvar_preferreddoctor,
                          'Checked-In', CAST(pvar_verifiedby AS UUID)
                      );

                      /* Find queue linked to the task for this tenant */
                      SELECT Queueid INTO lvar_queueid
                      FROM Queue
                      WHERE task = COALESCE(
                              CASE WHEN pvar_task IS NOT NULL AND pvar_task != ''
                                   THEN CAST(pvar_task AS UUID) END,
                              lvar_task_uuid
                          )
                        AND tenantid = lvar_tenantid
                        AND isdeleted = false
                      LIMIT 1;

                      /* Fall back to first queue for this tenant if no task-linked queue */
                      IF lvar_queueid IS NULL THEN
                          SELECT Queueid INTO lvar_queueid
                          FROM Queue
                          WHERE tenantid = lvar_tenantid AND isdeleted = false
                          ORDER BY createddate
                          LIMIT 1;
                      END IF;

                      IF lvar_queueid IS NOT NULL THEN
                          INSERT INTO PatientQueue(
                              PatientQueueid, tenantid, patientvisit, visitdatetime,
                              patientname, queuename, queuestatus, createduser
                          ) VALUES (
                              gen_random_uuid(), lvar_tenantid, lvar_patientvisitid,
                              COALESCE(lvar_preferreddate, NOW()),
                              lvar_patientname, lvar_queueid, 'Waiting',
                              CAST(pvar_verifiedby AS UUID)
                          );
                      END IF;

                  END IF; /* NOT EXISTS PatientVisit */
              END IF; /* Approved */

            END LOOP;

				pvar_returnMessage:='201.1';

			  END

$BODY$;


