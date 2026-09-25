CREATE TABLE IF NOT EXISTS People
(
Peopleid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,practitionerid Varchar(128) DEFAULT 'YYMM-999' NOT NULL
,firstname Varchar(128) NOT NULL
,lastname Varchar(128) NULL
,workprofile uuid REFERENCES WorkProfile(WorkProfileid) NOT NULL
,competencylevel uuid REFERENCES Competency(Competencyid) NOT NULL
,clinicaltask uuid REFERENCES ClinicalTask(ClinicalTaskid) NULL
,designation uuid REFERENCES Designation(Designationid) NOT NULL
,contactnumber Varchar(20) NOT NULL
,whatsappnumber Varchar(20) NOT NULL
,emailid Varchar(128) NOT NULL
,gender Varchar(1080) NOT NULL
,dob date NOT NULL
,age int NULL
,employmentstatus Varchar(1080) NOT NULL
,joiningdate date NULL
,contractrenewaldate date NULL
,photo Varchar(4000) NULL
,nationality Varchar(1080) NULL
,specifycountry uuid REFERENCES Country(Countryid) NULL
,idtype Varchar(1080) NOT NULL
,idnumber Varchar(128) NOT NULL
,iddocument Varchar(4000) NOT NULL
,paddressline1 Varchar(256) NOT NULL
,paddressline2 Varchar(256) NULL
,pzip int NOT NULL
,ptown Varchar(128) NULL
,pcityordistrict Varchar(256) NULL
,pstatename Varchar(256) NULL
,sameaspermanentaddress Boolean NULL
,caddressline1 Varchar(128) NOT NULL
,caddressline2 Varchar(128) NULL
,czip int NOT NULL
,ctown Varchar(128) NULL
,ccityordistrict Varchar(128) NULL
,cstatename Varchar(128) NULL
,registrationnumber Varchar(128) NULL
,validtill date NULL
,licenceupload Varchar(4000) NULL
,issuingauthority Varchar(128) NULL
,bio text NULL
,UNIQUE(tenantid,practitionerid,emailid)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


