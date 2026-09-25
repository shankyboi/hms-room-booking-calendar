 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Finance"
			  (
				  pvar_Financeid Varchar
			  )
			  RETURNS TABLE(
                paymentdate uuid
,paymentmode uuid
,receiptnumber uuid
,patient uuid
,receivablefor uuid
,bookingreferencenumber uuid
,billedamount uuid
,receivedamount uuid
,pendingamount int
,paymentstatus uuid
,collectedby uuid
,refundmode uuid
,refundedamount uuid
,refundedby uuid
,remarks uuid
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Financeid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 10:01:30*/
               
              RETURN QUERY
			  SELECT 
				 Finance.paymentdate
,Finance.paymentmode
,Finance.receiptnumber
,Finance.patient
,Finance.receivablefor
,Finance.bookingreferencenumber
,Finance.billedamount
,Finance.receivedamount
,Finance.pendingamount
,Finance.paymentstatus
,Finance.collectedby
,Finance.refundmode
,Finance.refundedamount
,Finance.refundedby
,Finance.remarks

				 ,Finance.createduser,Finance.createddate,Finance.modifieduser,Finance.modifieddate
				 ,Finance.tenantid
                 ,Finance.Financeid
                    
			  FROM Finance
			  WHERE CAST(Finance.Financeid AS Varchar)=pvar_Financeid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

