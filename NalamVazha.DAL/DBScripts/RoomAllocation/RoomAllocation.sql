CREATE TABLE IF NOT EXISTS RoomAllocation
(
RoomAllocationid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,roomallocationno Varchar(128) DEFAULT 'YYYY-MM-9999' NOT NULL
,ipdno uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NOT NULL
,block uuid REFERENCES Block(Blockid) NOT NULL
,building uuid REFERENCES Building(Buildingid) NOT NULL
,floor uuid REFERENCES Floor(Floorid) NOT NULL
,room uuid REFERENCES Room(Roomid) NOT NULL
,fromdate date NOT NULL
,todate date NULL
,status Varchar(1080) NOT NULL
,UNIQUE(tenantid,roomallocationno)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


