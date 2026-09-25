CREATE TABLE IF NOT EXISTS ClinicalTask
(
ClinicalTaskid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,workprofile uuid REFERENCES WorkProfile(WorkProfileid) NOT NULL
,competency uuid REFERENCES Competency(Competencyid) NOT NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


