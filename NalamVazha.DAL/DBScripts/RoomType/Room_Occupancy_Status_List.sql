
CREATE OR REPLACE FUNCTION public."Room_Occupancy_Status_List"(
	pvar_tenantid character varying,
	pvar_patientvisit character varying,
	pvar_patientname character varying,
	pvar_ipdno character varying,
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:03*/
			  		
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
                    FROM  RoomOccupancyStatus 
 LEFT OUTER JOIN tenant ON RoomOccupancyStatus.tenantid=tenant.tenantid
LEFT OUTER JOIN PatientVisit _PatientVisit ON RoomOccupancyStatus.patientvisit=_PatientVisit.PatientVisitid
LEFT OUTER JOIN PatientProfile __PatientProfile ON RoomOccupancyStatus.patientname=__PatientProfile.PatientProfileid
LEFT OUTER JOIN IPDApplicationForm ___IPDApplicationForm ON RoomOccupancyStatus.ipdno=___IPDApplicationForm.IPDApplicationFormid
INNER JOIN Block ____Block ON RoomOccupancyStatus.block=____Block.Blockid
INNER JOIN Building _____Building ON RoomOccupancyStatus.building=_____Building.Buildingid
INNER JOIN Floor ______Floor ON RoomOccupancyStatus.floor=______Floor.Floorid
INNER JOIN Room _______Room ON RoomOccupancyStatus.room=_______Room.Roomid

                    WHERE (lvar_tenantid is null or COALESCE(cast(RoomOccupancyStatus.tenantid as varchar), '') = Any(lvar_tenantid)) AND RoomOccupancyStatus.isdeleted=false
AND (pvar_patientvisit is null or pvar_patientvisit ='0' or LENGTH(CAST(pvar_patientvisit as Varchar))=0 or CAST(RoomOccupancyStatus.patientvisit as VARCHAR)=pvar_patientvisit)
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(RoomOccupancyStatus.patientname as VARCHAR)=pvar_patientname)
AND (pvar_ipdno is null or pvar_ipdno ='0' or LENGTH(CAST(pvar_ipdno as Varchar))=0 or CAST(RoomOccupancyStatus.ipdno as VARCHAR)=pvar_ipdno)
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(RoomOccupancyStatus.roomallocationno AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_PatientVisit.visitnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___IPDApplicationForm.firstname||' '||___IPDApplicationForm.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____Block.blockname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_____Building.buildingname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(______Floor.floorname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_______Room.roomnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(RoomOccupancyStatus.bookeddate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(RoomOccupancyStatus.status AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    RoomOccupancyStatus.tenantid
,tenant.businessname as _tenantName
,RoomOccupancyStatus.RoomOccupancyStatusid
,RoomOccupancyStatus.roomallocationno
,RoomOccupancyStatus.patientvisit
,CAST(_PatientVisit.visitnumber AS VARCHAR) as patientvisit_master
,RoomOccupancyStatus.patientname
,CAST(__PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber AS VARCHAR) as patientname_master
,RoomOccupancyStatus.ipdno
,CAST(___IPDApplicationForm.firstname||' '||___IPDApplicationForm.lastname AS VARCHAR) as ipdno_master
,RoomOccupancyStatus.block
,CAST(____Block.blockname AS VARCHAR) as block_master
,RoomOccupancyStatus.building
,CAST(_____Building.buildingname AS VARCHAR) as building_master
,RoomOccupancyStatus.floor
,CAST(______Floor.floorname AS VARCHAR) as floor_master
,RoomOccupancyStatus.room
,CAST(_______Room.roomnumber AS VARCHAR) as room_master
,CAST(COALESCE(to_char(RoomOccupancyStatus.bookeddate,'dd/MM/yyyy'),'') AS Varchar) as bookeddate
,RoomOccupancyStatus.status

                    
                    ,RoomOccupancyStatus.createduser,RoomOccupancyStatus.createddate,RoomOccupancyStatus.modifieduser,RoomOccupancyStatus.modifieddate
                    FROM  RoomOccupancyStatus 
 LEFT OUTER JOIN tenant ON RoomOccupancyStatus.tenantid=tenant.tenantid
LEFT OUTER JOIN PatientVisit _PatientVisit ON RoomOccupancyStatus.patientvisit=_PatientVisit.PatientVisitid
LEFT OUTER JOIN PatientProfile __PatientProfile ON RoomOccupancyStatus.patientname=__PatientProfile.PatientProfileid
LEFT OUTER JOIN IPDApplicationForm ___IPDApplicationForm ON RoomOccupancyStatus.ipdno=___IPDApplicationForm.IPDApplicationFormid
INNER JOIN Block ____Block ON RoomOccupancyStatus.block=____Block.Blockid
INNER JOIN Building _____Building ON RoomOccupancyStatus.building=_____Building.Buildingid
INNER JOIN Floor ______Floor ON RoomOccupancyStatus.floor=______Floor.Floorid
INNER JOIN Room _______Room ON RoomOccupancyStatus.room=_______Room.Roomid

                    WHERE (lvar_tenantid is null or COALESCE(cast(RoomOccupancyStatus.tenantid as varchar), '') = Any(lvar_tenantid)) AND RoomOccupancyStatus.isdeleted=false
AND (pvar_patientvisit is null or pvar_patientvisit ='0' or LENGTH(CAST(pvar_patientvisit as Varchar))=0 or CAST(RoomOccupancyStatus.patientvisit as VARCHAR)=pvar_patientvisit)
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(RoomOccupancyStatus.patientname as VARCHAR)=pvar_patientname)
AND (pvar_ipdno is null or pvar_ipdno ='0' or LENGTH(CAST(pvar_ipdno as Varchar))=0 or CAST(RoomOccupancyStatus.ipdno as VARCHAR)=pvar_ipdno)

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(RoomOccupancyStatus.roomallocationno AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_PatientVisit.visitnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(___IPDApplicationForm.firstname||' '||___IPDApplicationForm.lastname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____Block.blockname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_____Building.buildingname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(______Floor.floorname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_______Room.roomnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(RoomOccupancyStatus.bookeddate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(RoomOccupancyStatus.status AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'roomallocationno' THEN RoomOccupancyStatus.roomallocationno::TEXT
WHEN 'roomallocationno' THEN RoomOccupancyStatus.roomallocationno::TEXT
WHEN 'patientvisit' THEN _PatientVisit.visitnumber::TEXT
WHEN 'patientname' THEN __PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber::TEXT
WHEN 'ipdno' THEN ___IPDApplicationForm.firstname||' '||___IPDApplicationForm.lastname::TEXT
WHEN 'block' THEN ____Block.blockname::TEXT
WHEN 'building' THEN _____Building.buildingname::TEXT
WHEN 'floor' THEN ______Floor.floorname::TEXT
WHEN 'room' THEN _______Room.roomnumber::TEXT
WHEN 'status' THEN RoomOccupancyStatus.status::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'bookeddate' THEN RoomOccupancyStatus.bookeddate
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'roomallocationno' THEN RoomOccupancyStatus.roomallocationno::TEXT
WHEN 'roomallocationno' THEN RoomOccupancyStatus.roomallocationno::TEXT
WHEN 'patientvisit' THEN _PatientVisit.visitnumber::TEXT
WHEN 'patientname' THEN __PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber::TEXT
WHEN 'ipdno' THEN ___IPDApplicationForm.firstname||' '||___IPDApplicationForm.lastname::TEXT
WHEN 'block' THEN ____Block.blockname::TEXT
WHEN 'building' THEN _____Building.buildingname::TEXT
WHEN 'floor' THEN ______Floor.floorname::TEXT
WHEN 'room' THEN _______Room.roomnumber::TEXT
WHEN 'status' THEN RoomOccupancyStatus.status::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'bookeddate' THEN RoomOccupancyStatus.bookeddate
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC

                     limit case when pvar_pagesize is null or pvar_pagesize <= 0 then null else pvar_pagesize end
                    offset case when pvar_pagesize is null or pvar_pagesize <= 0 then 0 else pvar_pagenumber * pvar_pagesize end
			 		 	
			 	
                    ) d));
	
			  
					 	
			  END
              
$BODY$;
