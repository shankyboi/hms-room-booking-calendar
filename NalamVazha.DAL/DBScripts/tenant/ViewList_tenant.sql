
			  CREATE OR REPLACE FUNCTION  "ViewList_tenant"
              ()
			  RETURNS TABLE(tenantid uuid
,businessname Varchar,natureofbusiness Varchar,businessemail Varchar,businessphone Varchar,businesswebsite Varchar,organizationlogo Varchar,numberofemployees int,addressline1 Varchar,addressline2 Varchar,city Varchar,statename Varchar,zip Varchar,country Varchar,parentid uuid,parentid_master Varchar,username Varchar,userrole Varchar,password Varchar,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/09/2026 05:55:12*/
			  		
              
                RETURN QUERY
				SELECT  
				tenant.tenantid
,tenant.businessname
,tenant.natureofbusiness
,tenant.businessemail
,tenant.businessphone
,tenant.businesswebsite
,tenant.organizationlogo
,tenant.numberofemployees
,tenant.addressline1
,tenant.addressline2
,tenant.city
,tenant.statename
,tenant.zip
,tenant.country
,tenant.parentid
,CAST(_tenant.businessname AS VARCHAR) as parentid_master
,tenant.username
,tenant.userrole
,tenant.password

				
				,tenant.createduser,tenant.createddate,tenant.modifieduser,tenant.modifieddate
				FROM  tenant 
LEFT OUTER JOIN tenant _tenant ON tenant.parentid=_tenant.tenantid

				WHERE tenant.isdeleted=false 

				 ORDER BY tenant.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

