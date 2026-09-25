CREATE TABLE IF NOT EXISTS MailLogs
(
MailLogsid uuid  PRIMARY KEY
,entityname Varchar(128) NOT NULL
,entityid Varchar(128) NULL
,mailfor Varchar(256) NULL
,mailsubject Varchar(256) NULL
,mailto Varchar(4096) NULL
,mailbody text NULL
,issent Boolean NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


