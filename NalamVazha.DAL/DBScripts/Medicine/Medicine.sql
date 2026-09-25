CREATE TABLE IF NOT EXISTS Medicine
(
Medicineid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,medicationtype uuid REFERENCES MedicationType(MedicationTypeid) NOT NULL
,medicinename Varchar(128) NOT NULL
,price decimal(18,2) NOT NULL
,prescriptionrequired Boolean NULL
,sideeffect text NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


