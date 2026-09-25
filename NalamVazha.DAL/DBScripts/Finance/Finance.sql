CREATE TABLE IF NOT EXISTS Finance
(
Financeid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,paymentdate uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,paymentmode uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,receiptnumber uuid REFERENCES Receivable(Receivableid) NULL
,patient uuid REFERENCES PatientProfile(PatientProfileid) NULL
,receivablefor uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,bookingreferencenumber uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,billedamount uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,receivedamount uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,pendingamount int NULL
,paymentstatus uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,collectedby uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,refundmode uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,refundedamount uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,refundedby uuid REFERENCES BillingPayment(BillingPaymentid) NULL
,remarks uuid REFERENCES BillingPayment(BillingPaymentid) NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


