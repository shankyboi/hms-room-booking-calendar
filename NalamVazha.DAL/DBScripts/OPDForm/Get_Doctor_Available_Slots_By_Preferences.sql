CREATE OR REPLACE FUNCTION "Get_Doctor_Available_Slots_By_Preferences"
(
    pvar_peopleid uuid,
    pvar_appointmentdate timestamp,
    pvar_slotpreferences varchar,
    pvar_tenantid uuid DEFAULT NULL,
    pvar_taskid uuid DEFAULT NULL,
    pvar_taskname varchar DEFAULT NULL
)
RETURNS TABLE(slotfrom varchar, slotto varchar, isbooked boolean)
AS $BODY$
BEGIN
    RETURN QUERY
    WITH base AS (
        SELECT *
        FROM "Get_Doctor_Available_Slots_Latest"(
            pvar_peopleid,
            pvar_appointmentdate,
            pvar_tenantid,
            pvar_taskid,
            pvar_taskname
        )
    ),
    prefs AS (
        SELECT lower(trim(value)) AS preference
        FROM regexp_split_to_table(COALESCE(pvar_slotpreferences, ''), ',') AS value
        WHERE trim(value) <> ''
    ),
    flags AS (
        SELECT
            NOT EXISTS (SELECT 1 FROM prefs)
                OR EXISTS (SELECT 1 FROM prefs WHERE preference = 'any slot') AS has_any,
            EXISTS (SELECT 1 FROM prefs WHERE preference = 'early morning') AS has_early_morning,
            EXISTS (SELECT 1 FROM prefs WHERE preference = 'closer to lunch') AS has_closer_lunch,
            EXISTS (SELECT 1 FROM prefs WHERE preference = 'closer to day') AS has_closer_day
    ),
    bounds AS (
        SELECT
            MIN(slotfrom::time) AS first_slot,
            MAX(slotfrom::time) AS last_slot
        FROM base
    )
    SELECT filtered.slotfrom, filtered.slotto, filtered.isbooked
    FROM (
        SELECT DISTINCT b.slotfrom, b.slotto, b.isbooked
        FROM base b
        CROSS JOIN flags f
        CROSS JOIN bounds bd
        WHERE f.has_any
            OR (f.has_early_morning AND b.slotfrom::time < TIME '10:00')
            OR (f.has_closer_lunch AND ABS(EXTRACT(EPOCH FROM (b.slotfrom::time - TIME '12:00'))) <= 7200)
            OR (f.has_closer_day AND b.slotfrom::time >= (bd.last_slot - INTERVAL '2 hours'))
    ) filtered
    ORDER BY filtered.slotfrom::time;
END;
$BODY$
LANGUAGE plpgsql;
