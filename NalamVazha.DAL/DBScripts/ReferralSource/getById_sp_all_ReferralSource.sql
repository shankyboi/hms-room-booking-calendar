
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_ReferralSource"
              (
			  pvar_ReferralSourceid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"ReferralSourceid" uuid
,referralsourcename Varchar
,contactnumber Varchar
,websiteurl Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:40*/
			  		 
              RETURN QUERY
			  SELECT  
				 ReferralSource.tenantid
,tenant.businessname as _tenantname
,ReferralSource.ReferralSourceid
,ReferralSource.referralsourcename
,ReferralSource.contactnumber
,ReferralSource.websiteurl

				 ,ReferralSource.createduser,ReferralSource.createddate,ReferralSource.modifieduser,ReferralSource.modifieddate
                 
                 
				 
			  FROM  ReferralSource 
 LEFT OUTER JOIN tenant ON ReferralSource.tenantid=tenant.tenantid

			  WHERE CAST(ReferralSource.ReferralSourceid AS Varchar)=pvar_ReferralSourceid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

