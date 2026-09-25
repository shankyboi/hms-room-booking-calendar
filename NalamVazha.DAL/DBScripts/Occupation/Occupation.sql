CREATE TABLE IF NOT EXISTS Occupation
(
Occupationid uuid  PRIMARY KEY
,occupationname Varchar(128) NOT NULL
,occupationdesc Varchar(256) NULL
,UNIQUE(occupationname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


