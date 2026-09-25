CREATE TABLE IF NOT EXISTS RoomType
(
RoomTypeid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,name Varchar(128) NOT NULL
,prebookingdaylimit int NOT NULL
,minbookingdays int NOT NULL
,maxbookingdays int NOT NULL
,concessoneligibility Varchar(1080) NOT NULL
,suitabilityforvip Varchar(1080) NOT NULL
,gendersuitability Varchar(1080) DEFAULT '' NULL
,roomtypeicon Varchar(4000) NULL
,deposittype Varchar(1080) NULL
,costperday decimal(18,2) NOT NULL
,advanceperday decimal(18,2) NOT NULL
,bookingdeposit decimal(18,2) NULL
,variableofbookingdays decimal(18,2) NULL
,attendantcostperday decimal(18,2) NOT NULL
,attendantadvanceperday decimal(18,2) NOT NULL
,attendantbookingdeposit decimal(18,2) NULL
,attendantvariableofbookingdays decimal(18,2) NULL
,hourlychargesapplicable Boolean NULL
,chargeperhour decimal(18,2) NULL
,billingwaiverfordelayedstart Varchar(1080) NULL
,waiverpercentage decimal(18,2) NULL
,roomtransfercost Varchar(1080) NULL
,description text NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


