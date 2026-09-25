 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_Occupation"
              (
			  pvar_tenantid Varchar=null
              )
			  RETURNS TABLE(
                occupationname Varchar
,occupationdesc Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"Occupationid" uuid
               )
               AS $BODY$
               BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:36*/

			 
                RETURN QUERY
                SELECT 
                Occupation.occupationname
,Occupation.occupationdesc

                ,Occupation.createduser,Occupation.createddate,Occupation.modifieduser,Occupation.modifieddate
                ,Occupation.Occupationid
                FROM Occupation
			    
                 WHERE Occupation.isdeleted=false
                ;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

