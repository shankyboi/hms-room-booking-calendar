CREATE TABLE IF NOT EXISTS ReceivableConditions
(
ReceivableConditionsid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,receivablefor Varchar(1080) NOT NULL
,amount decimal(18,2) NULL
,ismandatory Boolean NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


