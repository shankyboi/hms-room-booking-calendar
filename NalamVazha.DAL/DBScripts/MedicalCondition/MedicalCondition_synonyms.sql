CREATE TABLE IF NOT EXISTS MedicalCondition_synonyms
(
MedicalCondition_synonymsid uuid PRIMARY KEY
,MedicalConditionid uuid REFERENCES MedicalCondition(MedicalConditionid)
,record_order int
,synonymname Varchar(128) NOT NULL
,slanguage Varchar(1080) NOT NULL
);


