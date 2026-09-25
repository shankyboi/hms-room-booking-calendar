
								CREATE OR REPLACE FUNCTION  "lookup_People_clinicaltask"
								(
                                pvar_tenantid Varchar=null
,pvar_workprofile Varchar(50)=null
,pvar_competency Varchar(50)=null

                                
                                )
								RETURNS TABLE("ClinicalTaskid" Varchar
,workprofile Varchar
,competency Varchar
) 
						 		AS $BODY$
                                declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 11:34:27*/
							    
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
								CAST(ClinicalTask.ClinicalTaskid AS Varchar) as ClinicalTaskid,CAST(ClinicalTask.workprofile AS Varchar) as workprofile,CAST(ClinicalTask.competency AS Varchar) as competency
								FROM ClinicalTask
								 WHERE COALESCE(cast(ClinicalTask.tenantid as varchar),'') = Any(lvar_tenantid) AND ClinicalTask.isdeleted=false
AND (pvar_workprofile IS NOT NULL AND CAST( ClinicalTask.workprofile AS Varchar) = pvar_workprofile)
AND (pvar_competency IS NOT NULL AND CAST( ClinicalTask.competency AS Varchar) = pvar_competency)

 ORDER BY ClinicalTask.competency ASC
                                ;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

