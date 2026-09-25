CREATE TABLE IF NOT EXISTS tenant
(
tenantid uuid  PRIMARY KEY
,businessname Varchar(50) NOT NULL
,shortcode Varchar(3) NOT NULL
,natureofbusiness Varchar(1080) NULL
,businessemail Varchar(128) NOT NULL
,businessphone Varchar(20) NOT NULL
,businesswebsite Varchar(256) NULL
,organizationlogo Varchar(4000) NOT NULL
,numberofemployees int NULL
,enablepatientautologin Boolean NULL
,allowdoctortoadmitpatients Boolean NULL
,preadmissionnoticedays int NULL
,addressline1 Varchar(256) NOT NULL
,addressline2 Varchar(256) NULL
,zip Varchar(256) NULL
,town Varchar(256) NULL
,statename Varchar(256) NULL
,country uuid REFERENCES Country(Countryid) NULL
,parentid uuid NULL
,username Varchar(256) NOT NULL
,userrole Varchar(256) NOT NULL
,password Varchar(256) NOT NULL
,UNIQUE(tenantid,shortcode)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
);

ALTER TABLE tenant ADD COLUMN IF NOT EXISTS enablepatientautologin Boolean NULL;
ALTER TABLE tenant ADD COLUMN IF NOT EXISTS allowdoctortoadmitpatients Boolean NULL;
ALTER TABLE tenant ADD COLUMN IF NOT EXISTS preadmissionnoticedays int NULL;


