CREATE TABLE IF NOT EXISTS TherapyCategory
(
TherapyCategoryid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,categoryname Varchar(128) NOT NULL
,categorydescription Varchar(256) NULL
,UNIQUE(tenantid,categoryname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


