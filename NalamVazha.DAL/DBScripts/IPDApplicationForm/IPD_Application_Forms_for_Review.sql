CREATE OR REPLACE FUNCTION public."IPD_Application_Forms_for_Review"(
	pvar_tenantid character varying,
	pvar_patientname character varying,
	pvar_bookingstatus character varying,
	pvar_verifiedstatus character varying,
	pvar_pagesize integer,
	pvar_pagenumber integer,
	pvar_searchterm character varying,
	pvar_sort_fields json,
	pvar_createddate_automatonfrom character varying,
	pvar_createddate_automatonto character varying)
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:01*/
			  		
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
                    FROM  IPDApplicationForm 
 LEFT OUTER JOIN tenant ON IPDApplicationForm.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON IPDApplicationForm.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN Country __Country ON IPDApplicationForm.countryoforigin=__Country.Countryid
LEFT OUTER JOIN Country ___Country ON IPDApplicationForm.passportissuingcountry=___Country.Countryid
LEFT OUTER JOIN Country ____Country ON IPDApplicationForm.visaissuedcountry=____Country.Countryid
INNER JOIN PatientConsent _____PatientConsent ON IPDApplicationForm.consentform=_____PatientConsent.PatientConsentid
LEFT OUTER JOIN TreatmentPackage ______TreatmentPackage ON IPDApplicationForm.packagename=______TreatmentPackage.TreatmentPackageid

                    WHERE (lvar_tenantid is null or COALESCE(cast(IPDApplicationForm.tenantid as varchar), '') = Any(lvar_tenantid)) AND IPDApplicationForm.isdeleted=false
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(IPDApplicationForm.patientname as VARCHAR)=pvar_patientname)
AND (pvar_bookingstatus is null or pvar_bookingstatus ='0' or LENGTH(CAST(pvar_bookingstatus as Varchar))=0 or CAST(IPDApplicationForm.bookingstatus as VARCHAR)=pvar_bookingstatus)
AND (pvar_verifiedstatus is null or pvar_verifiedstatus ='0' or LENGTH(CAST(pvar_verifiedstatus as Varchar))=0 or CAST(IPDApplicationForm.verifiedstatus as VARCHAR)=pvar_verifiedstatus)

AND (
    pvar_createddate_automatonfrom IS NULL
    OR LENGTH(TRIM(pvar_createddate_automatonfrom)) = 0
    OR IPDApplicationForm.createddate >= pvar_createddate_automatonfrom::timestamp
)

AND (
    pvar_createddate_automatonto IS NULL
    OR LENGTH(TRIM(pvar_createddate_automatonto)) = 0
    OR IPDApplicationForm.createddate < pvar_createddate_automatonto::timestamp
)

