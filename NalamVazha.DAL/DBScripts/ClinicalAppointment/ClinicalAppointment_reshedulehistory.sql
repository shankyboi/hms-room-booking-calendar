CREATE TABLE IF NOT EXISTS ClinicalAppointment_reshedulehistory
(
ClinicalAppointment_reshedulehistoryid uuid PRIMARY KEY
,ClinicalAppointmentid uuid REFERENCES ClinicalAppointment(ClinicalAppointmentid)
,record_order int
,resheduleddatetime Timestamp(3) NULL
,reshedulereason Varchar(1080) NULL
);


