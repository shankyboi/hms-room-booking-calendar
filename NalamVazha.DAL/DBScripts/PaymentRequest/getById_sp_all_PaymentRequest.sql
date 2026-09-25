
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_PaymentRequest"
              (
			  pvar_PaymentRequestid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"PaymentRequestid" uuid
,paymentgateway Varchar
,requestdatetime Varchar
,patientname Varchar
,people Varchar
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

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:48*/
			  		 
              RETURN QUERY
			  SELECT  
				 PaymentRequest.tenantid
,tenant.businessname as _tenantname
,PaymentRequest.PaymentRequestid
,PaymentRequest.paymentgateway
,CAST(COALESCE(to_char(PaymentRequest.requestdatetime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as requestdatetime
,CAST(CONCAT_WS(' ', NULLIF(_PatientProfile.firstname,''), NULLIF(_PatientProfile.lastname,''), NULLIF(_PatientProfile.mobilenumber,'')) AS VARCHAR) as patientname
,CAST(__People.firstname||' '||__People.lastname||' '||__People.contactnumber AS VARCHAR) as people
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

			  WHERE CAST(PaymentRequest.PaymentRequestid AS Varchar)=pvar_PaymentRequestid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

