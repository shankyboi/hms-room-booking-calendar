CREATE OR REPLACE FUNCTION public."Added_OPD_Form"(
	pvar_tenantid character varying,
	pvar_patientname character varying,
	pvar_verifiedstatus character varying,
	pvar_pagesize integer,
	pvar_pagenumber integer,
	pvar_searchterm character varying,
	pvar_sort_fields json,
	pvar_createddate_automatonfrom character varying,
	pvar_createddate_automatonto character varying,
	pvar_bookingnumber character varying,
	pvar_workflowstatus character varying,
	pvar_financialstatus character varying,
	pvar_paymentmethod character varying)
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
LEFT JOIN PatientProfile _PatientProfile ON OPDForm.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN Task __Task ON OPDForm.task=__Task.Taskid
LEFT OUTER JOIN People ___People ON OPDForm.preferreddoctor=___People.Peopleid
LEFT OUTER JOIN Assessment _Assessment ON _Assessment.opdform=OPDForm.OPDFormid AND COALESCE(_Assessment.isdeleted,false)=false
LEFT OUTER JOIN PatientCategory PatientCategory ON _PatientProfile.PatientCategory=PatientCategory.PatientCategoryid

                    WHERE (lvar_tenantid is null or COALESCE(cast(OPDForm.tenantid as varchar), '') = Any(lvar_tenantid)) AND OPDForm.isdeleted=false
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(OPDForm.patientname as VARCHAR)=pvar_patientname)
AND (pvar_verifiedstatus is null or pvar_verifiedstatus ='0' or LENGTH(CAST(pvar_verifiedstatus as Varchar))=0 or CAST(OPDForm.verifiedstatus as VARCHAR)=pvar_verifiedstatus)
AND (COALESCE(BTRIM(pvar_bookingnumber),'')='' OR OPDForm.bookingreferencenumber ILIKE '%'||BTRIM(pvar_bookingnumber)||'%')
AND (COALESCE(BTRIM(pvar_workflowstatus),'')='' OR LOWER(BTRIM(OPDForm.verifiedstatus))=LOWER(BTRIM(pvar_workflowstatus)))
AND (COALESCE(BTRIM(pvar_paymentmethod),'')='' OR EXISTS (SELECT 1 FROM billingpayment bp WHERE bp.opdnumber=OPDForm.OPDFormid AND COALESCE(bp.isdeleted,false)=false AND LOWER(BTRIM(bp.paymentmode))=LOWER(BTRIM(pvar_paymentmethod))))
AND (COALESCE(BTRIM(pvar_financialstatus),'')='' OR LOWER(BTRIM(pvar_financialstatus))=LOWER((SELECT CASE WHEN COUNT(*)=0 THEN '-' WHEN COALESCE(SUM(COALESCE(r.paidamount,0)),0)>0 AND COALESCE(SUM(COALESCE(r.paidamount,0)),0)>COALESCE(SUM(COALESCE(r.amount,0)),0) THEN 'Over Paid' WHEN COALESCE(SUM(COALESCE(r.amount,0)-COALESCE(r.paidamount,0)),0)<=0 THEN 'Paid' WHEN COALESCE(SUM(COALESCE(r.paidamount,0)),0)=0 THEN 'Not Paid' ELSE 'Partially Paid' END FROM receivable r WHERE r.opdnumber=OPDForm.OPDFormid AND COALESCE(r.isdeleted,false)=false)))
AND (
    pvar_createddate_automatonfrom IS NULL
    OR LENGTH(TRIM(pvar_createddate_automatonfrom)) = 0
    OR OPDForm.createddate >= pvar_createddate_automatonfrom::timestamp
)

