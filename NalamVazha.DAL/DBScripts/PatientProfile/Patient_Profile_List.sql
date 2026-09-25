
CREATE OR REPLACE FUNCTION public."Patient_Profile_List"(
	pvar_tenantid character varying,
	pvar_firstname character varying,
	pvar_dateofbirth_automatonfrom character varying,
	pvar_dateofbirth_automatonto character varying,
	pvar_emailaddress character varying,
	pvar_mobilenumber character varying,
	pvar_whatsappnumber character varying,
	pvar_idprooftype character varying,
	pvar_idproofnumber character varying,
	pvar_pzip character varying,
	pvar_blacklisted character varying,
	pvar_deceased character varying,
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 02/20/2026 10:34:25*/
			  		
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
                    FROM  PatientProfile 
 LEFT OUTER JOIN tenant ON PatientProfile.tenantid=tenant.tenantid
LEFT OUTER JOIN Country _Country ON PatientProfile.countryoforigin=_Country.Countryid
LEFT OUTER JOIN Occupation __Occupation ON PatientProfile.occupation=__Occupation.Occupationid
LEFT OUTER JOIN Blacklistreason ___Blacklistreason ON PatientProfile.reasonforblacklisting=___Blacklistreason.Blacklistreasonid
LEFT OUTER JOIN PatientCategory ____PatientCategory ON PatientProfile.PatientCategory=____PatientCategory.PatientCategoryid

                    WHERE (lvar_tenantid is null or COALESCE(cast(PatientProfile.tenantid as varchar), '') = Any(lvar_tenantid)) AND PatientProfile.isdeleted=false
--AND (pvar_firstname is null or pvar_firstname ='0' or LENGTH(CAST(pvar_firstname as Varchar))=0 or CAST(PatientProfile.firstname as VARCHAR)=pvar_firstname)
 AND (pvar_firstname is null or pvar_firstname ='0' or LENGTH(CAST(pvar_firstname as Varchar))=0 
     or CAST(PatientProfile.firstname as VARCHAR) ILIKE '%' || pvar_firstname || '%')
 AND(pvar_dateofbirth_automatonfrom IS NULL OR pvar_dateofbirth_automatonfrom = '' OR PatientProfile.dateofbirth >= CAST(pvar_dateofbirth_automatonfrom AS TIMESTAMP(3))) 
                                AND (pvar_dateofbirth_automatonto IS NULL OR pvar_dateofbirth_automatonto = '' OR PatientProfile.dateofbirth <= CAST(pvar_dateofbirth_automatonto AS TIMESTAMP(3)))
AND (pvar_emailaddress is null or pvar_emailaddress ='0' or LENGTH(CAST(pvar_emailaddress as Varchar))=0 or CAST(PatientProfile.emailaddress as VARCHAR)=pvar_emailaddress)
AND (pvar_mobilenumber is null or pvar_mobilenumber ='0' or LENGTH(CAST(pvar_mobilenumber as Varchar))=0 
     or CAST(PatientProfile.mobilenumber as VARCHAR) ILIKE '%' || pvar_mobilenumber || '%')
--AND (pvar_mobilenumber is null or pvar_mobilenumber ='0' or LENGTH(CAST(pvar_mobilenumber as Varchar))=0 or CAST(PatientProfile.mobilenumber as VARCHAR)=pvar_mobilenumber)
AND (pvar_whatsappnumber is null or pvar_whatsappnumber ='0' or LENGTH(CAST(pvar_whatsappnumber as Varchar))=0 or CAST(PatientProfile.whatsappnumber as VARCHAR)=pvar_whatsappnumber)
AND (pvar_idprooftype is null or pvar_idprooftype ='0' or LENGTH(CAST(pvar_idprooftype as Varchar))=0 or CAST(PatientProfile.idprooftype as VARCHAR)=pvar_idprooftype)
AND (pvar_idproofnumber is null or pvar_idproofnumber ='0' or LENGTH(CAST(pvar_idproofnumber as Varchar))=0 or CAST(PatientProfile.idproofnumber as VARCHAR)=pvar_idproofnumber)
AND (pvar_pzip is null or pvar_pzip ='0' or LENGTH(CAST(pvar_pzip as Varchar))=0 or CAST(PatientProfile.pzip as VARCHAR)=pvar_pzip)
AND (pvar_blacklisted is null or pvar_blacklisted ='0' or LENGTH(CAST(pvar_blacklisted as Varchar))=0 or CAST(PatientProfile.blacklisted as VARCHAR)=pvar_blacklisted)
AND (pvar_deceased is null or pvar_deceased ='false' or LENGTH(CAST(pvar_deceased as Varchar))=0 or CAST(PatientProfile.deceased as VARCHAR)=pvar_deceased)
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.registrationid AS VARCHAR) ilike pvar_searchterm)
 --OR ((pvar_searchterm is null) or CAST(PatientProfile.firstname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or (COALESCE(PatientProfile.firstname,'') || ' ' || COALESCE(PatientProfile.lastname,'')) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.gender AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(PatientProfile.dateofbirth,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.age AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.nationality AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_Country.countryname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.emailaddress AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.mobilenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.whatsappnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.photo AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.idprooftype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.idproofnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.uploadidproof AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.paddressline1 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.paddressline2 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.pzip AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.ptown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.pcityordistrict AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.ppstatename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.sameaspermanentaddress AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.caddressline1 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.caddressline2 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.czip AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.ctown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.ccityordistrict AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.cstatename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.languagesknown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.languagespreferrable AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.otherlanguages AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.maritalstatus AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.education AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__Occupation.occupationname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.meditationpractice AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.typeofpractice AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.creativeactivities AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.othercreativeactivities AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.insurancetype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.insurancecompany AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.policynumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.policyclaimlimit AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(PatientProfile.policyexpirydate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.referralsource AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.referrername AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.referrerphonenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.magazinename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.socialmediaplatform AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.otherreferral AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.blacklisted AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___Blacklistreason.reason AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.detailedremarks AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.deceased AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.causeofdeath AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(PatientProfile.dateandtimeofdeath,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    PatientProfile.tenantid
,tenant.businessname as _tenantName
,PatientProfile.PatientProfileid
,PatientProfile.registrationid
,PatientProfile.firstname
,PatientProfile.lastname
,PatientProfile.gender
,CAST(COALESCE(to_char(PatientProfile.dateofbirth,'dd/MM/yyyy'),'') AS Varchar) as dateofbirth
,PatientProfile.age
,PatientProfile.bloodgroup
,PatientProfile.nationality
,PatientProfile.countryoforigin
,CAST(_Country.countryname AS VARCHAR) as countryoforigin_master
,PatientProfile.emailaddress
,PatientProfile.mobilenumber
,PatientProfile.whatsappnumber
,PatientProfile.photo
,PatientProfile.idprooftype
,PatientProfile.idproofnumber
,PatientProfile.uploadidproof
,PatientProfile.paddressline1
,PatientProfile.paddressline2
,PatientProfile.pzip
,PatientProfile.ptown
,PatientProfile.pcityordistrict
,PatientProfile.ppstatename
,PatientProfile.sameaspermanentaddress
,PatientProfile.caddressline1
,PatientProfile.caddressline2
,PatientProfile.czip
,PatientProfile.ctown
,PatientProfile.ccityordistrict
,PatientProfile.cstatename
,PatientProfile.languagesknown
,PatientProfile.languagespreferrable
,PatientProfile.otherlanguages
,PatientProfile.maritalstatus
,PatientProfile.education
,PatientProfile.occupation
,CAST(__Occupation.occupationname AS VARCHAR) as occupation_master
,PatientProfile.meditationpractice
,PatientProfile.typeofpractice
,PatientProfile.creativeactivities
,PatientProfile.othercreativeactivities
,PatientProfile.insurancetype
,PatientProfile.insurancecompany
,PatientProfile.policynumber
,PatientProfile.policyclaimlimit
,CAST(COALESCE(to_char(PatientProfile.policyexpirydate,'dd/MM/yyyy'),'') AS Varchar) as policyexpirydate
,PatientProfile.referralsource
,PatientProfile.referrername
,PatientProfile.referrerphonenumber
,PatientProfile.magazinename
,PatientProfile.socialmediaplatform
,PatientProfile.otherreferral
,PatientProfile.blacklisted
,PatientProfile.reasonforblacklisting
,CAST(___Blacklistreason.reason AS VARCHAR) as reasonforblacklisting_master
,PatientProfile.detailedremarks
,PatientProfile.deceased
,PatientProfile.causeofdeath
,CAST(COALESCE(to_char(PatientProfile.dateandtimeofdeath,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as dateandtimeofdeath
,PatientProfile.PatientCategory as patientcategory
,COALESCE(____PatientCategory.patientcategoryname,'Not Categorized') as patientcategoryname

                    ,
                (SELECT json_agg(J) FROM (SELECT   
				 PatientProfile_emergencycontactinfo.personname as "Person Name"
,PatientProfile_emergencycontactinfo.relationship as "Relationship"
,PatientProfile_emergencycontactinfo.phonenumber as "Phone Number"

		 	   FROM  PatientProfile_emergencycontactinfo 

			  WHERE PatientProfile.PatientProfileid =PatientProfile_emergencycontactinfo.PatientProfileid
--AND COALESCE(PatientProfile_emergencycontactinfo.isdeleted,false) = false 
) J)
			    as automaton_PatientProfile_emergencycontactinfo

                    ,PatientProfile.createduser,PatientProfile.createddate,PatientProfile.modifieduser,PatientProfile.modifieddate
                    FROM  PatientProfile 
 LEFT OUTER JOIN tenant ON PatientProfile.tenantid=tenant.tenantid
LEFT OUTER JOIN Country _Country ON PatientProfile.countryoforigin=_Country.Countryid
LEFT OUTER JOIN Occupation __Occupation ON PatientProfile.occupation=__Occupation.Occupationid
LEFT OUTER JOIN Blacklistreason ___Blacklistreason ON PatientProfile.reasonforblacklisting=___Blacklistreason.Blacklistreasonid
LEFT OUTER JOIN PatientCategory ____PatientCategory ON PatientProfile.PatientCategory=____PatientCategory.PatientCategoryid

                    WHERE (lvar_tenantid is null or COALESCE(cast(PatientProfile.tenantid as varchar), '') = Any(lvar_tenantid)) AND PatientProfile.isdeleted=false
--AND (pvar_firstname is null or pvar_firstname ='0' or LENGTH(CAST(pvar_firstname as Varchar))=0 or CAST(PatientProfile.firstname as VARCHAR)=pvar_firstname)
 AND (pvar_firstname is null or pvar_firstname ='0' or LENGTH(CAST(pvar_firstname as Varchar))=0 
     or CAST(PatientProfile.firstname as VARCHAR) ILIKE '%' || pvar_firstname || '%')
 AND(pvar_dateofbirth_automatonfrom IS NULL OR pvar_dateofbirth_automatonfrom = '' OR PatientProfile.dateofbirth >= CAST(pvar_dateofbirth_automatonfrom AS TIMESTAMP(3))) 
                                AND (pvar_dateofbirth_automatonto IS NULL OR pvar_dateofbirth_automatonto = '' OR PatientProfile.dateofbirth <= CAST(pvar_dateofbirth_automatonto AS TIMESTAMP(3)))
AND (pvar_emailaddress is null or pvar_emailaddress ='0' or LENGTH(CAST(pvar_emailaddress as Varchar))=0 or CAST(PatientProfile.emailaddress as VARCHAR)=pvar_emailaddress)
AND (pvar_mobilenumber is null or pvar_mobilenumber ='0' or LENGTH(CAST(pvar_mobilenumber as Varchar))=0 
     or CAST(PatientProfile.mobilenumber as VARCHAR) ILIKE '%' || pvar_mobilenumber || '%')
--AND (pvar_mobilenumber is null or pvar_mobilenumber ='0' or LENGTH(CAST(pvar_mobilenumber as Varchar))=0 or CAST(PatientProfile.mobilenumber as VARCHAR)=pvar_mobilenumber)
AND (pvar_whatsappnumber is null or pvar_whatsappnumber ='0' or LENGTH(CAST(pvar_whatsappnumber as Varchar))=0 or CAST(PatientProfile.whatsappnumber as VARCHAR)=pvar_whatsappnumber)
AND (pvar_idprooftype is null or pvar_idprooftype ='0' or LENGTH(CAST(pvar_idprooftype as Varchar))=0 or CAST(PatientProfile.idprooftype as VARCHAR)=pvar_idprooftype)
AND (pvar_idproofnumber is null or pvar_idproofnumber ='0' or LENGTH(CAST(pvar_idproofnumber as Varchar))=0 or CAST(PatientProfile.idproofnumber as VARCHAR)=pvar_idproofnumber)
AND (pvar_pzip is null or pvar_pzip ='0' or LENGTH(CAST(pvar_pzip as Varchar))=0 or CAST(PatientProfile.pzip as VARCHAR)=pvar_pzip)
AND (pvar_blacklisted is null or pvar_blacklisted ='0' or LENGTH(CAST(pvar_blacklisted as Varchar))=0 or CAST(PatientProfile.blacklisted as VARCHAR)=pvar_blacklisted)
AND (pvar_deceased is null or pvar_deceased ='false' or LENGTH(CAST(pvar_deceased as Varchar))=0 or CAST(PatientProfile.deceased as VARCHAR)=pvar_deceased)

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.registrationid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.firstname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.gender AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(PatientProfile.dateofbirth,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.age AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.nationality AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_Country.countryname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.emailaddress AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.mobilenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.whatsappnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.photo AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.idprooftype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.idproofnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.uploadidproof AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.paddressline1 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.paddressline2 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.pzip AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.ptown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.pcityordistrict AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.ppstatename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.sameaspermanentaddress AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.caddressline1 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.caddressline2 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.czip AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.ctown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.ccityordistrict AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.cstatename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.languagesknown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.languagespreferrable AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.otherlanguages AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.maritalstatus AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.education AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__Occupation.occupationname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.meditationpractice AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.typeofpractice AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.creativeactivities AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.othercreativeactivities AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.insurancetype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.insurancecompany AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.policynumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.policyclaimlimit AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(PatientProfile.policyexpirydate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.referralsource AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.referrername AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.referrerphonenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.magazinename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.socialmediaplatform AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.otherreferral AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.blacklisted AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___Blacklistreason.reason AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.detailedremarks AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.deceased AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PatientProfile.causeofdeath AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(PatientProfile.dateandtimeofdeath,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'registrationid' THEN PatientProfile.registrationid::TEXT
WHEN 'registrationid' THEN PatientProfile.registrationid::TEXT
WHEN 'firstname' THEN PatientProfile.firstname::TEXT
WHEN 'lastname' THEN PatientProfile.lastname::TEXT
WHEN 'gender' THEN PatientProfile.gender::TEXT
WHEN 'age' THEN PatientProfile.age::TEXT
WHEN 'nationality' THEN PatientProfile.nationality::TEXT
WHEN 'countryoforigin' THEN _Country.countryname::TEXT
WHEN 'emailaddress' THEN PatientProfile.emailaddress::TEXT
WHEN 'mobilenumber' THEN PatientProfile.mobilenumber::TEXT
WHEN 'mobilenumber' THEN PatientProfile.mobilenumber::TEXT
WHEN 'whatsappnumber' THEN PatientProfile.whatsappnumber::TEXT
WHEN 'whatsappnumber' THEN PatientProfile.whatsappnumber::TEXT
WHEN 'photo' THEN PatientProfile.photo::TEXT
WHEN 'photo' THEN PatientProfile.photo::TEXT
WHEN 'idprooftype' THEN PatientProfile.idprooftype::TEXT
WHEN 'idproofnumber' THEN PatientProfile.idproofnumber::TEXT
WHEN 'uploadidproof' THEN PatientProfile.uploadidproof::TEXT
WHEN 'uploadidproof' THEN PatientProfile.uploadidproof::TEXT
WHEN 'paddressline1' THEN PatientProfile.paddressline1::TEXT
WHEN 'paddressline2' THEN PatientProfile.paddressline2::TEXT
WHEN 'pzip' THEN PatientProfile.pzip::TEXT
WHEN 'ptown' THEN PatientProfile.ptown::TEXT
WHEN 'pcityordistrict' THEN PatientProfile.pcityordistrict::TEXT
WHEN 'ppstatename' THEN PatientProfile.ppstatename::TEXT
WHEN 'sameaspermanentaddress' THEN PatientProfile.sameaspermanentaddress::TEXT
WHEN 'caddressline1' THEN PatientProfile.caddressline1::TEXT
WHEN 'caddressline2' THEN PatientProfile.caddressline2::TEXT
WHEN 'czip' THEN PatientProfile.czip::TEXT
WHEN 'ctown' THEN PatientProfile.ctown::TEXT
WHEN 'ccityordistrict' THEN PatientProfile.ccityordistrict::TEXT
WHEN 'cstatename' THEN PatientProfile.cstatename::TEXT
WHEN 'languagesknown' THEN PatientProfile.languagesknown::TEXT
WHEN 'languagesknown' THEN PatientProfile.languagesknown::TEXT
WHEN 'languagespreferrable' THEN PatientProfile.languagespreferrable::TEXT
WHEN 'languagespreferrable' THEN PatientProfile.languagespreferrable::TEXT
WHEN 'otherlanguages' THEN PatientProfile.otherlanguages::TEXT
WHEN 'maritalstatus' THEN PatientProfile.maritalstatus::TEXT
WHEN 'education' THEN PatientProfile.education::TEXT
WHEN 'occupation' THEN __Occupation.occupationname::TEXT
WHEN 'meditationpractice' THEN PatientProfile.meditationpractice::TEXT
WHEN 'typeofpractice' THEN PatientProfile.typeofpractice::TEXT
WHEN 'creativeactivities' THEN PatientProfile.creativeactivities::TEXT
WHEN 'creativeactivities' THEN PatientProfile.creativeactivities::TEXT
WHEN 'othercreativeactivities' THEN PatientProfile.othercreativeactivities::TEXT
WHEN 'insurancetype' THEN PatientProfile.insurancetype::TEXT
WHEN 'insurancecompany' THEN PatientProfile.insurancecompany::TEXT
WHEN 'policynumber' THEN PatientProfile.policynumber::TEXT
WHEN 'policyclaimlimit' THEN PatientProfile.policyclaimlimit::TEXT
WHEN 'referralsource' THEN PatientProfile.referralsource::TEXT
WHEN 'referrername' THEN PatientProfile.referrername::TEXT
WHEN 'referrerphonenumber' THEN PatientProfile.referrerphonenumber::TEXT
WHEN 'referrerphonenumber' THEN PatientProfile.referrerphonenumber::TEXT
WHEN 'magazinename' THEN PatientProfile.magazinename::TEXT
WHEN 'socialmediaplatform' THEN PatientProfile.socialmediaplatform::TEXT
WHEN 'otherreferral' THEN PatientProfile.otherreferral::TEXT
WHEN 'blacklisted' THEN PatientProfile.blacklisted::TEXT
WHEN 'reasonforblacklisting' THEN ___Blacklistreason.reason::TEXT
WHEN 'detailedremarks' THEN PatientProfile.detailedremarks::TEXT
WHEN 'deceased' THEN PatientProfile.deceased::TEXT
WHEN 'causeofdeath' THEN PatientProfile.causeofdeath::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'dateofbirth' THEN PatientProfile.dateofbirth
WHEN 'policyexpirydate' THEN PatientProfile.policyexpirydate
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'dateandtimeofdeath' THEN PatientProfile.dateandtimeofdeath
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'sameaspermanentaddress' THEN PatientProfile.sameaspermanentaddress
WHEN 'deceased' THEN PatientProfile.deceased
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'age' THEN PatientProfile.age::NUMERIC
WHEN 'pzip' THEN PatientProfile.pzip::NUMERIC
WHEN 'czip' THEN PatientProfile.czip::NUMERIC
WHEN 'policyclaimlimit' THEN PatientProfile.policyclaimlimit::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'registrationid' THEN PatientProfile.registrationid::TEXT
WHEN 'registrationid' THEN PatientProfile.registrationid::TEXT
WHEN 'firstname' THEN PatientProfile.firstname::TEXT
WHEN 'lastname' THEN PatientProfile.lastname::TEXT
WHEN 'gender' THEN PatientProfile.gender::TEXT
WHEN 'age' THEN PatientProfile.age::TEXT
WHEN 'nationality' THEN PatientProfile.nationality::TEXT
WHEN 'countryoforigin' THEN _Country.countryname::TEXT
WHEN 'emailaddress' THEN PatientProfile.emailaddress::TEXT
WHEN 'mobilenumber' THEN PatientProfile.mobilenumber::TEXT
WHEN 'mobilenumber' THEN PatientProfile.mobilenumber::TEXT
WHEN 'whatsappnumber' THEN PatientProfile.whatsappnumber::TEXT
WHEN 'whatsappnumber' THEN PatientProfile.whatsappnumber::TEXT
WHEN 'photo' THEN PatientProfile.photo::TEXT
WHEN 'photo' THEN PatientProfile.photo::TEXT
WHEN 'idprooftype' THEN PatientProfile.idprooftype::TEXT
WHEN 'idproofnumber' THEN PatientProfile.idproofnumber::TEXT
WHEN 'uploadidproof' THEN PatientProfile.uploadidproof::TEXT
WHEN 'uploadidproof' THEN PatientProfile.uploadidproof::TEXT
WHEN 'paddressline1' THEN PatientProfile.paddressline1::TEXT
WHEN 'paddressline2' THEN PatientProfile.paddressline2::TEXT
WHEN 'pzip' THEN PatientProfile.pzip::TEXT
WHEN 'ptown' THEN PatientProfile.ptown::TEXT
WHEN 'pcityordistrict' THEN PatientProfile.pcityordistrict::TEXT
WHEN 'ppstatename' THEN PatientProfile.ppstatename::TEXT
WHEN 'sameaspermanentaddress' THEN PatientProfile.sameaspermanentaddress::TEXT
WHEN 'caddressline1' THEN PatientProfile.caddressline1::TEXT
WHEN 'caddressline2' THEN PatientProfile.caddressline2::TEXT
WHEN 'czip' THEN PatientProfile.czip::TEXT
WHEN 'ctown' THEN PatientProfile.ctown::TEXT
WHEN 'ccityordistrict' THEN PatientProfile.ccityordistrict::TEXT
WHEN 'cstatename' THEN PatientProfile.cstatename::TEXT
WHEN 'languagesknown' THEN PatientProfile.languagesknown::TEXT
WHEN 'languagesknown' THEN PatientProfile.languagesknown::TEXT
WHEN 'languagespreferrable' THEN PatientProfile.languagespreferrable::TEXT
WHEN 'languagespreferrable' THEN PatientProfile.languagespreferrable::TEXT
WHEN 'otherlanguages' THEN PatientProfile.otherlanguages::TEXT
WHEN 'maritalstatus' THEN PatientProfile.maritalstatus::TEXT
WHEN 'education' THEN PatientProfile.education::TEXT
WHEN 'occupation' THEN __Occupation.occupationname::TEXT
WHEN 'meditationpractice' THEN PatientProfile.meditationpractice::TEXT
WHEN 'typeofpractice' THEN PatientProfile.typeofpractice::TEXT
WHEN 'creativeactivities' THEN PatientProfile.creativeactivities::TEXT
WHEN 'creativeactivities' THEN PatientProfile.creativeactivities::TEXT
WHEN 'othercreativeactivities' THEN PatientProfile.othercreativeactivities::TEXT
WHEN 'insurancetype' THEN PatientProfile.insurancetype::TEXT
WHEN 'insurancecompany' THEN PatientProfile.insurancecompany::TEXT
WHEN 'policynumber' THEN PatientProfile.policynumber::TEXT
WHEN 'policyclaimlimit' THEN PatientProfile.policyclaimlimit::TEXT
WHEN 'referralsource' THEN PatientProfile.referralsource::TEXT
WHEN 'referrername' THEN PatientProfile.referrername::TEXT
WHEN 'referrerphonenumber' THEN PatientProfile.referrerphonenumber::TEXT
WHEN 'referrerphonenumber' THEN PatientProfile.referrerphonenumber::TEXT
WHEN 'magazinename' THEN PatientProfile.magazinename::TEXT
WHEN 'socialmediaplatform' THEN PatientProfile.socialmediaplatform::TEXT
WHEN 'otherreferral' THEN PatientProfile.otherreferral::TEXT
WHEN 'blacklisted' THEN PatientProfile.blacklisted::TEXT
WHEN 'reasonforblacklisting' THEN ___Blacklistreason.reason::TEXT
WHEN 'detailedremarks' THEN PatientProfile.detailedremarks::TEXT
WHEN 'deceased' THEN PatientProfile.deceased::TEXT
WHEN 'causeofdeath' THEN PatientProfile.causeofdeath::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'dateofbirth' THEN PatientProfile.dateofbirth
WHEN 'policyexpirydate' THEN PatientProfile.policyexpirydate
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'dateandtimeofdeath' THEN PatientProfile.dateandtimeofdeath
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'sameaspermanentaddress' THEN PatientProfile.sameaspermanentaddress
WHEN 'deceased' THEN PatientProfile.deceased
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'age' THEN PatientProfile.age::NUMERIC
WHEN 'pzip' THEN PatientProfile.pzip::NUMERIC
WHEN 'czip' THEN PatientProfile.czip::NUMERIC
WHEN 'policyclaimlimit' THEN PatientProfile.policyclaimlimit::NUMERIC
			
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
