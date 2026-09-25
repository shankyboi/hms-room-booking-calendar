CREATE TABLE IF NOT EXISTS TaskTemplate_escalationdetails
(
TaskTemplate_escalationdetailsid uuid PRIMARY KEY
,TaskTemplateid uuid REFERENCES TaskTemplate(TaskTemplateid)
,record_order int
,priority Varchar(1080) NULL
,notifyto uuid REFERENCES users(usersid) NULL
,emailid Varchar(128) NULL
,mobilenumber Varchar(20) NULL
);
CREATE TABLE IF NOT EXISTS TaskTemplate_nextactiondetails
(
TaskTemplate_nextactiondetailsid uuid PRIMARY KEY
,TaskTemplateid uuid REFERENCES TaskTemplate(TaskTemplateid)
,record_order int
,actiontype uuid REFERENCES ActionType(ActionTypeid) NULL
,actionname uuid REFERENCES Actions(Actionsid) NULL
);