AND (
    pvar_createddate_automatonto IS NULL
    OR LENGTH(TRIM(pvar_createddate_automatonto)) = 0
    OR OPDForm.createddate < pvar_createddate_automatonto::timestamp
)

 AND COALESCE(OPDForm.verifiedstatus,'') !='Revised'
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(OPDForm.bookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_PatientProfile.firstname||' '||_PatientProfile.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(OPDForm.appointmentmode AS VARCHAR) ilike pvar_searchterm)
  OR ((pvar_searchterm is null) or CAST(__Task.taskname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___People.firstname||' '||___People.lastname AS VARCHAR) ilike pvar_searchterm)
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
,COALESCE(_PatientProfile.registrationid,'') as patientregistrationid
,COALESCE(_PatientProfile.mobilenumber,'') as mobilenumber
,COALESCE(_PatientProfile.gender,'') as gender
,COALESCE(EXTRACT(YEAR FROM AGE(CURRENT_DATE, _PatientProfile.dateofbirth))::int, 0) as age
,COALESCE(EXTRACT(MONTH FROM AGE(CURRENT_DATE, _PatientProfile.dateofbirth))::int, 0) as ageinmonths
,COALESCE(PatientCategory.patientcategoryname,'Not Categorized') as patientcategoryname
,CAST(CASE
        WHEN _PatientProfile.lastname IS NULL OR _PatientProfile.lastname = ''
        THEN _PatientProfile.firstname
        ELSE _PatientProfile.firstname || ' ' || _PatientProfile.lastname
    END AS VARCHAR
) as patientname_master
,OPDForm.appointmentmode
,OPDForm.task
,CAST(__Task.taskname AS VARCHAR) as task_master
,OPDForm.preferreddoctor
,CAST(CASE
        WHEN ___People.lastname IS NULL OR ___People.lastname = ''
        THEN ___People.firstname
        ELSE ___People.firstname || ' ' || ___People.lastname
    END AS VARCHAR
) as preferreddoctor_master
						,OPDForm.verifiedstatus
                        ,COALESCE((
    SELECT
        CASE
            WHEN COUNT(*) = 0 THEN '-'
            WHEN COALESCE(SUM(COALESCE(r.paidamount, 0)), 0) > 0
                 AND COALESCE(SUM(COALESCE(r.paidamount, 0)), 0) > COALESCE(SUM(COALESCE(r.amount, 0)), 0)
                THEN 'Over Paid'
            /* Waivers and negative adjustments can reduce the aggregate
               outstanding receivable to zero without a payment row. */
            WHEN COALESCE(SUM(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0)), 0) <= 0 THEN 'Paid'
            WHEN COALESCE(SUM(COALESCE(r.paidamount, 0)), 0) = 0 THEN 'Not Paid'
            WHEN COALESCE(SUM(COALESCE(r.paidamount, 0)), 0) = COALESCE(SUM(COALESCE(r.amount, 0)), 0) THEN 'Paid'
            WHEN COALESCE(SUM(COALESCE(r.paidamount, 0)), 0) < COALESCE(SUM(COALESCE(r.amount, 0)), 0) THEN 'Partially Paid'
            ELSE 'Not Paid'
        END
    FROM receivable r
    WHERE r.opdnumber = OPDForm.OPDFormid
      AND COALESCE(r.isdeleted, false) = false
), '-') AS opdfinancialstatus
,(SELECT a.Assessmentid FROM Assessment a WHERE a.opdform=OPDForm.OPDFormid AND COALESCE(a.isdeleted,false)=false ORDER BY a.createddate DESC LIMIT 1) as assessmentid
,(SELECT a.assessedby FROM Assessment a WHERE a.opdform=OPDForm.OPDFormid AND COALESCE(a.isdeleted,false)=false ORDER BY a.createddate DESC LIMIT 1) as assessment_assessedby
-- eligibleforfinaladmission is a workflow input/history value and is not a
-- physical column on Assessment. Do not project a.eligibleforfinaladmission here.
,"Can_Review_OPD_Assessment"(OPDForm.OPDFormid, NULLIF(lstr_usersid, '')::uuid) AS canreviewassessment
,"OPD_Has_Successful_Payment"(OPDForm.OPDFormid) AS ispaymentcomplete
,COALESCE((SELECT SUM(bp.amount) FROM billingpayment bp WHERE bp.opdnumber = OPDForm.OPDFormid AND COALESCE(bp.isdeleted, false) = false AND LOWER(TRIM(COALESCE(bp.paymentstatus, ''))) IN ('paid', 'success', 'completed') AND COALESCE(bp.amount, 0) > 0), 0) as total_paid
,COALESCE((
    SELECT GREATEST(
        COALESCE(SUM(CASE
            WHEN COALESCE(bp.amount, 0) < 0
              OR LOWER(TRIM(COALESCE(bp.receivablefor, ''))) = 'cancellation refund'
            THEN GREATEST(ABS(COALESCE(bp.amount, 0)), COALESCE(bp.refundedamount, 0))
            ELSE 0 END), 0)
        + COALESCE(SUM(CASE
            WHEN COALESCE(bp.amount, 0) > 0
             AND COALESCE(bp.refundedamount, 0) > 0
             AND LOWER(TRIM(COALESCE(bp.refundstatus, ''))) NOT IN ('failed', 'cancelled', 'canceled')
            THEN bp.refundedamount ELSE 0 END), 0),
        0
    )
    FROM billingpayment bp
    WHERE bp.opdnumber = OPDForm.OPDFormid
      AND COALESCE(bp.isdeleted, false) = false
), 0) AS total_refunded
,public."Get_OPD_Remaining_Refundable_Amount"(OPDForm.OPDFormid) AS remainingrefundableamount
,COALESCE((
    SELECT SUM(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0))
    FROM receivable r
    WHERE r.opdnumber = OPDForm.OPDFormid
      AND COALESCE(r.isdeleted, false) = false
), 0) AS netreceivable
,CASE
    WHEN LOWER(COALESCE(_LatestClinicalAppointment.status, '')) IN ('cancelled', 'canceled', 'rescheduled', 'resheduled') THEN NULL
    ELSE _LatestClinicalAppointment.appointmentdate
 END AS appointmentdate
