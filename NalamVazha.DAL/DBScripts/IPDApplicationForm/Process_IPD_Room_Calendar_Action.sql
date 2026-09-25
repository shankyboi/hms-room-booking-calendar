
CREATE OR REPLACE FUNCTION public."Process_IPD_Room_Calendar_Action"(
    pvar_action character varying,
    pvar_items json,
    pvar_tenantid uuid,
    pvar_modifieduser uuid,
    pvar_reviewcomments character varying,
    OUT pvar_returnmessage character varying
)
RETURNS character varying
LANGUAGE plpgsql
COST 100
VOLATILE
PARALLEL UNSAFE
AS $BODY$
DECLARE
    lvar_action varchar;

    lvar_item json;
    lvar_ipdapplicationformid uuid;

    lvar_verifiedstatus varchar;
    lvar_bookingstatus varchar;
    lvar_phase varchar := 'Intake';

    lvar_previousbookingstatus varchar;
    lvar_previousverifiedstatus varchar;
    lvar_previousphase varchar;

    lvar_bookingreferencenumber varchar;
    lvar_packagename uuid;
    lvar_isbookingdepositmandatory boolean;

    lvar_room json;
    lvar_actionmessage varchar;

    lvar_updatedcount integer;
BEGIN
    pvar_returnmessage := '';

    /*
     * Normalise action.
     *
     * "Doctor Review" becomes "DOCTOR_REVIEW".
     */
    lvar_action :=
        upper(
            replace(
                trim(
                    coalesce(
                        pvar_action,
                        ''
                    )
                ),
                ' ',
                '_'
            )
        );

    /*
     * Only one action can be sent in one request.
     */
    IF lvar_action NOT IN (
        'APPROVE',
        'CANCEL',
        'REWORK',
        'DOCTOR_REVIEW'
    ) THEN
        pvar_returnmessage :=
            'Invalid calendar action.';

        RETURN;
    END IF;

    /*
     * Validate common inputs.
     */
    IF pvar_items IS NULL
       OR json_typeof(pvar_items) <> 'array'
       OR json_array_length(pvar_items) = 0
    THEN
        pvar_returnmessage :=
            'At least one IPD application is required.';

        RETURN;
    END IF;

    IF pvar_tenantid IS NULL THEN
        pvar_returnmessage :=
            'Tenant ID is required.';

        RETURN;
    END IF;

    IF pvar_modifieduser IS NULL THEN
        pvar_returnmessage :=
            'Modified user is required.';

        RETURN;
    END IF;

    /*
     * Same authorization check used in Allot_Room.
     */
    IF NOT coalesce(
        public."Check_Authorization"(
            pvar_modifieduser,
            'IPDApplicationForm',
            'edit'
        ),
        false
    ) THEN
        pvar_returnmessage := '401.1';
        RETURN;
    END IF;

    /*
     * Set status values for non-approval actions.
     */
    CASE lvar_action
        WHEN 'CANCEL' THEN
            lvar_verifiedstatus := 'Cancelled';
            lvar_bookingstatus := 'Cancelled';

        WHEN 'REWORK' THEN
            lvar_verifiedstatus := 'Rework';
            lvar_bookingstatus := 'Rework Requested';

        WHEN 'DOCTOR_REVIEW' THEN
            lvar_verifiedstatus := 'Doctor Review';
            lvar_bookingstatus := 'Doctor Review';

        ELSE
            lvar_verifiedstatus := NULL;
            lvar_bookingstatus := NULL;
    END CASE;

    /*
     * First validate all selected records.
     * No database changes happen in this loop.
     */
    FOR lvar_item IN
        SELECT value
        FROM json_array_elements(pvar_items)
    LOOP
        IF json_typeof(lvar_item) <> 'object' THEN
            pvar_returnmessage :=
                'Invalid IPD action item.';

            RETURN;
        END IF;

        IF nullif(
            trim(
                lvar_item->>'IPDApplicationFormid'
            ),
            ''
        ) IS NULL THEN
            pvar_returnmessage :=
                'IPDApplicationFormid is required.';

            RETURN;
        END IF;

        BEGIN
            lvar_ipdapplicationformid :=
                (
                    lvar_item
                    ->>'IPDApplicationFormid'
                )::uuid;
        EXCEPTION
            WHEN OTHERS THEN
                pvar_returnmessage :=
                    'Invalid IPDApplicationFormid.';

                RETURN;
        END;

        /*
         * Application must belong to the given tenant.
         */
        IF NOT EXISTS (
            SELECT 1
            FROM IPDApplicationForm ipd
            WHERE
                ipd.IPDApplicationFormid =
                    lvar_ipdapplicationformid

                AND ipd.tenantid =
                    pvar_tenantid

                AND coalesce(
                    ipd.isdeleted,
                    false
                ) = false
        ) THEN
            pvar_returnmessage :=
                'One or more selected IPD applications were not found.';

            RETURN;
        END IF;

        /*
         * Approval requires at least one room row.
         */
        IF lvar_action = 'APPROVE' THEN
            IF lvar_item->'room' IS NULL
               OR json_typeof(
                   lvar_item->'room'
               ) <> 'array'
               OR json_array_length(
                   lvar_item->'room'
               ) = 0
            THEN
                pvar_returnmessage :=
                    'Room allocation is required for approval.';

                RETURN;
            END IF;
        END IF;
    END LOOP;

    /*
     * Process every selected IPD.
     */
    FOR lvar_item IN
        SELECT value
        FROM json_array_elements(pvar_items)
    LOOP
        lvar_ipdapplicationformid :=
            (
                lvar_item
                ->>'IPDApplicationFormid'
            )::uuid;

        /*
         * Capture previous values for review log.
         */
        SELECT
            ipd.bookingstatus,
            ipd.verifiedstatus,
            ipd.phase
        INTO
            lvar_previousbookingstatus,
            lvar_previousverifiedstatus,
            lvar_previousphase
        FROM IPDApplicationForm ipd
        WHERE
            ipd.IPDApplicationFormid =
                lvar_ipdapplicationformid

            AND ipd.tenantid =
                pvar_tenantid;

        /*
         * APPROVE
         */
        IF lvar_action = 'APPROVE' THEN
            SELECT
                ipd.bookingreferencenumber,
                ipd.packagename,
                coalesce(
                    ipd.isbookingdepositmandatory,
                    false
                )
            INTO
                lvar_bookingreferencenumber,
                lvar_packagename,
                lvar_isbookingdepositmandatory
            FROM IPDApplicationForm ipd
            WHERE
                ipd.IPDApplicationFormid =
                    lvar_ipdapplicationformid

                AND ipd.tenantid =
                    pvar_tenantid;

            lvar_room :=
                lvar_item->'room';

            /*
             * Existing Allot_Room handles:
             *
             * 1. Room conflict validation
             * 2. IPDApplicationForm_room
             * 3. RoomAllocation
             * 4. RoomOccupancyStatus
             * 5. IPD approval
             */
            SELECT public."Allot_Room"(
                lvar_ipdapplicationformid,
                pvar_tenantid,
                lvar_bookingreferencenumber,
                lvar_packagename,
                lvar_isbookingdepositmandatory,
                'Approved',
                lvar_room,
                pvar_modifieduser
            )
            INTO lvar_actionmessage;

            IF replace(
                coalesce(
                    lvar_actionmessage,
                    ''
                ),
                '"',
                ''
            ) <> '201.1' THEN
                RAISE EXCEPTION '%',
                    coalesce(
                        lvar_actionmessage,
                        'Room approval failed.'
                    );
            END IF;

            /*
             * Required calendar approval result:
             *
             * verifiedstatus = Approved
             * bookingstatus  = Provisional Booking
             */
            UPDATE IPDApplicationForm
            SET
                verifiedstatus =
                    'Approved',

                bookingstatus =
                    'Provisional Booking',

                phase =
                    'Intake',

                verifiedby =
                    pvar_modifieduser,

                verifieddate =
                    now(),

                bookingstatusdate =
                    now(),

                reviewcomments =
                    coalesce(
                        pvar_reviewcomments,
                        ''
                    ),

                modifieduser =
                    pvar_modifieduser,

                modifieddate =
                    now()
            WHERE
                IPDApplicationFormid =
                    lvar_ipdapplicationformid

                AND tenantid =
                    pvar_tenantid;

            GET DIAGNOSTICS
                lvar_updatedcount = ROW_COUNT;

            IF lvar_updatedcount = 0 THEN
                RAISE EXCEPTION
                    'Unable to update the approved IPD application.';
            END IF;

            /*
             * Approval review log.
             */
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
                'Approved',
                coalesce(
                    pvar_reviewcomments,
                    ''
                ),
                'Provisional Booking',
                'Intake',
                lvar_previousbookingstatus,
                lvar_previousphase,
                'Review',
                pvar_modifieduser
            );

        /*
         * CANCEL, REWORK and DOCTOR REVIEW
         */
        ELSE
            UPDATE IPDApplicationForm
            SET
                verifiedby =
                    pvar_modifieduser,

                verifiedstatus =
                    lvar_verifiedstatus,

                bookingstatus =
                    lvar_bookingstatus,

                phase =
                    lvar_phase,

                bookingstatusdate =
                    now(),

                verifieddate =
                    now(),

                reviewcomments =
                    coalesce(
                        pvar_reviewcomments,
                        ''
                    ),

                modifieduser =
                    pvar_modifieduser,

                modifieddate =
                    now()
            WHERE
                IPDApplicationFormid =
                    lvar_ipdapplicationformid

                AND tenantid =
                    pvar_tenantid;

            GET DIAGNOSTICS
                lvar_updatedcount = ROW_COUNT;

            IF lvar_updatedcount = 0 THEN
                RAISE EXCEPTION
                    'Unable to update IPD application.';
            END IF;

            IF lvar_action = 'CANCEL' THEN
                UPDATE RoomOccupancyStatus
                SET
                    isdeleted = true,
                    status = 'Cancelled',
                    modifieduser = pvar_modifieduser,
                    modifieddate = now()
                WHERE ipdno = lvar_ipdapplicationformid
                  AND COALESCE(isdeleted, false) = false;

                UPDATE RoomAllocation
                SET
                    status = 'Cancelled',
                    modifieduser = pvar_modifieduser,
                    modifieddate = now()
                WHERE ipdno = lvar_ipdapplicationformid
                  AND LOWER(COALESCE(status, '')) <> 'cancelled';
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
                lvar_verifiedstatus,
                coalesce(
                    pvar_reviewcomments,
                    ''
                ),
                lvar_bookingstatus,
                lvar_phase,
                lvar_previousbookingstatus,
                lvar_previousphase,
                'Review',
                pvar_modifieduser
            );
        END IF;
    END LOOP;

    pvar_returnmessage := '201.1';

EXCEPTION
    WHEN OTHERS THEN
        /*
         * When one selected IPD fails, the complete
         * request is rolled back.
         */
        pvar_returnmessage := SQLERRM;
END;
$BODY$;

ALTER FUNCTION public."Process_IPD_Room_Calendar_Action"(
    character varying,
    json,
    uuid,
    uuid,
    character varying
)
OWNER TO sa;
