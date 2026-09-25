
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_DoctorInternMap"
              (
			  pvar_DoctorInternMapid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"DoctorInternMapid" uuid
,seniordoctor Varchar
,interndoctor Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 07/21/2026 11:46:55*/
			  		 
              RETURN QUERY
			  SELECT  
				 DoctorInternMap.tenantid
,tenant.businessname as _tenantname
,DoctorInternMap.DoctorInternMapid
,CAST(_People.firstname||' '||_People.lastname AS VARCHAR) as seniordoctor
,CAST(__People.firstname||' '||__People.lastname AS VARCHAR) as interndoctor

				 ,DoctorInternMap.createduser,DoctorInternMap.createddate,DoctorInternMap.modifieduser,DoctorInternMap.modifieddate
                 
                 
				 
			  FROM  DoctorInternMap 
 LEFT OUTER JOIN tenant ON DoctorInternMap.tenantid=tenant.tenantid
INNER JOIN People _People ON DoctorInternMap.seniordoctor=_People.Peopleid
INNER JOIN People __People ON DoctorInternMap.interndoctor=__People.Peopleid

			  WHERE CAST(DoctorInternMap.DoctorInternMapid AS Varchar)=pvar_DoctorInternMapid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

