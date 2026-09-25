
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Arrival"
              (
			  pvar_Arrivalid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Arrivalid" uuid
,ipdnumber Varchar
,patient Varchar
,room Varchar
,estimatedarrival Varchar
,bookingstatus Varchar
,travelarrangement Varchar
,pickupfrom Varchar
,wheelchairassistance Varchar
,requireddinner Varchar
,specialrequest Varchar
,paymentstatus Varchar
,pendingamount Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 08:07:23*/
			  		 
              RETURN QUERY
			  SELECT  
				 Arrival.tenantid
,tenant.businessname as _tenantname
,Arrival.Arrivalid
,CAST(_IPDApplicationForm.bookingreferencenumber AS VARCHAR) as ipdnumber
,CAST(__PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber AS VARCHAR) as patient
,CAST(___Room.roomnumber AS VARCHAR) as room
,CAST(____IPDApplicationForm.estimatedarrival AS VARCHAR) as estimatedarrival
,CAST(_____IPDApplicationForm.bookingstatus AS VARCHAR) as bookingstatus
,CAST(______IPDApplicationForm.travelarrangement||' '||______IPDApplicationForm.typeoftravelrequired AS VARCHAR) as travelarrangement
,CAST(_______IPDApplicationForm.pickupfrom AS VARCHAR) as pickupfrom
,CAST(________IPDApplicationForm.wheelchairassistance AS VARCHAR) as wheelchairassistance
,CAST(_________IPDApplicationForm.requireddinner AS VARCHAR) as requireddinner
,CAST(__________IPDApplicationForm.specialrequest AS VARCHAR) as specialrequest
,CAST(___________BillingPayment.paymentstatus AS VARCHAR) as paymentstatus
,CAST(____________BillingPayment.amount AS VARCHAR) as pendingamount

				 ,Arrival.createduser,Arrival.createddate,Arrival.modifieduser,Arrival.modifieddate
                 
                 
				 
			  FROM  Arrival 
 LEFT OUTER JOIN tenant ON Arrival.tenantid=tenant.tenantid
LEFT OUTER JOIN IPDApplicationForm _IPDApplicationForm ON Arrival.ipdnumber=_IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN PatientProfile __PatientProfile ON Arrival.patient=__PatientProfile.PatientProfileid
LEFT OUTER JOIN Room ___Room ON Arrival.room=___Room.Roomid
LEFT OUTER JOIN IPDApplicationForm ____IPDApplicationForm ON Arrival.estimatedarrival=____IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN IPDApplicationForm _____IPDApplicationForm ON Arrival.bookingstatus=_____IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN IPDApplicationForm ______IPDApplicationForm ON Arrival.travelarrangement=______IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN IPDApplicationForm _______IPDApplicationForm ON Arrival.pickupfrom=_______IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN IPDApplicationForm ________IPDApplicationForm ON Arrival.wheelchairassistance=________IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN IPDApplicationForm _________IPDApplicationForm ON Arrival.requireddinner=_________IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN IPDApplicationForm __________IPDApplicationForm ON Arrival.specialrequest=__________IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN BillingPayment ___________BillingPayment ON Arrival.paymentstatus=___________BillingPayment.BillingPaymentid
LEFT OUTER JOIN BillingPayment ____________BillingPayment ON Arrival.pendingamount=____________BillingPayment.BillingPaymentid

			  WHERE CAST(Arrival.Arrivalid AS Varchar)=pvar_Arrivalid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

