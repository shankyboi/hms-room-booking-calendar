CREATE TABLE IF NOT EXISTS IPDApplicationForm
(
IPDApplicationFormid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,verifiedby uuid  NULL
,verifieddate Timestamp(3)  NULL
,reviewcomments Varchar(4000)  NULL
,bookingreferencenumber Varchar(128) DEFAULT 'YYYYMMDD-999' NOT NULL
,patientname uuid REFERENCES PatientProfile(PatientProfileid) NULL
,firstname Varchar(128) NOT NULL
,lastname Varchar(128) NULL
,gender Varchar(1080) NOT NULL
,mobilenumber Varchar(20) NOT NULL
,whatsappnumber Varchar(20) NOT NULL
,nationality Varchar(1080) NOT NULL
,countryoforigin uuid REFERENCES Country(Countryid) NULL
,generalcondition Varchar(1080) NULL
,bookingstatus Varchar(1080) NOT NULL
,phase Varchar(128) DEFAULT 'Intake' NULL
,bookingstatusdate Timestamp(3) NULL
,groupbooking Varchar(1080) NULL
,areyouthegroupleader Varchar(1080) NULL
,numberofmember int NULL
,groupleadersbookingreferencenumber Varchar(128) NULL
,paddressline1 Varchar(128) NOT NULL
,paddressline2 Varchar(128) NULL
,ppincode int NOT NULL
,ptown Varchar(128) NULL
,pcityordistrict Varchar(128) NULL
,pstatename Varchar(128) NULL
,sameaspermanentaddress Boolean NULL
,caddressline1 Varchar(128) NOT NULL
,caddressline2 Varchar(128) NULL
,cpincode int NOT NULL
,ctown Varchar(128) NULL
,ccityordistrict Varchar(128) NULL
,cstatename Varchar(128) NULL
,flexiblewithdates Varchar(1080) NULL
,flexiblewithroomtype Varchar(1080) NULL
,joinwaitinglist Varchar(1080) NULL
,passportnumber Varchar(128) NULL
,passportissuingcountry uuid REFERENCES Country(Countryid) NULL
,passportexpirydate date NULL
,uploadpassportcopy Varchar(4000) NULL
,visatype Varchar(1080) NULL
,visanumber Varchar(128) NULL
,visaissuedcountry uuid REFERENCES Country(Countryid) NULL
,visaissuedate date NULL
,visaexpirydate date NULL
,uploadvisacopy Varchar(4000) NULL
,doyourequireahospitalprovidedattendant Varchar(1080) NULL
,preferredduration Varchar(1080) NULL
,admissionreason text NULL
,consentform uuid REFERENCES PatientConsent(PatientConsentid) NULL
,consentfile Varchar(4000) NULL
,agreefortermsandconditions Boolean NOT NULL
,signature text NULL
,packagename uuid REFERENCES TreatmentPackage(TreatmentPackageid) NULL
,isbookingdepositmandatory Boolean NULL
,verifiedstatus Varchar(1080) NULL
,UNIQUE(tenantid,bookingreferencenumber)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
);
-- patientname NOT NULL
ALTER TABLE IPDApplicationForm
ALTER COLUMN patientname SET NOT NULL;

-- Drafts can be incomplete, but these values remain mandatory for every
-- non-draft booking.
ALTER TABLE IPDApplicationForm
ADD CONSTRAINT ck_ipdapplicationform_generalcondition_required
CHECK (
    lower(BTRIM(COALESCE(bookingstatus, ''))) = 'draft'
    OR NULLIF(BTRIM(generalcondition), '') IS NOT NULL
),
ADD CONSTRAINT ck_ipdapplicationform_consentform_required
CHECK (
    lower(BTRIM(COALESCE(bookingstatus, ''))) = 'draft'
    OR (
        consentform IS NOT NULL
        AND consentform <> '00000000-0000-0000-0000-000000000000'::uuid
    )
);

-- Add new fields
ALTER TABLE IPDApplicationForm
ADD COLUMN estimatedarrival TIMESTAMP(3),
ADD COLUMN travelarrangement VARCHAR(1080),
ADD COLUMN typeoftravelrequired VARCHAR(1080),
ADD COLUMN pickupfrom VARCHAR(128),
ADD COLUMN requiredparkingspace VARCHAR(1080),
ADD COLUMN wheelchairassistance VARCHAR(1080),
ADD COLUMN requireddinner VARCHAR(1080),
ADD COLUMN specialrequest VARCHAR(256);

ALTER TABLE IPDApplicationForm
ADD COLUMN IF NOT EXISTS phase VARCHAR(128) DEFAULT 'Intake',
ADD COLUMN IF NOT EXISTS bookingstatusdate TIMESTAMP(3),
ADD COLUMN IF NOT EXISTS isbookingdepositmandatory Boolean;


