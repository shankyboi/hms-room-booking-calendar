CREATE TABLE IF NOT EXISTS MailBox
(
MailBoxid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,senderdisplayname Varchar(128) NOT NULL
,senderemail Varchar(128) NOT NULL
,password Varchar(128) NOT NULL
,emailhostname Varchar(128) NOT NULL
,portnumber int NOT NULL
,applicableservice Varchar(1080) DEFAULT '' NOT NULL
,emailfooter text NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


