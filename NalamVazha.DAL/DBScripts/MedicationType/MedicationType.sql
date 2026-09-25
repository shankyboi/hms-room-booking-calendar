CREATE TABLE IF NOT EXISTS MedicationType
(
MedicationTypeid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,medicationtypename Varchar(128) NOT NULL
,medicationtypedescription Varchar(256) NULL
,UNIQUE(tenantid,medicationtypename)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


