CREATE TABLE IF NOT EXISTS TherapyKit
(
TherapyKitid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,therapykitname Varchar(128) NOT NULL
,kitprice Varchar(256) NOT NULL
,UNIQUE(tenantid,therapykitname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


