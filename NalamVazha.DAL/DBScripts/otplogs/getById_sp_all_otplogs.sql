
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_otplogs"
              (
			  pvar_otplogsid Varchar
			  )
              RETURNS TABLE(
                "otplogsid" uuid
,username Varchar
,otpcode int
,expirytime Varchar
,isused Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:45*/
			  		 
              RETURN QUERY
			  SELECT  
				 otplogs.otplogsid
,otplogs.username
,otplogs.otpcode
,CAST(COALESCE(to_char(otplogs.expirytime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as expirytime
,CAST(case when otplogs.isused=true then 'Yes' else 'No' End AS Varchar) as isused

				 ,otplogs.createduser,otplogs.createddate,otplogs.modifieduser,otplogs.modifieddate
                 
                 
				 
			  FROM  otplogs 

			  WHERE CAST(otplogs.otplogsid AS Varchar)=pvar_otplogsid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

