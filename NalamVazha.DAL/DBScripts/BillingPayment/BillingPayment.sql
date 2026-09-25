CREATE TABLE IF NOT EXISTS BillingPayment
(
BillingPaymentid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,receiptno Varchar(128) DEFAULT 'YYYYMMDD-99999' NOT NULL
,paymentfor Varchar(1080) NOT NULL
,patientvisit uuid REFERENCES PatientVisit(PatientVisitid) NULL
,patientname uuid REFERENCES PatientProfile(PatientProfileid) NULL
,amount decimal(18,2) NOT NULL
,paymentmode Varchar(1080) NOT NULL
,transactionreference Varchar(128) NULL
,paymentstatus Varchar(1080) NOT NULL
,collectedby uuid REFERENCES users(usersid) NULL
,counterid Varchar(1080) NULL
,remarks Varchar(256) NULL
,UNIQUE(tenantid,receiptno)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


