
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_PaymentResponse"
              (
			  pvar_PaymentResponseid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"PaymentResponseid" uuid
,paymentrequest Varchar
,paymenttype Varchar
,transactiontime Varchar
,orderid Varchar
,paymentid Varchar
,status Varchar
,amount decimal
,paymentmethod Varchar
,banktransactionid Varchar
,gatewayresponsecode Varchar
,gatewayresponsemessage Varchar
,responsesignature Varchar
,refundedamount decimal
,refundreason Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:53*/
			  		 
              RETURN QUERY
			  SELECT  
				 PaymentResponse.tenantid
,tenant.businessname as _tenantname
,PaymentResponse.PaymentResponseid
,CAST(_PaymentRequest.merchantid||' '||_PaymentRequest.orderid||' '||_PaymentRequest.customername AS VARCHAR) as paymentrequest
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

			  WHERE CAST(PaymentResponse.PaymentResponseid AS Varchar)=pvar_PaymentResponseid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

