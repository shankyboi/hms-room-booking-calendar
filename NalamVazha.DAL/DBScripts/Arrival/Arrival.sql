CREATE TABLE IF NOT EXISTS Arrival
(
Arrivalid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,ipdnumber uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NULL
,patient uuid REFERENCES PatientProfile(PatientProfileid) NULL
,room uuid REFERENCES Room(Roomid) NULL
,estimatedarrival uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NULL
,bookingstatus uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NULL
,travelarrangement uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NULL
,pickupfrom uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NULL
,wheelchairassistance uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NULL
,requireddinner uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NULL
,specialrequest uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NULL
,paymentstatus uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,pendingamount uuid REFERENCES BillingPayment(BillingPaymentid) NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


