DROP FUNCTION IF EXISTS "Get_Latest_Previous_OPD_Prefill_By_Patient"(Varchar,Varchar);

CREATE OR REPLACE FUNCTION "Get_Latest_Previous_OPD_Prefill_By_Patient"
(
	pvar_tenantid Varchar,
	pvar_patientname Varchar
)
RETURNS TABLE(
	"OPDFormid" uuid,
	patientid uuid,
	preferreddoctorid uuid,
	preferreddoctor Varchar,
	createddate Timestamp(5),
	"automaton_OPDForm_medicalinfo" json
)
AS $BODY$
BEGIN
	RETURN QUERY
	SELECT
		OPDForm.OPDFormid,
		OPDForm.patientname as patientid,
		OPDForm.preferreddoctor as preferreddoctorid,
		CAST(COALESCE(__People.firstname || ' ' || __People.lastname, '') AS VARCHAR) as preferreddoctor,
		OPDForm.createddate,
		(
			SELECT json_agg(J)
			FROM (
				SELECT
					OPDForm_medicalinfo.medicalconditionname as medicalconditionname,
					CAST(_MedicalCondition.conditionname AS VARCHAR) as medicalconditionname_text,
					OPDForm_medicalinfo.duration as duration,
					OPDForm_medicalinfo.unit as unit,
					OPDForm_medicalinfo.severitylevel as severitylevel,
					OPDForm_medicalinfo.record_order as record_order
				FROM OPDForm_medicalinfo
				LEFT JOIN MedicalCondition _MedicalCondition
					ON OPDForm_medicalinfo.medicalconditionname = _MedicalCondition.MedicalConditionid
				WHERE OPDForm.OPDFormid = OPDForm_medicalinfo.OPDFormid
				ORDER BY OPDForm_medicalinfo.record_order ASC
			) J
		) as "automaton_OPDForm_medicalinfo"
	FROM OPDForm
	LEFT JOIN People __People ON OPDForm.preferreddoctor = __People.Peopleid
	WHERE OPDForm.isdeleted = false
		AND (pvar_tenantid IS NULL OR pvar_tenantid = '' OR CAST(OPDForm.tenantid AS VARCHAR) = pvar_tenantid)
		AND CAST(OPDForm.patientname AS VARCHAR) = pvar_patientname
	ORDER BY COALESCE(OPDForm.modifieddate, OPDForm.createddate) DESC, OPDForm.createddate DESC
	LIMIT 1;
END
$BODY$
LANGUAGE plpgsql;
