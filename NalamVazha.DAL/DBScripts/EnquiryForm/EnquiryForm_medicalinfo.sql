CREATE TABLE IF NOT EXISTS EnquiryForm_medicalinfo
(
EnquiryForm_medicalinfoid uuid PRIMARY KEY
,EnquiryFormid uuid REFERENCES EnquiryForm(EnquiryFormid)
,record_order int
,medicalcondition uuid REFERENCES MedicalCondition(MedicalConditionid) NULL
,conditionname Varchar(128) NULL
,duration Varchar(1080) NULL
,severity Varchar(1080) NULL
);


