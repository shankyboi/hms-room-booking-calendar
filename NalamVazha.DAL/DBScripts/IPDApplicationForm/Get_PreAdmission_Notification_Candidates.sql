CREATE OR REPLACE FUNCTION public."Get_PreAdmission_Notification_Candidates"(
    pvar_businessdate date)
RETURNS TABLE(
    ipdapplicationformid uuid,
    tenantid uuid,
    createduser uuid)
LANGUAGE sql
STABLE
AS $BODY$
    SELECT DISTINCT
        ipd.ipdapplicationformid,
        ipd.tenantid,
        ipd.createduser
    FROM ipdapplicationform ipd
    INNER JOIN tenant t
        ON t.tenantid = ipd.tenantid
       AND COALESCE(t.isdeleted, false) = false
    INNER JOIN ipdapplicationform_preferreddatesofadmission admission
        ON admission.ipdapplicationformid = ipd.ipdapplicationformid
       AND COALESCE(admission.isdeleted, false) = false
    INNER JOIN patientprofile patient
        ON patient.patientprofileid = ipd.patientname
       AND COALESCE(patient.isdeleted, false) = false
    WHERE COALESCE(ipd.isdeleted, false) = false
      AND t.preadmissionnoticedays IS NOT NULL
      AND admission.dateofarrival - t.preadmissionnoticedays = pvar_businessdate
      AND NULLIF(BTRIM(patient.emailaddress), '') IS NOT NULL
      AND NOT EXISTS (
          SELECT 1
          FROM maillogs log
          WHERE COALESCE(log.isdeleted, false) = false
            AND log.issent = true
            AND log.entityname = 'IPDApplicationForm'
            AND log.entityid = ipd.ipdapplicationformid::varchar
            AND log.mailfor = 'Pre-Admission Notification'
      );
$BODY$;
