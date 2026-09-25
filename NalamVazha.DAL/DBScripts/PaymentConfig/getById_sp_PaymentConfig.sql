 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_PaymentConfig"
			  (
				  pvar_PaymentConfigid Varchar
			  )
			  RETURNS TABLE(
                paymentgatewayprovider Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,PaymentConfigid uuid
                ,keyinfo JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:16*/
               
              RETURN QUERY
			  SELECT 
				 PaymentConfig.paymentgatewayprovider

				 ,PaymentConfig.createduser,PaymentConfig.createddate,PaymentConfig.modifieduser,PaymentConfig.modifieddate
				 ,PaymentConfig.tenantid
                 ,PaymentConfig.PaymentConfigid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 PaymentConfig_keyinfo.PaymentConfigid
                                             ,PaymentConfig_keyinfo.PaymentConfig_keyinfoid   
											 ,PaymentConfig_keyinfo.keytype
,PaymentConfig_keyinfo.keyid
,PaymentConfig_keyinfo.keysecret
,PaymentConfig_keyinfo.returnurl
,PaymentConfig_keyinfo.notifyurl
,PaymentConfig_keyinfo.merchantid
,PaymentConfig_keyinfo.status
 
											  
											 FROM PaymentConfig_keyinfo
											 WHERE 
											 PaymentConfig_keyinfo.PaymentConfigid=PaymentConfig.PaymentConfigid
                                             
                                             ORDER BY record_order DESC
											) J) as keyinfo
   
			  FROM PaymentConfig
			  WHERE CAST(PaymentConfig.PaymentConfigid AS Varchar)=pvar_PaymentConfigid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

