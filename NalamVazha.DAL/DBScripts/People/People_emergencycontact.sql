CREATE TABLE IF NOT EXISTS People_emergencycontact
(
People_emergencycontactid uuid PRIMARY KEY
,Peopleid uuid REFERENCES People(Peopleid)
,record_order int
,personname Varchar(128) NOT NULL
,relationship Varchar(1080) NULL
,phonenumber Varchar(20) NOT NULL
);
CREATE TABLE IF NOT EXISTS People_educationinfo
(
People_educationinfoid uuid PRIMARY KEY
,Peopleid uuid REFERENCES People(Peopleid)
,record_order int
,fieldofstudy Varchar(128) NOT NULL
,degree Varchar(128) NOT NULL
,educationinstitution Varchar(128) NULL
,certificationnumber Varchar(128) NULL
,yearofgraduation int NULL
,degreestatus Varchar(1080) NULL
);
CREATE TABLE IF NOT EXISTS People_workexperience
(
People_workexperienceid uuid PRIMARY KEY
,Peopleid uuid REFERENCES People(Peopleid)
,record_order int
,designation Varchar(128) NOT NULL
,institutionname Varchar(128) NOT NULL
,fromdate date NULL
,todate date NULL
);
CREATE TABLE IF NOT EXISTS People_preferredlanguageinfo
(
People_preferredlanguageinfoid uuid PRIMARY KEY
,Peopleid uuid REFERENCES People(Peopleid)
,record_order int
,languagesknown Varchar(1080) NOT NULL
,proficiency Varchar(1080) NOT NULL
,ability Varchar(1080) DEFAULT '' NOT NULL
);
CREATE TABLE IF NOT EXISTS People_clinicaltaskinfo
(
People_clinicaltaskinfoid uuid PRIMARY KEY
,Peopleid uuid REFERENCES People(Peopleid)
,record_order int
,consultations Varchar(256) NULL
,workprofile uuid REFERENCES WorkProfile(WorkProfileid) NULL
,tasktype uuid REFERENCES TaskType(TaskTypeid) NULL
,taskname uuid REFERENCES Task(Taskid) NULL
,durationinminutes int NULL
,overbookingcount int NULL
,availableon Varchar(1080) DEFAULT '' NOT NULL
,workhourstarts Varchar(10) NULL
,workhourends Varchar(10) NULL
,priority Varchar(1080) NULL
,feesamount decimal(18,2) NULL
);

-- Existing databases may have this child table from before work-profile-based
-- clinical tasks were introduced. CREATE TABLE IF NOT EXISTS does not add new
-- columns to an existing table, so migrate it explicitly.
ALTER TABLE People_clinicaltaskinfo
ADD COLUMN IF NOT EXISTS workprofile uuid REFERENCES WorkProfile(WorkProfileid);


