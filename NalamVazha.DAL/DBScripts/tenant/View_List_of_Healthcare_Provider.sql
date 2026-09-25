
			  DROP FUNCTION IF EXISTS public."View_List_of_Healthcare_Provider"();

			  CREATE OR REPLACE FUNCTION  "View_List_of_Healthcare_Provider"
              ()
			  RETURNS TABLE(tenantid uuid
,businessname Varchar,shortcode Varchar,natureofbusiness Varchar,businessemail Varchar,businessphone Varchar,businesswebsite Varchar,organizationlogo Varchar,numberofemployees int,enablepatientautologin Varchar,allowdoctortoadmitpatients Varchar,preadmissionnoticedays int,addressline1 Varchar,addressline2 Varchar,zip Varchar,town Varchar,statename Varchar,country uuid,country_master Varchar,parentid uuid,parentid_master Varchar,username Varchar,userrole Varchar,password Varchar,createduser uuid
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
,CAST(case when tenant.enablepatientautologin=true then 'yes' else 'no' End AS Varchar)as enablepatientautologin
,CAST(case when tenant.allowdoctortoadmitpatients=true then 'yes' else 'no' End AS Varchar)as allowdoctortoadmitpatients
,tenant.preadmissionnoticedays
,tenant.addressline1
,tenant.addressline2
,tenant.zip
,tenant.town
,tenant.statename
,tenant.country
,CAST(_Country.countryname AS VARCHAR) as country_master
,tenant.parentid
,CAST(__tenant.businessname AS VARCHAR) as parentid_master
,tenant.username
,tenant.userrole
,tenant.password

				
				,tenant.createduser,tenant.createddate,tenant.modifieduser,tenant.modifieddate
				FROM  tenant 
LEFT OUTER JOIN Country _Country ON tenant.country=_Country.Countryid
LEFT OUTER JOIN tenant __tenant ON tenant.parentid=__tenant.tenantid

				WHERE tenant.isdeleted=false 

				 ORDER BY tenant.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

