-- NOTE: shifthours used to be `int`. It's a snapshot copy of the selected Shift's
-- shifthours value, which is now decimal(18,2) to support fractional-hour shifts
-- (see Shift.sql). Keeping this as int caused the same "value 'X.XX' is not valid"
-- model binding failure whenever a shift with fractional/decimal-formatted hours was
-- selected. On an existing database, run this once to migrate the column in place:
--   ALTER TABLE StaffAttendance ALTER COLUMN shifthours TYPE decimal(18,2) USING shifthours::decimal(18,2);
CREATE TABLE IF NOT EXISTS StaffAttendance
(
StaffAttendanceid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,shiftdate date NOT NULL
,shift uuid REFERENCES Shift(Shiftid) NOT NULL
,shiftstarttime Varchar(10) NULL
,shiftendtime Varchar(10) NULL
,shifthours decimal(18,2) NULL
,workprofile uuid REFERENCES WorkProfile(WorkProfileid) NOT NULL
,peoplename uuid REFERENCES People(Peopleid) NOT NULL
,punchdateandtime Timestamp(3) NOT NULL
,earlyinmin decimal(18,2) NULL
,earlyoutmin decimal(18,2) NULL
,latemin decimal(18,2) NULL
,workhours int NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


