CREATE TABLE IF NOT EXISTS AlertTemplates
(
AlertTemplatesid uuid  PRIMARY KEY
,entityname Varchar(128) NOT NULL
,entityaction Varchar(128) NULL
,sendasbatch Boolean NULL
,alerttype Varchar(1080) NULL
,alertsubject Varchar(256) NULL
,alertcopyto Varchar(50) NULL
,alertcarboncopyto Varchar(50) NULL
,alertcontent text NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


