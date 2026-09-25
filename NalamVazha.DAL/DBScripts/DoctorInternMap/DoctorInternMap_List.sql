
			  CREATE OR REPLACE FUNCTION  "DoctorInternMap_List"
              (pvar_tenantid Varchar
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,DoctorInternMapid uuid
,seniordoctor uuid,seniordoctor_master Varchar,interndoctor uuid,interndoctor_master Varchar,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 07/21/2026 11:46:56*/
			  		
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



              
                RETURN QUERY
				SELECT  
				DoctorInternMap.tenantid
,tenant.businessname as _tenantName
,DoctorInternMap.DoctorInternMapid
,DoctorInternMap.seniordoctor
,CAST(_People.firstname||' '||_People.lastname AS VARCHAR) as seniordoctor_master
,DoctorInternMap.interndoctor
,CAST(__People.firstname||' '||__People.lastname AS VARCHAR) as interndoctor_master

				
				,DoctorInternMap.createduser,DoctorInternMap.createddate,DoctorInternMap.modifieduser,DoctorInternMap.modifieddate
				FROM  DoctorInternMap 
 LEFT OUTER JOIN tenant ON DoctorInternMap.tenantid=tenant.tenantid
INNER JOIN People _People ON DoctorInternMap.seniordoctor=_People.Peopleid
INNER JOIN People __People ON DoctorInternMap.interndoctor=__People.Peopleid

				WHERE (lvar_tenantid is null or COALESCE(cast(DoctorInternMap.tenantid as varchar), '') = Any(lvar_tenantid)) AND DoctorInternMap.isdeleted=false

				 ORDER BY DoctorInternMap.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

