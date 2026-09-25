CREATE TABLE IF NOT EXISTS PatientConsent
(
PatientConsentid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,consenttype Varchar(1080) NOT NULL
,consentlanguage Varchar(1080) NOT NULL
,consentfile Varchar(4000) NOT NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
);

ALTER TABLE PatientConsent
ADD COLUMN IF NOT EXISTS consentlanguage Varchar(1080);

-- Preserve legacy consent records created before language was introduced.
UPDATE PatientConsent
SET consentlanguage = 'English'
WHERE consentlanguage IS NULL OR btrim(consentlanguage) = '';

ALTER TABLE PatientConsent
ALTER COLUMN consentlanguage SET NOT NULL;


