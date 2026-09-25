CREATE TABLE IF NOT EXISTS Building
(
Buildingid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,block uuid REFERENCES Block(Blockid) NOT NULL
,buildingcode Varchar(128) NOT NULL
,buildingname Varchar(128) NOT NULL
,buildingdescription text NULL
,buildingimage Varchar(4000) NULL
,buildingnearbylandmark Varchar(128) NULL
,UNIQUE(tenantid,buildingcode,buildingname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


