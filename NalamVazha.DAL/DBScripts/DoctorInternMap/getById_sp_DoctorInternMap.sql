 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_DoctorInternMap"
			  (
				  pvar_DoctorInternMapid Varchar
			  )
			  RETURNS TABLE(
                seniordoctor uuid
,interndoctor uuid
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,DoctorInternMapid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 07/21/2026 11:46:55*/
               
              RETURN QUERY
			  SELECT 
				 DoctorInternMap.seniordoctor
,DoctorInternMap.interndoctor

				 ,DoctorInternMap.createduser,DoctorInternMap.createddate,DoctorInternMap.modifieduser,DoctorInternMap.modifieddate
				 ,DoctorInternMap.tenantid
                 ,DoctorInternMap.DoctorInternMapid
                    
			  FROM DoctorInternMap
			  WHERE CAST(DoctorInternMap.DoctorInternMapid AS Varchar)=pvar_DoctorInternMapid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

