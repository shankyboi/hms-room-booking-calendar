CREATE TABLE IF NOT EXISTS TreatmentPackage
(
TreatmentPackageid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,packagename Varchar(128) NOT NULL
,noofdays int NOT NULL
,roomtypeamountaverage Varchar(256) NULL
,therapyamount Varchar(256) NULL
,therapykitamount Varchar(256) NULL
,therapyitemamount Varchar(256) NULL
,medicineamount Varchar(256) NULL
,calculatedpackagecost Varchar(256) NOT NULL
,packagecost decimal(18,2) NOT NULL
,packagebookingdeposit decimal(18,2) NOT NULL
,packagebookingadvance decimal(18,2) NOT NULL
,billingwaiverfordelayedstart Varchar(1080) NOT NULL
,waiverpercentage decimal(18,2) NULL
,roomtransfercost Varchar(1080) NULL
,packagedescription Varchar(256) NULL
,UNIQUE(tenantid,packagename)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


