CREATE TABLE IF NOT EXISTS Discharge
(
Dischargeid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,ipdnumber uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NULL
,patient uuid REFERENCES PatientProfile(PatientProfileid) NULL
,room uuid REFERENCES Room(Roomid) NULL
,discharge uuid REFERENCES RoomAllocation(RoomAllocationid) NOT NULL
,daysofstay int NULL
,pendingamount uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,paymentstatus uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,refundamount uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,refundstatus uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,feedbackstatus Varchar(128) NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


