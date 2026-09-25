CREATE TABLE IF NOT EXISTS otplogs
(
otplogsid uuid  PRIMARY KEY
,username Varchar(128) NULL
,otpcode int NULL
,expirytime Timestamp(3) NULL
,isused Boolean NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


