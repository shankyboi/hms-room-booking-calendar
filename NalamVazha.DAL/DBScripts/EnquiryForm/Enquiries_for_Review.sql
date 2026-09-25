
			  CREATE OR REPLACE FUNCTION  "Enquiries_for_Review"
              (pvar_tenantid Varchar
,pvar_enquirystatus Varchar(1024)
,pvar_verifiedstatus Varchar(1024)
,pvar_pagesize integer
,pvar_pagenumber integer
,pvar_searchterm varchar
,pvar_sort_fields json



                )
			  RETURNS json
			  AS $BODY$
               declare local_sortcolumn_array text[] = (
	                select array_agg(col) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)
                );
                declare local_sortorder_array text[] = (
	                select array_agg(dir) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)	
                );          
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;  
               
          	  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:41*/
			  		
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
                    FROM  EnquiryForm 
 LEFT OUTER JOIN tenant ON EnquiryForm.tenantid=tenant.tenantid
INNER JOIN EnquiryType _EnquiryType ON EnquiryForm.enquirytype=_EnquiryType.EnquiryTypeid
LEFT OUTER JOIN PatientProfile __PatientProfile ON EnquiryForm.patientname=__PatientProfile.PatientProfileid
LEFT OUTER JOIN RoomType ___RoomType ON EnquiryForm.preferredroomtype=___RoomType.RoomTypeid

                    WHERE (lvar_tenantid is null or COALESCE(cast(EnquiryForm.tenantid as varchar), '') = Any(lvar_tenantid)) AND EnquiryForm.isdeleted=false