,CASE
    WHEN LOWER(COALESCE(_LatestClinicalAppointment.status, '')) IN ('cancelled', 'canceled', 'rescheduled', 'resheduled') THEN NULL
    ELSE _LatestClinicalAppointment.durationfrom
 END AS durationfrom
,CASE
    WHEN LOWER(COALESCE(_LatestClinicalAppointment.status, '')) IN ('cancelled', 'canceled', 'rescheduled', 'resheduled') THEN NULL
    ELSE _LatestClinicalAppointment.durationto
 END AS durationto
,COALESCE(NULLIF(TRIM(_LatestClinicalAppointment.status), ''), '-') AS appointmentstatus

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
) J)
			    as automaton_OPDForm_appointmentpreferences

                    ,OPDForm.createduser,OPDForm.createddate,OPDForm.modifieduser,OPDForm.modifieddate
                    FROM  OPDForm
 LEFT OUTER JOIN tenant ON OPDForm.tenantid=tenant.tenantid
LEFT JOIN PatientProfile _PatientProfile ON OPDForm.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN Task __Task ON OPDForm.task=__Task.Taskid
LEFT OUTER JOIN People ___People ON OPDForm.preferreddoctor=___People.Peopleid
LEFT OUTER JOIN Assessment _Assessment ON _Assessment.opdform=OPDForm.OPDFormid AND COALESCE(_Assessment.isdeleted,false)=false
LEFT OUTER JOIN PatientCategory PatientCategory ON _PatientProfile.PatientCategory=PatientCategory.PatientCategoryid
LEFT JOIN LATERAL (
    SELECT ca.appointmentdate, ca.durationfrom, ca.durationto, ca.status
    FROM ClinicalAppointment ca
    WHERE ca.bookingid = CAST(OPDForm.OPDFormid AS VARCHAR)
      AND COALESCE(ca.isdeleted, false) = false
    ORDER BY ca.appointmentdate DESC, ca.durationfrom DESC, ca.createddate DESC
    LIMIT 1
) _LatestClinicalAppointment ON true
                    WHERE (lvar_tenantid is null or COALESCE(cast(OPDForm.tenantid as varchar), '') = Any(lvar_tenantid)) AND OPDForm.isdeleted=false
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(OPDForm.patientname as VARCHAR)=pvar_patientname)
AND (pvar_verifiedstatus is null or pvar_verifiedstatus ='0' or LENGTH(CAST(pvar_verifiedstatus as Varchar))=0 or CAST(OPDForm.verifiedstatus as VARCHAR)=pvar_verifiedstatus)
AND (COALESCE(BTRIM(pvar_bookingnumber),'')='' OR OPDForm.bookingreferencenumber ILIKE '%'||BTRIM(pvar_bookingnumber)||'%')
AND (COALESCE(BTRIM(pvar_workflowstatus),'')='' OR LOWER(BTRIM(OPDForm.verifiedstatus))=LOWER(BTRIM(pvar_workflowstatus)))
AND (COALESCE(BTRIM(pvar_paymentmethod),'')='' OR EXISTS (SELECT 1 FROM billingpayment bp WHERE bp.opdnumber=OPDForm.OPDFormid AND COALESCE(bp.isdeleted,false)=false AND LOWER(BTRIM(bp.paymentmode))=LOWER(BTRIM(pvar_paymentmethod))))
AND (COALESCE(BTRIM(pvar_financialstatus),'')='' OR LOWER(BTRIM(pvar_financialstatus))=LOWER((SELECT CASE WHEN COUNT(*)=0 THEN '-' WHEN COALESCE(SUM(COALESCE(r.paidamount,0)),0)>0 AND COALESCE(SUM(COALESCE(r.paidamount,0)),0)>COALESCE(SUM(COALESCE(r.amount,0)),0) THEN 'Over Paid' WHEN COALESCE(SUM(COALESCE(r.amount,0)-COALESCE(r.paidamount,0)),0)<=0 THEN 'Paid' WHEN COALESCE(SUM(COALESCE(r.paidamount,0)),0)=0 THEN 'Not Paid' ELSE 'Partially Paid' END FROM receivable r WHERE r.opdnumber=OPDForm.OPDFormid AND COALESCE(r.isdeleted,false)=false)))
 AND (
    pvar_createddate_automatonfrom IS NULL
    OR LENGTH(TRIM(pvar_createddate_automatonfrom)) = 0
    OR OPDForm.createddate >= pvar_createddate_automatonfrom::timestamp
)

