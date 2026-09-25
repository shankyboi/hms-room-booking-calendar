CREATE TABLE IF NOT EXISTS EnquiryType
(
EnquiryTypeid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,enquiryname Varchar(128) NOT NULL
,enquirydesc Varchar(128) NULL
,isroombookingrelated Varchar(1080) NOT NULL
,UNIQUE(tenantid,enquiryname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


