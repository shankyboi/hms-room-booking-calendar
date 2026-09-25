
								CREATE OR REPLACE FUNCTION  "lookup_TaskTemplate_tasktype"
								(
                                pvar_tenantid Varchar=null

                                
                                )
								RETURNS TABLE("TypeofTaskid" Varchar
,tasktype Varchar
) 
						 		AS $BODY$
                                declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:27:25*/
							    
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
								CAST(TypeofTask.TypeofTaskid AS Varchar) as TypeofTaskid,CAST(TypeofTask.tasktype AS Varchar) as tasktype
								FROM TypeofTask
								 WHERE COALESCE(cast(TypeofTask.tenantid as varchar),'') = Any(lvar_tenantid) AND TypeofTask.isdeleted=false

 ORDER BY TypeofTask.tasktype ASC
                                ;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

