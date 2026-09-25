
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_EnquiryType"
              (
			  pvar_EnquiryTypeid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"EnquiryTypeid" uuid
,enquiryname Varchar
,enquirydesc Varchar
,isroombookingrelated Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:50*/
			  		 
              RETURN QUERY
			  SELECT  
				 EnquiryType.tenantid
,tenant.businessname as _tenantname
,EnquiryType.EnquiryTypeid
,EnquiryType.enquiryname
,EnquiryType.enquirydesc
,EnquiryType.isroombookingrelated

				 ,EnquiryType.createduser,EnquiryType.createddate,EnquiryType.modifieduser,EnquiryType.modifieddate
                 
                 
				 
			  FROM  EnquiryType 
 LEFT OUTER JOIN tenant ON EnquiryType.tenantid=tenant.tenantid

			  WHERE CAST(EnquiryType.EnquiryTypeid AS Varchar)=pvar_EnquiryTypeid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

