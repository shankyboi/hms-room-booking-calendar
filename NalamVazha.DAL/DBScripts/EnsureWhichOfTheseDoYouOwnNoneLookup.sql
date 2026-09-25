CREATE OR REPLACE FUNCTION "EnsureWhichOfTheseDoYouOwnNoneLookup"()
RETURNS void
AS $BODY$
BEGIN
UPDATE "lookups"
SET fielddesc = CASE
	WHEN COALESCE(fielddesc, '') = '' THEN 'None'
	ELSE fielddesc || ',None'
END
WHERE entityname = 'ConcessionForm'
AND fieldname = 'whichofthesedoyouown'
AND NOT EXISTS (
	SELECT 1
	FROM regexp_split_to_table(COALESCE(fielddesc, ''), ',') AS option_value(value)
	WHERE lower(trim(option_value.value)) = 'none'
);
END
$BODY$
LANGUAGE plpgsql;
