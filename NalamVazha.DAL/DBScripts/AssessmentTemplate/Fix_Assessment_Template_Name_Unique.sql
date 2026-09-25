DO $$
DECLARE
	uniqueConstraint record;
BEGIN
	FOR uniqueConstraint IN
		SELECT
			c.conrelid::regclass AS table_name,
			c.conname AS constraint_name
		FROM pg_constraint c
		JOIN pg_class t ON t.oid = c.conrelid
		JOIN pg_namespace n ON n.oid = t.relnamespace
		WHERE c.contype = 'u'
		  AND n.nspname = 'public'
		  AND lower(t.relname) = lower('AssessmentTemplate')
		  AND EXISTS
		  (
			  SELECT 1
			  FROM unnest(c.conkey) AS constraintColumn(attnum)
			  JOIN pg_attribute a ON a.attrelid = c.conrelid AND a.attnum = constraintColumn.attnum
			  WHERE lower(a.attname) = 'tenantid'
		  )
		  AND EXISTS
		  (
			  SELECT 1
			  FROM unnest(c.conkey) AS constraintColumn(attnum)
			  JOIN pg_attribute a ON a.attrelid = c.conrelid AND a.attnum = constraintColumn.attnum
			  WHERE lower(a.attname) = 'templatename'
		  )
	LOOP
		EXECUTE format('ALTER TABLE %s DROP CONSTRAINT IF EXISTS %I', uniqueConstraint.table_name, uniqueConstraint.constraint_name);
	END LOOP;
END $$;

CREATE UNIQUE INDEX IF NOT EXISTS idx_assessmenttemplate_tenant_templatename_active
ON AssessmentTemplate (tenantid, upper(templatename::varchar))
WHERE COALESCE(isdeleted, false) = false;
