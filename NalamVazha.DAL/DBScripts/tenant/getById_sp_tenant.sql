 
			  DROP FUNCTION IF EXISTS public."getById_sp_tenant"(character varying);

			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_tenant"
			  (
				  pvar_tenantid Varchar
			  )
			  RETURNS TABLE(
                businessname Varchar
,shortcode Varchar
,natureofbusiness Varchar
,businessemail Varchar
,businessphone Varchar
,businesswebsite Varchar
,organizationlogo Varchar
,numberofemployees int
,enablepatientautologin Boolean
,allowdoctortoadmitpatients Boolean
,preadmissionnoticedays int
,addressline1 Varchar
,addressline2 Varchar
,zip Varchar
,town Varchar
,statename Varchar
,country uuid
,parentid uuid
,username Varchar
,userrole Varchar
,password Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,tenantid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:17*/
               
              RETURN QUERY
			  SELECT 
				 tenant.businessname
,tenant.shortcode
,tenant.natureofbusiness
,tenant.businessemail
,tenant.businessphone
,tenant.businesswebsite
,tenant.organizationlogo
,tenant.numberofemployees
,COALESCE(tenant.enablepatientautologin,true) as enablepatientautologin
,COALESCE(tenant.allowdoctortoadmitpatients,true) as allowdoctortoadmitpatients
,tenant.preadmissionnoticedays
,tenant.addressline1
,tenant.addressline2
,tenant.zip
,tenant.town
,tenant.statename
,tenant.country
,tenant.parentid
,tenant.username
,tenant.userrole
,tenant.password

				 ,tenant.createduser,tenant.createddate,tenant.modifieduser,tenant.modifieddate
				 
                 ,tenant.tenantid
                    
			  FROM tenant
			  WHERE CAST(tenant.tenantid AS Varchar)=pvar_tenantid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

