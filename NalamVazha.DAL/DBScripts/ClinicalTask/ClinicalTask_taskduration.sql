CREATE TABLE IF NOT EXISTS ClinicalTask_taskduration
(
ClinicalTask_taskdurationid uuid PRIMARY KEY
,ClinicalTaskid uuid REFERENCES ClinicalTask(ClinicalTaskid)
,record_order int
,workprofile uuid REFERENCES WorkProfile(WorkProfileid) NOT NULL
,tasktype uuid REFERENCES TaskType(TaskTypeid) NOT NULL
,taskname uuid REFERENCES Task(Taskid) NOT NULL
,durationinminutes int NOT NULL
,overbookingcount int NULL
);


