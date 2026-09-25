CREATE TABLE IF NOT EXISTS ShiftPlanning_people
(
ShiftPlanning_peopleid uuid PRIMARY KEY
,ShiftPlanningid uuid REFERENCES ShiftPlanning(ShiftPlanningid)
,record_order int
,personname uuid REFERENCES People(Peopleid) NOT NULL
,workprofile uuid REFERENCES WorkProfile(WorkProfileid) NULL
,coveragetype Varchar(1080) NOT NULL
);


