CREATE OR REPLACE FUNCTION "getById_sp_PaymentConfig_keyinfo"(
												 pvar_PaymentConfigid Varchar(50)
											 )
                                             RETURNS TABLE("PaymentConfigid" uuid,"PaymentConfig_keyinfoid" uuid ,keytype Varchar
,keyid Varchar
,keysecret Varchar
,returnurl Varchar
,notifyurl Varchar
,merchantid Varchar
,status Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
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
											 CAST(PaymentConfig_keyinfo.PaymentConfigid AS VARCHAR)=pvar_PaymentConfigid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