AND (
    pvar_createddate_automatonto IS NULL
    OR LENGTH(TRIM(pvar_createddate_automatonto)) = 0
    OR OPDForm.createddate < pvar_createddate_automatonto::timestamp
)


 AND COALESCE(OPDForm.verifiedstatus,'') !='Revised'

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(OPDForm.bookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
-- OR ((pvar_searchterm is null) or CAST(_PatientProfile.firstname||' '||_PatientProfile.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(
    CASE
        WHEN _PatientProfile.lastname IS NULL OR _PatientProfile.lastname = ''
        THEN _PatientProfile.firstname
        ELSE _PatientProfile.firstname || ' ' || _PatientProfile.lastname
    END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(OPDForm.appointmentmode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__Task.taskname AS VARCHAR) ilike pvar_searchterm)
 --OR ((pvar_searchterm is null) or CAST(___People.firstname||' '||___People.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(
    CASE
        WHEN ___People.lastname IS NULL OR ___People.lastname = ''
        THEN ___People.firstname
        ELSE ___People.firstname || ' ' || ___People.lastname
    END AS VARCHAR) ilike pvar_searchterm)
						  OR ((pvar_searchterm is null) or CAST(OPDForm.verifiedstatus AS VARCHAR) ilike pvar_searchterm)
)
                    ORDER BY
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'bookingreferencenumber' THEN OPDForm.bookingreferencenumber::TEXT
WHEN 'bookingreferencenumber' THEN OPDForm.bookingreferencenumber::TEXT
WHEN 'patientname' THEN
    CASE
        WHEN _PatientProfile.lastname IS NULL OR _PatientProfile.lastname = ''
        THEN _PatientProfile.firstname
        ELSE _PatientProfile.firstname || ' ' || _PatientProfile.lastname
    END::TEXT
WHEN 'appointmentmode' THEN OPDForm.appointmentmode::TEXT
WHEN 'task' THEN __Task.taskname::TEXT
WHEN 'preferreddoctor' THEN
    CASE
        WHEN ___People.lastname IS NULL OR ___People.lastname = ''
        THEN ___People.firstname
        ELSE ___People.firstname || ' ' || ___People.lastname
    END::TEXT
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
WHEN 'patientname' THEN
    CASE
        WHEN _PatientProfile.lastname IS NULL OR _PatientProfile.lastname = ''
        THEN _PatientProfile.firstname
        ELSE _PatientProfile.firstname || ' ' || _PatientProfile.lastname
    END::TEXT
WHEN 'appointmentmode' THEN OPDForm.appointmentmode::TEXT
WHEN 'task' THEN __Task.taskname::TEXT
WHEN 'preferreddoctor' THEN
    CASE
        WHEN ___People.lastname IS NULL OR ___People.lastname = ''
        THEN ___People.firstname
        ELSE ___People.firstname || ' ' || ___People.lastname
    END::TEXT
WHEN 'verifiedstatus' THEN OPDForm.verifiedstatus::TEXT

                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
                    ,OPDForm.createddate DESC
                    ,OPDForm.OPDFormid DESC

                    limit pvar_pagesize
                    offset pvar_pagenumber * pvar_pagesize

                    ) d));



			  END

$BODY$;

