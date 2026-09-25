CREATE TABLE IF NOT EXISTS PaymentRequest
(
PaymentRequestid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,paymentgateway Varchar(1080) NOT NULL
,requestdatetime Timestamp(3) NULL
,patientname uuid REFERENCES PatientProfile(PatientProfileid) NULL
,people uuid REFERENCES People(Peopleid) NULL
,paymenttype Varchar(1080) NOT NULL
,merchantid Varchar(128) NOT NULL
,orderid Varchar(128) NOT NULL
,paymentid Varchar(128) NOT NULL
,amount decimal(18,2) NOT NULL
,currency Varchar(1080) NOT NULL
,customername Varchar(128) NOT NULL
,customeremail Varchar(128) NULL
,customerphone Varchar(20) NOT NULL
,orderpaymentdesc Varchar(256) NULL
,returnurl Varchar(256) NULL
,notifyurl Varchar(256) NULL
,signatureorchecksum Varchar(128) NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


