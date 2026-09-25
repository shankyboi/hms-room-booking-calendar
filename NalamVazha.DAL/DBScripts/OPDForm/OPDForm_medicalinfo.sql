CREATE TABLE IF NOT EXISTS OPDForm_medicalinfo
(
OPDForm_medicalinfoid uuid PRIMARY KEY
,OPDFormid uuid REFERENCES OPDForm(OPDFormid)
,record_order int
,medicalconditionname uuid REFERENCES MedicalCondition(MedicalConditionid) NOT NULL
,duration decimal(18,2) NOT NULL
,unit Varchar(1080) NOT NULL
,severitylevel Varchar(1080) NOT NULL
);
CREATE TABLE IF NOT EXISTS OPDForm_medicationinfo
(
OPDForm_medicationinfoid uuid PRIMARY KEY
,OPDFormid uuid REFERENCES OPDForm(OPDFormid)
,record_order int
,medicinename Varchar(128) NOT NULL
,frequencyinaday Varchar(1080) NOT NULL
,medicationduration Varchar(1080) NULL
);
ALTER TABLE OPDForm_medicationinfo ADD COLUMN IF NOT EXISTS medicationduration Varchar(1080) NULL;
CREATE TABLE IF NOT EXISTS OPDForm_medicalrecords
(
OPDForm_medicalrecordsid uuid PRIMARY KEY
,OPDFormid uuid REFERENCES OPDForm(OPDFormid)
,record_order int
,medicalrecordname Varchar(128) NOT NULL
,medicalrecordfile Varchar(4000) NOT NULL
);
CREATE TABLE IF NOT EXISTS OPDForm_appointmentpreferences
(
OPDForm_appointmentpreferencesid uuid PRIMARY KEY
,OPDFormid uuid REFERENCES OPDForm(OPDFormid)
,record_order int
,preferreddate date NULL
,slotpreference Varchar(1080) DEFAULT '' NULL
);


