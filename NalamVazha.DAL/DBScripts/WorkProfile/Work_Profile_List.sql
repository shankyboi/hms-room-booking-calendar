
			  CREATE OR REPLACE FUNCTION  "Work_Profile_List"
              (pvar_tenantid Varchar
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,WorkProfileid uuid
,department uuid,department_master Varchar,workprofilename Varchar,rolename Varchar,isthisaclinicalprofile Varchar,workprofiledescription Varchar,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:30*/
			  		
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
				WorkProfile.tenantid
,tenant.businessname as _tenantName
,WorkProfile.WorkProfileid
,WorkProfile.department
,CAST(_Department.name AS VARCHAR) as department_master
,WorkProfile.workprofilename
,WorkProfile.rolename
,WorkProfile.isthisaclinicalprofile
,WorkProfile.workprofiledescription

				
				,WorkProfile.createduser,WorkProfile.createddate,WorkProfile.modifieduser,WorkProfile.modifieddate
				FROM  WorkProfile 
 LEFT OUTER JOIN tenant ON WorkProfile.tenantid=tenant.tenantid
INNER JOIN Department _Department ON WorkProfile.department=_Department.Departmentid

				WHERE (lvar_tenantid is null or COALESCE(cast(WorkProfile.tenantid as varchar), '') = Any(lvar_tenantid)) AND WorkProfile.isdeleted=false

				 ORDER BY WorkProfile.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

