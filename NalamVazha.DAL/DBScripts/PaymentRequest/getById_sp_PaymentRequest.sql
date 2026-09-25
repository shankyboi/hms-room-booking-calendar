 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_PaymentRequest"
			  (
				  pvar_PaymentRequestid Varchar
			  )
			  RETURNS TABLE(
                paymentgateway Varchar
,requestdatetime Timestamp(3)
,patientname uuid
,people uuid
,paymenttype Varchar
,merchantid Varchar
,orderid Varchar
,paymentid Varchar
,amount decimal
,currency Varchar
,customername Varchar
,customeremail Varchar
,customerphone Varchar
,orderpaymentdesc Varchar
,returnurl Varchar
,notifyurl Varchar
,signatureorchecksum Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,PaymentRequestid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:48*/
               
              RETURN QUERY
			  SELECT 
				 PaymentRequest.paymentgateway
,PaymentRequest.requestdatetime
,PaymentRequest.patientname
,PaymentRequest.people
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
				 ,PaymentRequest.tenantid
                 ,PaymentRequest.PaymentRequestid
                    
			  FROM PaymentRequest
			  WHERE CAST(PaymentRequest.PaymentRequestid AS Varchar)=pvar_PaymentRequestid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

