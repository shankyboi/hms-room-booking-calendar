ALTER TABLE IF EXISTS IPDApplicationForm_medicationinfo
ADD COLUMN IF NOT EXISTS quantity numeric(10,2) NULL;

ALTER TABLE IF EXISTS IPDApplicationForm_medicationinfo_history
ADD COLUMN IF NOT EXISTS quantity numeric(10,2) NULL;

ALTER TABLE IF EXISTS IPDApplicationForm_medicationinfo
DROP CONSTRAINT IF EXISTS ck_ipd_medicationinfo_quantity_positive;

ALTER TABLE IF EXISTS IPDApplicationForm_medicationinfo
ADD CONSTRAINT ck_ipd_medicationinfo_quantity_positive
CHECK (quantity IS NULL OR quantity > 0);

ALTER TABLE IF EXISTS IPDApplicationForm_medicationinfo_history
DROP CONSTRAINT IF EXISTS ck_ipd_medicationinfo_history_quantity_positive;

ALTER TABLE IF EXISTS IPDApplicationForm_medicationinfo_history
ADD CONSTRAINT ck_ipd_medicationinfo_history_quantity_positive
CHECK (quantity IS NULL OR quantity > 0);
