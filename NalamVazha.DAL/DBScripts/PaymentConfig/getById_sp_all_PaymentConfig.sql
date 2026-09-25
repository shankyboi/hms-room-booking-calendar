
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_PaymentConfig"
              (
			  pvar_PaymentConfigid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"PaymentConfigid" uuid
,paymentgatewayprovider Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"automaton_PaymentConfig_keyinfo" json
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:16*/
			  		 
              RETURN QUERY
			  SELECT  
				 PaymentConfig.tenantid
,tenant.businessname as _tenantname
,PaymentConfig.PaymentConfigid
,PaymentConfig.paymentgatewayprovider

				 ,PaymentConfig.createduser,PaymentConfig.createddate,PaymentConfig.modifieduser,PaymentConfig.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						PaymentConfig_keyinfo.keytype as "Key Type"
,PaymentConfig_keyinfo.keyid as "Key ID"
,PaymentConfig_keyinfo.keysecret as "Key Secret"
,PaymentConfig_keyinfo.returnurl as "Return Url"
,PaymentConfig_keyinfo.notifyurl as "notify Url"
,PaymentConfig_keyinfo.merchantid as "Merchant Id"
,PaymentConfig_keyinfo.status as "Status"

							
						FROM  PaymentConfig_keyinfo 

						WHERE PaymentConfig.PaymentConfigid =PaymentConfig_keyinfo.PaymentConfigid
) J)
						as automaton_PaymentConfig_keyinfo

                 
				 
			  FROM  PaymentConfig 
 LEFT OUTER JOIN tenant ON PaymentConfig.tenantid=tenant.tenantid

			  WHERE CAST(PaymentConfig.PaymentConfigid AS Varchar)=pvar_PaymentConfigid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

