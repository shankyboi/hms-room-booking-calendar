
CREATE OR REPLACE FUNCTION public."Approved_OPD_Forms"(
	pvar_tenantid character varying,
	pvar_patientname character varying,
	pvar_preferreddate_automatonfrom character varying,
	pvar_preferreddate_automatonto character varying,
	pvar_preferreddoctor character varying,
	pvar_pagesize integer,
	pvar_pagenumber integer,
	pvar_searchterm character varying,
	pvar_sort_fields json)
    RETURNS json
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
               declare local_sortcolumn_array text[] = (
	                select array_agg(col) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)
                );
                declare local_sortorder_array text[] = (
	                select array_agg(dir) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)
                );
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;

	  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 05:47:00*/

                SELECT  SPLIT_PART(pvar_tenantid, '|', 1),SPLIT_PART(pvar_tenantid, '|', 2) into lstr_usersid,pvar_tenantid;

                if(pvar_tenantid is null or pvar_tenantid='' or pvar_tenantid='00000000-0000-0000-0000-000000000000')
				then
                    SELECT STRING_TO_ARRAY(viewertenantids, ',') into lvar_tenantid
				    FROM users where users.usersid::varchar=lstr_usersid;
                    if(lvar_tenantid is NULL)
					then
						SELECT array_agg(tenant.tenantid) INTO lvar_tenantid FROM tenant;

					end if;
                else
				  lvar_tenantid=ARRAY[pvar_tenantid];
                end if;
                lvar_tenantid := lvar_tenantid || ARRAY[''::character varying] || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];

                    if(pvar_searchterm is not null and LENGTH(CAST(pvar_searchterm as Varchar)) > 0)
                    then
                    pvar_searchterm := '%' || pvar_searchterm || '%';
                    else
                    pvar_searchterm := null;
                    end if;

                    RETURN json_build_object(
                    'count'
                    ,(SELECT
                    COUNT(*)
                    FROM  OPDForm
LEFT OUTER JOIN tenant ON OPDForm.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON OPDForm.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN Task __Task ON OPDForm.task=__Task.Taskid
LEFT OUTER JOIN People ___People ON OPDForm.preferreddoctor=___People.Peopleid
LEFT OUTER JOIN PatientCategory PatientCategory ON _PatientProfile.PatientCategory=PatientCategory.PatientCategoryid

                    WHERE (lvar_tenantid is null or COALESCE(cast(OPDForm.tenantid as varchar), '') = Any(lvar_tenantid)) AND OPDForm.isdeleted=false
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(OPDForm.patientname as VARCHAR)=pvar_patientname)
AND (pvar_preferreddoctor is null or pvar_preferreddoctor ='0' or LENGTH(CAST(pvar_preferreddoctor as Varchar))=0 or CAST(OPDForm.preferreddoctor as VARCHAR)=pvar_preferreddoctor)
AND(pvar_preferreddate_automatonfrom IS NULL OR pvar_preferreddate_automatonfrom = '' OR OPDForm.preferreddate >= CAST(pvar_preferreddate_automatonfrom AS TIMESTAMP(3))) 
                                AND (pvar_preferreddate_automatonto IS NULL OR pvar_preferreddate_automatonto = '' OR OPDForm.preferreddate <= CAST(pvar_preferreddate_automatonto AS TIMESTAMP(3)))
AND OPDForm.verifiedstatus IN ('Approved', 'OPD Approved', 'Assessment Form - In Draft', 'Assessment Intern Review Pending', 'Assessment Doctor Review Pending', 'OPD Completed')
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(OPDForm.bookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CASE WHEN _PatientProfile.lastname IS NULL OR _PatientProfile.lastname = '' THEN _PatientProfile.firstname ELSE _PatientProfile.firstname || ' ' || _PatientProfile.lastname END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(OPDForm.appointmentmode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__Task.taskname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CASE WHEN ___People.lastname IS NULL OR ___People.lastname = '' THEN ___People.firstname ELSE ___People.firstname || ' ' || ___People.lastname END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(OPDForm.preferreddate,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
	  OR ((pvar_searchterm is null) or CAST(OPDForm.verifiedstatus AS VARCHAR) ilike pvar_searchterm)
))
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT
                    OPDForm.tenantid
,tenant.businessname as _tenantName
,OPDForm.OPDFormid
,OPDForm.bookingreferencenumber
,OPDForm.patientname
,COALESCE(_PatientProfile.blacklisted,'No') as blacklisted
,COALESCE(PatientCategory.patientcategoryname,'Not Categorized') as patientcategoryname
,CAST(CASE WHEN _PatientProfile.lastname IS NULL OR _PatientProfile.lastname = '' THEN _PatientProfile.firstname ELSE _PatientProfile.firstname || ' ' || _PatientProfile.lastname END AS VARCHAR) as patientname_master
,OPDForm.appointmentmode
,OPDForm.task
,CAST(__Task.taskname AS VARCHAR) as task_master
,OPDForm.preferreddoctor
,CAST(CASE WHEN ___People.lastname IS NULL OR ___People.lastname = '' THEN ___People.firstname ELSE ___People.firstname || ' ' || ___People.lastname END AS VARCHAR) as preferreddoctor_master
						,OPDForm.verifiedstatus
,(SELECT a.Assessmentid FROM Assessment a WHERE a.opdform=OPDForm.OPDFormid AND COALESCE(a.isdeleted,false)=false ORDER BY a.createddate DESC LIMIT 1) as assessmentid
,"Can_Review_OPD_Assessment"(OPDForm.OPDFormid, NULLIF(lstr_usersid, '')::uuid) AS canreviewassessment

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

                    ,OPDForm.createduser,OPDForm.createddate,OPDForm.modifieduser,OPDForm.modifieddate
                    FROM  OPDForm
LEFT OUTER JOIN tenant ON OPDForm.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON OPDForm.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN Task __Task ON OPDForm.task=__Task.Taskid
LEFT OUTER JOIN People ___People ON OPDForm.preferreddoctor=___People.Peopleid
LEFT OUTER JOIN PatientCategory PatientCategory ON _PatientProfile.PatientCategory=PatientCategory.PatientCategoryid

                    WHERE (lvar_tenantid is null or COALESCE(cast(OPDForm.tenantid as varchar), '') = Any(lvar_tenantid)) AND OPDForm.isdeleted=false
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(OPDForm.patientname as VARCHAR)=pvar_patientname)
AND (pvar_preferreddoctor is null or pvar_preferreddoctor ='0' or LENGTH(CAST(pvar_preferreddoctor as Varchar))=0 or CAST(OPDForm.preferreddoctor as VARCHAR)=pvar_preferreddoctor)
 AND(pvar_preferreddate_automatonfrom IS NULL OR pvar_preferreddate_automatonfrom = '' OR OPDForm.preferreddate >= CAST(pvar_preferreddate_automatonfrom AS TIMESTAMP(3))) 
                                AND (pvar_preferreddate_automatonto IS NULL OR pvar_preferreddate_automatonto = '' OR OPDForm.preferreddate <= CAST(pvar_preferreddate_automatonto AS TIMESTAMP(3)))
AND OPDForm.verifiedstatus IN ('Approved', 'OPD Approved', 'Assessment Form - In Draft', 'Assessment Intern Review Pending', 'Assessment Doctor Review Pending', 'OPD Completed')

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(OPDForm.bookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CASE WHEN _PatientProfile.lastname IS NULL OR _PatientProfile.lastname = '' THEN _PatientProfile.firstname ELSE _PatientProfile.firstname || ' ' || _PatientProfile.lastname END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(OPDForm.appointmentmode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__Task.taskname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CASE WHEN ___People.lastname IS NULL OR ___People.lastname = '' THEN ___People.firstname ELSE ___People.firstname || ' ' || ___People.lastname END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(OPDForm.preferreddate,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
						  OR ((pvar_searchterm is null) or CAST(OPDForm.verifiedstatus AS VARCHAR) ilike pvar_searchterm)
)
                    ORDER BY
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
WHEN 'bookingreferencenumber' THEN OPDForm.bookingreferencenumber::TEXT
WHEN 'bookingreferencenumber' THEN OPDForm.bookingreferencenumber::TEXT
WHEN 'patientname' THEN CASE WHEN _PatientProfile.lastname IS NULL OR _PatientProfile.lastname = '' THEN _PatientProfile.firstname ELSE _PatientProfile.firstname || ' ' || _PatientProfile.lastname END::TEXT
WHEN 'appointmentmode' THEN OPDForm.appointmentmode::TEXT
WHEN 'task' THEN __Task.taskname::TEXT
WHEN 'preferreddoctor' THEN CASE WHEN ___People.lastname IS NULL OR ___People.lastname = '' THEN ___People.firstname ELSE ___People.firstname || ' ' || ___People.lastname END::TEXT
WHEN 'verifiedstatus' THEN OPDForm.verifiedstatus::TEXT

                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'bookingreferencenumber' THEN OPDForm.bookingreferencenumber::TEXT
WHEN 'bookingreferencenumber' THEN OPDForm.bookingreferencenumber::TEXT
WHEN 'patientname' THEN CASE WHEN _PatientProfile.lastname IS NULL OR _PatientProfile.lastname = '' THEN _PatientProfile.firstname ELSE _PatientProfile.firstname || ' ' || _PatientProfile.lastname END::TEXT
WHEN 'appointmentmode' THEN OPDForm.appointmentmode::TEXT
WHEN 'task' THEN __Task.taskname::TEXT
WHEN 'preferreddoctor' THEN CASE WHEN ___People.lastname IS NULL OR ___People.lastname = '' THEN ___People.firstname ELSE ___People.firstname || ' ' || ___People.lastname END::TEXT
WHEN 'verifiedstatus' THEN OPDForm.verifiedstatus::TEXT

                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
                    limit pvar_pagesize
                    offset pvar_pagenumber * pvar_pagesize

                    ) d));

			  END

$BODY$;


