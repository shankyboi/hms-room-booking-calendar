
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Blacklistreason"
              (
			  pvar_Blacklistreasonid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Blacklistreasonid" uuid
,reason Varchar
,reasondesc Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:47*/
			  		 
              RETURN QUERY
			  SELECT  
				 Blacklistreason.tenantid
,tenant.businessname as _tenantname
,Blacklistreason.Blacklistreasonid
,Blacklistreason.reason
,Blacklistreason.reasondesc

				 ,Blacklistreason.createduser,Blacklistreason.createddate,Blacklistreason.modifieduser,Blacklistreason.modifieddate
                 
                 
				 
			  FROM  Blacklistreason 
 LEFT OUTER JOIN tenant ON Blacklistreason.tenantid=tenant.tenantid

			  WHERE CAST(Blacklistreason.Blacklistreasonid AS Varchar)=pvar_Blacklistreasonid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

