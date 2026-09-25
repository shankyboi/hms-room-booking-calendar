CREATE TABLE IF NOT EXISTS MedicalCondition
(
MedicalConditionid uuid  PRIMARY KEY
,conditionname Varchar(128) NOT NULL
,snomedid Varchar(128) NULL
,description Varchar(256) NULL
,UNIQUE(conditionname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


