
			  CREATE OR REPLACE FUNCTION  "Clinical_Appointment_List"
              (pvar_tenantid Varchar
,pvar_patient Varchar(1024)
,pvar_practitioner Varchar(1024)
,pvar_appointmentdate_automatonfrom Varchar(1024)
,pvar_appointmentdate_automatonto Varchar(1024)
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:34*/
			  		
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
                    FROM  ClinicalAppointment 
 LEFT OUTER JOIN tenant ON ClinicalAppointment.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON ClinicalAppointment.patient=_PatientProfile.PatientProfileid
LEFT OUTER JOIN People __People ON ClinicalAppointment.practitioner=__People.Peopleid
LEFT OUTER JOIN People ___People ON ClinicalAppointment.actualpractitioner=___People.Peopleid
LEFT OUTER JOIN IPDApplicationForm ON lower(trim(COALESCE(ClinicalAppointment.origin, ''))) = 'ipd'
    AND ClinicalAppointment.bookingid = IPDApplicationForm.IPDApplicationFormid::varchar

                    WHERE (lvar_tenantid is null or COALESCE(cast(ClinicalAppointment.tenantid as varchar), '') = Any(lvar_tenantid)) AND ClinicalAppointment.isdeleted=false
AND (pvar_patient is null or pvar_patient ='0' or LENGTH(CAST(pvar_patient as Varchar))=0 or CAST(ClinicalAppointment.patient as VARCHAR)=pvar_patient)
AND (pvar_practitioner is null or pvar_practitioner ='0' or LENGTH(CAST(pvar_practitioner as Varchar))=0 or CAST(ClinicalAppointment.practitioner as VARCHAR)=pvar_practitioner)
 AND(pvar_appointmentdate_automatonfrom IS NULL OR pvar_appointmentdate_automatonfrom = '' OR ClinicalAppointment.appointmentdate >= CAST(pvar_appointmentdate_automatonfrom AS TIMESTAMP(3))) 
                                AND (pvar_appointmentdate_automatonto IS NULL OR pvar_appointmentdate_automatonto = '' OR ClinicalAppointment.appointmentdate <= CAST(pvar_appointmentdate_automatonto AS TIMESTAMP(3)))
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.tasktype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''))), '') AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(NULLIF(btrim(concat_ws(' ', NULLIF(__People.firstname, ''), NULLIF(__People.lastname, ''))), '') AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.photo AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(NULLIF(btrim(concat_ws(' ', NULLIF(___People.firstname, ''), NULLIF(___People.lastname, ''))), '') AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(ClinicalAppointment.appointmentdate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.durationfrom AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.durationto AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.status AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.origin AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.bookingid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.tokennumber AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    ClinicalAppointment.tenantid
,tenant.businessname as _tenantName
,ClinicalAppointment.ClinicalAppointmentid
,ClinicalAppointment.tasktype
,ClinicalAppointment.patient
,CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''))), '') AS VARCHAR) as patient_master
,ClinicalAppointment.practitioner
,CAST(NULLIF(btrim(concat_ws(' ', NULLIF(__People.firstname, ''), NULLIF(__People.lastname, ''))), '') AS VARCHAR) as practitioner_master
,ClinicalAppointment.photo
,ClinicalAppointment.actualpractitioner
,CAST(NULLIF(btrim(concat_ws(' ', NULLIF(___People.firstname, ''), NULLIF(___People.lastname, ''))), '') AS VARCHAR) as actualpractitioner_master
,CAST(COALESCE(to_char(ClinicalAppointment.appointmentdate,'dd/MM/yyyy'),'') AS Varchar) as appointmentdate
,ClinicalAppointment.durationfrom
,ClinicalAppointment.durationto
,ClinicalAppointment.status
,ClinicalAppointment.origin
,ClinicalAppointment.bookingid
,IPDApplicationForm.verifiedstatus
,ClinicalAppointment.tokennumber
,COALESCE((
    SELECT pct.priority
    FROM People_clinicaltaskinfo pct
    LEFT JOIN TaskType tt ON pct.tasktype = tt.TaskTypeid
    WHERE pct.Peopleid = COALESCE(ClinicalAppointment.actualpractitioner, ClinicalAppointment.practitioner)
      AND LOWER(TRIM(COALESCE(tt.tasktypename, ''))) = LOWER(TRIM(COALESCE(ClinicalAppointment.tasktype, '')))
    ORDER BY pct.record_order ASC NULLS LAST
    LIMIT 1
), 'Medium') AS taskpriority

                    ,
                (SELECT json_agg(J) FROM (SELECT   
				 to_char(ClinicalAppointment_reshedulehistory.resheduleddatetime, 'dd/MM/yyyy HH24:MI') as "Resheduled Date Time"
,ClinicalAppointment_reshedulehistory.reshedulereason as "Reshedule Reason"

		 	   FROM  ClinicalAppointment_reshedulehistory 

			  WHERE ClinicalAppointment.ClinicalAppointmentid =ClinicalAppointment_reshedulehistory.ClinicalAppointmentid
) J)
			    as automaton_ClinicalAppointment_reshedulehistory

                    ,ClinicalAppointment.createduser,ClinicalAppointment.createddate,ClinicalAppointment.modifieduser,ClinicalAppointment.modifieddate
                    FROM  ClinicalAppointment 
 LEFT OUTER JOIN tenant ON ClinicalAppointment.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON ClinicalAppointment.patient=_PatientProfile.PatientProfileid
