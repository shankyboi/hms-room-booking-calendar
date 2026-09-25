
			  CREATE OR REPLACE FUNCTION  "Designation_List"
              (pvar_tenantid Varchar
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,Designationid uuid
,workprofile uuid,workprofile_master Varchar,designation Varchar,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:33*/
			  		
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
				Designation.tenantid
,tenant.businessname as _tenantName
,Designation.Designationid
,Designation.workprofile
,CAST(_WorkProfile.workprofilename AS VARCHAR) as workprofile_master
,Designation.designation

				
				,Designation.createduser,Designation.createddate,Designation.modifieduser,Designation.modifieddate
				FROM  Designation 
 LEFT OUTER JOIN tenant ON Designation.tenantid=tenant.tenantid
INNER JOIN WorkProfile _WorkProfile ON Designation.workprofile=_WorkProfile.WorkProfileid

				WHERE (lvar_tenantid is null or COALESCE(cast(Designation.tenantid as varchar), '') = Any(lvar_tenantid)) AND Designation.isdeleted=false

				 ORDER BY Designation.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

