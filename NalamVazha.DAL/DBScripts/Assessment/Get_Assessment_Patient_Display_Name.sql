CREATE OR REPLACE FUNCTION "Get_Assessment_Patient_Display_Name"
(
	pvar_assessmentid Varchar(50)
)
RETURNS TABLE(patientname Varchar)
AS $BODY$
BEGIN
RETURN QUERY
SELECT CAST(NULLIF(btrim(concat_ws(' ', NULLIF(PatientProfile.firstname, ''), NULLIF(PatientProfile.lastname, ''))), '') AS Varchar) as patientname
FROM Assessment
LEFT JOIN PatientProfile ON Assessment.patientname = PatientProfile.PatientProfileid
WHERE CAST(Assessment.Assessmentid AS Varchar) = pvar_assessmentid
AND COALESCE(Assessment.isdeleted, false) = false;
END
$BODY$
LANGUAGE plpgsql;
