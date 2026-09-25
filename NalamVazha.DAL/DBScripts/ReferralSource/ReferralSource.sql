CREATE TABLE IF NOT EXISTS ReferralSource
(
ReferralSourceid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,referralsourcename Varchar(128) NOT NULL
,contactnumber Varchar(20) NULL
,websiteurl Varchar(256) NULL
,UNIQUE(tenantid,referralsourcename)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


