CREATE TABLE IF NOT EXISTS TherapyKit_kititems
(
TherapyKit_kititemsid uuid PRIMARY KEY
,TherapyKitid uuid REFERENCES TherapyKit(TherapyKitid)
,record_order int
,therapyitem uuid REFERENCES TherapyItem(TherapyItemid) NOT NULL
,price decimal(18,2) NULL
,count int NOT NULL
,linetotal Varchar(256) NOT NULL
);


