 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_otplogs"
			  (
				  pvar_otplogsid Varchar
			  )
			  RETURNS TABLE(
                username Varchar
,otpcode int
,expirytime Timestamp(3)
,isused Boolean
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,otplogsid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:45*/
               
              RETURN QUERY
			  SELECT 
				 otplogs.username
,otplogs.otpcode
,otplogs.expirytime
,COALESCE(otplogs.isused,true) as isused

				 ,otplogs.createduser,otplogs.createddate,otplogs.modifieduser,otplogs.modifieddate
				 
                 ,otplogs.otplogsid
                    
			  FROM otplogs
			  WHERE CAST(otplogs.otplogsid AS Varchar)=pvar_otplogsid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

