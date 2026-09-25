CREATE TABLE IF NOT EXISTS Competency
(
Competencyid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,competencyname Varchar(128) NOT NULL
,description Varchar(256) NULL
,UNIQUE(tenantid,competencyname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


