
			  CREATE OR REPLACE FUNCTION  "Daily_Task__List"
              (pvar_tenantid Varchar
,pvar_tasktype Varchar(1024)
,pvar_taskname Varchar(1024)
,pvar_patientname Varchar(1024)
,pvar_ipdreferencenumber Varchar(1024)
,pvar_opdreferencenumber Varchar(1024)
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:39:40*/
			  		
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
                    FROM  DailyTask 
 LEFT OUTER JOIN tenant ON DailyTask.tenantid=tenant.tenantid
INNER JOIN TypeofTask _TypeofTask ON DailyTask.tasktype=_TypeofTask.TypeofTaskid
LEFT OUTER JOIN TaskTemplate __TaskTemplate ON DailyTask.taskname=__TaskTemplate.TaskTemplateid
LEFT OUTER JOIN PatientProfile ___PatientProfile ON DailyTask.patientname=___PatientProfile.PatientProfileid
LEFT OUTER JOIN IPDApplicationForm ____IPDApplicationForm ON DailyTask.ipdreferencenumber=____IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN OPDForm _____OPDForm ON DailyTask.opdreferencenumber=_____OPDForm.OPDFormid
LEFT OUTER JOIN PatientProfile _______OPDPatientProfile ON _____OPDForm.patientname=_______OPDPatientProfile.PatientProfileid

                    WHERE (lvar_tenantid is null or COALESCE(cast(DailyTask.tenantid as varchar), '') = Any(lvar_tenantid)) AND DailyTask.isdeleted=false
AND (pvar_tasktype is null or pvar_tasktype ='0' or LENGTH(CAST(pvar_tasktype as Varchar))=0 or CAST(DailyTask.tasktype as VARCHAR)=pvar_tasktype)
AND (pvar_taskname is null or pvar_taskname ='0' or LENGTH(CAST(pvar_taskname as Varchar))=0 or CAST(DailyTask.taskname as VARCHAR)=pvar_taskname)
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(DailyTask.patientname as VARCHAR)=pvar_patientname)
AND (pvar_ipdreferencenumber is null or pvar_ipdreferencenumber ='0' or LENGTH(CAST(pvar_ipdreferencenumber as Varchar))=0 or CAST(DailyTask.ipdreferencenumber as VARCHAR)=pvar_ipdreferencenumber)
AND (pvar_opdreferencenumber is null or pvar_opdreferencenumber ='0' or LENGTH(CAST(pvar_opdreferencenumber as Varchar))=0 or CAST(DailyTask.opdreferencenumber as VARCHAR)=pvar_opdreferencenumber)
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(DailyTask.taskno AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(DailyTask.dateandtime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_TypeofTask.tasktype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__TaskTemplate.taskname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(DailyTask.activityname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__TaskTemplate.priority AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___PatientProfile.firstname||' '||___PatientProfile.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_______OPDPatientProfile.firstname||' '||_______OPDPatientProfile.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(DailyTask.patientcategory AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____IPDApplicationForm.bookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_____OPDForm.bookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(DailyTask.status AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(DailyTask.amount AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    DailyTask.tenantid
,tenant.businessname as _tenantName
,DailyTask.DailyTaskid
,DailyTask.taskno
,CAST(COALESCE(to_char(DailyTask.dateandtime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as dateandtime
,DailyTask.tasktype
,CAST(_TypeofTask.tasktype AS VARCHAR) as tasktype_master
,DailyTask.taskname
,CAST(__TaskTemplate.taskname AS VARCHAR) as taskname_master
,CAST(__TaskTemplate.priority AS VARCHAR) as priority_master
,DailyTask.activityname
,DailyTask.description
,COALESCE(NULLIF(DailyTask.priority, ''), CAST(__TaskTemplate.priority AS VARCHAR)) as priority_display
,DailyTask.assignedto
,CAST(NULLIF(btrim(concat_ws(' ', NULLIF(______AssignedTo.firstname, ''), NULLIF(______AssignedTo.lastname, ''))), '') AS VARCHAR) as assignedto_master
,COALESCE((
    SELECT json_agg(json_build_object(
        'notifyto', _Escalation.notifyto,
        'notifyto_master', btrim(concat_ws(' ', _EscalationUser.firstname, _EscalationUser.lastname)),
        'emailid', _Escalation.emailid,
        'priority', _Escalation.priority
    ) ORDER BY _Escalation.record_order)
    FROM TaskTemplate_escalationdetails _Escalation
    LEFT JOIN users _EscalationUser ON _Escalation.notifyto=_EscalationUser.usersid
    WHERE _Escalation.TaskTemplateid=DailyTask.taskname
      AND NULLIF(btrim(COALESCE(_Escalation.emailid, '')), '') IS NOT NULL
), '[]'::json) as escalationdetails
,COALESCE((
    SELECT json_agg(json_build_object(
        'actionid', __NextAction.Actionsid,
        'actionname', __NextAction.actionname,
        'actiontypeid', _NextActionDetail.actiontype
    ) ORDER BY _NextActionDetail.record_order)
    FROM TaskTemplate_nextactiondetails _NextActionDetail
    INNER JOIN Actions __NextAction ON _NextActionDetail.actionname=__NextAction.Actionsid
    WHERE _NextActionDetail.TaskTemplateid=DailyTask.taskname
      AND COALESCE(__NextAction.isdeleted, false)=false
), '[]'::json) as nextactiondetails
,COALESCE(DailyTask.patientname, _____OPDForm.patientname) as patientname
,CAST(NULLIF(btrim(concat_ws(' ',
    NULLIF(COALESCE(___PatientProfile.firstname, _______OPDPatientProfile.firstname), ''),
    NULLIF(COALESCE(___PatientProfile.lastname, _______OPDPatientProfile.lastname), '')
)), '') AS VARCHAR) as patientname_master
,DailyTask.patientcategory
,DailyTask.ipdreferencenumber
,CAST(____IPDApplicationForm.bookingreferencenumber AS VARCHAR) as ipdreferencenumber_master
,DailyTask.opdreferencenumber
,CAST(_____OPDForm.bookingreferencenumber AS VARCHAR) as opdreferencenumber_master
,DailyTask.status
,CASE WHEN DailyTask.ipdreferencenumber IS NOT NULL OR DailyTask.opdreferencenumber IS NOT NULL
      THEN COALESCE(______ReceivableSummary.balance, 0)
      ELSE COALESCE(DailyTask.amount, 0)
 END as amount
,COALESCE(______ReceivableSummary.totalamount, COALESCE(DailyTask.amount, 0)) as receivable_total
,COALESCE(______ReceivableSummary.paidamount, 0) as receivable_paid
,COALESCE(______ReceivableSummary.balance, COALESCE(DailyTask.amount, 0)) as receivable_balance
,DailyTask.closeddate
,DailyTask.closedby
,CAST(NULLIF(btrim(concat_ws(' ', NULLIF(______ClosedBy.firstname, ''), NULLIF(______ClosedBy.lastname, ''))), '') AS VARCHAR) as closedby_master

                    
                    ,DailyTask.createduser,DailyTask.createddate,DailyTask.modifieduser,DailyTask.modifieddate
                    FROM  DailyTask 
 LEFT OUTER JOIN tenant ON DailyTask.tenantid=tenant.tenantid
INNER JOIN TypeofTask _TypeofTask ON DailyTask.tasktype=_TypeofTask.TypeofTaskid
LEFT OUTER JOIN TaskTemplate __TaskTemplate ON DailyTask.taskname=__TaskTemplate.TaskTemplateid
LEFT OUTER JOIN PatientProfile ___PatientProfile ON DailyTask.patientname=___PatientProfile.PatientProfileid
LEFT OUTER JOIN IPDApplicationForm ____IPDApplicationForm ON DailyTask.ipdreferencenumber=____IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN OPDForm _____OPDForm ON DailyTask.opdreferencenumber=_____OPDForm.OPDFormid
LEFT OUTER JOIN PatientProfile _______OPDPatientProfile ON _____OPDForm.patientname=_______OPDPatientProfile.PatientProfileid
LEFT OUTER JOIN users ______ClosedBy ON DailyTask.closedby=______ClosedBy.usersid
LEFT OUTER JOIN users ______AssignedTo ON COALESCE(DailyTask.assignedto, DailyTask.createduser)=______AssignedTo.usersid
LEFT JOIN LATERAL (
    SELECT
        SUM(COALESCE(r.amount, 0)) as totalamount,
        SUM(COALESCE(r.paidamount, 0)) as paidamount,
        SUM(GREATEST(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0), 0)) as balance
    FROM Receivable r
    WHERE ((DailyTask.ipdreferencenumber IS NOT NULL AND r.ipdnumber=DailyTask.ipdreferencenumber)
        OR (DailyTask.opdreferencenumber IS NOT NULL AND r.opdnumber=DailyTask.opdreferencenumber))
      AND COALESCE(r.isdeleted, false)=false
      AND COALESCE(r.remarks, '') NOT ILIKE '%inactive%'
) ______ReceivableSummary ON true

                    WHERE (lvar_tenantid is null or COALESCE(cast(DailyTask.tenantid as varchar), '') = Any(lvar_tenantid)) AND DailyTask.isdeleted=false
AND (pvar_tasktype is null or pvar_tasktype ='0' or LENGTH(CAST(pvar_tasktype as Varchar))=0 or CAST(DailyTask.tasktype as VARCHAR)=pvar_tasktype)
AND (pvar_taskname is null or pvar_taskname ='0' or LENGTH(CAST(pvar_taskname as Varchar))=0 or CAST(DailyTask.taskname as VARCHAR)=pvar_taskname)
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(DailyTask.patientname as VARCHAR)=pvar_patientname)
AND (pvar_ipdreferencenumber is null or pvar_ipdreferencenumber ='0' or LENGTH(CAST(pvar_ipdreferencenumber as Varchar))=0 or CAST(DailyTask.ipdreferencenumber as VARCHAR)=pvar_ipdreferencenumber)
AND (pvar_opdreferencenumber is null or pvar_opdreferencenumber ='0' or LENGTH(CAST(pvar_opdreferencenumber as Varchar))=0 or CAST(DailyTask.opdreferencenumber as VARCHAR)=pvar_opdreferencenumber)

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(DailyTask.taskno AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(DailyTask.dateandtime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_TypeofTask.tasktype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__TaskTemplate.taskname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(DailyTask.activityname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__TaskTemplate.priority AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___PatientProfile.firstname||' '||___PatientProfile.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_______OPDPatientProfile.firstname||' '||_______OPDPatientProfile.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(DailyTask.patientcategory AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____IPDApplicationForm.bookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_____OPDForm.bookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(DailyTask.status AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(DailyTask.amount AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'taskno' THEN DailyTask.taskno::TEXT
WHEN 'taskno' THEN DailyTask.taskno::TEXT
WHEN 'tasktype' THEN _TypeofTask.tasktype::TEXT
WHEN 'taskname' THEN __TaskTemplate.taskname::TEXT
WHEN 'priority' THEN __TaskTemplate.priority::TEXT
WHEN 'patientname' THEN ___PatientProfile.firstname||' '||___PatientProfile.lastname::TEXT
WHEN 'patientcategory' THEN DailyTask.patientcategory::TEXT
WHEN 'ipdreferencenumber' THEN ____IPDApplicationForm.bookingreferencenumber::TEXT
WHEN 'opdreferencenumber' THEN _____OPDForm.bookingreferencenumber::TEXT
WHEN 'status' THEN DailyTask.status::TEXT
WHEN 'amount' THEN DailyTask.amount::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'dateandtime' THEN DailyTask.dateandtime
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'amount' THEN DailyTask.amount::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'taskno' THEN DailyTask.taskno::TEXT
WHEN 'taskno' THEN DailyTask.taskno::TEXT
WHEN 'tasktype' THEN _TypeofTask.tasktype::TEXT
WHEN 'taskname' THEN __TaskTemplate.taskname::TEXT
WHEN 'priority' THEN __TaskTemplate.priority::TEXT
WHEN 'patientname' THEN ___PatientProfile.firstname||' '||___PatientProfile.lastname::TEXT
WHEN 'patientcategory' THEN DailyTask.patientcategory::TEXT
WHEN 'ipdreferencenumber' THEN ____IPDApplicationForm.bookingreferencenumber::TEXT
WHEN 'opdreferencenumber' THEN _____OPDForm.bookingreferencenumber::TEXT
WHEN 'status' THEN DailyTask.status::TEXT
WHEN 'amount' THEN DailyTask.amount::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'dateandtime' THEN DailyTask.dateandtime
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'amount' THEN DailyTask.amount::NUMERIC
			
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
