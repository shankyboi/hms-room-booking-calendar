CREATE TABLE IF NOT EXISTS TaskTemplate
(
TaskTemplateid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,tasktype uuid REFERENCES TypeofTask(TypeofTaskid) NULL
,taskname Varchar(128) NULL
,priority Varchar(1080) NULL
,duration int NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


