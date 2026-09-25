CREATE TABLE IF NOT EXISTS Task
(
Taskid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,tasktype uuid REFERENCES TaskType(TaskTypeid) NOT NULL
,taskname Varchar(128) NOT NULL
,taskdesc Varchar(256) NULL
,UNIQUE(tenantid,taskname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


