CREATE TABLE IF NOT EXISTS Shift_breakdurationdetails
(
Shift_breakdurationdetailsid uuid PRIMARY KEY
,Shiftid uuid REFERENCES Shift(Shiftid)
,record_order int
,breakname Varchar(128) NOT NULL
,starttime Varchar(10) NOT NULL
,endtime Varchar(10) NOT NULL
,durationinmin decimal(18,2) NOT NULL
);


