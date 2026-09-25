CREATE TABLE IF NOT EXISTS DailyTask
(
DailyTaskid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,taskno Varchar(128) DEFAULT 'YYYY-DOY-99999' NOT NULL
,dateandtime Timestamp(3) NULL
,tasktype uuid REFERENCES TypeofTask(TypeofTaskid) NOT NULL
,taskname uuid REFERENCES TaskTemplate(TaskTemplateid) NULL
,patientname uuid REFERENCES PatientProfile(PatientProfileid) NULL
,patientcategory Varchar(128) NULL
,ipdreferencenumber uuid REFERENCES IPDApplicationForm(IPDApplicationFormid) NULL
,opdreferencenumber uuid REFERENCES OPDForm(OPDFormid) NULL
,status Varchar(128) NULL
,amount int NULL
,activityname Varchar(256) NULL
,description Text NULL
,priority Varchar(32) NULL
,assignedto uuid REFERENCES users(usersid) NULL
,sourceappointmentid uuid REFERENCES ClinicalAppointment(ClinicalAppointmentid) NULL
,closedby uuid REFERENCES users(usersid) NULL
,closeddate Timestamp(3) NULL
,UNIQUE(tenantid,taskno)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
);

ALTER TABLE DailyTask ADD COLUMN IF NOT EXISTS activityname Varchar(256);
ALTER TABLE DailyTask ADD COLUMN IF NOT EXISTS description Text;
ALTER TABLE DailyTask ADD COLUMN IF NOT EXISTS priority Varchar(32);
ALTER TABLE DailyTask ADD COLUMN IF NOT EXISTS assignedto uuid REFERENCES users(usersid);
ALTER TABLE DailyTask ADD COLUMN IF NOT EXISTS sourceappointmentid uuid REFERENCES ClinicalAppointment(ClinicalAppointmentid);
CREATE UNIQUE INDEX IF NOT EXISTS ux_dailytask_sourceappointment
    ON DailyTask(sourceappointmentid) WHERE sourceappointmentid IS NOT NULL AND isdeleted=false;

