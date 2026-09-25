CREATE TABLE IF NOT EXISTS PaymentResponse
(
PaymentResponseid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,paymentrequest uuid REFERENCES PaymentRequest(PaymentRequestid) NULL
,paymenttype Varchar(1080) NULL
,transactiontime Timestamp(3) NOT NULL
,orderid Varchar(128) NOT NULL
,paymentid Varchar(128) NULL
,status Varchar(1080) NULL
,amount decimal(18,2) NULL
,paymentmethod Varchar(1080) NULL
,banktransactionid Varchar(256) NULL
,gatewayresponsecode Varchar(128) NULL
,gatewayresponsemessage Varchar(1024) NULL
,responsesignature Varchar(1024) NULL
,refundedamount decimal(18,2) NULL
,refundreason Varchar(256) NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


