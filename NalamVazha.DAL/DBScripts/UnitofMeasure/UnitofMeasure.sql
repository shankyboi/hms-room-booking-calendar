CREATE TABLE IF NOT EXISTS UnitofMeasure
(
UnitofMeasureid uuid  PRIMARY KEY
,unitofmeasurename Varchar(128) NOT NULL
,unitofmeasuredesc Varchar(256) NULL
,UNIQUE(unitofmeasurename)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


