
CREATE OR REPLACE FUNCTION public."Payment_Request_List"(
	pvar_tenantid character varying,
	pvar_paymentgateway character varying,
	pvar_requestdatetime_automatonfrom character varying,
	pvar_requestdatetime_automatonto character varying,
	pvar_patientname character varying,
	pvar_people character varying,
	pvar_paymentid character varying,
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
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 08:09:46*/
			  		
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
                    FROM  PaymentRequest 
 LEFT OUTER JOIN tenant ON PaymentRequest.tenantid=tenant.tenantid
LEFT OUTER JOIN PatientProfile _PatientProfile ON PaymentRequest.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN People __People ON PaymentRequest.people=__People.Peopleid

                    WHERE (lvar_tenantid is null or COALESCE(cast(PaymentRequest.tenantid as varchar), '') = Any(lvar_tenantid)) AND PaymentRequest.isdeleted=false
AND (pvar_paymentgateway is null or pvar_paymentgateway ='0' or LENGTH(CAST(pvar_paymentgateway as Varchar))=0 or CAST(PaymentRequest.paymentgateway as VARCHAR)=pvar_paymentgateway)
 AND(pvar_requestdatetime_automatonfrom IS NULL OR pvar_requestdatetime_automatonfrom = '' OR PaymentRequest.requestdatetime >= CAST(pvar_requestdatetime_automatonfrom AS TIMESTAMP(3))) 
                                AND (pvar_requestdatetime_automatonto IS NULL OR pvar_requestdatetime_automatonto = '' OR PaymentRequest.requestdatetime <= CAST(pvar_requestdatetime_automatonto AS TIMESTAMP(3)))
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(PaymentRequest.patientname as VARCHAR)=pvar_patientname)
AND (pvar_people is null or pvar_people ='0' or LENGTH(CAST(pvar_people as Varchar))=0 or CAST(PaymentRequest.people as VARCHAR)=pvar_people)
AND (pvar_paymentid is null or pvar_paymentid ='0' or LENGTH(CAST(pvar_paymentid as Varchar))=0 or CAST(PaymentRequest.paymentid as VARCHAR)=pvar_paymentid)
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.paymentgateway AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(PaymentRequest.requestdatetime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CONCAT_WS(' ', NULLIF(_PatientProfile.firstname,''), NULLIF(_PatientProfile.lastname,''), NULLIF(_PatientProfile.mobilenumber,'')) AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__People.firstname||' '||__People.lastname||' '||__People.contactnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.paymenttype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.merchantid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.orderid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.paymentid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.amount AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.currency AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.customername AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.customeremail AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.customerphone AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.orderpaymentdesc AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.returnurl AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.notifyurl AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.signatureorchecksum AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    PaymentRequest.tenantid
,tenant.businessname as _tenantName
,PaymentRequest.PaymentRequestid
,PaymentRequest.paymentgateway
,CAST(COALESCE(to_char(PaymentRequest.requestdatetime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as requestdatetime
,PaymentRequest.patientname
,CAST(CONCAT_WS(' ', NULLIF(_PatientProfile.firstname,''), NULLIF(_PatientProfile.lastname,''), NULLIF(_PatientProfile.mobilenumber,'')) AS VARCHAR) as patientname_master
,PaymentRequest.people
,CAST(__People.firstname||' '||__People.lastname||' '||__People.contactnumber AS VARCHAR) as people_master
,PaymentRequest.paymenttype
,PaymentRequest.merchantid
,PaymentRequest.orderid
,PaymentRequest.paymentid
,PaymentRequest.amount
,PaymentRequest.currency
,PaymentRequest.customername
,PaymentRequest.customeremail
,PaymentRequest.customerphone
,PaymentRequest.orderpaymentdesc
,PaymentRequest.returnurl
,PaymentRequest.notifyurl
,PaymentRequest.signatureorchecksum

                    
                    ,PaymentRequest.createduser,PaymentRequest.createddate,PaymentRequest.modifieduser,PaymentRequest.modifieddate
                    FROM  PaymentRequest 
 LEFT OUTER JOIN tenant ON PaymentRequest.tenantid=tenant.tenantid
LEFT OUTER JOIN PatientProfile _PatientProfile ON PaymentRequest.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN People __People ON PaymentRequest.people=__People.Peopleid

                    WHERE (lvar_tenantid is null or COALESCE(cast(PaymentRequest.tenantid as varchar), '') = Any(lvar_tenantid)) AND PaymentRequest.isdeleted=false
AND (pvar_paymentgateway is null or pvar_paymentgateway ='0' or LENGTH(CAST(pvar_paymentgateway as Varchar))=0 or CAST(PaymentRequest.paymentgateway as VARCHAR)=pvar_paymentgateway)
 AND(pvar_requestdatetime_automatonfrom IS NULL OR pvar_requestdatetime_automatonfrom = '' OR PaymentRequest.requestdatetime >= CAST(pvar_requestdatetime_automatonfrom AS TIMESTAMP(3))) 
                                AND (pvar_requestdatetime_automatonto IS NULL OR pvar_requestdatetime_automatonto = '' OR PaymentRequest.requestdatetime <= CAST(pvar_requestdatetime_automatonto AS TIMESTAMP(3)))
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(PaymentRequest.patientname as VARCHAR)=pvar_patientname)
AND (pvar_people is null or pvar_people ='0' or LENGTH(CAST(pvar_people as Varchar))=0 or CAST(PaymentRequest.people as VARCHAR)=pvar_people)
AND (pvar_paymentid is null or pvar_paymentid ='0' or LENGTH(CAST(pvar_paymentid as Varchar))=0 or CAST(PaymentRequest.paymentid as VARCHAR)=pvar_paymentid)

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.paymentgateway AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(PaymentRequest.requestdatetime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(CONCAT_WS(' ', NULLIF(_PatientProfile.firstname,''), NULLIF(_PatientProfile.lastname,''), NULLIF(_PatientProfile.mobilenumber,'')) AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__People.firstname||' '||__People.lastname||' '||__People.contactnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.paymenttype AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.merchantid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.orderid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.paymentid AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.amount AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.currency AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.customername AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.customeremail AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.customerphone AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.orderpaymentdesc AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.returnurl AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.notifyurl AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(PaymentRequest.signatureorchecksum AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'paymentgateway' THEN PaymentRequest.paymentgateway::TEXT
WHEN 'patientname' THEN CONCAT_WS(' ', NULLIF(_PatientProfile.firstname,''), NULLIF(_PatientProfile.lastname,''), NULLIF(_PatientProfile.mobilenumber,''))::TEXT
WHEN 'people' THEN __People.firstname||' '||__People.lastname||' '||__People.contactnumber::TEXT
WHEN 'paymenttype' THEN PaymentRequest.paymenttype::TEXT
WHEN 'merchantid' THEN PaymentRequest.merchantid::TEXT
WHEN 'orderid' THEN PaymentRequest.orderid::TEXT
WHEN 'paymentid' THEN PaymentRequest.paymentid::TEXT
WHEN 'amount' THEN PaymentRequest.amount::TEXT
WHEN 'currency' THEN PaymentRequest.currency::TEXT
WHEN 'customername' THEN PaymentRequest.customername::TEXT
WHEN 'customeremail' THEN PaymentRequest.customeremail::TEXT
WHEN 'customerphone' THEN PaymentRequest.customerphone::TEXT
WHEN 'customerphone' THEN PaymentRequest.customerphone::TEXT
WHEN 'orderpaymentdesc' THEN PaymentRequest.orderpaymentdesc::TEXT
WHEN 'returnurl' THEN PaymentRequest.returnurl::TEXT
WHEN 'notifyurl' THEN PaymentRequest.notifyurl::TEXT
WHEN 'signatureorchecksum' THEN PaymentRequest.signatureorchecksum::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'requestdatetime' THEN PaymentRequest.requestdatetime
		WHEN 'createddate' THEN PaymentRequest.createddate
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'amount' THEN PaymentRequest.amount::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'paymentgateway' THEN PaymentRequest.paymentgateway::TEXT
WHEN 'patientname' THEN CONCAT_WS(' ', NULLIF(_PatientProfile.firstname,''), NULLIF(_PatientProfile.lastname,''), NULLIF(_PatientProfile.mobilenumber,''))::TEXT
WHEN 'people' THEN __People.firstname||' '||__People.lastname||' '||__People.contactnumber::TEXT
WHEN 'paymenttype' THEN PaymentRequest.paymenttype::TEXT
WHEN 'merchantid' THEN PaymentRequest.merchantid::TEXT
WHEN 'orderid' THEN PaymentRequest.orderid::TEXT
WHEN 'paymentid' THEN PaymentRequest.paymentid::TEXT
WHEN 'amount' THEN PaymentRequest.amount::TEXT
WHEN 'currency' THEN PaymentRequest.currency::TEXT
WHEN 'customername' THEN PaymentRequest.customername::TEXT
WHEN 'customeremail' THEN PaymentRequest.customeremail::TEXT
WHEN 'customerphone' THEN PaymentRequest.customerphone::TEXT
WHEN 'customerphone' THEN PaymentRequest.customerphone::TEXT
WHEN 'orderpaymentdesc' THEN PaymentRequest.orderpaymentdesc::TEXT
WHEN 'returnurl' THEN PaymentRequest.returnurl::TEXT
WHEN 'notifyurl' THEN PaymentRequest.notifyurl::TEXT
WHEN 'signatureorchecksum' THEN PaymentRequest.signatureorchecksum::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'requestdatetime' THEN PaymentRequest.requestdatetime
			WHEN 'createddate' THEN PaymentRequest.createddate
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'amount' THEN PaymentRequest.amount::NUMERIC
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC,
                    PaymentRequest.requestdatetime DESC NULLS LAST

                    limit pvar_pagesize
                    offset pvar_pagenumber * pvar_pagesize			 	
			 	
                    ) d));
	
			  
					 	
			  END
              
$BODY$;
