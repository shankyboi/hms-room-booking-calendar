CREATE TABLE IF NOT EXISTS Department
(
Departmentid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,name Varchar(128) NOT NULL
,description Varchar(256) NULL
,UNIQUE(tenantid,name)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


