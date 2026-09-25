CREATE TABLE IF NOT EXISTS RoomGroup
(
RoomGroupid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,groupname Varchar(128) NOT NULL
,groupnumber Varchar(128) NOT NULL
,groupdesc Varchar(256) NULL
,UNIQUE(tenantid,groupname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


