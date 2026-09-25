
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Finance"
              (
			  pvar_Financeid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Financeid" uuid
,paymentdate Varchar
,paymentmode Varchar
,receiptnumber Varchar
,patient Varchar
,receivablefor Varchar
,bookingreferencenumber Varchar
,billedamount Varchar
,receivedamount Varchar
,pendingamount int
,paymentstatus Varchar
,collectedby Varchar
,refundmode Varchar
,refundedamount Varchar
,refundedby Varchar
,remarks Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 10:01:30*/
			  		 
              RETURN QUERY
			  SELECT  
				 Finance.tenantid
,tenant.businessname as _tenantname
,Finance.Financeid
,CAST(_BillingPayment.paymentdate AS VARCHAR) as paymentdate
,CAST(__BillingPayment.paymentmode AS VARCHAR) as paymentmode
,CAST(___Receivable.receivableno AS VARCHAR) as receiptnumber
,CAST(____PatientProfile.firstname||' '||____PatientProfile.lastname AS VARCHAR) as patient
,CAST(_____BillingPayment.receivablefor AS VARCHAR) as receivablefor
,CAST(______BillingPayment.ipdnumber||' '||______BillingPayment.opdnumber AS VARCHAR) as bookingreferencenumber
,CAST(_______BillingPayment.amount AS VARCHAR) as billedamount
,CAST(________BillingPayment.receivedamount AS VARCHAR) as receivedamount
,Finance.pendingamount
,CAST(_________BillingPayment.paymentstatus AS VARCHAR) as paymentstatus
,CAST(__________BillingPayment.collectedby AS VARCHAR) as collectedby
,CAST(___________BillingPayment.refundmode AS VARCHAR) as refundmode
,CAST(____________BillingPayment.refundedamount AS VARCHAR) as refundedamount
,CAST(_____________BillingPayment.refundedby AS VARCHAR) as refundedby
,CAST(______________BillingPayment.remarks AS VARCHAR) as remarks

				 ,Finance.createduser,Finance.createddate,Finance.modifieduser,Finance.modifieddate
                 
                 
				 
			  FROM  Finance 
 LEFT OUTER JOIN tenant ON Finance.tenantid=tenant.tenantid
LEFT OUTER JOIN BillingPayment _BillingPayment ON Finance.paymentdate=_BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment __BillingPayment ON Finance.paymentmode=__BillingPayment.BillingPaymentid
LEFT OUTER JOIN Receivable ___Receivable ON Finance.receiptnumber=___Receivable.Receivableid
LEFT OUTER JOIN PatientProfile ____PatientProfile ON Finance.patient=____PatientProfile.PatientProfileid
LEFT OUTER JOIN BillingPayment _____BillingPayment ON Finance.receivablefor=_____BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment ______BillingPayment ON Finance.bookingreferencenumber=______BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment _______BillingPayment ON Finance.billedamount=_______BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment ________BillingPayment ON Finance.receivedamount=________BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment _________BillingPayment ON Finance.paymentstatus=_________BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment __________BillingPayment ON Finance.collectedby=__________BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment ___________BillingPayment ON Finance.refundmode=___________BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment ____________BillingPayment ON Finance.refundedamount=____________BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment _____________BillingPayment ON Finance.refundedby=_____________BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment ______________BillingPayment ON Finance.remarks=______________BillingPayment.BillingPaymentid

			  WHERE CAST(Finance.Financeid AS Varchar)=pvar_Financeid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

