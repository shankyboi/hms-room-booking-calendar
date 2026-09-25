CREATE TABLE IF NOT EXISTS DoctorInternMap
(
DoctorInternMapid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,seniordoctor uuid REFERENCES People(Peopleid) NOT NULL
,interndoctor uuid REFERENCES People(Peopleid) NOT NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


