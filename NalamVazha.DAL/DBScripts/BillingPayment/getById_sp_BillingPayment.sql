 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_BillingPayment"
			  (
				  pvar_BillingPaymentid Varchar
			  )
			  RETURNS TABLE(
                receiptno Varchar
,paymentfor Varchar
,patientvisit uuid
,patientname uuid
,amount decimal
,paymentmode Varchar
,transactionreference Varchar
,paymentstatus Varchar
,collectedby uuid
,counterid Varchar
,remarks Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,BillingPaymentid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:29*/
               
              RETURN QUERY
			  SELECT 
				 BillingPayment.receiptno
,BillingPayment.paymentfor
,BillingPayment.patientvisit
,BillingPayment.patientname
,BillingPayment.amount
,BillingPayment.paymentmode
,BillingPayment.transactionreference
,BillingPayment.paymentstatus
,BillingPayment.collectedby
,BillingPayment.counterid
,BillingPayment.remarks

				 ,BillingPayment.createduser,BillingPayment.createddate,BillingPayment.modifieduser,BillingPayment.modifieddate
				 ,BillingPayment.tenantid
                 ,BillingPayment.BillingPaymentid
                    
			  FROM BillingPayment
			  WHERE CAST(BillingPayment.BillingPaymentid AS Varchar)=pvar_BillingPaymentid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

