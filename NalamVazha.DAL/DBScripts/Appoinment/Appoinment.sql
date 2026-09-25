CREATE TABLE IF NOT EXISTS Appoinment
(
Appoinmentid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids Jsonb  NULL
,patient uuid REFERENCES ClinicalAppointment(ClinicalAppointmentid) NULL
,origin uuid REFERENCES ClinicalAppointment(ClinicalAppointmentid) NULL
,bookingreferencenumber uuid REFERENCES ClinicalAppointment(ClinicalAppointmentid) NULL
,doctor uuid REFERENCES ClinicalAppointment(ClinicalAppointmentid) NULL
,appointmentdate uuid REFERENCES ClinicalAppointment(ClinicalAppointmentid) NULL
,task uuid REFERENCES ClinicalAppointment(ClinicalAppointmentid) NULL
,duration uuid REFERENCES ClinicalAppointment(ClinicalAppointmentid) NULL
,status uuid REFERENCES ClinicalAppointment(ClinicalAppointmentid) NULL

,createduser uuid NOT NULL
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


