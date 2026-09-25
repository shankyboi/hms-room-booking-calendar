CREATE TABLE IF NOT EXISTS DoctorInternMap_doctorinterndetail
(
DoctorInternMap_doctorinterndetailid uuid PRIMARY KEY
,DoctorInternMapid uuid REFERENCES DoctorInternMap(DoctorInternMapid)
,record_order int
,interndoctor uuid REFERENCES People(Peopleid) NOT NULL
,isactive Boolean NULL
);


