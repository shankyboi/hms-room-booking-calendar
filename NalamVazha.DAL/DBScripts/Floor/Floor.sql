CREATE TABLE IF NOT EXISTS Floor
(
Floorid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,block uuid REFERENCES Block(Blockid) NOT NULL
,building uuid REFERENCES Building(Buildingid) NOT NULL
,floorname Varchar(128) NOT NULL
,UNIQUE(tenantid,floorname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


