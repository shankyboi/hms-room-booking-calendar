CREATE TABLE IF NOT EXISTS TherapyItem
(
TherapyItemid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,therapyitemcategory uuid REFERENCES TherapyItemCategory(TherapyItemCategoryid) NOT NULL
,therapyitemname Varchar(128) NOT NULL
,price decimal(18,2) NOT NULL
,therapyitemimage Varchar(4000) NULL
,UNIQUE(tenantid,therapyitemname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


