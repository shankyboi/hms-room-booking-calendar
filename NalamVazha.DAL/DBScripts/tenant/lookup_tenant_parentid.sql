
								CREATE OR REPLACE FUNCTION  "lookup_tenant_parentid"
								(
                                pvar_businesstype character varying

                                
                                )
								RETURNS TABLE("tenantid" Varchar
,businessname Varchar
) 
						 		AS $BODY$
                                
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:17*/
							    
                                
                                RETURN QUERY        
								SELECT  
								CAST(tenant.tenantid AS Varchar) as tenantid,CAST(tenant.businessname AS Varchar) as businessname
								FROM tenant
								WHERE tenant.natureofbusiness ilike '%Lead%'
                                
                                AND tenant.isdeleted=false 
                                
                                ;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