AND IPDApplicationForm.verifiedstatus !='Revised'
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.bookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''))), '') AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.firstname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.gender AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.mobilenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.whatsappnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.nationality AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__Country.countryname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.generalcondition AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.bookingstatus AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.groupbooking AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.areyouthegroupleader AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.numberofmember AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.groupleadersbookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.paddressline1 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.paddressline2 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.ppincode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.ptown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.pcityordistrict AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.pstatename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.sameaspermanentaddress AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.caddressline1 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.caddressline2 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.cpincode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.ctown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.ccityordistrict AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.cstatename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.flexiblewithdates AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.flexiblewithroomtype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.joinwaitinglist AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.passportnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___Country.countryname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(IPDApplicationForm.passportexpirydate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.uploadpassportcopy AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.visatype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.visanumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____Country.countryname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(IPDApplicationForm.visaissuedate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(IPDApplicationForm.visaexpirydate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.uploadvisacopy AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.doyourequireahospitalprovidedattendant AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.preferredduration AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.admissionreason AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_____PatientConsent.consenttype||' '||_____PatientConsent.consentlanguage AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.consentfile AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.agreefortermsandconditions AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.signature AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(______TreatmentPackage.packagename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.isbookingdepositmandatory AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.verifiedstatus AS VARCHAR) ilike pvar_searchterm)
OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(IPDApplicationForm.estimatedarrival,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.travelarrangement AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.typeoftravelrequired AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.pickupfrom AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.requiredparkingspace AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.wheelchairassistance AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.requireddinner AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.specialrequest AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    IPDApplicationForm.tenantid
,tenant.businessname as _tenantName
,IPDApplicationForm.IPDApplicationFormid
,IPDApplicationForm.bookingreferencenumber
,IPDApplicationForm.patientname
,COALESCE(_PatientProfile.blacklisted,'No') as blacklisted
,COALESCE(PatientCategory.patientcategoryname,'Not Categorized') as patientcategoryname
,CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''))), '') AS VARCHAR) as patientname_master
,_PatientProfile.age
,_PatientProfile.ageinmonths
,IPDApplicationForm.firstname
,IPDApplicationForm.lastname
,IPDApplicationForm.gender
,IPDApplicationForm.mobilenumber
,IPDApplicationForm.whatsappnumber
,IPDApplicationForm.nationality
,IPDApplicationForm.countryoforigin
,CAST(__Country.countryname AS VARCHAR) as countryoforigin_master
,IPDApplicationForm.generalcondition
,IPDApplicationForm.bookingstatus
-- Room status must reflect whether a room has actually been allotted (a live row in
-- IPDApplicationForm_room), not just guess it from bookingstatus alone. Previously
-- 'Pending' always showed "Room Blocked" even with zero rooms allotted, and statuses
-- like 'Admission Approved' (which don't map to any WHEN) fell through to '-' even
-- when a room WAS already allotted.
,CASE
    WHEN LOWER(BTRIM(COALESCE(IPDApplicationForm.bookingstatus, ''))) = 'provisional booking'
        THEN 'Room Blocked'
    WHEN LOWER(BTRIM(COALESCE(IPDApplicationForm.bookingstatus, ''))) IN (
        'provisional confirmed', 'assessment form - in draft',
        'assessment form - review pending', 'assessment form reviewed',
        'screening scheduled', 'admission approved', 'arrival confirmed',
        'consultation scheduled', 'admission confirmed'
    ) THEN 'Room Booked'
    WHEN LOWER(BTRIM(COALESCE(IPDApplicationForm.bookingstatus, ''))) = 'admitted'
        THEN 'Room Occupied'
    WHEN LOWER(BTRIM(COALESCE(IPDApplicationForm.bookingstatus, ''))) = 'discharged'
        THEN 'Checked Out'
    WHEN LOWER(BTRIM(COALESCE(IPDApplicationForm.bookingstatus, ''))) IN (
        'cancelled - refunded', 'cancellation requested',
        'cancellation approved - refunded', 'cancellation requested - refunded',
        'cancelled by patient', 'cancelled by front desk',
        'cancelled', 'cancelled - initiated'
    ) THEN 'Released'
    ELSE '-'
END AS ipdroomstatus

,CASE
    WHEN public."Get_IPD_Remaining_Overpayment_Amount"(IPDApplicationForm.IPDApplicationFormid) > 0.009
        THEN 'Over Paid'
    ELSE COALESCE((
    SELECT 
        CASE 
            WHEN COALESCE(SUM(COALESCE(r.amount, 0)), 0) = 0 THEN 'Not Paid'
            WHEN COALESCE(SUM(COALESCE(r.paidamount, 0)), 0) = 0 THEN 'Not Paid'
            WHEN COALESCE(SUM(COALESCE(r.paidamount, 0)), 0) = COALESCE(SUM(COALESCE(r.amount, 0)), 0) THEN 'Paid'
            WHEN COALESCE(SUM(COALESCE(r.paidamount, 0)), 0) > COALESCE(SUM(COALESCE(r.amount, 0)), 0) THEN 'Over Paid'
            WHEN COALESCE(SUM(COALESCE(r.paidamount, 0)), 0) < COALESCE(SUM(COALESCE(r.amount, 0)), 0) THEN 'Partially Paid'
            ELSE 'Not Paid'
        END
    FROM receivable r
    WHERE r.ipdnumber = IPDApplicationForm.IPDApplicationFormid
      AND COALESCE(r.isdeleted, false) = false
    ), '-')
END AS ipdfinancialstatus

,IPDApplicationForm.groupbooking
,IPDApplicationForm.areyouthegroupleader
,IPDApplicationForm.numberofmember
,IPDApplicationForm.groupleadersbookingreferencenumber
,IPDApplicationForm.paddressline1
,IPDApplicationForm.paddressline2
,IPDApplicationForm.ppincode
,IPDApplicationForm.ptown
,IPDApplicationForm.pcityordistrict
,IPDApplicationForm.pstatename
,IPDApplicationForm.sameaspermanentaddress
,IPDApplicationForm.caddressline1
,IPDApplicationForm.caddressline2
,IPDApplicationForm.cpincode
,IPDApplicationForm.ctown
,IPDApplicationForm.ccityordistrict
,IPDApplicationForm.cstatename
,IPDApplicationForm.flexiblewithdates
,IPDApplicationForm.flexiblewithroomtype
,IPDApplicationForm.joinwaitinglist
,IPDApplicationForm.passportnumber
,IPDApplicationForm.passportissuingcountry
,CAST(___Country.countryname AS VARCHAR) as passportissuingcountry_master
,CAST(COALESCE(to_char(IPDApplicationForm.passportexpirydate,'dd/MM/yyyy'),'') AS Varchar) as passportexpirydate
,IPDApplicationForm.uploadpassportcopy
,IPDApplicationForm.visatype
,IPDApplicationForm.visanumber
,IPDApplicationForm.visaissuedcountry
,CAST(____Country.countryname AS VARCHAR) as visaissuedcountry_master
,CAST(COALESCE(to_char(IPDApplicationForm.visaissuedate,'dd/MM/yyyy'),'') AS Varchar) as visaissuedate
,CAST(COALESCE(to_char(IPDApplicationForm.visaexpirydate,'dd/MM/yyyy'),'') AS Varchar) as visaexpirydate
,IPDApplicationForm.uploadvisacopy
,IPDApplicationForm.doyourequireahospitalprovidedattendant
,IPDApplicationForm.preferredduration
,IPDApplicationForm.admissionreason
,IPDApplicationForm.consentform
,CAST(_____PatientConsent.consenttype||' '||_____PatientConsent.consentlanguage AS VARCHAR) as consentform_master
,IPDApplicationForm.consentfile
,IPDApplicationForm.agreefortermsandconditions
 
,IPDApplicationForm.packagename
,CAST(______TreatmentPackage.packagename AS VARCHAR) as packagename_master
,IPDApplicationForm.isbookingdepositmandatory
,IPDApplicationForm.verifiedstatus
,CAST(COALESCE(to_char(IPDApplicationForm.estimatedarrival,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as estimatedarrival
,IPDApplicationForm.travelarrangement
,IPDApplicationForm.typeoftravelrequired
,IPDApplicationForm.pickupfrom
,IPDApplicationForm.requiredparkingspace
,IPDApplicationForm.wheelchairassistance
,IPDApplicationForm.requireddinner
,IPDApplicationForm.specialrequest
,CAST(COALESCE(to_char(IPDApplicationForm.createddate,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as createddate
,CAST(COALESCE(to_char(IPDApplicationForm.modifieddate,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as modifieddate
                    ,
               (SELECT json_agg(J) FROM (
    SELECT
        to_char(COALESCE(r.minfromdate, pda.dateofarrival), 'dd/MM/yyyy')   AS "Date of Arrival",
        to_char(COALESCE(r.maxtodate,   pda.dateofdeparture), 'dd/MM/yyyy') AS "Date of Departure",
        CASE
            WHEN r.minfromdate IS NOT NULL AND r.maxtodate IS NOT NULL
                THEN ((r.maxtodate::date - r.minfromdate::date) + 1)
            ELSE pda.daysofstay
        END AS "Days of Stay"
    FROM IPDApplicationForm_preferreddatesofadmission pda
    LEFT JOIN (
        SELECT
            IPDApplicationFormid,
            MIN(fromdate) AS minfromdate,
            MAX(todate)   AS maxtodate
        FROM IPDApplicationForm_room
        WHERE COALESCE(isdeleted, false) = false
          AND EXISTS (
                SELECT 1
                FROM unnest(string_to_array(allottedto, ',')) AS role
                WHERE trim(role) = 'Patient'
          )
        GROUP BY IPDApplicationFormid
    ) r ON r.IPDApplicationFormid = pda.IPDApplicationFormid
    WHERE IPDApplicationForm.IPDApplicationFormid = pda.IPDApplicationFormid
      AND COALESCE(pda.isdeleted, false) = false
) J ) AS automaton_IPDApplicationForm_preferreddatesofadmission
,
                (SELECT json_agg(J) FROM (SELECT   
				 CAST(_MedicalCondition.conditionname AS VARCHAR) as "Medical Condition Name"
,IPDApplicationForm_medicalinfo.duration as "Duration"
,IPDApplicationForm_medicalinfo.unit as "Unit"
,IPDApplicationForm_medicalinfo.severitylevel as "Severity Level"

		 	   FROM  IPDApplicationForm_medicalinfo 
INNER JOIN MedicalCondition _MedicalCondition ON IPDApplicationForm_medicalinfo.medicalconditionname=_MedicalCondition.MedicalConditionid

			  WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_medicalinfo.IPDApplicationFormid
AND COALESCE(IPDApplicationForm_medicalinfo.isdeleted,false) = false 
) J)
			    as automaton_IPDApplicationForm_medicalinfo
,
                (SELECT json_agg(J) FROM (SELECT   
				 IPDApplicationForm_medicationinfo.medicinename as "Medicine Name"
,IPDApplicationForm_medicationinfo.frequencyinaday as "Frequency in a day"
,IPDApplicationForm_medicationinfo.medicationduration as "Medication Duration"
,IPDApplicationForm_medicationinfo.quantity as "Quantity"

		 	   FROM  IPDApplicationForm_medicationinfo 

			  WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_medicationinfo.IPDApplicationFormid
AND COALESCE(IPDApplicationForm_medicationinfo.isdeleted,false) = false 
) J)
			    as automaton_IPDApplicationForm_medicationinfo
,
                (SELECT json_agg(J) FROM (SELECT   
				 IPDApplicationForm_medicalrecords.medicalrecordname as "Medical Record Name"
,IPDApplicationForm_medicalrecords.medicalrecordfile as "Medical Record File"

		 	   FROM  IPDApplicationForm_medicalrecords 

			  WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_medicalrecords.IPDApplicationFormid
AND COALESCE(IPDApplicationForm_medicalrecords.isdeleted,false) = false 
) J)
			    as automaton_IPDApplicationForm_medicalrecords
,
                (SELECT json_agg(J) FROM (SELECT   
				 IPDApplicationForm_attendantinfo.attendantname as "Attendant Name"
,IPDApplicationForm_attendantinfo.age as "Age"
,IPDApplicationForm_attendantinfo.gender as "Gender"
,IPDApplicationForm_attendantinfo.phonenumber as "Phone Number"

		 	   FROM  IPDApplicationForm_attendantinfo 

			  WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_attendantinfo.IPDApplicationFormid
AND COALESCE(IPDApplicationForm_attendantinfo.isdeleted,false) = false 
) J)
			    as automaton_IPDApplicationForm_attendantinfo
,
                (SELECT json_agg(J) FROM (SELECT   
				 CAST(_RoomType.name AS VARCHAR) as "Room Type"

		 	   FROM  IPDApplicationForm_roompreference 
INNER JOIN RoomType _RoomType ON IPDApplicationForm_roompreference.roomtype=_RoomType.RoomTypeid

			  WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_roompreference.IPDApplicationFormid
AND COALESCE(IPDApplicationForm_roompreference.isdeleted,false) = false 
) J)
			    as automaton_IPDApplicationForm_roompreference
,
                (SELECT json_agg(J) FROM (SELECT   
				 IPDApplicationForm_room.allottedto as "Allotted to"
,CAST(_Room.roomnumber AS VARCHAR) as "Room Number"
,to_char(IPDApplicationForm_room.fromdate, 'dd/MM/yyyy HH24:MI') as "From Date"
,to_char(IPDApplicationForm_room.todate, 'dd/MM/yyyy HH24:MI') as "To Date"

		 	   FROM  IPDApplicationForm_room 
LEFT OUTER JOIN Room _Room ON IPDApplicationForm_room.roomnumber=_Room.Roomid

			  WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_room.IPDApplicationFormid
AND COALESCE(IPDApplicationForm_room.isdeleted,false) = false 
) J)
			    as automaton_IPDApplicationForm_room

,COALESCE((Select billingpayment.billingpaymentid::varchar from billingpayment where billingpayment.ipdnumber=IPDApplicationForm.IPDApplicationFormid and billingpayment.receivablefor IN ('IPD Booking Deposit','IPD Booking Deposit - Patient') and billingpayment.paymentstatus !='Success'),'') as billingpaymentid
                    ,IPDApplicationForm.createduser,IPDApplicationForm.createddate,IPDApplicationForm.modifieduser,IPDApplicationForm.modifieddate
                    
					 ,(Select Assessmentid from Assessment where ipdform=IPDApplicationForm.IPDApplicationFormid and
				   isdeleted=false  LIMIT 1) as screeningsssessment

				   ,COALESCE((
    -- Sum the raw (amount - paidamount) difference per row -- do NOT floor each row at
    -- zero first. A discount/credit row (e.g. amount = -100) must be allowed to net
    -- against the other charges, matching Get_Unified_Pending_Receivables_IPD (the
    -- function behind the Payment Collection page's "Pending Amounts" table), which
    -- computes balance = amount - paidamount with no per-row clamp. Only the final
    -- total is floored at zero, since a facility-owes-patient balance isn't a
    -- "pending amount".
    SELECT GREATEST(SUM(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0)), 0)
    FROM receivable r
    LEFT JOIN receivableconditions rc
        ON LOWER(TRIM(rc.receivablefor)) = LOWER(TRIM(r.receivablefor))
       AND COALESCE(rc.isdeleted, false) = false
    WHERE r.ipdnumber = IPDApplicationForm.IPDApplicationFormid
      AND COALESCE(r.isdeleted, false) = false
      AND COALESCE(r.paymentstatus, '') <> 'Paid'
      AND COALESCE(rc.ismandatory, false) = true
      AND ABS(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0)) > 0.009
), 0) AS requiredpendingpayment

,COALESCE((
    SELECT SUM(r.amount - COALESCE(r.paidamount, 0))
    FROM receivable r
    WHERE r.ipdnumber = IPDApplicationForm.IPDApplicationFormid
      AND COALESCE(r.isdeleted, false) = false
), 0) AS totalpendingamount
,public."Get_IPD_Remaining_Overpayment_Amount"(IPDApplicationForm.IPDApplicationFormid) AS remainingoverpaymentamount
					
					FROM  IPDApplicationForm 
 LEFT OUTER JOIN tenant ON IPDApplicationForm.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON IPDApplicationForm.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN Country __Country ON IPDApplicationForm.countryoforigin=__Country.Countryid
LEFT OUTER JOIN Country ___Country ON IPDApplicationForm.passportissuingcountry=___Country.Countryid
LEFT OUTER JOIN Country ____Country ON IPDApplicationForm.visaissuedcountry=____Country.Countryid
INNER JOIN PatientConsent _____PatientConsent ON IPDApplicationForm.consentform=_____PatientConsent.PatientConsentid
LEFT OUTER JOIN TreatmentPackage ______TreatmentPackage ON IPDApplicationForm.packagename=______TreatmentPackage.TreatmentPackageid
LEFT OUTER JOIN PatientCategory PatientCategory ON _PatientProfile.PatientCategory=PatientCategory.PatientCategoryid

                 WHERE (lvar_tenantid is null or COALESCE(cast(IPDApplicationForm.tenantid as varchar), '') = Any(lvar_tenantid)) AND IPDApplicationForm.isdeleted=false
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(IPDApplicationForm.patientname as VARCHAR)=pvar_patientname)
AND (pvar_bookingstatus is null or pvar_bookingstatus ='0' or LENGTH(CAST(pvar_bookingstatus as Varchar))=0 or CAST(IPDApplicationForm.bookingstatus as VARCHAR)=pvar_bookingstatus)
AND (pvar_verifiedstatus is null or pvar_verifiedstatus ='0' or LENGTH(CAST(pvar_verifiedstatus as Varchar))=0 or CAST(IPDApplicationForm.verifiedstatus as VARCHAR)=pvar_verifiedstatus)
 AND IPDApplicationForm.verifiedstatus !='Revised'
AND (
    pvar_createddate_automatonfrom IS NULL
    OR LENGTH(TRIM(pvar_createddate_automatonfrom)) = 0
    OR IPDApplicationForm.createddate >= pvar_createddate_automatonfrom::timestamp
)

AND (
    pvar_createddate_automatonto IS NULL
    OR LENGTH(TRIM(pvar_createddate_automatonto)) = 0
    OR IPDApplicationForm.createddate < pvar_createddate_automatonto::timestamp
)
                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.bookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''))), '') AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.firstname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.gender AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.mobilenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.whatsappnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.nationality AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__Country.countryname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.generalcondition AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.bookingstatus AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.groupbooking AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.areyouthegroupleader AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.numberofmember AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.groupleadersbookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.paddressline1 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.paddressline2 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.ppincode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.ptown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.pcityordistrict AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.pstatename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.sameaspermanentaddress AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.caddressline1 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.caddressline2 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.cpincode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.ctown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.ccityordistrict AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.cstatename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.flexiblewithdates AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.flexiblewithroomtype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.joinwaitinglist AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.passportnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___Country.countryname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(IPDApplicationForm.passportexpirydate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.uploadpassportcopy AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.visatype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.visanumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____Country.countryname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(IPDApplicationForm.visaissuedate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(IPDApplicationForm.visaexpirydate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.uploadvisacopy AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.doyourequireahospitalprovidedattendant AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.preferredduration AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.admissionreason AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_____PatientConsent.consenttype||' '||_____PatientConsent.consentlanguage AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.consentfile AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.agreefortermsandconditions AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.signature AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(______TreatmentPackage.packagename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.isbookingdepositmandatory AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.verifiedstatus AS VARCHAR) ilike pvar_searchterm)
OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(IPDApplicationForm.estimatedarrival,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.travelarrangement AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.typeoftravelrequired AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.pickupfrom AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.requiredparkingspace AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.wheelchairassistance AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.requireddinner AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(IPDApplicationForm.specialrequest AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'bookingreferencenumber' THEN IPDApplicationForm.bookingreferencenumber::TEXT
WHEN 'bookingreferencenumber' THEN IPDApplicationForm.bookingreferencenumber::TEXT
WHEN 'patientname' THEN NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''))), '')::TEXT
WHEN 'firstname' THEN IPDApplicationForm.firstname::TEXT
WHEN 'lastname' THEN IPDApplicationForm.lastname::TEXT
WHEN 'gender' THEN IPDApplicationForm.gender::TEXT
WHEN 'mobilenumber' THEN IPDApplicationForm.mobilenumber::TEXT
WHEN 'mobilenumber' THEN IPDApplicationForm.mobilenumber::TEXT
WHEN 'whatsappnumber' THEN IPDApplicationForm.whatsappnumber::TEXT
WHEN 'whatsappnumber' THEN IPDApplicationForm.whatsappnumber::TEXT
WHEN 'nationality' THEN IPDApplicationForm.nationality::TEXT
WHEN 'countryoforigin' THEN __Country.countryname::TEXT
WHEN 'generalcondition' THEN IPDApplicationForm.generalcondition::TEXT
WHEN 'bookingstatus' THEN IPDApplicationForm.bookingstatus::TEXT
WHEN 'groupbooking' THEN IPDApplicationForm.groupbooking::TEXT
WHEN 'areyouthegroupleader' THEN IPDApplicationForm.areyouthegroupleader::TEXT
WHEN 'areyouthegroupleader' THEN IPDApplicationForm.areyouthegroupleader::TEXT
WHEN 'numberofmember' THEN IPDApplicationForm.numberofmember::TEXT
WHEN 'groupleadersbookingreferencenumber' THEN IPDApplicationForm.groupleadersbookingreferencenumber::TEXT
WHEN 'paddressline1' THEN IPDApplicationForm.paddressline1::TEXT
WHEN 'paddressline2' THEN IPDApplicationForm.paddressline2::TEXT
WHEN 'ppincode' THEN IPDApplicationForm.ppincode::TEXT
WHEN 'ptown' THEN IPDApplicationForm.ptown::TEXT
WHEN 'pcityordistrict' THEN IPDApplicationForm.pcityordistrict::TEXT
WHEN 'pstatename' THEN IPDApplicationForm.pstatename::TEXT
WHEN 'sameaspermanentaddress' THEN IPDApplicationForm.sameaspermanentaddress::TEXT
WHEN 'caddressline1' THEN IPDApplicationForm.caddressline1::TEXT
WHEN 'caddressline2' THEN IPDApplicationForm.caddressline2::TEXT
WHEN 'cpincode' THEN IPDApplicationForm.cpincode::TEXT
WHEN 'ctown' THEN IPDApplicationForm.ctown::TEXT
WHEN 'ccityordistrict' THEN IPDApplicationForm.ccityordistrict::TEXT
WHEN 'cstatename' THEN IPDApplicationForm.cstatename::TEXT
WHEN 'flexiblewithdates' THEN IPDApplicationForm.flexiblewithdates::TEXT
WHEN 'flexiblewithroomtype' THEN IPDApplicationForm.flexiblewithroomtype::TEXT
WHEN 'joinwaitinglist' THEN IPDApplicationForm.joinwaitinglist::TEXT
WHEN 'passportnumber' THEN IPDApplicationForm.passportnumber::TEXT
WHEN 'passportissuingcountry' THEN ___Country.countryname::TEXT
WHEN 'uploadpassportcopy' THEN IPDApplicationForm.uploadpassportcopy::TEXT
WHEN 'uploadpassportcopy' THEN IPDApplicationForm.uploadpassportcopy::TEXT
WHEN 'visatype' THEN IPDApplicationForm.visatype::TEXT
WHEN 'visanumber' THEN IPDApplicationForm.visanumber::TEXT
WHEN 'visaissuedcountry' THEN ____Country.countryname::TEXT
WHEN 'uploadvisacopy' THEN IPDApplicationForm.uploadvisacopy::TEXT
WHEN 'uploadvisacopy' THEN IPDApplicationForm.uploadvisacopy::TEXT
WHEN 'doyourequireahospitalprovidedattendant' THEN IPDApplicationForm.doyourequireahospitalprovidedattendant::TEXT
WHEN 'preferredduration' THEN IPDApplicationForm.preferredduration::TEXT
WHEN 'admissionreason' THEN IPDApplicationForm.admissionreason::TEXT
WHEN 'admissionreason' THEN IPDApplicationForm.admissionreason::TEXT
WHEN 'consentform' THEN _____PatientConsent.consenttype||' '||_____PatientConsent.consentlanguage::TEXT
WHEN 'consentfile' THEN IPDApplicationForm.consentfile::TEXT
WHEN 'consentfile' THEN IPDApplicationForm.consentfile::TEXT
WHEN 'agreefortermsandconditions' THEN IPDApplicationForm.agreefortermsandconditions::TEXT
WHEN 'signature' THEN IPDApplicationForm.signature::TEXT
WHEN 'packagename' THEN ______TreatmentPackage.packagename::TEXT
WHEN 'isbookingdepositmandatory' THEN IPDApplicationForm.isbookingdepositmandatory::TEXT
WHEN 'verifiedstatus' THEN IPDApplicationForm.verifiedstatus::TEXT
	WHEN 'travelarrangement' THEN IPDApplicationForm.travelarrangement::TEXT
WHEN 'typeoftravelrequired' THEN IPDApplicationForm.typeoftravelrequired::TEXT
WHEN 'pickupfrom' THEN IPDApplicationForm.pickupfrom::TEXT
WHEN 'requiredparkingspace' THEN IPDApplicationForm.requiredparkingspace::TEXT
WHEN 'wheelchairassistance' THEN IPDApplicationForm.wheelchairassistance::TEXT
WHEN 'requireddinner' THEN IPDApplicationForm.requireddinner::TEXT
WHEN 'specialrequest' THEN IPDApplicationForm.specialrequest::TEXT
	
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'passportexpirydate' THEN IPDApplicationForm.passportexpirydate
WHEN 'visaissuedate' THEN IPDApplicationForm.visaissuedate
WHEN 'visaexpirydate' THEN IPDApplicationForm.visaexpirydate
		  ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'estimatedarrival' THEN IPDApplicationForm.estimatedarrival

                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'sameaspermanentaddress' THEN IPDApplicationForm.sameaspermanentaddress
WHEN 'agreefortermsandconditions' THEN IPDApplicationForm.agreefortermsandconditions
WHEN 'isbookingdepositmandatory' THEN IPDApplicationForm.isbookingdepositmandatory
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'numberofmember' THEN IPDApplicationForm.numberofmember::NUMERIC
WHEN 'ppincode' THEN IPDApplicationForm.ppincode::NUMERIC
WHEN 'cpincode' THEN IPDApplicationForm.cpincode::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'bookingreferencenumber' THEN IPDApplicationForm.bookingreferencenumber::TEXT
WHEN 'bookingreferencenumber' THEN IPDApplicationForm.bookingreferencenumber::TEXT
WHEN 'patientname' THEN NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''))), '')::TEXT
WHEN 'firstname' THEN IPDApplicationForm.firstname::TEXT
WHEN 'lastname' THEN IPDApplicationForm.lastname::TEXT
WHEN 'gender' THEN IPDApplicationForm.gender::TEXT
WHEN 'mobilenumber' THEN IPDApplicationForm.mobilenumber::TEXT
WHEN 'mobilenumber' THEN IPDApplicationForm.mobilenumber::TEXT
WHEN 'whatsappnumber' THEN IPDApplicationForm.whatsappnumber::TEXT
WHEN 'whatsappnumber' THEN IPDApplicationForm.whatsappnumber::TEXT
WHEN 'nationality' THEN IPDApplicationForm.nationality::TEXT
WHEN 'countryoforigin' THEN __Country.countryname::TEXT
WHEN 'generalcondition' THEN IPDApplicationForm.generalcondition::TEXT
WHEN 'bookingstatus' THEN IPDApplicationForm.bookingstatus::TEXT
WHEN 'groupbooking' THEN IPDApplicationForm.groupbooking::TEXT
WHEN 'areyouthegroupleader' THEN IPDApplicationForm.areyouthegroupleader::TEXT
WHEN 'areyouthegroupleader' THEN IPDApplicationForm.areyouthegroupleader::TEXT
WHEN 'numberofmember' THEN IPDApplicationForm.numberofmember::TEXT
WHEN 'groupleadersbookingreferencenumber' THEN IPDApplicationForm.groupleadersbookingreferencenumber::TEXT
WHEN 'paddressline1' THEN IPDApplicationForm.paddressline1::TEXT
WHEN 'paddressline2' THEN IPDApplicationForm.paddressline2::TEXT
WHEN 'ppincode' THEN IPDApplicationForm.ppincode::TEXT
WHEN 'ptown' THEN IPDApplicationForm.ptown::TEXT
WHEN 'pcityordistrict' THEN IPDApplicationForm.pcityordistrict::TEXT
WHEN 'pstatename' THEN IPDApplicationForm.pstatename::TEXT
WHEN 'sameaspermanentaddress' THEN IPDApplicationForm.sameaspermanentaddress::TEXT
WHEN 'caddressline1' THEN IPDApplicationForm.caddressline1::TEXT
WHEN 'caddressline2' THEN IPDApplicationForm.caddressline2::TEXT
WHEN 'cpincode' THEN IPDApplicationForm.cpincode::TEXT
WHEN 'ctown' THEN IPDApplicationForm.ctown::TEXT
WHEN 'ccityordistrict' THEN IPDApplicationForm.ccityordistrict::TEXT
WHEN 'cstatename' THEN IPDApplicationForm.cstatename::TEXT
WHEN 'flexiblewithdates' THEN IPDApplicationForm.flexiblewithdates::TEXT
WHEN 'flexiblewithroomtype' THEN IPDApplicationForm.flexiblewithroomtype::TEXT
WHEN 'joinwaitinglist' THEN IPDApplicationForm.joinwaitinglist::TEXT
WHEN 'passportnumber' THEN IPDApplicationForm.passportnumber::TEXT
WHEN 'passportissuingcountry' THEN ___Country.countryname::TEXT
WHEN 'uploadpassportcopy' THEN IPDApplicationForm.uploadpassportcopy::TEXT
WHEN 'uploadpassportcopy' THEN IPDApplicationForm.uploadpassportcopy::TEXT
WHEN 'visatype' THEN IPDApplicationForm.visatype::TEXT
WHEN 'visanumber' THEN IPDApplicationForm.visanumber::TEXT
WHEN 'visaissuedcountry' THEN ____Country.countryname::TEXT
WHEN 'uploadvisacopy' THEN IPDApplicationForm.uploadvisacopy::TEXT
WHEN 'uploadvisacopy' THEN IPDApplicationForm.uploadvisacopy::TEXT
WHEN 'doyourequireahospitalprovidedattendant' THEN IPDApplicationForm.doyourequireahospitalprovidedattendant::TEXT
WHEN 'preferredduration' THEN IPDApplicationForm.preferredduration::TEXT
WHEN 'admissionreason' THEN IPDApplicationForm.admissionreason::TEXT
WHEN 'admissionreason' THEN IPDApplicationForm.admissionreason::TEXT
WHEN 'consentform' THEN _____PatientConsent.consenttype||' '||_____PatientConsent.consentlanguage::TEXT
WHEN 'consentfile' THEN IPDApplicationForm.consentfile::TEXT
WHEN 'consentfile' THEN IPDApplicationForm.consentfile::TEXT
WHEN 'agreefortermsandconditions' THEN IPDApplicationForm.agreefortermsandconditions::TEXT
WHEN 'signature' THEN IPDApplicationForm.signature::TEXT
WHEN 'packagename' THEN ______TreatmentPackage.packagename::TEXT
WHEN 'isbookingdepositmandatory' THEN IPDApplicationForm.isbookingdepositmandatory::TEXT
WHEN 'verifiedstatus' THEN IPDApplicationForm.verifiedstatus::TEXT
			WHEN 'travelarrangement' THEN IPDApplicationForm.travelarrangement::TEXT
