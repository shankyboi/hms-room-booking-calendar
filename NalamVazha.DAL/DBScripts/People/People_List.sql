
CREATE OR REPLACE FUNCTION public."People_List"(
	pvar_tenantid character varying,
	pvar_workprofile character varying,
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/09/2026 13:07:41*/
			  		
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
                    FROM  People 
 LEFT OUTER JOIN tenant ON People.tenantid=tenant.tenantid
LEFT OUTER JOIN WorkProfile _WorkProfile ON COALESCE(People.workprofile, NULLIF(People.draftdata->>'workprofile', '')::uuid)=_WorkProfile.WorkProfileid
LEFT OUTER JOIN Competency __Competency ON COALESCE(People.competencylevel, NULLIF(People.draftdata->>'competencylevel', '')::uuid)=__Competency.Competencyid
LEFT OUTER JOIN Designation ___Designation ON COALESCE(People.designation, NULLIF(People.draftdata->>'designation', '')::uuid)=___Designation.Designationid
LEFT OUTER JOIN Country ____Country ON People.specifycountry=____Country.Countryid

                    WHERE (lvar_tenantid is null or COALESCE(cast(People.tenantid as varchar), '') = Any(lvar_tenantid)) AND People.isdeleted=false
 AND (pvar_workprofile is null or pvar_workprofile = '0' or LENGTH(pvar_workprofile) = 0 or CAST(COALESCE(People.workprofile, NULLIF(People.draftdata->>'workprofile', '')::uuid) AS VARCHAR) = pvar_workprofile)
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.practitionerid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CASE WHEN People.status='Draft' THEN People.draftdata->>'firstname' ELSE People.firstname END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CASE WHEN People.status='Draft' THEN People.draftdata->>'lastname' ELSE People.lastname END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_WorkProfile.workprofilename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__Competency.competencyname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___Designation.designation AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CASE WHEN People.status='Draft' THEN People.draftdata->>'contactnumber' ELSE People.contactnumber END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.whatsappnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CASE WHEN People.status='Draft' THEN People.draftdata->>'emailid' ELSE People.emailid END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.gender AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(People.dob,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.age AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.employmentstatus AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(People.joiningdate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(People.contractrenewaldate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.photo AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.nationality AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____Country.countryname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.idtype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.idnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.iddocument AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.paddressline1 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.paddressline2 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.pzip AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.ptown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.pcityordistrict AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.pstatename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.sameaspermanentaddress AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.caddressline1 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.caddressline2 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.czip AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.ctown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.ccityordistrict AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.cstatename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.registrationnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(People.validtill,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.licenceupload AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.issuingauthority AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.bio AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.screeningmeetinglink AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    People.tenantid
,tenant.businessname as _tenantName
,People.Peopleid
,People.practitionerid
,COALESCE(People.draftdata->>'firstname', People.firstname) AS firstname
,COALESCE(People.draftdata->>'lastname', People.lastname) AS lastname
,COALESCE(People.workprofile, NULLIF(People.draftdata->>'workprofile', '')::uuid) AS workprofile
,CAST(_WorkProfile.workprofilename AS VARCHAR) as workprofile_master
,COALESCE(People.competencylevel, NULLIF(People.draftdata->>'competencylevel', '')::uuid) AS competencylevel
,CAST(__Competency.competencyname AS VARCHAR) as competencylevel_master
,COALESCE(People.designation, NULLIF(People.draftdata->>'designation', '')::uuid) AS designation
,CAST(___Designation.designation AS VARCHAR) as designation_master
,COALESCE(People.draftdata->>'contactnumber', People.contactnumber) AS contactnumber
,COALESCE(People.draftdata->>'whatsappnumber', People.whatsappnumber) AS whatsappnumber
,COALESCE(People.draftdata->>'emailid', People.emailid) AS emailid
,People.gender
,CAST(COALESCE(to_char(People.dob,'dd/MM/yyyy'),'') AS Varchar) as dob
,People.age
,People.employmentstatus
,CAST(COALESCE(to_char(People.joiningdate,'dd/MM/yyyy'),'') AS Varchar) as joiningdate
,CAST(COALESCE(to_char(People.contractrenewaldate,'dd/MM/yyyy'),'') AS Varchar) as contractrenewaldate
,COALESCE(People.draftdata->>'photo', People.photo) AS photo
,People.nationality
,People.specifycountry
,CAST(____Country.countryname AS VARCHAR) as specifycountry_master
,People.idtype
,People.idnumber
,People.iddocument
,People.paddressline1
,People.paddressline2
,People.pzip
,People.ptown
,People.pcityordistrict
,People.pstatename
,People.sameaspermanentaddress
,People.caddressline1
,People.caddressline2
,People.czip
,People.ctown
,People.ccityordistrict
,People.cstatename
,People.registrationnumber
,CAST(COALESCE(to_char(People.validtill,'dd/MM/yyyy'),'') AS Varchar) as validtill
,People.licenceupload
,People.issuingauthority
,People.bio
,People.screeningmeetinglink
,People.status
                    ,
                (SELECT json_agg(J) FROM (SELECT   
				 People_emergencycontact.personname as "Person Name"
,People_emergencycontact.relationship as "Relationship"
,People_emergencycontact.phonenumber as "Phone Number"

		 	   FROM  People_emergencycontact 

			  WHERE People.Peopleid =People_emergencycontact.Peopleid
) J)
			    as automaton_People_emergencycontact
,
                (SELECT json_agg(J) FROM (SELECT   
				 People_educationinfo.fieldofstudy as "Field of Study"
,People_educationinfo.degree as "Degree"
,People_educationinfo.educationinstitution as "Education Institution"
,People_educationinfo.certificationnumber as "Certification Number"
,People_educationinfo.yearofgraduation as "Year of Graduation"
,People_educationinfo.degreestatus as "Degree Status"

		 	   FROM  People_educationinfo 

			  WHERE People.Peopleid =People_educationinfo.Peopleid
) J)
			    as automaton_People_educationinfo
,
                (SELECT json_agg(J) FROM (SELECT   
				 People_workexperience.designation as "Designation"
,People_workexperience.institutionname as "Institution Name"
,to_char(People_workexperience.fromdate, 'dd/MM/yyyy') as "From Date"
,to_char(People_workexperience.todate, 'dd/MM/yyyy') as "To Date"

		 	   FROM  People_workexperience 

			  WHERE People.Peopleid =People_workexperience.Peopleid
) J)
			    as automaton_People_workexperience
,
                (SELECT json_agg(J) FROM (SELECT   
				 People_preferredlanguageinfo.languagesknown as "Languages Known"
,People_preferredlanguageinfo.proficiency as "Proficiency"
,People_preferredlanguageinfo.ability as "Ability"

		 	   FROM  People_preferredlanguageinfo 

			  WHERE People.Peopleid =People_preferredlanguageinfo.Peopleid
) J)
			    as automaton_People_preferredlanguageinfo
,
                (SELECT json_agg(J) FROM (SELECT   
				 CAST(_TaskType.tasktypename AS VARCHAR) as "Task Type"
,CAST(__Task.taskname AS VARCHAR) as "Task Name"
,People_clinicaltaskinfo.durationinminutes as "Duration in Minutes"
,People_clinicaltaskinfo.overbookingcount as "Over Booking Count"
,People_clinicaltaskinfo.availableon as "Available on"
,People_clinicaltaskinfo.workhourstarts as "Work Hour Starts"
,People_clinicaltaskinfo.workhourends as "Work Hour Ends"
,People_clinicaltaskinfo.priority as "Priority"
,People_clinicaltaskinfo.feesamount as "Fees Amount"

		 	   FROM  People_clinicaltaskinfo 
LEFT OUTER JOIN TaskType _TaskType ON People_clinicaltaskinfo.tasktype=_TaskType.TaskTypeid
LEFT OUTER JOIN Task __Task ON People_clinicaltaskinfo.taskname=__Task.Taskid

			  WHERE People.Peopleid =People_clinicaltaskinfo.Peopleid
) J)
			    as automaton_People_clinicaltaskinfo

                    ,People.createduser,People.createddate,People.modifieduser,People.modifieddate
                    FROM  People 
 LEFT OUTER JOIN tenant ON People.tenantid=tenant.tenantid
LEFT OUTER JOIN WorkProfile _WorkProfile ON COALESCE(People.workprofile, NULLIF(People.draftdata->>'workprofile', '')::uuid)=_WorkProfile.WorkProfileid
LEFT OUTER JOIN Competency __Competency ON COALESCE(People.competencylevel, NULLIF(People.draftdata->>'competencylevel', '')::uuid)=__Competency.Competencyid
LEFT OUTER JOIN Designation ___Designation ON COALESCE(People.designation, NULLIF(People.draftdata->>'designation', '')::uuid)=___Designation.Designationid
LEFT OUTER JOIN Country ____Country ON People.specifycountry=____Country.Countryid

                    WHERE (lvar_tenantid is null or COALESCE(cast(People.tenantid as varchar), '') = Any(lvar_tenantid)) AND People.isdeleted=false
 AND (pvar_workprofile is null or pvar_workprofile = '0' or LENGTH(pvar_workprofile) = 0 or CAST(COALESCE(People.workprofile, NULLIF(People.draftdata->>'workprofile', '')::uuid) AS VARCHAR) = pvar_workprofile)

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.practitionerid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CASE WHEN People.status='Draft' THEN People.draftdata->>'firstname' ELSE People.firstname END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CASE WHEN People.status='Draft' THEN People.draftdata->>'lastname' ELSE People.lastname END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_WorkProfile.workprofilename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__Competency.competencyname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___Designation.designation AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CASE WHEN People.status='Draft' THEN People.draftdata->>'contactnumber' ELSE People.contactnumber END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.whatsappnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CASE WHEN People.status='Draft' THEN People.draftdata->>'emailid' ELSE People.emailid END AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.gender AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(People.dob,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.age AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.employmentstatus AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(People.joiningdate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(People.contractrenewaldate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.photo AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.nationality AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____Country.countryname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.idtype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.idnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.iddocument AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.paddressline1 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.paddressline2 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.pzip AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.ptown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.pcityordistrict AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.pstatename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.sameaspermanentaddress AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.caddressline1 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.caddressline2 AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.czip AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.ctown AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.ccityordistrict AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.cstatename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.registrationnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(People.validtill,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.licenceupload AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.issuingauthority AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.bio AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(People.screeningmeetinglink AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'practitionerid' THEN People.practitionerid::TEXT
WHEN 'practitionerid' THEN People.practitionerid::TEXT
WHEN 'firstname' THEN People.firstname::TEXT
WHEN 'lastname' THEN People.lastname::TEXT
WHEN 'workprofile' THEN _WorkProfile.workprofilename::TEXT
WHEN 'competencylevel' THEN __Competency.competencyname::TEXT
WHEN 'designation' THEN ___Designation.designation::TEXT
WHEN 'contactnumber' THEN People.contactnumber::TEXT
WHEN 'contactnumber' THEN People.contactnumber::TEXT
WHEN 'whatsappnumber' THEN People.whatsappnumber::TEXT
WHEN 'whatsappnumber' THEN People.whatsappnumber::TEXT
WHEN 'emailid' THEN People.emailid::TEXT
WHEN 'gender' THEN People.gender::TEXT
WHEN 'age' THEN People.age::TEXT
WHEN 'employmentstatus' THEN People.employmentstatus::TEXT
WHEN 'photo' THEN People.photo::TEXT
WHEN 'photo' THEN People.photo::TEXT
WHEN 'nationality' THEN People.nationality::TEXT
WHEN 'specifycountry' THEN ____Country.countryname::TEXT
WHEN 'idtype' THEN People.idtype::TEXT
WHEN 'idnumber' THEN People.idnumber::TEXT
WHEN 'iddocument' THEN People.iddocument::TEXT
WHEN 'iddocument' THEN People.iddocument::TEXT
WHEN 'paddressline1' THEN People.paddressline1::TEXT
WHEN 'paddressline2' THEN People.paddressline2::TEXT
WHEN 'pzip' THEN People.pzip::TEXT
WHEN 'ptown' THEN People.ptown::TEXT
WHEN 'pcityordistrict' THEN People.pcityordistrict::TEXT
WHEN 'pstatename' THEN People.pstatename::TEXT
WHEN 'sameaspermanentaddress' THEN People.sameaspermanentaddress::TEXT
WHEN 'caddressline1' THEN People.caddressline1::TEXT
WHEN 'caddressline2' THEN People.caddressline2::TEXT
WHEN 'czip' THEN People.czip::TEXT
WHEN 'ctown' THEN People.ctown::TEXT
WHEN 'ccityordistrict' THEN People.ccityordistrict::TEXT
WHEN 'cstatename' THEN People.cstatename::TEXT
WHEN 'registrationnumber' THEN People.registrationnumber::TEXT
WHEN 'licenceupload' THEN People.licenceupload::TEXT
WHEN 'licenceupload' THEN People.licenceupload::TEXT
WHEN 'issuingauthority' THEN People.issuingauthority::TEXT
WHEN 'bio' THEN People.bio::TEXT
WHEN 'bio' THEN People.bio::TEXT
		WHEN 'screeningmeetinglink' THEN People.screeningmeetinglink::TEXT
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'dob' THEN People.dob
WHEN 'joiningdate' THEN People.joiningdate
WHEN 'contractrenewaldate' THEN People.contractrenewaldate
WHEN 'validtill' THEN People.validtill
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'sameaspermanentaddress' THEN People.sameaspermanentaddress
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'age' THEN People.age::NUMERIC
WHEN 'pzip' THEN People.pzip::NUMERIC
WHEN 'czip' THEN People.czip::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'practitionerid' THEN People.practitionerid::TEXT
WHEN 'practitionerid' THEN People.practitionerid::TEXT
WHEN 'firstname' THEN People.firstname::TEXT
WHEN 'lastname' THEN People.lastname::TEXT
WHEN 'workprofile' THEN _WorkProfile.workprofilename::TEXT
WHEN 'competencylevel' THEN __Competency.competencyname::TEXT
WHEN 'designation' THEN ___Designation.designation::TEXT
WHEN 'contactnumber' THEN People.contactnumber::TEXT
WHEN 'contactnumber' THEN People.contactnumber::TEXT
WHEN 'whatsappnumber' THEN People.whatsappnumber::TEXT
WHEN 'whatsappnumber' THEN People.whatsappnumber::TEXT
WHEN 'emailid' THEN People.emailid::TEXT
WHEN 'gender' THEN People.gender::TEXT
WHEN 'age' THEN People.age::TEXT
WHEN 'employmentstatus' THEN People.employmentstatus::TEXT
WHEN 'photo' THEN People.photo::TEXT
WHEN 'photo' THEN People.photo::TEXT
WHEN 'nationality' THEN People.nationality::TEXT
WHEN 'specifycountry' THEN ____Country.countryname::TEXT
WHEN 'idtype' THEN People.idtype::TEXT
WHEN 'idnumber' THEN People.idnumber::TEXT
WHEN 'iddocument' THEN People.iddocument::TEXT
WHEN 'iddocument' THEN People.iddocument::TEXT
WHEN 'paddressline1' THEN People.paddressline1::TEXT
WHEN 'paddressline2' THEN People.paddressline2::TEXT
WHEN 'pzip' THEN People.pzip::TEXT
WHEN 'ptown' THEN People.ptown::TEXT
WHEN 'pcityordistrict' THEN People.pcityordistrict::TEXT
WHEN 'pstatename' THEN People.pstatename::TEXT
WHEN 'sameaspermanentaddress' THEN People.sameaspermanentaddress::TEXT
WHEN 'caddressline1' THEN People.caddressline1::TEXT
WHEN 'caddressline2' THEN People.caddressline2::TEXT
WHEN 'czip' THEN People.czip::TEXT
WHEN 'ctown' THEN People.ctown::TEXT
WHEN 'ccityordistrict' THEN People.ccityordistrict::TEXT
WHEN 'cstatename' THEN People.cstatename::TEXT
WHEN 'registrationnumber' THEN People.registrationnumber::TEXT
WHEN 'licenceupload' THEN People.licenceupload::TEXT
WHEN 'licenceupload' THEN People.licenceupload::TEXT
WHEN 'issuingauthority' THEN People.issuingauthority::TEXT
WHEN 'bio' THEN People.bio::TEXT
WHEN 'bio' THEN People.bio::TEXT
	WHEN 'screeningmeetinglink' THEN People.screeningmeetinglink::TEXT		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'dob' THEN People.dob
WHEN 'joiningdate' THEN People.joiningdate
WHEN 'contractrenewaldate' THEN People.contractrenewaldate
WHEN 'validtill' THEN People.validtill
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'sameaspermanentaddress' THEN People.sameaspermanentaddress
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'age' THEN People.age::NUMERIC
WHEN 'pzip' THEN People.pzip::NUMERIC
WHEN 'czip' THEN People.czip::NUMERIC
			
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

