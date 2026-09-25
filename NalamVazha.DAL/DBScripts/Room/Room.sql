CREATE TABLE IF NOT EXISTS Room
(
Roomid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,roomcode Varchar(128) DEFAULT 'YYYY-MM-9999' NOT NULL
,block uuid REFERENCES Block(Blockid) NOT NULL
,building uuid REFERENCES Building(Buildingid) NOT NULL
,floor uuid REFERENCES Floor(Floorid) NOT NULL
,roomtype uuid REFERENCES RoomType(RoomTypeid) NOT NULL
,bookingdeposit decimal(18,2) NULL
,roomgroup uuid REFERENCES RoomGroup(RoomGroupid) NOT NULL
,roomnumber Varchar(128) NOT NULL
,roomimage Varchar(4000) NULL
,occupancystatus Varchar(1080) NOT NULL
,maintenancestatus Varchar(1080) NOT NULL
,housekeepingstatus Varchar(1080) NOT NULL
,nextdaycheckin Varchar(1080) NOT NULL
,nextdaycheckout Varchar(1080) NOT NULL
,UNIQUE(tenantid,roomcode,roomnumber)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


