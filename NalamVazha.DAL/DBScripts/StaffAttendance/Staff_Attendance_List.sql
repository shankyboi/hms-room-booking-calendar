
			  CREATE OR REPLACE FUNCTION  "Staff_Attendance_List"
              (pvar_tenantid Varchar
,pvar_shiftdate_automatonfrom Varchar(1024)
,pvar_shiftdate_automatonto Varchar(1024)
,pvar_peoplename Varchar(1024)
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:24*/
			  		
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
                    FROM  StaffAttendance 
 LEFT OUTER JOIN tenant ON StaffAttendance.tenantid=tenant.tenantid
INNER JOIN Shift _Shift ON StaffAttendance.shift=_Shift.Shiftid
INNER JOIN WorkProfile __WorkProfile ON StaffAttendance.workprofile=__WorkProfile.WorkProfileid
INNER JOIN People ___People ON StaffAttendance.peoplename=___People.Peopleid

                    WHERE (lvar_tenantid is null or COALESCE(cast(StaffAttendance.tenantid as varchar), '') = Any(lvar_tenantid)) AND StaffAttendance.isdeleted=false
 AND(pvar_shiftdate_automatonfrom IS NULL OR pvar_shiftdate_automatonfrom = '' OR StaffAttendance.shiftdate >= CAST(pvar_shiftdate_automatonfrom AS TIMESTAMP(3))) 
                                AND (pvar_shiftdate_automatonto IS NULL OR pvar_shiftdate_automatonto = '' OR StaffAttendance.shiftdate <= CAST(pvar_shiftdate_automatonto AS TIMESTAMP(3)))
AND (pvar_peoplename is null or pvar_peoplename ='0' or LENGTH(CAST(pvar_peoplename as Varchar))=0 or CAST(StaffAttendance.peoplename as VARCHAR)=pvar_peoplename)
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(StaffAttendance.shiftdate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_Shift.shiftname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.shiftstarttime AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.shiftendtime AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.shifthours AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__WorkProfile.workprofilename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___People.firstname||' '||___People.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(StaffAttendance.punchdateandtime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.earlyinmin AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.earlyoutmin AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.latemin AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.workhours AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    StaffAttendance.tenantid
,tenant.businessname as _tenantName
,StaffAttendance.StaffAttendanceid
,CAST(COALESCE(to_char(StaffAttendance.shiftdate,'dd/MM/yyyy'),'') AS Varchar) as shiftdate
,StaffAttendance.shift
,CAST(_Shift.shiftname AS VARCHAR) as shift_master
,StaffAttendance.shiftstarttime
,StaffAttendance.shiftendtime
,StaffAttendance.shifthours
,StaffAttendance.workprofile
,CAST(__WorkProfile.workprofilename AS VARCHAR) as workprofile_master
,StaffAttendance.peoplename
,CAST(___People.firstname||' '||___People.lastname AS VARCHAR) as peoplename_master
,CAST(COALESCE(to_char(StaffAttendance.punchdateandtime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as punchdateandtime
,StaffAttendance.earlyinmin
,StaffAttendance.earlyoutmin
,StaffAttendance.latemin
,StaffAttendance.workhours

                    
                    ,StaffAttendance.createduser,StaffAttendance.createddate,StaffAttendance.modifieduser,StaffAttendance.modifieddate
                    FROM  StaffAttendance 
 LEFT OUTER JOIN tenant ON StaffAttendance.tenantid=tenant.tenantid
INNER JOIN Shift _Shift ON StaffAttendance.shift=_Shift.Shiftid
INNER JOIN WorkProfile __WorkProfile ON StaffAttendance.workprofile=__WorkProfile.WorkProfileid
INNER JOIN People ___People ON StaffAttendance.peoplename=___People.Peopleid

                    WHERE (lvar_tenantid is null or COALESCE(cast(StaffAttendance.tenantid as varchar), '') = Any(lvar_tenantid)) AND StaffAttendance.isdeleted=false
 AND(pvar_shiftdate_automatonfrom IS NULL OR pvar_shiftdate_automatonfrom = '' OR StaffAttendance.shiftdate >= CAST(pvar_shiftdate_automatonfrom AS TIMESTAMP(3))) 
                                AND (pvar_shiftdate_automatonto IS NULL OR pvar_shiftdate_automatonto = '' OR StaffAttendance.shiftdate <= CAST(pvar_shiftdate_automatonto AS TIMESTAMP(3)))
AND (pvar_peoplename is null or pvar_peoplename ='0' or LENGTH(CAST(pvar_peoplename as Varchar))=0 or CAST(StaffAttendance.peoplename as VARCHAR)=pvar_peoplename)

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(StaffAttendance.shiftdate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_Shift.shiftname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.shiftstarttime AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.shiftendtime AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.shifthours AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__WorkProfile.workprofilename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___People.firstname||' '||___People.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(StaffAttendance.punchdateandtime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.earlyinmin AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.earlyoutmin AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.latemin AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(StaffAttendance.workhours AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'shift' THEN _Shift.shiftname::TEXT
WHEN 'shiftstarttime' THEN StaffAttendance.shiftstarttime::TEXT
WHEN 'shiftstarttime' THEN StaffAttendance.shiftstarttime::TEXT
WHEN 'shiftendtime' THEN StaffAttendance.shiftendtime::TEXT
WHEN 'shiftendtime' THEN StaffAttendance.shiftendtime::TEXT
WHEN 'shifthours' THEN StaffAttendance.shifthours::TEXT
WHEN 'workprofile' THEN __WorkProfile.workprofilename::TEXT
WHEN 'peoplename' THEN ___People.firstname||' '||___People.lastname::TEXT
WHEN 'earlyinmin' THEN StaffAttendance.earlyinmin::TEXT
WHEN 'earlyoutmin' THEN StaffAttendance.earlyoutmin::TEXT
WHEN 'latemin' THEN StaffAttendance.latemin::TEXT
WHEN 'workhours' THEN StaffAttendance.workhours::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'shiftdate' THEN StaffAttendance.shiftdate
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'punchdateandtime' THEN StaffAttendance.punchdateandtime
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'shifthours' THEN StaffAttendance.shifthours::NUMERIC
WHEN 'earlyinmin' THEN StaffAttendance.earlyinmin::NUMERIC
WHEN 'earlyoutmin' THEN StaffAttendance.earlyoutmin::NUMERIC
WHEN 'latemin' THEN StaffAttendance.latemin::NUMERIC
WHEN 'workhours' THEN StaffAttendance.workhours::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'shift' THEN _Shift.shiftname::TEXT
WHEN 'shiftstarttime' THEN StaffAttendance.shiftstarttime::TEXT
WHEN 'shiftstarttime' THEN StaffAttendance.shiftstarttime::TEXT
WHEN 'shiftendtime' THEN StaffAttendance.shiftendtime::TEXT
WHEN 'shiftendtime' THEN StaffAttendance.shiftendtime::TEXT
WHEN 'shifthours' THEN StaffAttendance.shifthours::TEXT
WHEN 'workprofile' THEN __WorkProfile.workprofilename::TEXT
WHEN 'peoplename' THEN ___People.firstname||' '||___People.lastname::TEXT
WHEN 'earlyinmin' THEN StaffAttendance.earlyinmin::TEXT
WHEN 'earlyoutmin' THEN StaffAttendance.earlyoutmin::TEXT
WHEN 'latemin' THEN StaffAttendance.latemin::TEXT
WHEN 'workhours' THEN StaffAttendance.workhours::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'shiftdate' THEN StaffAttendance.shiftdate
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'punchdateandtime' THEN StaffAttendance.punchdateandtime
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'shifthours' THEN StaffAttendance.shifthours::NUMERIC
WHEN 'earlyinmin' THEN StaffAttendance.earlyinmin::NUMERIC
WHEN 'earlyoutmin' THEN StaffAttendance.earlyoutmin::NUMERIC
WHEN 'latemin' THEN StaffAttendance.latemin::NUMERIC
WHEN 'workhours' THEN StaffAttendance.workhours::NUMERIC
			
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

