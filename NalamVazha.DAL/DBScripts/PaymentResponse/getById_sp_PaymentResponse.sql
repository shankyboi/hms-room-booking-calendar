 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_PaymentResponse"
			  (
				  pvar_PaymentResponseid Varchar
			  )
			  RETURNS TABLE(
                paymentrequest uuid
,paymenttype Varchar
,transactiontime Timestamp(3)
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
,tenantid uuid

                ,PaymentResponseid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:53*/
               
              RETURN QUERY
			  SELECT 
				 PaymentResponse.paymentrequest
,PaymentResponse.paymenttype
,PaymentResponse.transactiontime
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
				 ,PaymentResponse.tenantid
                 ,PaymentResponse.PaymentResponseid
                    
			  FROM PaymentResponse
			  WHERE CAST(PaymentResponse.PaymentResponseid AS Varchar)=pvar_PaymentResponseid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

