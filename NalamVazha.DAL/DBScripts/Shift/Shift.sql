-- NOTE: shifthours used to be `int`, but the Add/Update Shift UI computes this value
-- from start/end time and always formats it with 2 decimal places (e.g. "8.00") to
-- support fractional-hour shifts (e.g. 7.5h). Posting "8.00" to an int column/param
-- fails model binding entirely ("The value '8.00' is not valid for shifthours"), so
-- this was widened to decimal(18,2) to match the sibling totalbreakinhrs column.
-- On an existing database, run this once to migrate the column in place:
--   ALTER TABLE Shift ALTER COLUMN shifthours TYPE decimal(18,2) USING shifthours::decimal(18,2);
CREATE TABLE IF NOT EXISTS Shift
(
Shiftid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,shiftcode Varchar(128) DEFAULT 'YYYY-MM-9999' NOT NULL
,shiftname Varchar(128) NOT NULL
,shiftstarttime Varchar(10) NOT NULL
,shiftendtime Varchar(10) NOT NULL
,shifthours decimal(18,2) NOT NULL
,description Varchar(256) NULL
,totalbreakinmins Varchar(256) NOT NULL
,totalbreakinhrs decimal(18,2) NOT NULL
,workhours Varchar(256) NOT NULL
,UNIQUE(tenantid,shiftcode,shiftname)

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


