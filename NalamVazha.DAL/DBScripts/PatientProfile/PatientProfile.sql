CREATE TABLE IF NOT EXISTS PatientProfile
(
PatientProfileid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,registrationid Varchar(128) DEFAULT 'YYYY-MM-999' NOT NULL
,firstname Varchar(128) NOT NULL
,lastname Varchar(128) NULL
,gender Varchar(1080) NOT NULL
,dateofbirth date NOT NULL
,age int NOT NULL
,nationality Varchar(1080) NOT NULL
,countryoforigin uuid REFERENCES Country(Countryid) NULL
,emailaddress Varchar(128) NOT NULL
,mobilenumber Varchar(20) NOT NULL
,whatsappnumber Varchar(20) NOT NULL
,photo Varchar(4000) NULL
,paddressline1 Varchar(256) NOT NULL
,paddressline2 Varchar(256) NULL
,pzip int NOT NULL
,ptown Varchar(128) NULL
,pcityordistrict Varchar(256) NULL
,ppstatename Varchar(256) NULL
,sameaspermanentaddress Boolean NULL
,caddressline1 Varchar(128) NULL
,caddressline2 Varchar(128) NULL
,czip int NULL
,ctown Varchar(128) NULL
,ccityordistrict Varchar(128) NULL
,cstatename Varchar(128) NULL
,idprooftype Varchar(1080) NOT NULL
,idproofnumber Varchar(128) NOT NULL
,uploadidproof Varchar(4000) NOT NULL
,languagesknown Varchar(1080) DEFAULT '' NULL
,languagespreferrable Varchar(1080) DEFAULT '' NULL
,otherlanguages Varchar(256) NULL
,maritalstatus Varchar(1080) NULL
,education Varchar(1080) NULL
,occupation uuid REFERENCES Occupation(Occupationid) NULL
,meditationpractice Varchar(1080) NULL
,typeofpractice Varchar(128) NULL
,creativeactivities Varchar(1080) DEFAULT '' NULL
,othercreativeactivities Varchar(128) NULL
,insurancetype Varchar(1080) NULL
,insurancecompany Varchar(128) NULL
,policynumber Varchar(128) NULL
,policyclaimlimit decimal(18,2) NULL
,policyexpirydate date NULL
,referralsource uuid REFERENCES ReferralSource(ReferralSourceid) NULL
,referraltype Varchar(1080) NULL
,referrername Varchar(128) NULL
,referrerphonenumber Varchar(20) NULL
,magazinename Varchar(128) NULL
,socialmediaplatform Varchar(1080) NULL
,otherreferral Varchar(128) NULL
,blacklisted Varchar(1080) NULL
,reasonforblacklisting uuid REFERENCES Blacklistreason(Blacklistreasonid) NULL
,detailedremarks Varchar(256) NULL
,deceased Boolean NULL
,causeofdeath Varchar(128) NULL
,dateandtimeofdeath Timestamp(3) NULL
,UNIQUE(tenantid,registrationid,idproofnumber)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