AND (pvar_enquirystatus is null or pvar_enquirystatus ='0' or LENGTH(CAST(pvar_enquirystatus as Varchar))=0 or CAST(EnquiryForm.enquirystatus as VARCHAR)=pvar_enquirystatus)
AND (pvar_verifiedstatus is null or pvar_verifiedstatus ='0' or LENGTH(CAST(pvar_verifiedstatus as Varchar))=0 or CAST(EnquiryForm.verifiedstatus as VARCHAR)=pvar_verifiedstatus)
 AND EnquiryForm.verifiedstatus !='Revised'
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.enquirynumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(EnquiryForm.enquirydate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_EnquiryType.enquiryname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.isroombookingrelated AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.firstname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.gender AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.age AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.phonenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.emailaddress AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.preferredcontactmethod AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.enquiryreason AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.enquiredvia AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___RoomType.name AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(EnquiryForm.preferreddateofarrival,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(EnquiryForm.preferreddateofdeparture,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.enquirystatus AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.verifiedstatus AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    EnquiryForm.tenantid
,tenant.businessname as _tenantName
,EnquiryForm.EnquiryFormid
,EnquiryForm.enquirynumber
,CAST(COALESCE(to_char(EnquiryForm.enquirydate,'dd/MM/yyyy'),'') AS Varchar) as enquirydate
,EnquiryForm.enquirytype
,CAST(_EnquiryType.enquiryname AS VARCHAR) as enquirytype_master
,EnquiryForm.isroombookingrelated
,EnquiryForm.patientname
,CAST(__PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber AS VARCHAR) as patientname_master
,EnquiryForm.firstname
,EnquiryForm.lastname
,EnquiryForm.gender
,EnquiryForm.age
,EnquiryForm.phonenumber
,EnquiryForm.emailaddress
,EnquiryForm.preferredcontactmethod
,EnquiryForm.enquiryreason
,EnquiryForm.enquiredvia
,EnquiryForm.preferredroomtype
,CAST(___RoomType.name AS VARCHAR) as preferredroomtype_master
,CAST(COALESCE(to_char(EnquiryForm.preferreddateofarrival,'dd/MM/yyyy'),'') AS Varchar) as preferreddateofarrival
,CAST(COALESCE(to_char(EnquiryForm.preferreddateofdeparture,'dd/MM/yyyy'),'') AS Varchar) as preferreddateofdeparture
,CAST(case when EnquiryForm.joinwaitinglist=true then 'Yes' else 'No' End AS Varchar)as joinwaitinglist
,EnquiryForm.enquirystatus
,EnquiryForm.verifiedstatus

                    ,
                (SELECT json_agg(J) FROM (SELECT   
				 CAST(_MedicalCondition.conditionname AS VARCHAR) as "Medical Condition"
,EnquiryForm_medicalinfo.conditionname as "Condition Name"
,EnquiryForm_medicalinfo.duration as "Duration"
,EnquiryForm_medicalinfo.severity as "Severity"

		 	   FROM  EnquiryForm_medicalinfo 
LEFT OUTER JOIN MedicalCondition _MedicalCondition ON EnquiryForm_medicalinfo.medicalcondition=_MedicalCondition.MedicalConditionid

			  WHERE EnquiryForm.EnquiryFormid =EnquiryForm_medicalinfo.EnquiryFormid
) J)
			    as automaton_EnquiryForm_medicalinfo

                    ,EnquiryForm.createduser,EnquiryForm.createddate,EnquiryForm.modifieduser,EnquiryForm.modifieddate
                    FROM  EnquiryForm 
 LEFT OUTER JOIN tenant ON EnquiryForm.tenantid=tenant.tenantid
INNER JOIN EnquiryType _EnquiryType ON EnquiryForm.enquirytype=_EnquiryType.EnquiryTypeid
LEFT OUTER JOIN PatientProfile __PatientProfile ON EnquiryForm.patientname=__PatientProfile.PatientProfileid
LEFT OUTER JOIN RoomType ___RoomType ON EnquiryForm.preferredroomtype=___RoomType.RoomTypeid

                    WHERE (lvar_tenantid is null or COALESCE(cast(EnquiryForm.tenantid as varchar), '') = Any(lvar_tenantid)) AND EnquiryForm.isdeleted=false
AND (pvar_enquirystatus is null or pvar_enquirystatus ='0' or LENGTH(CAST(pvar_enquirystatus as Varchar))=0 or CAST(EnquiryForm.enquirystatus as VARCHAR)=pvar_enquirystatus)
AND (pvar_verifiedstatus is null or pvar_verifiedstatus ='0' or LENGTH(CAST(pvar_verifiedstatus as Varchar))=0 or CAST(EnquiryForm.verifiedstatus as VARCHAR)=pvar_verifiedstatus)
 AND EnquiryForm.verifiedstatus !='Revised'

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.enquirynumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(EnquiryForm.enquirydate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_EnquiryType.enquiryname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.isroombookingrelated AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.firstname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.gender AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.age AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.phonenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.emailaddress AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.preferredcontactmethod AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.enquiryreason AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.enquiredvia AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___RoomType.name AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(EnquiryForm.preferreddateofarrival,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(EnquiryForm.preferreddateofdeparture,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.enquirystatus AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(EnquiryForm.verifiedstatus AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'enquirynumber' THEN EnquiryForm.enquirynumber::TEXT
WHEN 'enquirynumber' THEN EnquiryForm.enquirynumber::TEXT
WHEN 'enquirytype' THEN _EnquiryType.enquiryname::TEXT
WHEN 'isroombookingrelated' THEN EnquiryForm.isroombookingrelated::TEXT
WHEN 'patientname' THEN __PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber::TEXT
WHEN 'firstname' THEN EnquiryForm.firstname::TEXT
WHEN 'lastname' THEN EnquiryForm.lastname::TEXT
WHEN 'gender' THEN EnquiryForm.gender::TEXT
WHEN 'age' THEN EnquiryForm.age::TEXT
WHEN 'phonenumber' THEN EnquiryForm.phonenumber::TEXT
WHEN 'phonenumber' THEN EnquiryForm.phonenumber::TEXT
WHEN 'emailaddress' THEN EnquiryForm.emailaddress::TEXT
WHEN 'preferredcontactmethod' THEN EnquiryForm.preferredcontactmethod::TEXT
WHEN 'enquiryreason' THEN EnquiryForm.enquiryreason::TEXT
WHEN 'enquiryreason' THEN EnquiryForm.enquiryreason::TEXT
WHEN 'enquiredvia' THEN EnquiryForm.enquiredvia::TEXT
WHEN 'preferredroomtype' THEN ___RoomType.name::TEXT
WHEN 'enquirystatus' THEN EnquiryForm.enquirystatus::TEXT
WHEN 'verifiedstatus' THEN EnquiryForm.verifiedstatus::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'enquirydate' THEN EnquiryForm.enquirydate
WHEN 'preferreddateofarrival' THEN EnquiryForm.preferreddateofarrival
WHEN 'preferreddateofdeparture' THEN EnquiryForm.preferreddateofdeparture
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'joinwaitinglist' THEN EnquiryForm.joinwaitinglist
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'age' THEN EnquiryForm.age::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'enquirynumber' THEN EnquiryForm.enquirynumber::TEXT
WHEN 'enquirynumber' THEN EnquiryForm.enquirynumber::TEXT
WHEN 'enquirytype' THEN _EnquiryType.enquiryname::TEXT
WHEN 'isroombookingrelated' THEN EnquiryForm.isroombookingrelated::TEXT
WHEN 'patientname' THEN __PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber::TEXT
WHEN 'firstname' THEN EnquiryForm.firstname::TEXT
WHEN 'lastname' THEN EnquiryForm.lastname::TEXT
WHEN 'gender' THEN EnquiryForm.gender::TEXT
WHEN 'age' THEN EnquiryForm.age::TEXT
WHEN 'phonenumber' THEN EnquiryForm.phonenumber::TEXT
WHEN 'phonenumber' THEN EnquiryForm.phonenumber::TEXT
WHEN 'emailaddress' THEN EnquiryForm.emailaddress::TEXT
WHEN 'preferredcontactmethod' THEN EnquiryForm.preferredcontactmethod::TEXT
WHEN 'enquiryreason' THEN EnquiryForm.enquiryreason::TEXT
WHEN 'enquiryreason' THEN EnquiryForm.enquiryreason::TEXT
WHEN 'enquiredvia' THEN EnquiryForm.enquiredvia::TEXT
WHEN 'preferredroomtype' THEN ___RoomType.name::TEXT
WHEN 'enquirystatus' THEN EnquiryForm.enquirystatus::TEXT
WHEN 'verifiedstatus' THEN EnquiryForm.verifiedstatus::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'enquirydate' THEN EnquiryForm.enquirydate
WHEN 'preferreddateofarrival' THEN EnquiryForm.preferreddateofarrival
WHEN 'preferreddateofdeparture' THEN EnquiryForm.preferreddateofdeparture
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'joinwaitinglist' THEN EnquiryForm.joinwaitinglist
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'age' THEN EnquiryForm.age::NUMERIC
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC

                    limit pvar_pagesize
                    offset pvar_pagenumber * pvar_pagesize			 	
			 	
                    ) d));
	
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

