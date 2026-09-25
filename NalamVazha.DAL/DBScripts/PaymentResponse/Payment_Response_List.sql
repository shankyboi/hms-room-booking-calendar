
CREATE OR REPLACE FUNCTION public."Payment_Response_List"(
	pvar_tenantid character varying,
	pvar_paymentrequest character varying,
	pvar_paymenttype character varying,
	pvar_status character varying,
	pvar_paymentmethod character varying,
	pvar_pagesize integer,
	pvar_pagenumber integer,
	pvar_searchterm character varying,
	pvar_sort_fields json)
    RETURNS json
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
               declare local_sortcolumn_array text[] = (
	                select array_agg(col) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)
                );
                declare local_sortorder_array text[] = (
	                select array_agg(dir) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)	
                );          
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;  
               
          	  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 07:53:43*/
			  		
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

                    if(pvar_searchterm is not null and LENGTH(CAST(pvar_searchterm as Varchar)) > 0)
                    then
                    pvar_searchterm := '%' || pvar_searchterm || '%';
                    else
                    pvar_searchterm := null;
                    end if;
              
                    RETURN json_build_object(
                    'count'
                    ,(SELECT  
                    COUNT(*)
                    FROM  PaymentResponse 
 LEFT OUTER JOIN tenant ON PaymentResponse.tenantid=tenant.tenantid
LEFT OUTER JOIN PaymentRequest _PaymentRequest ON PaymentResponse.paymentrequest=_PaymentRequest.PaymentRequestid

                    WHERE (lvar_tenantid is null or COALESCE(cast(PaymentResponse.tenantid as varchar), '') = Any(lvar_tenantid)) AND PaymentResponse.isdeleted=false
AND (pvar_paymentrequest is null or pvar_paymentrequest ='0' or LENGTH(CAST(pvar_paymentrequest as Varchar))=0 or CAST(PaymentResponse.paymentrequest as VARCHAR)=pvar_paymentrequest)
AND (pvar_paymenttype is null or pvar_paymenttype ='0' or LENGTH(CAST(pvar_paymenttype as Varchar))=0 or CAST(PaymentResponse.paymenttype as VARCHAR)=pvar_paymenttype)
AND (pvar_status is null or pvar_status ='0' or LENGTH(CAST(pvar_status as Varchar))=0 or CAST(PaymentResponse.status as VARCHAR)=pvar_status)
AND (pvar_paymentmethod is null or pvar_paymentmethod ='0' or LENGTH(CAST(pvar_paymentmethod as Varchar))=0 or CAST(PaymentResponse.paymentmethod as VARCHAR)=pvar_paymentmethod)
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_PaymentRequest.merchantid||' '||_PaymentRequest.orderid||' '||_PaymentRequest.customername AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.paymenttype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(PaymentResponse.transactiontime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.orderid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.paymentid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.status AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.amount AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.paymentmethod AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.banktransactionid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.gatewayresponsecode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.gatewayresponsemessage AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.responsesignature AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.refundedamount AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.refundreason AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    PaymentResponse.tenantid
,tenant.businessname as _tenantName
,PaymentResponse.PaymentResponseid
,PaymentResponse.paymentrequest
,CAST(_PaymentRequest.merchantid||' '||_PaymentRequest.orderid||' '||_PaymentRequest.customername AS VARCHAR) as paymentrequest_master
,CAST(_PaymentRequest.customername AS VARCHAR) as patientname
,_PaymentRequest.paymentid as paymentrequest_reference
,PaymentResponse.paymenttype
,CAST(COALESCE(to_char(PaymentResponse.transactiontime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as transactiontime
,PaymentResponse.orderid
,PaymentResponse.paymentid
,PaymentResponse.status
,PaymentResponse.amount
,PaymentResponse.paymentmethod
,PaymentResponse.banktransactionid
,PaymentResponse.gatewayresponsecode
,PaymentResponse.gatewayresponsemessage
,PaymentResponse.responsesignature
,PaymentResponse.refundedamount
,PaymentResponse.refundreason

                    
                    ,PaymentResponse.createduser,PaymentResponse.createddate,PaymentResponse.modifieduser,PaymentResponse.modifieddate
                    FROM  PaymentResponse 
 LEFT OUTER JOIN tenant ON PaymentResponse.tenantid=tenant.tenantid
LEFT OUTER JOIN PaymentRequest _PaymentRequest ON PaymentResponse.paymentrequest=_PaymentRequest.PaymentRequestid

                    WHERE (lvar_tenantid is null or COALESCE(cast(PaymentResponse.tenantid as varchar), '') = Any(lvar_tenantid)) AND PaymentResponse.isdeleted=false
AND (pvar_paymentrequest is null or pvar_paymentrequest ='0' or LENGTH(CAST(pvar_paymentrequest as Varchar))=0 or CAST(PaymentResponse.paymentrequest as VARCHAR)=pvar_paymentrequest)
AND (pvar_paymenttype is null or pvar_paymenttype ='0' or LENGTH(CAST(pvar_paymenttype as Varchar))=0 or CAST(PaymentResponse.paymenttype as VARCHAR)=pvar_paymenttype)
AND (pvar_status is null or pvar_status ='0' or LENGTH(CAST(pvar_status as Varchar))=0 or CAST(PaymentResponse.status as VARCHAR)=pvar_status)
AND (pvar_paymentmethod is null or pvar_paymentmethod ='0' or LENGTH(CAST(pvar_paymentmethod as Varchar))=0 or CAST(PaymentResponse.paymentmethod as VARCHAR)=pvar_paymentmethod)

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_PaymentRequest.merchantid||' '||_PaymentRequest.orderid||' '||_PaymentRequest.customername AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.paymenttype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(PaymentResponse.transactiontime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.orderid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.paymentid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.status AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.amount AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.paymentmethod AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.banktransactionid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.gatewayresponsecode AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.gatewayresponsemessage AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.responsesignature AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.refundedamount AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentResponse.refundreason AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'paymentrequest' THEN _PaymentRequest.merchantid||' '||_PaymentRequest.orderid||' '||_PaymentRequest.customername::TEXT
WHEN 'paymenttype' THEN PaymentResponse.paymenttype::TEXT
WHEN 'orderid' THEN PaymentResponse.orderid::TEXT
WHEN 'paymentid' THEN PaymentResponse.paymentid::TEXT
WHEN 'status' THEN PaymentResponse.status::TEXT
WHEN 'amount' THEN PaymentResponse.amount::TEXT
WHEN 'paymentmethod' THEN PaymentResponse.paymentmethod::TEXT
WHEN 'banktransactionid' THEN PaymentResponse.banktransactionid::TEXT
WHEN 'gatewayresponsecode' THEN PaymentResponse.gatewayresponsecode::TEXT
WHEN 'gatewayresponsemessage' THEN PaymentResponse.gatewayresponsemessage::TEXT
WHEN 'responsesignature' THEN PaymentResponse.responsesignature::TEXT
WHEN 'refundedamount' THEN PaymentResponse.refundedamount::TEXT
WHEN 'refundreason' THEN PaymentResponse.refundreason::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'transactiontime' THEN PaymentResponse.transactiontime
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'amount' THEN PaymentResponse.amount::NUMERIC
WHEN 'refundedamount' THEN PaymentResponse.refundedamount::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'paymentrequest' THEN _PaymentRequest.merchantid||' '||_PaymentRequest.orderid||' '||_PaymentRequest.customername::TEXT
WHEN 'paymenttype' THEN PaymentResponse.paymenttype::TEXT
WHEN 'orderid' THEN PaymentResponse.orderid::TEXT
WHEN 'paymentid' THEN PaymentResponse.paymentid::TEXT
WHEN 'status' THEN PaymentResponse.status::TEXT
WHEN 'amount' THEN PaymentResponse.amount::TEXT
WHEN 'paymentmethod' THEN PaymentResponse.paymentmethod::TEXT
WHEN 'banktransactionid' THEN PaymentResponse.banktransactionid::TEXT
WHEN 'gatewayresponsecode' THEN PaymentResponse.gatewayresponsecode::TEXT
WHEN 'gatewayresponsemessage' THEN PaymentResponse.gatewayresponsemessage::TEXT
WHEN 'responsesignature' THEN PaymentResponse.responsesignature::TEXT
WHEN 'refundedamount' THEN PaymentResponse.refundedamount::TEXT
WHEN 'refundreason' THEN PaymentResponse.refundreason::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'transactiontime' THEN PaymentResponse.transactiontime
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'amount' THEN PaymentResponse.amount::NUMERIC
WHEN 'refundedamount' THEN PaymentResponse.refundedamount::NUMERIC
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC,
PaymentResponse.transactiontime DESC NULLS LAST
                    limit pvar_pagesize
                    offset pvar_pagenumber * pvar_pagesize			 	
			 	
                    ) d));
	
			  
					 	
			  END
              
$BODY$;
