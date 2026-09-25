 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_users"
			  (
				  pvar_usersid Varchar
			  )
			  RETURNS TABLE(
                firstname Varchar
,lastname Varchar
,profilepicture Varchar
,username Varchar
,userpassword Varchar
,passwordkey Varchar
,emailid Varchar
,mobilenumber Varchar
,userrole Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,usersid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:21*/
               
              RETURN QUERY
			  SELECT 
				 users.firstname
,users.lastname
,users.profilepicture
,users.username
,users.userpassword
,users.passwordkey
,users.emailid
,users.mobilenumber
,users.userrole

				 ,users.createduser,users.createddate,users.modifieduser,users.modifieddate
				 ,users.tenantid
                 ,users.usersid
                    
			  FROM users
			  WHERE CAST(users.usersid AS Varchar)=pvar_usersid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

