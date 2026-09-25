
			  CREATE OR REPLACE FUNCTION  "List_of_User_Profiles"
              (pvar_tenantid Varchar
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,usersid uuid
,firstname Varchar,lastname Varchar,profilepicture Varchar,username Varchar,userpassword Varchar,passwordkey Varchar,emailid Varchar,mobilenumber Varchar,userrole Varchar,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:22*/
			  		
                SELECT  SPLIT_PART(pvar_tenantid, '|', 1),SPLIT_PART(pvar_tenantid, '|', 2) into lstr_usersid,pvar_tenantid;
                if(pvar_tenantid='' or pvar_tenantid='00000000-0000-0000-0000-000000000000')	
				then
                  lvar_tenantid :=   ARRAY[''::character varying] || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];

                else 
				  lvar_tenantid=ARRAY[pvar_tenantid];
                end if;

              
                RETURN QUERY
				SELECT  
				users.tenantid
,tenant.businessname as _tenantName
,users.usersid
,users.firstname
,users.lastname
,users.profilepicture
,users.username
,users.userpassword
,users.passwordkey
,users.emailid
,users.mobilenumber
,users.userrole

				
				,users.createduser,users.createddate,users.modifieduser,users.modifieddate
				FROM  users 
 LEFT OUTER JOIN tenant ON users.tenantid=tenant.tenantid

				WHERE (lvar_tenantid is null or COALESCE(cast(users.tenantid as varchar), '') = Any(lvar_tenantid)) AND users.isdeleted=false

				 ORDER BY users.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

