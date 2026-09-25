CREATE TABLE IF NOT EXISTS EnquiryForm
(
EnquiryFormid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,verifiedby uuid  NULL
,verifieddate Timestamp(3)  NULL
,reviewcomments Varchar(4000)  NULL
,enquirynumber Varchar(128) DEFAULT 'YYYYMMDD-9999' NOT NULL
,enquirydate date NOT NULL
,enquirytype uuid REFERENCES EnquiryType(EnquiryTypeid) NOT NULL
,isroombookingrelated Varchar(1080) NULL
,patientname uuid REFERENCES PatientProfile(PatientProfileid) NULL
,firstname Varchar(128) NOT NULL
,lastname Varchar(128) NULL
,gender Varchar(1080) NOT NULL
,age Bigint NOT NULL
,phonenumber Varchar(20) NOT NULL
,emailaddress Varchar(128) NULL
,preferredcontactmethod Varchar(1080) NULL
,enquiryreason text NULL
,enquiredvia Varchar(1080) NULL
,preferredroomtype uuid REFERENCES RoomType(RoomTypeid) NULL
,preferreddateofarrival date NULL
,preferreddateofdeparture date NULL
,joinwaitinglist Boolean NULL
,enquirystatus Varchar(1080) NOT NULL
,verifiedstatus Varchar(1080) NULL
,UNIQUE(tenantid,enquirynumber)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


