-- Allow Direct IPD and regular IPD drafts to be persisted before the medical
-- condition and consent steps are completed. Non-draft records remain guarded
-- by conditional constraints.
ALTER TABLE IPDApplicationForm
    ALTER COLUMN generalcondition DROP NOT NULL,
    ALTER COLUMN consentform DROP NOT NULL;

ALTER TABLE IPDApplicationForm
    DROP CONSTRAINT IF EXISTS ck_ipdapplicationform_generalcondition_required,
    DROP CONSTRAINT IF EXISTS ck_ipdapplicationform_consentform_required;

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
