CREATE TABLE IF NOT EXISTS TaskActionLog
(
TaskActionLogid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,taskname uuid REFERENCES DailyTask(DailyTaskid) NULL
,tasktype uuid REFERENCES TypeofTask(TypeofTaskid) NULL
,actiondate Timestamp(3) NULL
,actionby uuid REFERENCES users(usersid) NULL
,comments Varchar(1028) NULL
,assignto uuid REFERENCES users(usersid) NULL
,escalateto uuid REFERENCES users(usersid) NULL
,summary Varchar(128) NULL
,description Varchar(256) NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


