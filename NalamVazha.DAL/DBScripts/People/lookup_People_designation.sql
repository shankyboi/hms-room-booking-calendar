
								CREATE OR REPLACE FUNCTION  "lookup_People_designation"
								(
                                pvar_tenantid Varchar=null
,pvar_workprofile Varchar(50)=null

                                
                                )
								RETURNS TABLE("Designationid" Varchar
,designation Varchar
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
								CAST(Designation.Designationid AS Varchar) as Designationid,CAST(Designation.designation AS Varchar) as designation
								FROM Designation
								 WHERE COALESCE(cast(Designation.tenantid as varchar),'') = Any(lvar_tenantid) AND Designation.isdeleted=false
AND (pvar_workprofile IS NOT NULL AND CAST( Designation.workprofile AS Varchar) = pvar_workprofile)

 ORDER BY Designation.designation ASC
                                ;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

