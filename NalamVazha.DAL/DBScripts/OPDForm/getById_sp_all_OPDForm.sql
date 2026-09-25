
-- The returned detail shape now includes patient status/category fields. A
-- PostgreSQL function must be dropped before changing its OUT column list.
DROP FUNCTION IF EXISTS public."getById_sp_all_OPDForm"(character varying);

CREATE OR REPLACE FUNCTION public."getById_sp_all_OPDForm"(
	pvar_opdformid character varying)
    RETURNS TABLE(tenantid uuid, _tenantname character varying, "OPDFormid" uuid, bookingreferencenumber character varying, patientname character varying, patientid uuid, blacklisted character varying, patientcategoryname character varying, appointmentmode character varying, preferreddoctor character varying, preferreddoctorid uuid, taskid uuid, taskname character varying, verifiedstatus character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, "automaton_OPDForm_medicalinfo" json, "automaton_OPDForm_medicationinfo" json, "automaton_OPDForm_medicalrecords" json, "automaton_OPDForm_appointmentpreferences" json, automaton_review_logs json, automaton_review_logs_history json, authorized_users text, authorized_users_mobile text, verifieddate timestamp without time zone, reviewcomments character varying)
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:53*/

            RETURN QUERY
            SELECT
            OPDForm.tenantid
,tenant.businessname as _tenantname
,OPDForm.OPDFormid
,OPDForm.bookingreferencenumber
,CAST(
    COALESCE(
        NULLIF(
            CONCAT_WS(
                ' ',
                NULLIF(BTRIM(_PatientProfile.firstname), ''),
                NULLIF(BTRIM(_PatientProfile.lastname), '')
            ),
            ''
        ),
        '-'
    ) AS VARCHAR
) as patientname
,OPDForm.patientname as patientid
,COALESCE(_PatientProfile.blacklisted,'No') as blacklisted
,COALESCE(PatientCategory.patientcategoryname,'Not Categorized') as patientcategoryname
,OPDForm.appointmentmode
,CAST(COALESCE(NULLIF(TRIM(CONCAT_WS(' ', __People.firstname, __People.lastname)), ''), '-') AS VARCHAR) as preferreddoctor
,OPDForm.preferreddoctor as preferreddoctorid
,OPDForm.task as taskid
,CAST(__Task.taskname AS VARCHAR) as taskname
,OPDForm.verifiedstatus

            ,OPDForm.createduser,OPDForm.createddate,OPDForm.modifieduser,OPDForm.modifieddate
            ,
						(SELECT json_agg(J) FROM (SELECT
						CAST(_MedicalCondition.conditionname AS VARCHAR) as "Medical Condition Name"
,OPDForm_medicalinfo.duration as "Duration"
,OPDForm_medicalinfo.unit as "Unit"
,OPDForm_medicalinfo.severitylevel as "Severity Level"


						FROM  OPDForm_medicalinfo
INNER JOIN MedicalCondition _MedicalCondition ON OPDForm_medicalinfo.medicalconditionname=_MedicalCondition.MedicalConditionid

						WHERE OPDForm.OPDFormid =OPDForm_medicalinfo.OPDFormid
) J)
						as automaton_OPDForm_medicalinfo
,
						(SELECT json_agg(J) FROM (SELECT
						OPDForm_medicationinfo.medicinename as "Medicine Name"
,OPDForm_medicationinfo.frequencyinaday as "Frequency in a day"
,OPDForm_medicationinfo.medicationduration as "Medication Duration"


						FROM  OPDForm_medicationinfo

						WHERE OPDForm.OPDFormid =OPDForm_medicationinfo.OPDFormid
) J)
						as automaton_OPDForm_medicationinfo
,
						(SELECT json_agg(J) FROM (SELECT
						OPDForm_medicalrecords.medicalrecordname as "Medical Record Name"
,OPDForm_medicalrecords.medicalrecordfile as "Medical Record File"


						FROM  OPDForm_medicalrecords

						WHERE OPDForm.OPDFormid =OPDForm_medicalrecords.OPDFormid
) J)
						as automaton_OPDForm_medicalrecords
