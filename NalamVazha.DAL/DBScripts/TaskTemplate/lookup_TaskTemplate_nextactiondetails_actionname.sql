
								        CREATE OR REPLACE FUNCTION  "lookup_TaskTemplate_nextactiondetails_actionname"
								        (
                                        pvar_tenantid Varchar=null
,pvar_actiontype Varchar(50)=null

                                        
                                        )
								        RETURNS TABLE("Actionsid" Varchar
,actionname Varchar
) 
						 		        AS $BODY$
                                        declare lvar_tenantid varchar[];declare lstr_usersid varchar;
								        BEGIN
								        /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:27:26*/
							            
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
								        CAST(Actions.Actionsid AS Varchar) as Actionsid,CAST(Actions.actionname AS Varchar) as actionname
								        FROM Actions
								         WHERE COALESCE(cast(Actions.tenantid as varchar),'') = Any(lvar_tenantid) AND Actions.isdeleted=false
AND (pvar_actiontype IS NOT NULL AND CAST( Actions.actiontype AS Varchar) = pvar_actiontype)

 ORDER BY Actions.actionname ASC
                                        ;
								
											
								        END
                                        $BODY$
                                        LANGUAGE plpgsql;

