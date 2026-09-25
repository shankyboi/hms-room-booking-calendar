CREATE OR REPLACE FUNCTION public."verify_IPDApplicationForm"(
    pvar_ipdapplicationformid character varying,
    pvar_verifiedby character varying,
    pvar_verifiedstatus character varying,
    pvar_reviewcomments character varying,
    OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    lvar_ipdapplicationformid_array UUID[];
    lvar_ipdapplicationformid UUID;
    lvar_invalid_status_count INTEGER;
    lvar_total_ipdapplicationform_count INTEGER;
    lvar_previousbookingstatus VARCHAR;
    lvar_previousphase VARCHAR;
    lvar_phase VARCHAR;
    lvar_bookingstatus VARCHAR;
BEGIN
    /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 05:57:33*/

    lvar_ipdapplicationformid_array := STRING_TO_ARRAY(pvar_ipdapplicationformid, ',');

    lvar_total_ipdapplicationform_count := array_length(lvar_ipdapplicationformid_array, 1);

    SELECT COUNT(*) INTO lvar_invalid_status_count
    FROM IPDApplicationForm
    WHERE ipdapplicationformid = ANY(lvar_ipdapplicationformid_array)
      AND verifiedstatus NOT IN ('Request for Doctor Review','Ready For Doctor Review','Ready For Frontdesk Review','Doctor Review Requested');

    lvar_invalid_status_count := 0;

    IF lvar_invalid_status_count > 0 THEN
        CASE
            WHEN lvar_total_ipdapplicationform_count = 1 THEN
                pvar_returnMessage := 'The given record is already reviewed. Please try again.';
            WHEN lvar_invalid_status_count = lvar_total_ipdapplicationform_count THEN
                pvar_returnMessage := 'All given records are already reviewed. Please try again.';
            ELSE
                pvar_returnMessage := 'Some of the given records are already reviewed. Please try again.';
        END CASE;
        RETURN;
    END IF;

    /* Rework is the review action; Rework Requested is the resulting workflow state. */
    lvar_bookingstatus := CASE
        WHEN lower(trim(COALESCE(pvar_verifiedstatus, ''))) = 'rework' THEN 'Rework Requested'
        ELSE pvar_verifiedstatus
    END;

    lvar_phase := CASE
        WHEN lower(trim(COALESCE(lvar_bookingstatus, ''))) IN (
            'pending',
            'draft',
            'provisional booking',
            'provisional confirmed',
            'waitlisted',
            'rejected',
            'rework requested',
            'doctor review requested',
            'ipd approved by doctor',
            'request for doctor review',
            'ready for doctor review',
            'ready for frontdesk review',
            'cancellation requested',
            'cancelled by patient',
            'cancelled'
        ) THEN 'Intake'
        WHEN lower(trim(COALESCE(lvar_bookingstatus, ''))) IN (
            'assessment form - in draft',
            'assessment form - review pending',
            'assessment form reviewed',
            'screening scheduled',
            'admission approved',
            'approved for admission',
            'screening completed by the patient - in draft',
            'screening completed by the patient',
            'screening completed by the patient - intern doctor review pending',
            'intern doctor reviewed'
        ) THEN 'Assessment'
        WHEN lower(trim(COALESCE(lvar_bookingstatus, ''))) IN (
            'arrival confirmed',
            'confirmed for arrival',
            'patient arrived',
            'consultation scheduled',
            'admission confirmed',
            'admitted'
        ) THEN 'Admission'
        WHEN lower(trim(COALESCE(lvar_bookingstatus, ''))) IN (
            'discharged',
            'extended stay'
        ) THEN 'Stay'
        ELSE 'Intake'
    END;

    FOREACH lvar_ipdapplicationformid IN ARRAY lvar_ipdapplicationformid_array
    LOOP
        SELECT bookingstatus, phase
        INTO lvar_previousbookingstatus, lvar_previousphase
        FROM IPDApplicationForm
        WHERE ipdapplicationformid = lvar_ipdapplicationformid;

        UPDATE IPDApplicationForm
        SET verifiedby = CAST(pvar_verifiedby AS UUID),
            verifiedstatus = pvar_verifiedstatus,
            bookingstatus = lvar_bookingstatus,
            phase = lvar_phase,
            bookingstatusdate = NOW(),
            verifieddate = NOW(),
            reviewcomments = pvar_reviewcomments
        WHERE ipdapplicationformid = lvar_ipdapplicationformid;

        IF pvar_verifiedstatus = 'Rejected' THEN
            UPDATE IPDApplicationForm
            SET bookingstatus = pvar_verifiedstatus,
                phase = lvar_phase,
                bookingstatusdate = NOW()
            WHERE ipdapplicationformid = lvar_ipdapplicationformid;
        END IF;

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
        VALUES
        (
            lvar_ipdapplicationformid,
            pvar_verifiedstatus,
            pvar_reviewcomments,
            lvar_bookingstatus,
            lvar_phase,
            lvar_previousbookingstatus,
            lvar_previousphase,
            'Review',
            CAST(pvar_verifiedby AS UUID)
        );
    END LOOP;

    pvar_returnMessage := '201.1';
END
$BODY$;
