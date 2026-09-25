 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Blacklistreason"
			  (
				  pvar_Blacklistreasonid Varchar
			  )
			  RETURNS TABLE(
                reason Varchar
,reasondesc Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Blacklistreasonid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:47*/
               
              RETURN QUERY
			  SELECT 
				 Blacklistreason.reason
,Blacklistreason.reasondesc

				 ,Blacklistreason.createduser,Blacklistreason.createddate,Blacklistreason.modifieduser,Blacklistreason.modifieddate
				 ,Blacklistreason.tenantid
                 ,Blacklistreason.Blacklistreasonid
                    
			  FROM Blacklistreason
			  WHERE CAST(Blacklistreason.Blacklistreasonid AS Varchar)=pvar_Blacklistreasonid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

