ALTER TABLE IPDApplicationForm
ADD COLUMN IF NOT EXISTS phase VARCHAR(128) DEFAULT 'Intake',
ADD COLUMN IF NOT EXISTS bookingstatusdate TIMESTAMP(3);

ALTER TABLE reviewlogsIPDApplicationForm
ADD COLUMN IF NOT EXISTS bookingstatus VARCHAR(1080),
ADD COLUMN IF NOT EXISTS phase VARCHAR(128),
ADD COLUMN IF NOT EXISTS previousbookingstatus VARCHAR(1080),
ADD COLUMN IF NOT EXISTS previousphase VARCHAR(128),
ADD COLUMN IF NOT EXISTS logtype VARCHAR(128);

UPDATE IPDApplicationForm
SET phase = CASE
    WHEN lower(trim(COALESCE(bookingstatus, ''))) IN (
        'assessment form - in draft',
        'assessment form - review pending',
        'assessment form reviewed',
        'screening scheduled',
        'admission approved',
        'screening completed by the patient - in draft',
        'screening completed by the patient',
        'screening completed by the patient - intern doctor review pending',
        'intern doctor reviewed'
    ) THEN 'Assessment'
    WHEN lower(trim(COALESCE(bookingstatus, ''))) IN (
        'arrival confirmed',
        'confirmed for arrival',
        'patient arrived',
        'consultation scheduled',
        'admission confirmed',
        'admitted'
    ) THEN 'Admission'
    WHEN lower(trim(COALESCE(bookingstatus, ''))) IN (
        'discharged',
        'extended stay'
    ) THEN 'Stay'
    ELSE 'Intake'
END
WHERE phase IS NULL OR phase = '';

INSERT INTO reviewlogsIPDApplicationForm
(
    ipdapplicationformid,
    verifiedstatus,
    reviewcomments,
    bookingstatus,
    phase,
    previousbookingstatus,
    previousphase,
    logtype,
    createduser
)
SELECT
    ipd.ipdapplicationformid,
    ipd.bookingstatus,
    'Initial booking status',
    ipd.bookingstatus,
    COALESCE(NULLIF(ipd.phase, ''), 'Intake'),
    NULL,
    NULL,
    'Booking Status',
    ipd.createduser
FROM IPDApplicationForm ipd
WHERE NOT EXISTS (
    SELECT 1
    FROM reviewlogsIPDApplicationForm logs
    WHERE logs.ipdapplicationformid = ipd.ipdapplicationformid
)
AND COALESCE(ipd.bookingstatus, '') <> '';
