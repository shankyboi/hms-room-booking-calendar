CREATE TABLE IF NOT EXISTS GroupBooking
(
GroupBookingid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,groupcode Varchar(128) NOT NULL
,groupname Varchar(128) NOT NULL
,countofmembers int NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


