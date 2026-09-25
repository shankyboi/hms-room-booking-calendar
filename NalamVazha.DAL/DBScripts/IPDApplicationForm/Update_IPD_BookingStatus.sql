-- FUNCTION: public.Update_IPD_BookingStatus(uuid, character varying, uuid)

-- DROP FUNCTION IF EXISTS public."Update_IPD_BookingStatus"(uuid, character varying, uuid);

CREATE OR REPLACE FUNCTION public."Update_IPD_BookingStatus"(
	pvar_ipdapplicationformid uuid,
	pvar_bookingstatus character varying,
	pvar_modifieduser uuid,
	OUT "returnMessage" character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    lvar_registrationid varchar(50);
    lvar_patientprofileid uuid;
    lvar_tenantid uuid;
    lvar_previousbookingstatus varchar;
    lvar_verifiedstatus varchar;
    lvar_previousphase varchar;
    lvar_phase varchar;
    lvar_outstanding_balance numeric := 0;
BEGIN
    SELECT bookingstatus, verifiedstatus, phase
    INTO lvar_previousbookingstatus, lvar_verifiedstatus, lvar_previousphase
    FROM ipdapplicationform
    WHERE ipdapplicationformid = pvar_ipdapplicationformid;

    IF lower(trim(COALESCE(pvar_bookingstatus, ''))) IN ('discharge initiated', 'discharged') THEN
        SELECT COALESCE(SUM(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0)), 0)
        INTO lvar_outstanding_balance
        FROM receivable r
        WHERE r.ipdnumber = pvar_ipdapplicationformid
          AND COALESCE(r.isdeleted, false) = false;

        IF lvar_outstanding_balance > 0.009 THEN
            "returnMessage" := 'Patient cannot be discharged while a payment balance is pending.';
            RETURN;
        END IF;
    END IF;

    IF lower(trim(COALESCE(pvar_bookingstatus, ''))) = 'discharge initiated'
       AND lower(trim(COALESCE(lvar_previousbookingstatus, ''))) <> 'admitted' THEN
        "returnMessage" := 'Discharge can be initiated only for an admitted patient.';
        RETURN;
    END IF;

    IF lower(trim(COALESCE(pvar_bookingstatus, ''))) = 'discharged'
       AND lower(trim(COALESCE(lvar_previousbookingstatus, ''))) <> 'discharge initiated' THEN
        "returnMessage" := 'Discharge must be initiated before the patient can be marked as discharged.';
        RETURN;
    END IF;

    lvar_phase := CASE
        WHEN lower(trim(COALESCE(pvar_bookingstatus, ''))) IN (
            'pending',
            'draft',
            'provisional booking',
            'provisional confirmed',
            'waitlisted',
            'rejected',
            'rework requested',
            'doctor review requested',
            'ipd approved by doctor',
            'cancellation requested',
            'cancelled by patient',
            'cancelled'
        ) THEN 'Intake'
        WHEN lower(trim(COALESCE(pvar_bookingstatus, ''))) IN (
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
        WHEN lower(trim(COALESCE(pvar_bookingstatus, ''))) IN (
            'arrival confirmed',
            'confirmed for arrival',
            'patient arrived',
            'consultation scheduled',
            'admission confirmed',
            'admitted'
        ) THEN 'Admission'
        WHEN lower(trim(COALESCE(pvar_bookingstatus, ''))) IN (
            'discharge initiated',
            'discharged',
            'extended stay'
        ) THEN 'Stay'
        ELSE COALESCE(NULLIF(lvar_previousphase, ''), 'Intake')
    END;

    UPDATE ipdapplicationform
    SET
        bookingstatus = pvar_bookingstatus,
        phase = lvar_phase,
        bookingstatusdate = now(),
        modifieduser = pvar_modifieduser,
        modifieddate = now()
    WHERE ipdapplicationformid = pvar_ipdapplicationformid;

    IF lower(trim(pvar_bookingstatus)) = 'provisional confirmed' THEN

        SELECT patientname, tenantid
        INTO lvar_patientprofileid, lvar_tenantid
        FROM ipdapplicationform
        WHERE ipdapplicationformid = pvar_ipdapplicationformid;

        SELECT registrationid
        INTO lvar_registrationid
        FROM patientprofile
        WHERE patientprofileid = lvar_patientprofileid;

        IF lvar_registrationid LIKE '%-TEMP%' THEN
            lvar_registrationid := generate_formatted_numbers(
                lvar_tenantid,
                'YYMMDD-9999',
                'patientprofile',
                'registrationid'
            );

            UPDATE patientprofile
            SET registrationid = lvar_registrationid
            WHERE patientprofileid = lvar_patientprofileid;
        END IF;

        UPDATE RoomAllocation
        SET
            status = 'Booked',
            modifieduser = pvar_modifieduser,
            modifieddate = NOW()
        WHERE ipdno = pvar_ipdapplicationformid;

        UPDATE RoomOccupancyStatus
        SET
            status = 'Booked',
            modifieduser = pvar_modifieduser,
            modifieddate = NOW()
        WHERE ipdno = pvar_ipdapplicationformid
          AND isdeleted = false;

    ELSIF lower(trim(pvar_bookingstatus)) = 'admitted' THEN



    UPDATE ipdapplicationform
    SET
        estimatedarrival = CURRENT_TIMESTAMP,
        modifieduser = pvar_modifieduser,
        modifieddate = NOW()
    WHERE ipdapplicationformid = pvar_ipdapplicationformid;
	

        UPDATE RoomAllocation
        SET
            status = 'Occupied',
            modifieduser = pvar_modifieduser,
            modifieddate = NOW()
        WHERE ipdno = pvar_ipdapplicationformid;

        UPDATE RoomOccupancyStatus
        SET
            status = 'Occupied',
            modifieduser = pvar_modifieduser,
            modifieddate = NOW()
        WHERE ipdno = pvar_ipdapplicationformid
          AND isdeleted = false;

    ELSIF lower(trim(pvar_bookingstatus)) = 'discharged' THEN

        UPDATE IPDApplicationForm_preferreddatesofadmission
        SET
            dateofdeparture = CURRENT_DATE,
            daysofstay = GREATEST((CURRENT_DATE - dateofarrival) + 1, 1),
            action_date = NOW(),
            action_by = pvar_modifieduser,
            action = 'Discharged'
        WHERE ipdapplicationformid = pvar_ipdapplicationformid
          AND COALESCE(isdeleted, false) = false;

        PERFORM public."Update_IPD_DischargeDepartureDate"(pvar_ipdapplicationformid, pvar_modifieduser);

    ELSIF lower(trim(COALESCE(pvar_bookingstatus, ''))) IN (
        'cancellation requested',
        'cancelled by patient',
        'cancelled by front desk',
        'cancelled',
        'cancelled - initiated',
        'cancelled - refunded',
        'cancellation requested - refunded',
        'cancellation approved - refunded'
    ) THEN
        UPDATE RoomOccupancyStatus
        SET
            isdeleted = true,
            status = 'Cancelled',
            modifieduser = pvar_modifieduser,
            modifieddate = NOW()
        WHERE ipdno = pvar_ipdapplicationformid
          AND COALESCE(isdeleted, false) = false;

        UPDATE RoomAllocation
        SET
            status = 'Cancelled',
            modifieduser = pvar_modifieduser,
            modifieddate = NOW()
        WHERE ipdno = pvar_ipdapplicationformid
          AND LOWER(COALESCE(status, '')) <> 'cancelled';
    END IF;

    IF lvar_previousbookingstatus IS DISTINCT FROM pvar_bookingstatus
       OR lvar_previousphase IS DISTINCT FROM lvar_phase THEN
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
            pvar_ipdapplicationformid,
            lvar_verifiedstatus,
            CONCAT(
                'Booking status changed from ',
                COALESCE(lvar_previousbookingstatus, '-'),
                ' to ',
                COALESCE(pvar_bookingstatus, '-')
            ),
            pvar_bookingstatus,
            lvar_phase,
            lvar_previousbookingstatus,
            lvar_previousphase,
            'Booking Status',
            pvar_modifieduser
        );
    END IF;

    "returnMessage" := '201.1';
