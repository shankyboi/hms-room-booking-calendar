CREATE TABLE IF NOT EXISTS OPDForm
(
OPDFormid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,verifiedby uuid  NULL
,verifieddate Timestamp(3)  NULL
,reviewcomments Varchar(4000)  NULL
,bookingreferencenumber Varchar(128) DEFAULT 'YYYYMMDD-999' NOT NULL
,patientname uuid REFERENCES PatientProfile(PatientProfileid) NOT NULL
,appointmentmode Varchar(1080) NULL
,preferreddoctor uuid REFERENCES People(Peopleid) NULL
,verifiedstatus Varchar(1080) NULL
,UNIQUE(tenantid,bookingreferencenumber)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
);

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'public'
          AND lower(table_name) = 'opdform'
          AND lower(column_name) = 'preferreddate'
    ) THEN
        ALTER TABLE public.OPDForm
        ALTER COLUMN preferreddate DROP NOT NULL;
    END IF;
END $$;

ALTER TABLE OPDForm ADD COLUMN IF NOT EXISTS appointmentmode Varchar(1080) NULL;

