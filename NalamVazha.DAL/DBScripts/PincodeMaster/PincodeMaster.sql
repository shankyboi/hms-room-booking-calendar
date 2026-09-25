CREATE TABLE IF NOT EXISTS PincodeMaster
(
PincodeMasterid uuid  PRIMARY KEY
,circlename Varchar(256) NULL
,regionname Varchar(256) NULL
,divisionname Varchar(256) NULL
,officename Varchar(256) NULL
,pincode Varchar(256) NULL
,officetype Varchar(256) NULL
,delivery Varchar(256) NULL
,district Varchar(256) NULL
,statename Varchar(256) NULL
,latitude Varchar(256) NULL
,longitude Varchar(256) NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


