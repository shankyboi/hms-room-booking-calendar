CREATE TABLE IF NOT EXISTS Block
(
Blockid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,blockcode Varchar(128) NOT NULL
,blockname Varchar(128) NOT NULL
,blockdescription Varchar(128) NULL
,blockimage Varchar(4000) NULL
,blocklocationurl Varchar(256) NULL
,blocknearbylandmark Varchar(128) NULL
,UNIQUE(tenantid,blockcode,blockname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


