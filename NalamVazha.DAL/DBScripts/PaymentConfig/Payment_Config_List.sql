
			  CREATE OR REPLACE FUNCTION  "Payment_Config_List"
              (pvar_tenantid Varchar
,pvar_paymentgatewayprovider Varchar(1024)
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,PaymentConfigid uuid
,paymentgatewayprovider Varchar,"automaton_PaymentConfig_keyinfo" json,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:16*/
			  		
                SELECT  SPLIT_PART(pvar_tenantid, '|', 1),SPLIT_PART(pvar_tenantid, '|', 2) into lstr_usersid,pvar_tenantid;
		        
                if(pvar_tenantid is null or pvar_tenantid='' or pvar_tenantid='00000000-0000-0000-0000-000000000000')	
				then
                    SELECT STRING_TO_ARRAY(viewertenantids, ',') into lvar_tenantid
				    FROM users where users.usersid::varchar=lstr_usersid;	
                    if(lvar_tenantid is NULL)
					then 
						SELECT array_agg(tenant.tenantid) INTO lvar_tenantid FROM tenant;
               
					end if;
                else 
				  lvar_tenantid=ARRAY[pvar_tenantid];
                end if;
                lvar_tenantid := lvar_tenantid || ARRAY[''::character varying] || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];



              
                RETURN QUERY
				SELECT  
				PaymentConfig.tenantid
,tenant.businessname as _tenantName
,PaymentConfig.PaymentConfigid
,PaymentConfig.paymentgatewayprovider

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

				,PaymentConfig.createduser,PaymentConfig.createddate,PaymentConfig.modifieduser,PaymentConfig.modifieddate
				FROM  PaymentConfig 
 LEFT OUTER JOIN tenant ON PaymentConfig.tenantid=tenant.tenantid

				WHERE (lvar_tenantid is null or COALESCE(cast(PaymentConfig.tenantid as varchar), '') = Any(lvar_tenantid)) AND PaymentConfig.isdeleted=false
AND (pvar_paymentgatewayprovider is null or pvar_paymentgatewayprovider ='0' or LENGTH(CAST(pvar_paymentgatewayprovider as Varchar))=0 or CAST(PaymentConfig.paymentgatewayprovider as VARCHAR)=pvar_paymentgatewayprovider)

				 ORDER BY PaymentConfig.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

