CREATE TABLE IF NOT EXISTS WorkProfile
(
WorkProfileid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,department uuid REFERENCES Department(Departmentid) NOT NULL
,workprofilename Varchar(128) NOT NULL
,rolename Varchar(1080) NOT NULL
,isthisaclinicalprofile Varchar(1080) NOT NULL
,workprofiledescription Varchar(256) NULL
,UNIQUE(tenantid,workprofilename)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


