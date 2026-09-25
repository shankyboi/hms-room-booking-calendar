CREATE TABLE IF NOT EXISTS RoomOccupancyStatus
(
RoomOccupancyStatusid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,roomallocationno Varchar(128) DEFAULT 'YYYY-MM-9999' NOT NULL
,patientvisit uuid REFERENCES PatientVisit(PatientVisitid) NOT NULL
,patientname uuid REFERENCES PatientProfile(PatientProfileid) NULL
,ipdno uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NOT NULL
,block uuid REFERENCES Block(Blockid) NOT NULL
,building uuid REFERENCES Building(Buildingid) NOT NULL
,floor uuid REFERENCES Floor(Floorid) NOT NULL
,room uuid REFERENCES Room(Roomid) NOT NULL
,bookeddate date NOT NULL
,status Varchar(1080) NOT NULL
,UNIQUE(tenantid,roomallocationno)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


