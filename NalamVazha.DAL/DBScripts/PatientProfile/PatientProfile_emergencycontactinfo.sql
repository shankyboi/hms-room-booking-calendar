CREATE TABLE IF NOT EXISTS PatientProfile_emergencycontactinfo
(
PatientProfile_emergencycontactinfoid uuid PRIMARY KEY
,PatientProfileid uuid REFERENCES PatientProfile(PatientProfileid)
,record_order int
,personname Varchar(128) NOT NULL
,relationship Varchar(1080) NOT NULL
,phonenumber Varchar(20) NOT NULL
);


