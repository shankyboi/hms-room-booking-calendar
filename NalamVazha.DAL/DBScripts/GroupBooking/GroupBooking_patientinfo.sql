CREATE TABLE IF NOT EXISTS GroupBooking_patientinfo
(
GroupBooking_patientinfoid uuid PRIMARY KEY
,GroupBookingid uuid REFERENCES GroupBooking(GroupBookingid)
,record_order int
,name Varchar(128) NOT NULL
,emailid Varchar(128) NOT NULL
,phonenumber Varchar(20) NOT NULL
);
CREATE TABLE IF NOT EXISTS GroupBooking_contacts
(
GroupBooking_contactsid uuid PRIMARY KEY
,GroupBookingid uuid REFERENCES GroupBooking(GroupBookingid)
,record_order int
,person Varchar(128) NOT NULL
,mobile Varchar(20) NOT NULL
,email Varchar(128) NULL
);