LEFT OUTER JOIN People __People ON ClinicalAppointment.practitioner=__People.Peopleid
LEFT OUTER JOIN People ___People ON ClinicalAppointment.actualpractitioner=___People.Peopleid
LEFT OUTER JOIN IPDApplicationForm ON lower(trim(COALESCE(ClinicalAppointment.origin, ''))) = 'ipd'
    AND ClinicalAppointment.bookingid = IPDApplicationForm.IPDApplicationFormid::varchar

                    WHERE (lvar_tenantid is null or COALESCE(cast(ClinicalAppointment.tenantid as varchar), '') = Any(lvar_tenantid)) AND ClinicalAppointment.isdeleted=false
AND (pvar_patient is null or pvar_patient ='0' or LENGTH(CAST(pvar_patient as Varchar))=0 or CAST(ClinicalAppointment.patient as VARCHAR)=pvar_patient)
AND (pvar_practitioner is null or pvar_practitioner ='0' or LENGTH(CAST(pvar_practitioner as Varchar))=0 or CAST(ClinicalAppointment.practitioner as VARCHAR)=pvar_practitioner)
 AND(pvar_appointmentdate_automatonfrom IS NULL OR pvar_appointmentdate_automatonfrom = '' OR ClinicalAppointment.appointmentdate >= CAST(pvar_appointmentdate_automatonfrom AS TIMESTAMP(3))) 
                                AND (pvar_appointmentdate_automatonto IS NULL OR pvar_appointmentdate_automatonto = '' OR ClinicalAppointment.appointmentdate <= CAST(pvar_appointmentdate_automatonto AS TIMESTAMP(3)))

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.tasktype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''))), '') AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(NULLIF(btrim(concat_ws(' ', NULLIF(__People.firstname, ''), NULLIF(__People.lastname, ''))), '') AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.photo AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(NULLIF(btrim(concat_ws(' ', NULLIF(___People.firstname, ''), NULLIF(___People.lastname, ''))), '') AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(ClinicalAppointment.appointmentdate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.durationfrom AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.durationto AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.status AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.origin AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.bookingid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(ClinicalAppointment.tokennumber AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'tasktype' THEN ClinicalAppointment.tasktype::TEXT
WHEN 'patient' THEN NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''))), '')::TEXT
WHEN 'practitioner' THEN NULLIF(btrim(concat_ws(' ', NULLIF(__People.firstname, ''), NULLIF(__People.lastname, ''))), '')::TEXT
WHEN 'photo' THEN ClinicalAppointment.photo::TEXT
WHEN 'photo' THEN ClinicalAppointment.photo::TEXT
WHEN 'actualpractitioner' THEN NULLIF(btrim(concat_ws(' ', NULLIF(___People.firstname, ''), NULLIF(___People.lastname, ''))), '')::TEXT
WHEN 'durationfrom' THEN ClinicalAppointment.durationfrom::TEXT
WHEN 'durationfrom' THEN ClinicalAppointment.durationfrom::TEXT
WHEN 'durationto' THEN ClinicalAppointment.durationto::TEXT
WHEN 'durationto' THEN ClinicalAppointment.durationto::TEXT
WHEN 'status' THEN ClinicalAppointment.status::TEXT
WHEN 'origin' THEN ClinicalAppointment.origin::TEXT
WHEN 'bookingid' THEN ClinicalAppointment.bookingid::TEXT
WHEN 'tokennumber' THEN ClinicalAppointment.tokennumber::TEXT
WHEN 'tokennumber' THEN ClinicalAppointment.tokennumber::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'appointmentdate' THEN ClinicalAppointment.appointmentdate
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'tasktype' THEN ClinicalAppointment.tasktype::TEXT
WHEN 'patient' THEN NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''))), '')::TEXT
WHEN 'practitioner' THEN NULLIF(btrim(concat_ws(' ', NULLIF(__People.firstname, ''), NULLIF(__People.lastname, ''))), '')::TEXT
WHEN 'photo' THEN ClinicalAppointment.photo::TEXT
WHEN 'photo' THEN ClinicalAppointment.photo::TEXT
WHEN 'actualpractitioner' THEN NULLIF(btrim(concat_ws(' ', NULLIF(___People.firstname, ''), NULLIF(___People.lastname, ''))), '')::TEXT
WHEN 'durationfrom' THEN ClinicalAppointment.durationfrom::TEXT
WHEN 'durationfrom' THEN ClinicalAppointment.durationfrom::TEXT
WHEN 'durationto' THEN ClinicalAppointment.durationto::TEXT
WHEN 'durationto' THEN ClinicalAppointment.durationto::TEXT
WHEN 'status' THEN ClinicalAppointment.status::TEXT
WHEN 'origin' THEN ClinicalAppointment.origin::TEXT
WHEN 'bookingid' THEN ClinicalAppointment.bookingid::TEXT
WHEN 'tokennumber' THEN ClinicalAppointment.tokennumber::TEXT
WHEN 'tokennumber' THEN ClinicalAppointment.tokennumber::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'appointmentdate' THEN ClinicalAppointment.appointmentdate
			
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

