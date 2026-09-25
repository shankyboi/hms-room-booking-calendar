-- FUNCTION: public.get_IPD_Room_Calendar_Cards(character varying, character varying, character varying)

-- DROP FUNCTION IF EXISTS public."get_IPD_Room_Calendar_Cards"(character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public."get_IPD_Room_Calendar_Cards"(
	pvar_tenantid character varying DEFAULT NULL::character varying,
	pvar_fromdate character varying DEFAULT NULL::character varying,
	pvar_todate character varying DEFAULT NULL::character varying)
    RETURNS TABLE("recordType" character varying, "allocationId" uuid, "applicationId" uuid, "bookingReferenceNumber" character varying, "patientName" character varying,
"patientGender" character varying,
"patientPhone" character varying,
"patientEmail" character varying,
"generalCondition" character varying,

"flexibleWithDates" boolean,
"flexibleWithRoomType" boolean,
"joinWaitingList" boolean,

"bookingStatus" character varying, "calendarStatus" character varying, "allottedTo" character varying, "recordOrder" integer, "roomId" uuid, "roomTypeId" uuid, "roomNumber" character varying, "fromDate" date, "toDate" date, "createdDate" timestamp without time zone, "roomPreferences" jsonb, "attendantRoomPreferences" jsonb, "patientDates" jsonb, attendant jsonb, "attendantDates" jsonb, "hasAttendant" boolean) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    lvar_tenantid varchar[];
    lstr_usersid varchar;
    lstr_tenantid varchar;
    ldt_fromdate date;
    ldt_todate date;
BEGIN
   
    SELECT
        SPLIT_PART(
            COALESCE(pvar_tenantid, ''),
            '|',
            1
        ),
        SPLIT_PART(
            COALESCE(pvar_tenantid, ''),
            '|',
            2
        )
    INTO
        lstr_usersid,
        lstr_tenantid;

    /*
     * When tenant is empty, use the login user's
     * permitted viewer tenant IDs.
     */
    IF (
        lstr_tenantid IS NULL
        OR lstr_tenantid = ''
        OR lstr_tenantid =
           '00000000-0000-0000-0000-000000000000'
    ) THEN
        SELECT STRING_TO_ARRAY(
            u.viewertenantids,
            ','
        )
        INTO lvar_tenantid
        FROM users u
        WHERE u.usersid::varchar = lstr_usersid;

        /*
         * Preserve the existing project fallback:
         * when no viewer tenants exist, allow all tenants.
         */
        IF lvar_tenantid IS NULL THEN
            SELECT ARRAY_AGG(t.tenantid::varchar)
            INTO lvar_tenantid
            FROM tenant t;
        END IF;
    ELSE
        lvar_tenantid :=
            ARRAY[lstr_tenantid];
    END IF;

    lvar_tenantid :=
        COALESCE(
            lvar_tenantid,
            ARRAY[]::varchar[]
        )
        || ARRAY[''::varchar]
        || ARRAY[
            '00000000-0000-0000-0000-000000000000'
           ::varchar
        ];

    /*
     * Empty dates are allowed.
     * When supplied, expected format is YYYY-MM-DD.
     */
    ldt_fromdate :=
        NULLIF(
            TRIM(COALESCE(pvar_fromdate, '')),
            ''
        )::date;

    ldt_todate :=
        NULLIF(
            TRIM(COALESCE(pvar_todate, '')),
            ''
        )::date;

    RETURN QUERY

    WITH fixed_cards AS
    (
        /*
         * These records are already allocated.
         *
         * Every row in ipdapplicationform_room becomes
         * one calendar segment. This supports patient,
         * attendant and split-room allocations.
         */
        SELECT
            'FIXED'::varchar
                AS "recordType",

            iar.ipdapplicationform_roomid
                AS "allocationId",

            ipd.ipdapplicationformid
                AS "applicationId",

            ipd.bookingreferencenumber
                AS "bookingReferenceNumber",

            COALESCE(
                NULLIF(
                    BTRIM(
                        CONCAT_WS(
                            ' ',
                            ipd.firstname,
                            ipd.lastname
                        )
                    ),
                    ''
                ),
                ipd.bookingreferencenumber
            )::varchar
                AS "patientName",

          ipd.gender::varchar
    AS "patientGender",

ipd.mobilenumber::varchar
    AS "patientPhone",

COALESCE(
    NULLIF(
        BTRIM(pp.emailaddress),
        ''
    ),
    ''
)::varchar
    AS "patientEmail",

ipd.generalcondition::varchar
    AS "generalCondition",

LOWER(
    TRIM(
        COALESCE(ipd.flexiblewithdates, '')
    )
) IN ('yes', 'true', 'y', '1')
    AS "flexibleWithDates",

LOWER(
    TRIM(
        COALESCE(ipd.flexiblewithroomtype, '')
    )
) IN ('yes', 'true', 'y', '1')
    AS "flexibleWithRoomType",

LOWER(
    TRIM(
        COALESCE(ipd.joinwaitinglist, '')
    )
) IN ('yes', 'true', 'y', '1')
    AS "joinWaitingList",

ipd.bookingstatus
    AS "bookingStatus",

            CASE
                WHEN LOWER(TRIM(ipd.bookingstatus))
                     = 'admitted'
                    THEN 'Occupied'

                WHEN LOWER(TRIM(ipd.bookingstatus))
                     = 'provisional booking'
                    THEN 'Blocked'

                WHEN LOWER(TRIM(ipd.bookingstatus))
                     = 'provisional confirmed'
                    THEN 'Booked'

                ELSE ipd.bookingstatus
            END::varchar
                AS "calendarStatus",

            COALESCE(
                NULLIF(
                    TRIM(iar.allottedto),
                    ''
                ),
                'Patient'
            )::varchar
                AS "allottedTo",

            COALESCE(iar.record_order, 1)
                AS "recordOrder",

            rm.roomid
                AS "roomId",

            rm.roomtype
                AS "roomTypeId",

            COALESCE(
                NULLIF(
                    TRIM(rm.roomnumber),
                    ''
                ),
                rm.roomcode
            )::varchar
                AS "roomNumber",

            iar.fromdate::date
                AS "fromDate",

            iar.todate::date
                AS "toDate",

            ipd.createddate::timestamp
                AS "createdDate",

            '[]'::jsonb
                AS "roomPreferences",
'[]'::jsonb
    AS "attendantRoomPreferences",

            '[]'::jsonb
                AS "patientDates",

            '{}'::jsonb
                AS "attendant",

            '[]'::jsonb
                AS "attendantDates",

            CASE
                WHEN LOWER(
                    TRIM(
                        COALESCE(
                            iar.allottedto,
                            ''
                        )
                    )
                ) = 'attendant'
                    THEN true
                ELSE false
            END
                AS "hasAttendant"

        FROM ipdapplicationform ipd
LEFT JOIN patientprofile pp
    ON pp.patientprofileid = ipd.patientname
        INNER JOIN ipdapplicationform_room iar
            ON iar.ipdapplicationformid =
               ipd.ipdapplicationformid
           AND COALESCE(iar.isdeleted, false) = false

        INNER JOIN room rm
            ON rm.roomid = iar.roomnumber
           AND COALESCE(rm.isdeleted, false) = false

        WHERE
            COALESCE(ipd.isdeleted, false) = false

            AND (
                lvar_tenantid IS NULL
                OR COALESCE(
                    ipd.tenantid::varchar,
                    ''
                ) = ANY(lvar_tenantid)
            )

            AND LOWER(TRIM(ipd.bookingstatus))
                IN
                (
                    'admitted',
                    'provisional booking',
                    'provisional confirmed'
                )

            AND iar.roomnumber IS NOT NULL
            AND iar.fromdate IS NOT NULL
            AND iar.todate IS NOT NULL

            /*
            AND
            (
                ldt_fromdate IS NULL
                OR ldt_todate IS NULL
                OR
                (
                    iar.fromdate::date < ldt_todate
                    AND iar.todate::date > ldt_fromdate
                )
            )
            */
    ),

    pending_cards AS
    (
        /*
         * One row per pending IPD application.
         *
         * Repeating child records are returned as JSON
         * arrays ordered by record_order.
         */
        SELECT
            'PENDING'::varchar
                AS "recordType",

            NULL::uuid
                AS "allocationId",

            ipd.ipdapplicationformid
                AS "applicationId",

            ipd.bookingreferencenumber
                AS "bookingReferenceNumber",

            COALESCE(
                NULLIF(
                    BTRIM(
                        CONCAT_WS(
                            ' ',
                            ipd.firstname,
                            ipd.lastname
                        )
                    ),
                    ''
                ),
                ipd.bookingreferencenumber
            )::varchar
                AS "patientName",

          ipd.gender::varchar
    AS "patientGender",

ipd.mobilenumber::varchar
    AS "patientPhone",

COALESCE(
    NULLIF(
        BTRIM(pp.emailaddress),
        ''
    ),
    ''
)::varchar
    AS "patientEmail",

ipd.generalcondition::varchar
    AS "generalCondition",

LOWER(
    TRIM(
        COALESCE(ipd.flexiblewithdates, '')
    )
) IN ('yes', 'true', 'y', '1')
    AS "flexibleWithDates",

LOWER(
    TRIM(
        COALESCE(ipd.flexiblewithroomtype, '')
    )
) IN ('yes', 'true', 'y', '1')
    AS "flexibleWithRoomType",

LOWER(
    TRIM(
        COALESCE(ipd.joinwaitinglist, '')
    )
) IN ('yes', 'true', 'y', '1')
    AS "joinWaitingList",

ipd.bookingstatus
    AS "bookingStatus",

            'Pending'::varchar
                AS "calendarStatus",

            NULL::varchar
                AS "allottedTo",

            NULL::integer
                AS "recordOrder",

            NULL::uuid
                AS "roomId",

            NULL::uuid
                AS "roomTypeId",

            NULL::varchar
                AS "roomNumber",

            NULL::date
                AS "fromDate",

            NULL::date
                AS "toDate",

            ipd.createddate::timestamp
                AS "createdDate",

         COALESCE(
    pref.room_preferences,
    '[]'::jsonb
)
    AS "roomPreferences",

COALESCE(
    attpref.attendant_room_preferences,
    '[]'::jsonb
)
    AS "attendantRoomPreferences",

COALESCE(
    pdates.patient_dates,
    '[]'::jsonb
)
    AS "patientDates",

            COALESCE(
                att.attendant_data,
                '{}'::jsonb
            )
                AS "attendant",

            COALESCE(
                adates.attendant_dates,
                '[]'::jsonb
            )
                AS "attendantDates",

            CASE
                WHEN att.attendant_data IS NOT NULL
                    THEN true
                ELSE false
            END
                AS "hasAttendant"

        FROM ipdapplicationform ipd
		LEFT JOIN patientprofile pp
    ON pp.patientprofileid = ipd.patientname
/*
 * Attendant room-type preferences.
 */
LEFT JOIN LATERAL
(
    SELECT JSONB_AGG(
        JSONB_BUILD_OBJECT(
            /*
             * The generated child tables in this project
             * use zero-based record_order.
             */
            'rank',
            COALESCE(arp.record_order, 0) + 1,

            'roomTypeId',
            arp.roomtypeatt
        )
        ORDER BY
            arp.record_order ASC NULLS LAST
    ) AS attendant_room_preferences

    FROM ipdapplicationform_attendantroompreference arp

    WHERE
        arp.ipdapplicationformid =
            ipd.ipdapplicationformid

        AND COALESCE(
            arp.isdeleted,
            false
        ) = false
) attpref ON true
        /*
         * Patient room-type preferences.
         */
        LEFT JOIN LATERAL
        (
            SELECT JSONB_AGG(
                JSONB_BUILD_OBJECT(
                    'rank',
                    COALESCE(rp.record_order, 1),

                    'roomTypeId',
                    rp.roomtype
                )
                ORDER BY
                    rp.record_order ASC NULLS LAST,
                    rp.action_date ASC
            ) AS room_preferences

            FROM ipdapplicationform_roompreference rp

            WHERE
                rp.ipdapplicationformid =
                    ipd.ipdapplicationformid

                AND COALESCE(
                    rp.isdeleted,
                    false
                ) = false
        ) pref ON true

        /*
         * Patient preferred dates.
         */
        LEFT JOIN LATERAL
        (
            SELECT JSONB_AGG(
                JSONB_BUILD_OBJECT(
                    'rank',
                    COALESCE(pd.record_order, 1),

                    'arrivalDate',
                    pd.dateofarrival,

                    'departureDate',
                    pd.dateofdeparture,

                    'daysOfStay',
                    pd.daysofstay
                )
                ORDER BY
                    pd.record_order ASC NULLS LAST,
                    pd.action_date ASC
            ) AS patient_dates

            FROM
                ipdapplicationform_preferreddatesofadmission
                    pd

            WHERE
                pd.ipdapplicationformid =
                    ipd.ipdapplicationformid

                AND COALESCE(
                    pd.isdeleted,
                    false
                ) = false
        ) pdates ON true

        /*
         * Current frontend supports one attendant.
         * Therefore, take the first active attendant row.
         */
        LEFT JOIN LATERAL
        (
            SELECT JSONB_BUILD_OBJECT(
                'id',
                ai.ipdapplicationform_attendantinfoid,

                'name',
                ai.attendantname,

                'age',
                ai.age,

                'gender',
                ai.gender,

                'phone',
                ai.phonenumber
            ) AS attendant_data

            FROM ipdapplicationform_attendantinfo ai

            WHERE
                ai.ipdapplicationformid =
                    ipd.ipdapplicationformid

                AND COALESCE(
                    ai.isdeleted,
                    false
                ) = false

            ORDER BY
                ai.record_order ASC NULLS LAST,
                ai.action_date ASC

            LIMIT 1
        ) att ON true

        /*
         * Attendant preferred dates.
         */
        LEFT JOIN LATERAL
        (
            SELECT JSONB_AGG(
                JSONB_BUILD_OBJECT(
                    'rank',
                    COALESCE(ad.record_order, 1),

                    'arrivalDate',
                    ad.dateofarrivalatt,

                    'departureDate',
                    ad.dateofdepartureatt,

                    'daysOfStay',
                    ad.daysofstayatt
                )
                ORDER BY
                    ad.record_order ASC NULLS LAST,
                    ad.createddate ASC
            ) AS attendant_dates

            FROM
                ipdapplicationform_attendantpreferreddates
                    ad

            WHERE
                ad.ipdapplicationformid =
                    ipd.ipdapplicationformid

                AND COALESCE(
                    ad.isdeleted,
                    false
                ) = false
        ) adates ON true

        WHERE
            COALESCE(ipd.isdeleted, false) = false

            AND (
                lvar_tenantid IS NULL
                OR COALESCE(
                    ipd.tenantid::varchar,
                    ''
                ) = ANY(lvar_tenantid)
            )

            AND LOWER(TRIM(ipd.bookingstatus))
                = 'pending'

            /*
             * Include the application when:
             *
             * 1. No calendar dates were supplied, or
             * 2. It has no preferred dates, so JS can push it
             *    to Review Queue, or
             * 3. At least one preferred date overlaps the
             *    requested calendar range.
             */
            AND
            (
                ldt_fromdate IS NULL
                OR ldt_todate IS NULL

                OR NOT EXISTS
                (
                    SELECT 1
                    FROM
                        ipdapplicationform_preferreddatesofadmission
                            check_pd
                    WHERE
                        check_pd.ipdapplicationformid =
                            ipd.ipdapplicationformid

                        AND COALESCE(
                            check_pd.isdeleted,
                            false
                        ) = false
                )

                OR EXISTS
                (
                    SELECT 1
                    FROM
                        ipdapplicationform_preferreddatesofadmission
                            check_pd
                    WHERE
                        check_pd.ipdapplicationformid =
                            ipd.ipdapplicationformid

                        AND COALESCE(
                            check_pd.isdeleted,
                            false
                        ) = false

                        AND check_pd.dateofarrival <
                            ldt_todate

                        AND check_pd.dateofdeparture >
                            ldt_fromdate
                )
            )
    ),

    all_calendar_rows AS
    (
        SELECT *
        FROM fixed_cards

        UNION ALL

        SELECT *
        FROM pending_cards
    )

    SELECT *
    FROM all_calendar_rows result_rows

    ORDER BY
        CASE
            WHEN result_rows."recordType" = 'FIXED'
                THEN 1
            ELSE 2
        END,

        result_rows."createdDate" ASC,

        result_rows."recordOrder" ASC NULLS LAST;

END;
$BODY$;

ALTER FUNCTION public."get_IPD_Room_Calendar_Cards"(character varying, character varying, character varying)
    OWNER TO sa;
