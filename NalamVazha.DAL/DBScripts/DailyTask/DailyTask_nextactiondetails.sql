CREATE TABLE IF NOT EXISTS DailyTask_nextactiondetails
(
DailyTask_nextactiondetailsid uuid PRIMARY KEY
,DailyTaskid uuid REFERENCES DailyTask(DailyTaskid)
,record_order int
,nextaction Varchar(256) NULL
);


