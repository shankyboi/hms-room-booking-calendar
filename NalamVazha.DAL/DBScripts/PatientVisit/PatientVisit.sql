CREATE TABLE IF NOT EXISTS PatientVisit
(
PatientVisitid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,visitnumber Varchar(128) DEFAULT 'YYYYMMDD-99999' NOT NULL
,visitdatetime Timestamp(3) NOT NULL
,patientname uuid REFERENCES PatientProfile(PatientProfileid) NOT NULL
,visittype Varchar(1080) NOT NULL
,ipdnumber uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NULL
,opdnumber uuid REFERENCES OPDForm(OPDFormid) NULL
,consultingdoctor uuid REFERENCES People(Peopleid) NULL
,visitstatus Varchar(1080) NULL
,notes Varchar(256) NULL
,UNIQUE(tenantid,visitnumber)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


