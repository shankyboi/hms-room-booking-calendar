CREATE TABLE IF NOT EXISTS ClinicalAppointment
(
ClinicalAppointmentid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,tasktype Varchar(1080) NOT NULL
,patient uuid REFERENCES PatientProfile(PatientProfileid) NOT NULL
,practitioner uuid REFERENCES People(Peopleid) NULL
,photo Varchar(4000) NULL
,actualpractitioner uuid REFERENCES People(Peopleid) NULL
,appointmentdate date NOT NULL
,durationfrom Varchar(10) NOT NULL
,durationto Varchar(10) NOT NULL
,status Varchar(1080) NOT NULL
,origin Varchar(1080) NULL
,bookingid Varchar(128) NULL
,tokennumber Varchar(128) DEFAULT 'YYYYMMDD-9999' NOT NULL
,UNIQUE(tenantid,tokennumber)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


