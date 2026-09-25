
			  DROP FUNCTION IF EXISTS public."getById_sp_all_tenant"(character varying);

			  CREATE OR REPLACE FUNCTION  "getById_sp_all_tenant"
              (
			  pvar_tenantid Varchar
			  )
              RETURNS TABLE(
                "tenantid" uuid
,businessname Varchar
,shortcode Varchar
,natureofbusiness Varchar
,businessemail Varchar
,businessphone Varchar
,businesswebsite Varchar
,organizationlogo Varchar
,numberofemployees int
,enablepatientautologin Varchar
,allowdoctortoadmitpatients Varchar
,preadmissionnoticedays int
,addressline1 Varchar
,addressline2 Varchar
,zip Varchar
,town Varchar
,statename Varchar
,country Varchar
,parentid Varchar
,username Varchar
,userrole Varchar
,password Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:17*/
			  		 
              RETURN QUERY
			  SELECT  
				 tenant.tenantid
,tenant.businessname
,tenant.shortcode
,tenant.natureofbusiness
,tenant.businessemail
,tenant.businessphone
,tenant.businesswebsite
,tenant.organizationlogo
,tenant.numberofemployees
,CAST(case when tenant.enablepatientautologin=true then 'Yes' else 'No' End AS Varchar) as enablepatientautologin
,CAST(case when tenant.allowdoctortoadmitpatients=true then 'Yes' else 'No' End AS Varchar) as allowdoctortoadmitpatients
,tenant.preadmissionnoticedays
,tenant.addressline1
,tenant.addressline2
,tenant.zip
,tenant.town
,tenant.statename
,CAST(_Country.countryname AS VARCHAR) as country
,CAST(__tenant.businessname AS VARCHAR) as parentid
,tenant.username
,tenant.userrole
,tenant.password

				 ,tenant.createduser,tenant.createddate,tenant.modifieduser,tenant.modifieddate
                 
                 
				 
			  FROM  tenant 
LEFT OUTER JOIN Country _Country ON tenant.country=_Country.Countryid
LEFT OUTER JOIN tenant __tenant ON tenant.parentid=__tenant.tenantid

			  WHERE CAST(tenant.tenantid AS Varchar)=pvar_tenantid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

