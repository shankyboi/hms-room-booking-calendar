-- FUNCTION: public.get_all_RoomType(character varying, character varying, character varying)

--DROP FUNCTION IF EXISTS public."get_all_RoomType"(character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public."get_all_RoomType"(
    pvar_tenantid character varying DEFAULT NULL::character varying,
    pvar_patient_gender character varying DEFAULT NULL::character varying,
    pvar_attendant_gender character varying DEFAULT NULL::character varying
)
RETURNS TABLE(
    name character varying,
    prebookingdaylimit integer,
    minbookingdays integer,
    maxbookingdays integer,
    concessoneligibility character varying,
    suitabilityforvip character varying,
    gendersuitability character varying,
    roomtypeicon character varying,
    deposittype character varying,
    costperday numeric,
    advanceperday numeric,
    bookingdeposit numeric,
    variableofbookingdays numeric,
    attendantcostperday numeric,
    attendantadvanceperday numeric,
    attendantbookingdeposit numeric,
    attendantvariableofbookingdays numeric,
    hourlychargesapplicable boolean,
    chargeperhour numeric,
    billingwaiverfordelayedstart character varying,
    waiverpercentage numeric,
    roomtransfercost character varying,
    description text,
    createduser uuid,
    createddate timestamp without time zone,
    modifieduser uuid,
    modifieddate timestamp without time zone,
    tenantid uuid,
    "RoomTypeid" uuid
)
LANGUAGE plpgsql
COST 100
VOLATILE PARALLEL UNSAFE
ROWS 1000
AS $BODY$
DECLARE
    lvar_tenantid varchar[];
    lstr_usersid varchar;
    l_patient_gender varchar;
BEGIN
    /*
        Tenant handling:
        Expected tenant input can be:
        - tenantid
        - userid|tenantid
    */
    SELECT
        SPLIT_PART(COALESCE(pvar_tenantid, ''), '|', 1),
        SPLIT_PART(COALESCE(pvar_tenantid, ''), '|', 2)
    INTO
        lstr_usersid,
        pvar_tenantid;

    IF (
        pvar_tenantid IS NULL
        OR pvar_tenantid = ''
        OR pvar_tenantid = '00000000-0000-0000-0000-000000000000'
    ) THEN
        SELECT STRING_TO_ARRAY(viewertenantids, ',')
        INTO lvar_tenantid
        FROM users
        WHERE users.usersid::varchar = lstr_usersid;

        IF lvar_tenantid IS NULL THEN
            SELECT ARRAY_AGG(tenant.tenantid::varchar)
            INTO lvar_tenantid
            FROM tenant;
        END IF;
    ELSE
        lvar_tenantid := ARRAY[pvar_tenantid];
    END IF;

    lvar_tenantid := lvar_tenantid
        || ARRAY[''::character varying]
        || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];

    /*
        Normalize patient gender:
        Male   / male      => male
        Female / female    => female
        Other  / Others    => other
    */
    l_patient_gender := REPLACE(
        LOWER(TRIM(COALESCE(pvar_patient_gender, ''))),
        'others',
        'other'
    );

    RETURN QUERY
    SELECT
        rt.name,
        rt.prebookingdaylimit,
        rt.minbookingdays,
        rt.maxbookingdays,
        rt.concessoneligibility,
        rt.suitabilityforvip,
        rt.gendersuitability,
        rt.roomtypeicon,
        rt.deposittype,
        rt.costperday,
        rt.advanceperday,
        rt.bookingdeposit,
        rt.variableofbookingdays,
        rt.attendantcostperday,
        rt.attendantadvanceperday,
        rt.attendantbookingdeposit,
        rt.attendantvariableofbookingdays,
        COALESCE(rt.hourlychargesapplicable, true) AS hourlychargesapplicable,
        rt.chargeperhour,
        rt.billingwaiverfordelayedstart,
        rt.waiverpercentage,
        rt.roomtransfercost,
        rt.description,
        rt.createduser,
        rt.createddate,
        rt.modifieduser,
        rt.modifieddate,
        rt.tenantid,
        rt.RoomTypeid
    FROM RoomType rt
    WHERE
        (
            lvar_tenantid IS NULL
            OR COALESCE(rt.tenantid::varchar, '') = ANY(lvar_tenantid)
        )
        AND rt.isdeleted = false

        /*
            Gender Applicability rule:

            Male patient:
                show room types where Gender Applicability contains Male OR All

            Female patient:
                show room types where Gender Applicability contains Female OR All

            Other patient:
                show room types where Gender Applicability contains Other OR All

            Examples:
                gendersuitability = 'Male'
                    Male only

                gendersuitability = 'Female'
                    Female only

                gendersuitability = 'Other'
                    Other only

                gendersuitability = 'All'
                    Male + Female + Other

                gendersuitability = 'Female,Male'
                    Male + Female only

                gendersuitability = 'Female,Male,All'
                    Male + Female + Other because it contains All

            Attendant gender is intentionally NOT used for room type filtering.
        */
        AND (
            l_patient_gender = ''

            OR EXISTS (
                SELECT 1
                FROM unnest(
                    string_to_array(
                        REPLACE(
                            REPLACE(
                                REPLACE(
                                    REPLACE(
                                        LOWER(TRIM(COALESCE(rt.gendersuitability, ''))),
                                        ' ',
                                        ''
                                    ),
                                    '-',
                                    ''
                                ),
                                '_',
                                ''
                            ),
                            'others',
                            'other'
                        ),
                        ','
                    )
                ) AS g(gender_value)
                WHERE
                    /*
                        All means applicable for Male, Female, and Other.
                    */
                    g.gender_value IN (
                        'all',
                        'allgender',
                        'allgenders'
                    )

                    /*
                        Specific patient gender match.
                    */
                    OR g.gender_value = l_patient_gender
            )
        );

END;
$BODY$;