,
						(SELECT json_agg(J) FROM (SELECT
						to_char(OPDForm_appointmentpreferences.preferreddate, 'dd/MM/yyyy') as "Preferred Date"
,OPDForm_appointmentpreferences.slotpreference as "Slot Preference"

						FROM  OPDForm_appointmentpreferences

						WHERE OPDForm.OPDFormid =OPDForm_appointmentpreferences.OPDFormid
						ORDER BY OPDForm_appointmentpreferences.record_order ASC, OPDForm_appointmentpreferences.preferreddate ASC
) J)
						as automaton_OPDForm_appointmentpreferences

 ,(SELECT json_agg(J) FROM (
                    SELECT
                    CAST(COALESCE(to_char(reviewlogsOPDForm.createddate,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as "Reviewed On"
                    ,users.firstname as "Reviewed By"
                    ,reviewlogsOPDForm.verifiedstatus "Status"
                    ,COALESCE(reviewlogsOPDForm.reviewcomments,'-')  "Comments"
                    FROM reviewlogsOPDForm
                    INNER JOIN users
                    ON reviewlogsOPDForm.createduser=users.usersid
                    WHERE reviewlogsOPDForm.OPDFormid=OPDForm.OPDFormid
                    ORDER BY OPDForm.bookingreferencenumber DESC
                    ) J)
                    as automaton_review_logs
                    ,(SELECT json_agg(J) FROM (
                    SELECT
                    COALESCE(INOPDForm.bookingreferencenumber,'-') as "Booking Reference Number"
                    ,CAST(COALESCE(to_char(reviewlogsOPDForm.createddate,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as "Reviewed On"
                    ,users.firstname as "Reviewed By"
                    ,reviewlogsOPDForm.verifiedstatus "Status"
                    ,COALESCE(reviewlogsOPDForm.reviewcomments,'-')  "Comments"
                    ,reviewlogsOPDForm.OPDFormid as "OPDFormid"
                    FROM reviewlogsOPDForm
                    INNER JOIN users
                    ON reviewlogsOPDForm.createduser=users.usersid

                    INNER JOIN OPDForm INOPDForm ON reviewlogsOPDForm.OPDFormid=INOPDForm.OPDFormid
                    /* The review log on an OPD detail page belongs to that
                       exact OPD visit.  Matching only the booking prefix mixed
                       review events from the patient's other OPD visits. */
                    WHERE reviewlogsOPDForm.OPDFormid = OPDForm.OPDFormid

                    ORDER BY INOPDForm.bookingreferencenumber DESC
                    ) J)
                    as automaton_review_logs_history

            ,(SELECT STRING_AGG(u.emailid, ', ') AS authorized_users
            FROM users u
            JOIN RoleAuthorization ra ON (
            u.userrole = ANY(string_to_array(ra.viewactionroles, ',')) -- Split the authorizedroles column by commas
            )
            WHERE ra.actionname = 'CheckerView' and controllername='OPDForm') as authorized_users
            ,(SELECT STRING_AGG(u.mobilenumber, ', ') AS authorized_users_mobile
            FROM users u
            JOIN RoleAuthorization ra ON (
            u.userrole = ANY(string_to_array(ra.viewactionroles, ',')) -- Split the authorizedroles column by commas
            AND u.isdeleted=false
            )
            WHERE ra.actionname = 'CheckerView' and controllername='OPDForm') as authorized_users_mobile
            ,OPDForm.verifieddate
            ,OPDForm.reviewcomments
            FROM  OPDForm
 LEFT OUTER JOIN tenant ON OPDForm.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON OPDForm.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN People __People ON OPDForm.preferreddoctor=__People.Peopleid
LEFT OUTER JOIN Task __Task ON OPDForm.task=__Task.Taskid
LEFT OUTER JOIN PatientCategory PatientCategory ON _PatientProfile.PatientCategory=PatientCategory.PatientCategoryid

            WHERE CAST(OPDForm.OPDFormid AS Varchar)=pvar_OPDFormid ;


			  END
              $BODY$;

