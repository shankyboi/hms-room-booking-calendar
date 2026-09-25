CREATE TABLE IF NOT EXISTS Country
(
Countryid uuid  PRIMARY KEY
,countryname Varchar(128) NOT NULL
,countrycode Varchar(128) NULL
,countryshortcode Varchar(128) NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


