 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_DoctorInternMap"
              (
			  pvar_tenantid Varchar=null
              )
			 RETURNS TABLE(
                seniordoctor uuid
,interndoctor uuid
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,"DoctorInternMapid" uuid
                 
               )
               AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
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

				 DoctorInternMap.seniordoctor
,DoctorInternMap.interndoctor

				 ,DoctorInternMap.createduser,DoctorInternMap.createddate,DoctorInternMap.modifieduser,DoctorInternMap.modifieddate
				,DoctorInternMap.tenantid 
                ,DoctorInternMap.DoctorInternMapid
				 
			  FROM DoctorInternMap
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(DoctorInternMap.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND DoctorInternMap.isdeleted=false
			  ;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

