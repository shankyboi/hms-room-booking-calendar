CREATE TABLE IF NOT EXISTS IPDApplicationForm_preferreddatesofadmission
(
IPDApplicationForm_preferreddatesofadmissionid uuid PRIMARY KEY
,IPDApplicationFormid uuid REFERENCES IPDApplicationForm(IPDApplicationFormid)
,record_order int
,dateofarrival date NOT NULL
,dateofdeparture date NOT NULL
,daysofstay int NOT NULL
,action_date Timestamp(3) NOT NULL DEFAULT NOW()
,action_by uuid
,action character varying(50)
,isdeleted boolean NOT NULL DEFAULT FALSE
);
CREATE TABLE IF NOT EXISTS IPDApplicationForm_medicalinfo
(
IPDApplicationForm_medicalinfoid uuid PRIMARY KEY
,IPDApplicationFormid uuid REFERENCES IPDApplicationForm(IPDApplicationFormid)
,record_order int
,medicalconditionname uuid REFERENCES MedicalCondition(MedicalConditionid) NOT NULL
,duration decimal(18,2) NOT NULL
,unit Varchar(1080) NOT NULL
,severitylevel Varchar(1080) NOT NULL
,action_date Timestamp(3) NOT NULL DEFAULT NOW()
,action_by uuid
,action character varying(50)
,isdeleted boolean NOT NULL DEFAULT FALSE
);
CREATE TABLE IF NOT EXISTS IPDApplicationForm_medicationinfo
(
IPDApplicationForm_medicationinfoid uuid PRIMARY KEY
,IPDApplicationFormid uuid REFERENCES IPDApplicationForm(IPDApplicationFormid)
,record_order int
,medicinename Varchar(128) NOT NULL
,frequencyinaday Varchar(1080) NULL
,medicationduration Varchar(1080) NULL
,quantity numeric(10,2) NULL
,action_date Timestamp(3) NOT NULL DEFAULT NOW()
,action_by uuid
,action character varying(50)
,isdeleted boolean NOT NULL DEFAULT FALSE
);
ALTER TABLE IPDApplicationForm_medicationinfo
ADD COLUMN IF NOT EXISTS medicationduration Varchar(1080) NULL;
ALTER TABLE IPDApplicationForm_medicationinfo
ADD COLUMN IF NOT EXISTS quantity numeric(10,2) NULL;
ALTER TABLE IF EXISTS IPDApplicationForm_medicationinfo_history
ADD COLUMN IF NOT EXISTS medicationduration Varchar(1080) NULL;
ALTER TABLE IF EXISTS IPDApplicationForm_medicationinfo_history
ADD COLUMN IF NOT EXISTS quantity numeric(10,2) NULL;
CREATE TABLE IF NOT EXISTS IPDApplicationForm_medicalrecords
(
IPDApplicationForm_medicalrecordsid uuid PRIMARY KEY
,IPDApplicationFormid uuid REFERENCES IPDApplicationForm(IPDApplicationFormid)
,record_order int
,medicalrecordname Varchar(128) NOT NULL
,medicalrecordfile Varchar(4000) NOT NULL
,action_date Timestamp(3) NOT NULL DEFAULT NOW()
,action_by uuid
,action character varying(50)
,isdeleted boolean NOT NULL DEFAULT FALSE
);
CREATE TABLE IF NOT EXISTS IPDApplicationForm_attendantinfo
(
IPDApplicationForm_attendantinfoid uuid PRIMARY KEY
,IPDApplicationFormid uuid REFERENCES IPDApplicationForm(IPDApplicationFormid)
,record_order int
,attendantname Varchar(128) NOT NULL
,age int NOT NULL
,gender Varchar(1080) NOT NULL
,phonenumber Varchar(20) NOT NULL
,action_date Timestamp(3) NOT NULL DEFAULT NOW()
,action_by uuid
,action character varying(50)
,isdeleted boolean NOT NULL DEFAULT FALSE
);
CREATE TABLE IF NOT EXISTS IPDApplicationForm_roompreference
(
IPDApplicationForm_roompreferenceid uuid PRIMARY KEY
,IPDApplicationFormid uuid REFERENCES IPDApplicationForm(IPDApplicationFormid)
,record_order int
,roomtype uuid REFERENCES RoomType(RoomTypeid) NOT NULL
,action_date Timestamp(3) NOT NULL DEFAULT NOW()
,action_by uuid
,action character varying(50)
,isdeleted boolean NOT NULL DEFAULT FALSE
);
CREATE TABLE IF NOT EXISTS IPDApplicationForm_room
(
IPDApplicationForm_roomid uuid PRIMARY KEY
,IPDApplicationFormid uuid REFERENCES IPDApplicationForm(IPDApplicationFormid)
,record_order int
,allottedto Varchar(1080) NULL
,roomnumber uuid REFERENCES Room(Roomid) NULL
,fromdate Timestamp(3) NULL
,todate Timestamp(3) NULL
,action_date Timestamp(3) NOT NULL DEFAULT NOW()
,action_by uuid
,action character varying(50)
,isdeleted boolean NOT NULL DEFAULT FALSE
);