WHEN 'typeoftravelrequired' THEN IPDApplicationForm.typeoftravelrequired::TEXT
WHEN 'pickupfrom' THEN IPDApplicationForm.pickupfrom::TEXT
WHEN 'requiredparkingspace' THEN IPDApplicationForm.requiredparkingspace::TEXT
WHEN 'wheelchairassistance' THEN IPDApplicationForm.wheelchairassistance::TEXT
WHEN 'requireddinner' THEN IPDApplicationForm.requireddinner::TEXT
WHEN 'specialrequest' THEN IPDApplicationForm.specialrequest::TEXT

                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'passportexpirydate' THEN IPDApplicationForm.passportexpirydate
WHEN 'visaissuedate' THEN IPDApplicationForm.visaissuedate
WHEN 'visaexpirydate' THEN IPDApplicationForm.visaexpirydate
			   ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'estimatedarrival' THEN IPDApplicationForm.estimatedarrival

                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'sameaspermanentaddress' THEN IPDApplicationForm.sameaspermanentaddress
WHEN 'agreefortermsandconditions' THEN IPDApplicationForm.agreefortermsandconditions
WHEN 'isbookingdepositmandatory' THEN IPDApplicationForm.isbookingdepositmandatory
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'numberofmember' THEN IPDApplicationForm.numberofmember::NUMERIC
WHEN 'ppincode' THEN IPDApplicationForm.ppincode::NUMERIC
WHEN 'cpincode' THEN IPDApplicationForm.cpincode::NUMERIC
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
                    ,
                    CASE
                    WHEN COALESCE(array_length(local_sortcolumn_array, 1), 0) = 0
                      OR local_sortcolumn_array[1] IS NULL
                      OR local_sortorder_array[1] IS NULL
                    THEN IPDApplicationForm.createddate
                    ELSE NULL
                    END DESC

                    limit pvar_pagesize
                    offset pvar_pagenumber * pvar_pagesize			 	
			 	
                    ) d));
	
			  
					 	
			  END
              
$BODY$;