EXCEPTION WHEN OTHERS THEN
    "returnMessage" := SQLERRM;
END
$BODY$;

-- Repair active room holds left behind by cancellation paths deployed before
-- cancellation was made responsible for releasing room inventory.
UPDATE RoomOccupancyStatus occupancy
SET
    isdeleted = true,
    status = 'Cancelled',
    modifieduser = COALESCE(ipd.modifieduser, occupancy.modifieduser),
    modifieddate = NOW()
FROM IPDApplicationForm ipd
WHERE occupancy.ipdno = ipd.ipdapplicationformid
  AND COALESCE(occupancy.isdeleted, false) = false
  AND LOWER(BTRIM(COALESCE(ipd.bookingstatus, ''))) IN (
      'cancellation requested', 'cancelled by patient', 'cancelled by front desk',
      'cancelled', 'cancelled - initiated', 'cancelled - refunded',
      'cancellation requested - refunded', 'cancellation approved - refunded'
  );

UPDATE RoomAllocation allocation
SET
    status = 'Cancelled',
    modifieduser = COALESCE(ipd.modifieduser, allocation.modifieduser),
    modifieddate = NOW()
FROM IPDApplicationForm ipd
WHERE allocation.ipdno = ipd.ipdapplicationformid
  AND LOWER(COALESCE(allocation.status, '')) <> 'cancelled'
  AND LOWER(BTRIM(COALESCE(ipd.bookingstatus, ''))) IN (
      'cancellation requested', 'cancelled by patient', 'cancelled by front desk',
      'cancelled', 'cancelled - initiated', 'cancelled - refunded',
      'cancellation requested - refunded', 'cancellation approved - refunded'
  );


