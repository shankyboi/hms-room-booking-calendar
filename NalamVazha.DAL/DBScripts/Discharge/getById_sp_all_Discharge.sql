
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Discharge"
              (
			  pvar_Dischargeid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Dischargeid" uuid
,ipdnumber Varchar
,patient Varchar
,room Varchar
,discharge Varchar
,daysofstay int
,pendingamount Varchar
,paymentstatus Varchar
,refundamount Varchar
,refundstatus Varchar
,feedbackstatus Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:43:40*/
			  		 
              RETURN QUERY
			  SELECT  
				 Discharge.tenantid
,tenant.businessname as _tenantname
,Discharge.Dischargeid
,CAST(_IPDApplicationForm.bookingreferencenumber AS VARCHAR) as ipdnumber
,CAST(__PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber AS VARCHAR) as patient
,CAST(___Room.roomnumber AS VARCHAR) as room
,CAST(____RoomAllocation.todate AS VARCHAR) as discharge
,Discharge.daysofstay
,CAST(_____BillingPayment.amount AS VARCHAR) as pendingamount
,CAST(______BillingPayment.paymentstatus AS VARCHAR) as paymentstatus
,CAST(_______BillingPayment.refundedamount AS VARCHAR) as refundamount
,CAST(________BillingPayment.refundstatus AS VARCHAR) as refundstatus
,Discharge.feedbackstatus

				 ,Discharge.createduser,Discharge.createddate,Discharge.modifieduser,Discharge.modifieddate
                 
                 
				 
			  FROM  Discharge 
 LEFT OUTER JOIN tenant ON Discharge.tenantid=tenant.tenantid
LEFT OUTER JOIN IPDApplicationForm _IPDApplicationForm ON Discharge.ipdnumber=_IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN PatientProfile __PatientProfile ON Discharge.patient=__PatientProfile.PatientProfileid
LEFT OUTER JOIN Room ___Room ON Discharge.room=___Room.Roomid
INNER JOIN RoomAllocation ____RoomAllocation ON Discharge.discharge=____RoomAllocation.RoomAllocationid
LEFT OUTER JOIN BillingPayment _____BillingPayment ON Discharge.pendingamount=_____BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment ______BillingPayment ON Discharge.paymentstatus=______BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment _______BillingPayment ON Discharge.refundamount=_______BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment ________BillingPayment ON Discharge.refundstatus=________BillingPayment.BillingPaymentid

			  WHERE CAST(Discharge.Dischargeid AS Varchar)=pvar_Dischargeid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

