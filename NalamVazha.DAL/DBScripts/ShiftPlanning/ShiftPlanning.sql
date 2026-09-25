CREATE TABLE IF NOT EXISTS ShiftPlanning
(
ShiftPlanningid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,shiftname uuid REFERENCES Shift(Shiftid) NOT NULL
,validfrom date NOT NULL
,validto date NOT NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)

ALTER TABLE public.shiftplanning
    ADD COLUMN IF NOT EXISTS shiftstarttime varchar(10) NULL;

ALTER TABLE public.shiftplanning
    ADD COLUMN IF NOT EXISTS shiftendtime varchar(10) NULL;

