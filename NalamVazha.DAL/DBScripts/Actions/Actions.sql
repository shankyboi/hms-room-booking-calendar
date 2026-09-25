CREATE TABLE IF NOT EXISTS Actions
(
Actionsid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,actiontype uuid REFERENCES ActionType(ActionTypeid) NULL
,actionname Varchar(128) NULL
,description Varchar(256) NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


